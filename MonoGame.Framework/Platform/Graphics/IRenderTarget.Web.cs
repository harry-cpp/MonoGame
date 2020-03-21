// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using WebGLDotNET;
using static WebHelper;

namespace Microsoft.Xna.Framework.Graphics
{
    internal partial interface IRenderTarget
    {
        WebGLTexture GLTexture { get; }
        uint GLTarget { get; }
        WebGLRenderbuffer GLColorBuffer { get; set; }
        WebGLRenderbuffer GLDepthBuffer { get; set; }
        WebGLRenderbuffer GLStencilBuffer { get; set; }
        int MultiSampleCount { get; }
        int LevelCount { get; }

        uint GetFramebufferTarget(RenderTargetBinding renderTargetBinding);
    }
}
