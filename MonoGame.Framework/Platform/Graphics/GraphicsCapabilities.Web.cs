// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using WebGLDotNET;
using static WebHelper;

namespace Microsoft.Xna.Framework.Graphics
{
    internal partial class GraphicsCapabilities
    {
        /// <summary>
        /// True, if GL_ARB_framebuffer_object is supported; false otherwise.
        /// </summary>
        internal bool SupportsFramebufferObjectARB { get; private set; }

        /// <summary>
        /// True, if GL_EXT_framebuffer_object is supported; false otherwise.
        /// </summary>
        internal bool SupportsFramebufferObjectEXT { get; private set; }

        /// <summary>
        /// True, if GL_IMG_multisampled_render_to_texture is supported; false otherwise.
        /// </summary>
        internal bool SupportsFramebufferObjectIMG { get; private set; }

        private void PlatformInitialize(GraphicsDevice device)
        {
            var exts = gl.GetSupportedExtensions();

            SupportsNonPowerOfTwo = exts.Contains("OES_fbo_render_mipmap");
            SupportsTextureFilterAnisotropic = exts.Contains("EXT_texture_filter_anisotropic");

			SupportsDepth24 = false;
			SupportsPackedDepthStencil = exts.Contains("WEBGL_depth_texture");
			SupportsDepthNonLinear = false;
            SupportsTextureMaxLevel = true;

            // Texture compression
            SupportsS3tc = exts.Contains("WEBGL_compressed_texture_s3tc");
            SupportsDxt1 = SupportsS3tc;
            SupportsPvrtc = exts.Contains("WEBGL_compressed_texture_pvrtc");
            SupportsEtc1 = exts.Contains("WEBGL_compressed_texture_etc1");
            SupportsAtitc = exts.Contains("WEBGL_compressed_texture_atc");

            // Framebuffer objects
            SupportsFramebufferObjectARB = true;
            SupportsFramebufferObjectEXT = true;
            SupportsFramebufferObjectIMG = false;

            // Anisotropic filtering
            int anisotropy = 0;
            if (SupportsTextureFilterAnisotropic)
            {
                var ext = gl.GetExtension("EXT_texture_filter_anisotropic");
                anisotropy = (int)gl.GetParameter(0x84FF); // 0x84FF / MAX_TEXTURE_MAX_ANISOTROPY_EXT
                GraphicsExtensions.CheckGLError();
            }
            MaxTextureAnisotropy = anisotropy;

            // sRGB
            SupportsSRgb = exts.Contains("EXT_sRGB") || exts.Contains("WEBGL_compressed_texture_s3tc_srgb");

            // TODO: Implement OpenGL support for texture arrays
            // once we can author shaders that use texture arrays.
            SupportsTextureArrays = false;

            SupportsDepthClamp = false;

            SupportsVertexTextures = false; // For now, until we implement vertex textures in OpenGL.

            _maxMultiSampleCount = (int)gl.GetParameter(WebGL2RenderingContextBase.SAMPLES);

            SupportsInstancing = false;
        }
    }
}
