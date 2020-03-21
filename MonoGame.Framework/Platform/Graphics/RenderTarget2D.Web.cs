// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using WebGLDotNET;
using static WebHelper;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class RenderTarget2D
    {
        WebGLTexture IRenderTarget.GLTexture
        {
            get { return glTexture; }
        }

        uint IRenderTarget.GLTarget
        {
            get { return glTarget; }
        }

        WebGLRenderbuffer IRenderTarget.GLColorBuffer { get; set; }
        WebGLRenderbuffer IRenderTarget.GLDepthBuffer { get; set; }
        WebGLRenderbuffer IRenderTarget.GLStencilBuffer { get; set; }

        uint IRenderTarget.GetFramebufferTarget(RenderTargetBinding renderTargetBinding)
        {
            return glTarget;
        }

        private void PlatformConstruct(
            GraphicsDevice graphicsDevice, 
            int width, 
            int height, 
            bool mipMap,
            DepthFormat preferredDepthFormat,
            int preferredMultiSampleCount,
            RenderTargetUsage usage, 
            bool shared)
        {
            graphicsDevice.PlatformCreateRenderTarget(this, width, height, mipMap, this.Format, preferredDepthFormat, preferredMultiSampleCount, usage);
        }

        private void PlatformGraphicsDeviceResetting()
        {

        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
                this.GraphicsDevice.PlatformDeleteRenderTarget(this);

            base.Dispose(disposing);
        }
    }
}
