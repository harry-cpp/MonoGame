// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using WebAssembly;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Xna.Framework
{
    partial class TitleContainer
    {
        static partial void PlatformInit()
        {
            using (var window = (JSObject)Runtime.GetGlobalObject("window"))
            using (var location = (JSObject)window.GetObjectProperty("location"))
            {
                var address = (string)location.GetObjectProperty("href");

                if (address.Contains("/"))
                {
                    address = address.Substring(0, address.LastIndexOf('/') + 1);
                }

                Location = address;
            }
        }

        private static Stream PlatformOpenStream(string safeName)
        {
            throw new NotSupportedException("Please use LoadAsync or PlatformOpenAsync");
        }

        private static async Task<Stream> PlatformOpenStreamAsync(string safeName)
        {
            var request = new HttpClient() { BaseAddress = new Uri(Location) };
            var response = await request.GetAsync(safeName);

            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsStreamAsync();
        }
    }
}
