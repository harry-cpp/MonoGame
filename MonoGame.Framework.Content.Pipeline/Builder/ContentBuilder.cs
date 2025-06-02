// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Net;
using System.Reflection;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace MonoGame.Framework.Content.Pipeline.Builder;

public abstract class ContentBuilder
{
    private readonly Dictionary<string, ContentInfo> _content = [];
    private readonly Dictionary<string, string> _outputContent = [];
    private uint _succeededToBuild = 0;
    private uint _failedToBuild = 0;

    public ContentBuilderParams Parameters { get; set; } = new ContentBuilderParams();

    /// <summary>
    /// Gets or sets the logger to be used by <see cref="ContentBuilder"/>.
    /// </summary>
    /// <value><see cref="ContentBuildLogger"/> by default.</value>
    public ContentBuildLogger Logger { get; set; } = new ContentBuildLogger();

    public virtual IContentCache ContentCache { get; init; } = new ContentCache();

    public abstract IContentCollection CollectContent(ContentBuilderParams args);

    // for server!
    // tells the server that the file changes should affect the ContentCollection and CollectContent should be re-run.
    // public virtual bool CheckNeedsRecollect(pass file watcher changes) => false;

    // will have a default implementation, but there are moments where you want to override the default implementation

    public ContentFileCache? BuildAndWriteContent(string relativePath, ContentInfo contentInfo)
    {
        ContentFileCache? contentFileCache = null;
        Logger.PushFile(relativePath);
        try
        {
            contentFileCache = ProcessContent(relativePath, contentInfo, true).contentFileCache;
            _succeededToBuild++;
        }
        catch (Exception ex)
        {
            Logger.Log(LogLevel.Error, $"Countent failed to build:\n{ex}");
            _failedToBuild++;
        }
        Logger.PopFile();
        return contentFileCache;
    }

    public (ContentFileCache? contentFileCache, object? processedObject) BuildAndLoadContent(string relativePath, ContentInfo contentInfo)
    {
        Logger.PushFile(relativePath);
        try
        {
            var content = ProcessContent(relativePath, contentInfo, false);
            _succeededToBuild++;
            return content;
        }
        catch (Exception ex)
        {
            Logger.Log(LogLevel.Error, $"Countent failed to build:\n{ex}");
            _failedToBuild++;
        }
        Logger.PopFile();
        return (null, null);
    }

    private (ContentFileCache? contentFileCache, object? processedObject) ProcessContent(string relativePath, ContentInfo contentInfo, bool writeToDisk)
    {
        ContentFileCache? contentFileCache = null;
        var filePath = Path.Combine(Parameters.RootedSourceDirectory, relativePath);
        var relativeDestPath = Path.Combine(contentInfo.ContentRoot, contentInfo.GetOutputPath(relativePath));
        var outputPath = Path.Combine(Parameters.RootedOutputDirectory, relativeDestPath);
        var outputDir = Path.GetDirectoryName(outputPath);

        if (string.IsNullOrWhiteSpace(outputDir))
        {
            return (contentFileCache, null);
        }

        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        if (!contentInfo.ShouldBuild)
        {
            Logger.Log($"Output: {relativeDestPath}");
            contentFileCache = ContentCache.ReadContentFileCache(this, relativePath, contentInfo.ContentRoot);
            if (contentFileCache != null)
            {
                Logger.Log($"Cache: Found");
                return (contentFileCache, null);
            }
            Logger.Log($"Cache: Not Found");

            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
            File.Copy(filePath, outputPath);

            contentFileCache = new ContentFileCache();
            contentFileCache.AddDependency(this, relativePath);
            contentFileCache.AddOutputFile(this, outputPath);
            ContentCache.WriteContentFileCache(this, relativePath, contentFileCache);
            return (contentFileCache, null);
        }

        if (!ContentBuilderHelper.GetImporter(relativePath, contentInfo.Importer, out IContentImporter importer))
        {
            Logger.Log(LogLevel.Warning, "Importer: Not found :(");
            return (contentFileCache, null);
        }
        Logger.Log($"Imposter: {importer.GetType().Name}");
        if (!ContentBuilderHelper.GetProcessor(importer, contentInfo.Processor, out IContentProcessor processor))
        {
            Logger.Log(LogLevel.Warning, "Processor: Not found :(");
            return (contentFileCache, null);
        }
        Logger.Log($"Processor: {processor.GetType().Name}");
        Logger.Log($"Output: {relativeDestPath}");

        contentFileCache = ContentCache.ReadContentFileCache(this, relativePath, contentInfo.ContentRoot, true, importer, processor);
        if (contentFileCache != null)
        {
            Logger.Log($"Cache: Found");
            return (contentFileCache, null);
        }
        Logger.Log($"Cache: Not Found");

        contentFileCache = new ContentFileCache
        {
            ContentRoot = contentInfo.ContentRoot,
            CompressContent = Parameters.CompressContent,
            GraphicsProfile = Parameters.GraphicsProfile,
            ShouldBuild = true,
            Importer = importer,
            Processor = processor
        };
        contentFileCache.AddDependency(this, relativePath);
        contentFileCache.AddOutputFile(this, outputPath);

        var importContext = new ContentBuilderImporterContext(this, contentFileCache);
        var importedObject = importer.Import(filePath, importContext);

        var processorContext = new ContentBuilderProcessorContext(this, contentFileCache, contentInfo.ContentRoot, outputPath);
        var processedObject = processor.Process(importedObject, processorContext);

        if (writeToDisk)
        {
            var compiler = new ContentCompiler();
            using var stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);
            compiler.Compile(stream, processedObject, Parameters.Platform, Parameters.GraphicsProfile, Parameters.CompressContent, Parameters.RootedOutputDirectory, outputDir);
            ContentCache.WriteContentFileCache(this, relativePath, contentFileCache);
        }

