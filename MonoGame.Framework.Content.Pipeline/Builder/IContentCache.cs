// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Content.Pipeline.Builder;

/// <summary>
/// An interface for storing information about the compiled content.
/// </summary>
public interface IContentCache
{
    /// <summary>
    /// Loads the content cache for the specified builder.
    /// </summary>
    /// <param name="builder">A <see cref="ContentBuilder"/> that the cache file is related to.</param>
    void LoadCache(ContentBuilder builder);

    /// <summary>
    /// Reads (from memory or disk) cached information about the content file.
    /// </summary>
    /// <param name="relativePath">A relative path to the content file.</param>
    /// <returns>An instance of <see cref="ContentFileCache"/> if any cache information about the content is found, <c>null</c> otherwise.</returns>
    ContentFileCache? ReadContentFileCache(string relativePath);

    /// <summary>
    /// Writes down (to memory or disk) information about the content file.
    /// 
    /// Do note that <see cref="FlushCache"/> will always be called before exiting the content builder.
    /// </summary>
    /// <param name="relativePath">A relative path to the content file.</param>
    /// <param name="fileCache">A <see cref="ContentFileCache"/> containing the information about the source file and compiled content.</param>
    void WriteContentFileCache(string relativePath, ContentFileCache fileCache);

    /// <summary>
    /// Saves any pending content cache information to disk for the specified builder.
    /// </summary>
    /// <param name="builder">A <see cref="ContentBuilder"/> that the cache file is related to.</param>
    void FlushCache(ContentBuilder builder);
}
