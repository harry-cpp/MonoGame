// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using WebGLDotNET;
using static WebHelper;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class SamplerState
    {
        internal const uint TextureParameterNameTextureMaxAnisotropy = 0x84FE;
        private readonly float[] _openGLBorderColor = new float[4];

        internal void Activate(GraphicsDevice device, uint target, bool useMipmaps = false)
        {
            if (GraphicsDevice == null)
            {
                // We're now bound to a device... no one should
                // be changing the state of this object now!
                GraphicsDevice = device;
            }
            Debug.Assert(GraphicsDevice == device, "The state was created for a different device!");

            switch (Filter)
            {
                case TextureFilter.Point:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        gl.TexParameteri(target, TextureParameterNameTextureMaxAnisotropy, 1);
                        GraphicsExtensions.CheckGLError();
                    }
                    gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_MIN_FILTER, (int)(useMipmaps ? WebGL2RenderingContextBase.NEAREST_MIPMAP_NEAREST : WebGL2RenderingContextBase.NEAREST));
                    GraphicsExtensions.CheckGLError();
                    gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_MAG_FILTER, (int)WebGL2RenderingContextBase.NEAREST);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.Linear:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        gl.TexParameteri(target, TextureParameterNameTextureMaxAnisotropy, 1);
                        GraphicsExtensions.CheckGLError();
                    }
                    gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_MIN_FILTER, (int)(useMipmaps ? WebGL2RenderingContextBase.LINEAR_MIPMAP_LINEAR : WebGL2RenderingContextBase.LINEAR));
                    GraphicsExtensions.CheckGLError();
                    gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_MAG_FILTER, (int)WebGL2RenderingContextBase.LINEAR);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.Anisotropic:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        gl.TexParameteri(target, TextureParameterNameTextureMaxAnisotropy, MathHelper.Clamp(this.MaxAnisotropy, 1, GraphicsDevice.GraphicsCapabilities.MaxTextureAnisotropy));
                        GraphicsExtensions.CheckGLError();
                    }
                    gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_MIN_FILTER, (int)(useMipmaps ? WebGL2RenderingContextBase.LINEAR_MIPMAP_LINEAR : WebGL2RenderingContextBase.LINEAR));
                    GraphicsExtensions.CheckGLError();
                    gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_MAG_FILTER, (int)WebGL2RenderingContextBase.LINEAR);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.PointMipLinear:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        gl.TexParameteri(target, TextureParameterNameTextureMaxAnisotropy, 1);
                        GraphicsExtensions.CheckGLError();
                    }
                    gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_MIN_FILTER, (int)(useMipmaps ? WebGL2RenderingContextBase.NEAREST_MIPMAP_LINEAR : WebGL2RenderingContextBase.NEAREST));
                    GraphicsExtensions.CheckGLError();
                    gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_MAG_FILTER, (int)WebGL2RenderingContextBase.NEAREST);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.LinearMipPoint:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        gl.TexParameteri(target, TextureParameterNameTextureMaxAnisotropy, 1);
                        GraphicsExtensions.CheckGLError();
                    }
                    gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_MIN_FILTER, (int)(useMipmaps ? WebGL2RenderingContextBase.LINEAR_MIPMAP_NEAREST : WebGL2RenderingContextBase.LINEAR));
                    GraphicsExtensions.CheckGLError();
                    gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_MAG_FILTER, (int)WebGL2RenderingContextBase.LINEAR);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.MinLinearMagPointMipLinear:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        gl.TexParameteri(target, TextureParameterNameTextureMaxAnisotropy, 1);
                        GraphicsExtensions.CheckGLError();
                    }
                    gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_MIN_FILTER, (int)(useMipmaps ? WebGL2RenderingContextBase.LINEAR_MIPMAP_LINEAR : WebGL2RenderingContextBase.LINEAR));
                    GraphicsExtensions.CheckGLError();
                    gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_MAG_FILTER, (int)WebGL2RenderingContextBase.NEAREST);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.MinLinearMagPointMipPoint:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        gl.TexParameteri(target, TextureParameterNameTextureMaxAnisotropy, 1);
                        GraphicsExtensions.CheckGLError();
                    }
                    gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_MIN_FILTER, (int)(useMipmaps ? WebGL2RenderingContextBase.LINEAR_MIPMAP_NEAREST : WebGL2RenderingContextBase.LINEAR));
                    GraphicsExtensions.CheckGLError();
                    gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_MAG_FILTER, (int)WebGL2RenderingContextBase.NEAREST);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.MinPointMagLinearMipLinear:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        gl.TexParameteri(target, TextureParameterNameTextureMaxAnisotropy, 1);
                        GraphicsExtensions.CheckGLError();
                    }
                    gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_MIN_FILTER, (int)(useMipmaps ? WebGL2RenderingContextBase.NEAREST_MIPMAP_LINEAR : WebGL2RenderingContextBase.NEAREST));
                    GraphicsExtensions.CheckGLError();
                    gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_MAG_FILTER, (int)WebGL2RenderingContextBase.LINEAR);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.MinPointMagLinearMipPoint:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        gl.TexParameteri(target, TextureParameterNameTextureMaxAnisotropy, 1);
                        GraphicsExtensions.CheckGLError();
                    }
                    gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_MIN_FILTER, (int)(useMipmaps ? WebGL2RenderingContextBase.NEAREST_MIPMAP_NEAREST : WebGL2RenderingContextBase.NEAREST));
                    GraphicsExtensions.CheckGLError();
                    gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_MAG_FILTER, (int)WebGL2RenderingContextBase.LINEAR);
                    GraphicsExtensions.CheckGLError();
                    break;
                default:
                    throw new NotSupportedException();
            }

            // Set up texture addressing.
            gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_WRAP_S, (int)GetWrapMode(AddressU));
            GraphicsExtensions.CheckGLError();
            gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_WRAP_T, (int)GetWrapMode(AddressV));
            GraphicsExtensions.CheckGLError();

            // LOD bias is not supported by glTexParameter in OpenGL ES 2.0
            gl.TexParameterf(target, WebGL2RenderingContextBase.MAX_TEXTURE_LOD_BIAS, MipMapLevelOfDetailBias);
            GraphicsExtensions.CheckGLError();

            // Comparison samplers are not supported in OpenGL ES 2.0 (without an extension, anyway)
            switch (FilterMode)
            {
                case TextureFilterMode.Comparison:
                    gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_COMPARE_MODE, (int)WebGL2RenderingContextBase.COMPARE_REF_TO_TEXTURE);
                    GraphicsExtensions.CheckGLError();
                    gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_COMPARE_MODE, (int)ComparisonFunction.GetDepthFunction());
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilterMode.Default:
                    gl.TexParameteri(target, WebGL2RenderingContextBase.TEXTURE_COMPARE_MODE, (int)WebGL2RenderingContextBase.NONE);
                    GraphicsExtensions.CheckGLError();
                    break;
                default:
                    throw new InvalidOperationException("Invalid filter mode!");
            }

            if (GraphicsDevice.GraphicsCapabilities.SupportsTextureMaxLevel)
            {
                if (this.MaxMipLevel > 0)
                {
                    gl.TexParameteri(WebGL2RenderingContextBase.TEXTURE_2D, WebGL2RenderingContextBase.TEXTURE_MAX_LEVEL, this.MaxMipLevel);
                }
                else
                {
                    gl.TexParameteri(WebGL2RenderingContextBase.TEXTURE_2D, WebGL2RenderingContextBase.TEXTURE_MAX_LEVEL, 1000);
                }
                GraphicsExtensions.CheckGLError();
            }
        }

        private double GetWrapMode(TextureAddressMode textureAddressMode)
        {
            switch (textureAddressMode)
            {
                case TextureAddressMode.Clamp:
                    return WebGL2RenderingContextBase.CLAMP_TO_EDGE;
                case TextureAddressMode.Wrap:
                    return WebGL2RenderingContextBase.REPEAT;
                case TextureAddressMode.Mirror:
                    return WebGL2RenderingContextBase.MIRRORED_REPEAT;
                default:
                    throw new ArgumentException("No support for " + textureAddressMode);
            }
        }
    }
}
