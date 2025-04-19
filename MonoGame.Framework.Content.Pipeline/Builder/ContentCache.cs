// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Framework.Content.Pipeline.Builder;

class ContentCache : IContentCache
{
    const string CacheFileName = "cache.yaml";

    private Dictionary<string, ContentFileCache> _cache = [];

    public void LoadCache(ContentBuilder builder)
    {
        var cacheFilePath = Path.Combine(builder.Parameters.RootedIntermediateDirectory, CacheFileName);
        try
        {
            if (File.Exists(cacheFilePath))
            {
                var text = File.ReadAllText(cacheFilePath);
                _cache = ContentBuilderHelper.Deserializer.Deserialize<Dictionary<string, ContentFileCache>>(text) ?? [];
            }
        }
        catch
        {
            builder.Logger.Log(LogLevel.Error, "Failed to load the Cache!");
        }
    }

    public ContentFileCache? ReadContentFileCache(string relativePath) => _cache.TryGetValue(relativePath, out ContentFileCache? fileCache) ? fileCache : null;

    public void WriteContentFileCache(string relativePath, ContentFileCache fileCache) => _cache[relativePath] = fileCache;

    public void FlushCache(ContentBuilder builder)
    {
        var cacheFilePath = Path.Combine(builder.Parameters.RootedIntermediateDirectory, CacheFileName);
        var dirPath = Path.GetDirectoryName(cacheFilePath);
        if (string.IsNullOrEmpty(dirPath))
        {
            return;
        }

        if (!File.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        var text = ContentBuilderHelper.Serializer.Serialize(_cache);
        File.WriteAllText(cacheFilePath, text);
    }
}
