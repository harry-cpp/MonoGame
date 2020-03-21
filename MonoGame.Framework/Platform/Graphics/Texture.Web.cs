// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using static WebHelper;
using WebGLDotNET;

namespace Microsoft.Xna.Framework.Graphics
{
    public abstract partial class Texture
    {
        internal WebGLTexture glTexture;
        internal uint glTarget;
        internal uint glTextureUnit = WebGL2RenderingContextBase.TEXTURE0;
        internal uint glInternalFormat;
        internal uint glFormat;
        internal uint glType;
        internal SamplerState glLastSamplerState;

        private void PlatformGraphicsDeviceResetting()
        {
            DeleteGLTexture();
            glLastSamplerState = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                DeleteGLTexture();
                glLastSamplerState = null;
            }

            base.Dispose(disposing);
        }

        private void DeleteGLTexture()
        {
            if (glTexture != null)
                GraphicsDevice.DisposeTexture(glTexture);
            glTexture = null;
        }
    }
}

