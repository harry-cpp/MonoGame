// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using System.Net.Http;
using MonoGame.Framework.Utilities;

namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// Provides functionality for opening a stream in the title storage area.
    /// </summary>
    public static partial class TitleContainer
    {
        static partial void PlatformInit();

        static TitleContainer()
        {
            Location = string.Empty;
            PlatformInit();
        }

        static internal string Location { get; private set; }

        static bool PrepareContent(string filePath)
        {
            try
            {
                var relativePath = Path.GetRelativePath("Content", filePath); // TODO: Grab ContentManager root info!
                var dirPath = Path.GetDirectoryName(filePath) ?? "";

                if(!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                using HttpClient client = new();
                client.DefaultRequestHeaders.Add("Path", relativePath);

                using var stream = client.GetStreamAsync("http://localhost:8006/").Result;
                using var fileStream = File.Create(filePath);
                stream.CopyTo(fileStream);

                return true;
            }
            catch
            { }

            return false;
        }

        /// <summary>
        /// Returns an open stream to an existing file in the title storage area.
        /// </summary>
        /// <param name="name">The filepath relative to the title storage area.</param>
        /// <returns>An open stream if file is found.</returns>
        /// <exception cref="ArgumentNullException">If name is null or invalid.</exception>
        /// <exception cref="FileNotFoundException">If file is not found.</exception>
        public static Stream OpenStream(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            // We do not accept absolute paths here.
            if (Path.IsPathRooted(name))
                throw new ArgumentException("Invalid filename. TitleContainer.OpenStream requires a relative path.", name);

            if (!File.Exists(name) && !PrepareContent(name))
            {
                throw new ArgumentNullException("Could not prepare content: " + name);
            }

            // Normalize the file path.
            var safeName = NormalizeRelativePath(name);

            // Call the platform code to open the stream.  Any errors
            // at this point should result in a file not found.
            Stream stream;
            try
            {
                stream = PlatformOpenStream(safeName);
                if (stream == null)
                    throw FileNotFoundException(name, null);
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException(name, ex);
            }

            return stream;
        }

        /// <summary>
        /// Faster version of <see cref="OpenStream"/> as it doesn't rely on exceptions in error cases.
        /// </summary>
        internal static Stream OpenStreamNoException(string name)
        {
            if (string.IsNullOrEmpty(name) || Path.IsPathRooted(name))
            {
                return null;
            }

            string safeName = NormalizeRelativePath(name);
            Stream stream;
            try
            {
                stream = PlatformOpenStream(safeName);

                return stream;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static Exception FileNotFoundException(string name, Exception inner)
        {
            return new FileNotFoundException("Error loading \"" + name + "\". File not found.", inner);
        }

        internal static string NormalizeRelativePath(string name)
        {
            var uri = new Uri("file:///" + FileHelpers.UrlEncode(name));
            var path = uri.LocalPath;
            path = path.Substring(1);
            return path.Replace(FileHelpers.NotSeparator, FileHelpers.Separator);
        }
    }
}