        return (contentFileCache, processedObject);
    }

    public void Run(ContentBuilderParams parameters)
    {
        Parameters = parameters;
        Directory.SetCurrentDirectory(Parameters.WorkingDirectory);

        Logger.IndentCharacter = ' ';
        Logger.IndentCharacterSize = 2;

        Logger.PushFile("Starting Content Builder");
        foreach (var prop in parameters.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            Logger.Log($"{prop.Name}: {prop.GetValue(parameters)}");
        }
        Logger.PopFile();

        ContentCache.LoadCache(this);
        var contentCollection = CollectContent(Parameters);
        ScanFiles(contentCollection, Parameters.RootedSourceDirectory);

        switch (Parameters.Mode)
        {
            case ContentBuilderMode.Builder:
                RunBuild();
                break;
            case ContentBuilderMode.Server:
                RunServer();
                break;
        }
    }

    public void Run(string[] args) => Run(ContentBuilderParams.Parse(args));

    private void ScanFiles(IContentCollection contentCollection, string directory)
    {
        foreach (var dir in Directory.GetDirectories(directory))
        {
            ScanFiles(contentCollection, dir);
        }

        foreach (var filePath in Directory.GetFiles(directory))
        {
            ContentInfo? contentInfo = null;
            var relativePath = Path.GetRelativePath(Parameters.RootedSourceDirectory, filePath);
            if (contentCollection.GetContentInfo(relativePath, ref contentInfo) && contentInfo != null)
            {
                _content[relativePath] = contentInfo;
                _outputContent[Path.Combine(contentInfo.ContentRoot, contentInfo.GetOutputPath(relativePath))] = relativePath;
            }
        }
    }

    private void RunBuild()
    {
        foreach (var pair in _content)
        {
            if (_content.TryGetValue(pair.Key, out ContentInfo? contentInfo))
            {
                BuildAndWriteContent(pair.Key, contentInfo);
            }
        }

        if (!Parameters.SkipClean)
        {
            ContentCache.CleanCache(this);
        }
        ContentCache.FlushCache(this);

        Logger.PushFile("Content Builder Finished");
        Logger.Log($"{_succeededToBuild} succeeded, {_failedToBuild} failed");
        Logger.PopFile();
    }

    private void RunServer()
    {
        Console.CancelKeyPress += delegate
        {
            // We don't want to call CleanCache in server mode as we don't go through all the files!
            ContentCache.FlushCache(this);
        };

        Logger.Log($"Starting server on: http://localhost:{Parameters.ServerPort}/");
        while (true)
        {
            if (!HttpListener.IsSupported)
            {
                Logger.Log("HttpListener is not supported on this system.");
                return;
            }

            var listener = new HttpListener();
            listener.Prefixes.Add($"http://*:{Parameters.ServerPort}/");
            listener.Start();
            Logger.Log("Listening...");

            while (true)
            {
                try
                {
                    RunServerCycle(listener);
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, $"Error during a cycle: {ex}");
                }
            }
        }
    }

    void RunServerCycle(HttpListener listener)
    {
        var context = listener.GetContext();
        var request = context.Request;
        var relativePath = request.Headers["path"] ?? "";
        var filePath = Path.Combine(Parameters.RootedOutputDirectory, relativePath);

        if (_outputContent.TryGetValue(relativePath, out var inputPath))
        {
            if (_content.TryGetValue(inputPath, out ContentInfo? contentInfo))
            {
                BuildAndWriteContent(inputPath, contentInfo);
            }
        }

        var response = context.Response;
        var fileExists = File.Exists(filePath);
        var currentLastModifiedTime = fileExists ? File.GetLastWriteTimeUtc(filePath).Ticks : 0;

        var sentLastModifiedTimeStr = request.Headers["LastModifiedTime"];
        try
        {
            if (fileExists && sentLastModifiedTimeStr != null)
            {
                var sentLastModifiedTime = long.Parse(sentLastModifiedTimeStr);

                if (sentLastModifiedTime == currentLastModifiedTime)
                {
                    fileExists = false;
                }
            }
        }
        catch
        {
            Logger.Log(LogLevel.Error, $"Sent last modified time is invalid so ignoring it: {sentLastModifiedTimeStr}");
        }

        if (!fileExists)
        {
            response.ContentLength64 = 1;
            response.OutputStream.Write([0], 0, 1);
            response.OutputStream.Close();
            return;
        }

        var lastModifiedTime = BitConverter.GetBytes(currentLastModifiedTime);
        var buffer = File.ReadAllBytes(filePath);

        response.ContentLength64 = lastModifiedTime.Length + buffer.Length;
        response.OutputStream.Write(lastModifiedTime, 0, lastModifiedTime.Length);
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }
}
