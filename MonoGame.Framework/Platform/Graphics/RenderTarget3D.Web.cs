using System;
using WebGLDotNET;
using static WebHelper;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class RenderTarget3D
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

        private void PlatformConstruct(GraphicsDevice graphicsDevice, int width, int height, bool mipMap,
            DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage)
        {

        }
    }
}
