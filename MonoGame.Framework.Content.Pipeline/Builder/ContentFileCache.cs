
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Framework.Content.Pipeline.Builder;

/// <summary>
/// Contains cached information about a single source content file.
/// </summary>
public record ContentFileCache
{
    /// <summary>
    /// <c>true</c> if the content was built, <c>false</c> if the content was copied.
    /// </summary>
    public bool ShouldBuild { get; init; } = false;

    /// <summary>
    /// An <see cref="IContentImporter"/> that was used for the building of the content file,
    /// </summary>
    public IContentImporter? Importer { get; init; } = null;

    /// <summary>
    /// An <see cref="IContentProcessor"/> that was used for the building of the content file,
    /// </summary>
    public IContentProcessor? Processor { get; init; } = null;

    /// <summary>
    /// Indicates if the content was compressed.
    /// </summary>
    public bool CompressContent { get; init; } = false;

    /// <summary>
    /// Indicates the <see cref="GraphicsProfile"/> that was used when the content was built.
    /// </summary>
    public GraphicsProfile GraphicsProfile { get; init; } = GraphicsProfile.HiDef;

    /// <summary>
    /// A dictionary of keys of dependency files that either the <see cref="IContentImporter"/> or <see cref="IContentProcessor"/> included
    /// and values of the last modified times for those files.
    /// </summary>
    public Dictionary<string, DateTime> DependencyFiles { get; init; } = [];

    /// <summary>
    /// A list of output files that the <see cref="IContentProcessor"/> included.
    /// </summary>
    public List<string> OutputFiles { get; init; } = [];

    /// <summary>
    /// Adds the specified file as a dependency for the current content.
    /// </summary>
    /// <param name="builder">A <see cref="ContentBuilder"/> the added depedency is related to.</param>
    /// <param name="dependencyPath">A relative or absolute path to the dependency file.</param>
    public void AddDependency(ContentBuilder builder, string dependencyPath)
    {
        string fullDependencyPath;
        string relativeDependencyPath;

        if (Path.IsPathRooted(dependencyPath))
        {
            fullDependencyPath = dependencyPath;
            relativeDependencyPath = Path.GetRelativePath(builder.Parameters.RootedSourceDirectory, dependencyPath);
        }
        else
        {
            fullDependencyPath = Path.Combine(builder.Parameters.RootedSourceDirectory, dependencyPath);
            relativeDependencyPath = dependencyPath;
        }

        if (!File.Exists(fullDependencyPath))
        {
            return;
        }

        var lastModifiedTime = File.GetLastWriteTimeUtc(fullDependencyPath);
        DependencyFiles[relativeDependencyPath] = lastModifiedTime;
    }

    /// <summary>
    /// Adds the specified file as an output file related to the current content file.
    /// </summary>
    /// <param name="builder">A <see cref="ContentBuilder"/> the output file is related to.</param>
    /// <param name="outputPath">A relative or absolute path to the output file.</param>
    public void AddOutputFile(ContentBuilder builder, string outputPath)
    {
        var relativeOutputFile = Path.IsPathRooted(outputPath) ? Path.GetRelativePath(builder.Parameters.RootedOutputDirectory, outputPath) : outputPath;
        OutputFiles.Add(relativeOutputFile);
    }

    /// <summary>
    /// Returns if the passed parameters match the current set of information about the content file.
    /// </summary>
    /// <param name="builder">A <see cref="ContentBuilder"/> that is compiling the contento.</param>
    /// <param name="shouldBuild">If the content file will be built or copied.</param>
    /// <param name="importer">An <see cref="IContentImporter"/> the content file will be passed through.</param>
    /// <param name="processor">An <see cref="IContentProcessor"/> the content file will be passed through.</param>
    /// <returns></returns>
    public bool IsValid(ContentBuilder builder, bool shouldBuild = false, IContentImporter? importer = null, IContentProcessor? processor = null)
    {
        if (builder.Parameters.GraphicsProfile != GraphicsProfile ||
            builder.Parameters.CompressContent != CompressContent ||
            shouldBuild != ShouldBuild ||
            !ContentBuilderHelper.ArePropsEqual(Importer, importer) ||
            !ContentBuilderHelper.ArePropsEqual(Processor, processor))
        {
            return false;
        }

        foreach (var dependency in DependencyFiles)
        {
            var dependencyFullPath = Path.Combine(builder.Parameters.RootedSourceDirectory, dependency.Key);
            var dependencyModifiedTime = File.GetLastWriteTimeUtc(dependencyFullPath);

            if (dependencyModifiedTime != dependency.Value)
            {
                return false;
            }
        }

        foreach (var outputPath in OutputFiles)
        {
            var fullOutputPath = Path.Combine(builder.Parameters.RootedOutputDirectory, outputPath);

            if (!File.Exists(fullOutputPath))
            {
                return false;
            }
        }

        return true;
    }
}
