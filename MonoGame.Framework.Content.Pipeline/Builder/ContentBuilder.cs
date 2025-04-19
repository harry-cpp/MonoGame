// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Net;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace MonoGame.Framework.Content.Pipeline.Builder;

public abstract class ContentBuilder
{
    private Dictionary<string, ContentInfo> _content = [];
    private Dictionary<string, string> _outputContent = [];

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

    public void ProcessContent(string relativePath, ContentInfo contentInfo)
    {
        Logger.PushFile(relativePath);
        try
        {
            ProcessContentInternal(relativePath, contentInfo);
        }
        catch (Exception ex)
        {
            Logger.Log(LogLevel.Error, $"Countent failed to build:\n{ex}");
        }
        Logger.PopFile();
    }

    private void ProcessContentInternal(string relativePath, ContentInfo contentInfo)
    {
        var filePath = Path.Combine(Parameters.RootedSourceDirectory, relativePath);
        var relativeDestPath = contentInfo.GetOutputPath(relativePath);
        var outputPath = Path.Combine(Parameters.RootedOutputDirectory, relativeDestPath);
        var outputDir = Path.GetDirectoryName(outputPath);

        if (string.IsNullOrWhiteSpace(outputDir))
        {
            return;
        }

        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        if (!contentInfo.ShouldBuild)
        {
            Logger.Log($"Output: {relativeDestPath}");
            if (ContentCache.ReadContentFileCache(relativePath)?.IsValid(this) ?? false)
            {
                Logger.Log($"Cache: Found");
                return;
            }
            Logger.Log($"Cache: Not Found");

            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
            File.Copy(filePath, outputPath);

            var copiedFileCache = new ContentFileCache();
            copiedFileCache.AddDependency(this, relativePath);
            copiedFileCache.AddOutputFile(this, outputPath);
            ContentCache.WriteContentFileCache(relativePath, copiedFileCache);
            return;
        }

        if (!ContentBuilderHelper.GetImporter(relativePath, contentInfo.Importer, out IContentImporter importer))
        {
            Logger.Log(LogLevel.Warning, "Importer: Not found :(");
            return;
        }
        Logger.Log($"Imposter: {importer.GetType().Name}");
        if (!ContentBuilderHelper.GetProcessor(importer, contentInfo.Processor, out IContentProcessor processor))
        {
            Logger.Log(LogLevel.Warning, "Processor: Not found :(");
            return;
        }
        Logger.Log($"Processor: {processor.GetType().Name}");
        Logger.Log($"Output: {relativeDestPath}");

        var contentFileCache = ContentCache.ReadContentFileCache(relativePath);
        if (contentFileCache?.IsValid(this, true, importer, processor) ?? false)
        {
            Logger.Log($"Cache: Found");
            return;
        }
        Logger.Log($"Cache: Not Found");

        contentFileCache = new ContentFileCache
        {
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

        var processorContext = new ContentBuilderProcessorContext(this, contentFileCache, outputPath);
        var processedObject = processor.Process(importedObject, processorContext);

        var compiler = new ContentCompiler();
        using var stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);
        compiler.Compile(stream, processedObject, Parameters.Platform, Parameters.GraphicsProfile, Parameters.CompressContent, Parameters.RootedOutputDirectory, outputDir);

        ContentCache.WriteContentFileCache(relativePath, contentFileCache);
    }

    public void Run(ContentBuilderParams parameters)
    {
        Parameters = parameters;
        Directory.SetCurrentDirectory(Parameters.WorkingDirectory);

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
                _outputContent[contentInfo.GetOutputPath(relativePath)] = relativePath;
            }
        }
    }

    private void RunBuild()
    {
        foreach (var pair in _content)
        {
            if (_content.TryGetValue(pair.Key, out ContentInfo? contentInfo))
            {
                ProcessContent(pair.Key, contentInfo);
            }
        }

        ContentCache.FlushCache(this);
    }

    private void RunServer()
    {
        Console.CancelKeyPress += delegate
        {
            ContentCache.FlushCache(this);
        };

        Console.WriteLine($"Starting server on: http://localhost:{Parameters.ServerPort}/");
        while (true)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("HttpListener is not supported on this system.");
                return;
            }

            var listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{Parameters.ServerPort}/");
            listener.Start();
            Console.WriteLine("Listening...");

            while (true)
            {
                var context = listener.GetContext();
                var request = context.Request;

                var relativePath = request.Headers["path"] ?? "";
                var filePath = Path.Combine(Parameters.RootedOutputDirectory, relativePath);

                if (_outputContent.TryGetValue(relativePath, out var inputPath))
                {
                    try
                    {
                        if (_content.TryGetValue(inputPath, out ContentInfo? contentInfo))
                        {
                            ProcessContent(inputPath, contentInfo);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                var response = context.Response;
                var buffer = File.Exists(filePath) ? File.ReadAllBytes(Path.Combine(Parameters.RootedOutputDirectory, filePath)) : [0];
                response.ContentLength64 = buffer.Length;
                var output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
        }
    }
}
