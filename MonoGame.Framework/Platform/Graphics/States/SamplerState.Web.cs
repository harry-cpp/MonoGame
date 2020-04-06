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
                    gl.TexParameteri(target, WebGLRenderingContextBase.TEXTURE_MIN_FILTER, (int)(useMipmaps ? WebGLRenderingContextBase.NEAREST_MIPMAP_NEAREST : WebGLRenderingContextBase.NEAREST));
                    GraphicsExtensions.CheckGLError();
                    gl.TexParameteri(target, WebGLRenderingContextBase.TEXTURE_MAG_FILTER, (int)WebGLRenderingContextBase.NEAREST);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.Linear:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        gl.TexParameteri(target, TextureParameterNameTextureMaxAnisotropy, 1);
                        GraphicsExtensions.CheckGLError();
                    }
                    gl.TexParameteri(target, WebGLRenderingContextBase.TEXTURE_MIN_FILTER, (int)(useMipmaps ? WebGLRenderingContextBase.LINEAR_MIPMAP_LINEAR : WebGLRenderingContextBase.LINEAR));
                    GraphicsExtensions.CheckGLError();
                    gl.TexParameteri(target, WebGLRenderingContextBase.TEXTURE_MAG_FILTER, (int)WebGLRenderingContextBase.LINEAR);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.Anisotropic:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        gl.TexParameteri(target, TextureParameterNameTextureMaxAnisotropy, MathHelper.Clamp(this.MaxAnisotropy, 1, GraphicsDevice.GraphicsCapabilities.MaxTextureAnisotropy));
                        GraphicsExtensions.CheckGLError();
                    }
                    gl.TexParameteri(target, WebGLRenderingContextBase.TEXTURE_MIN_FILTER, (int)(useMipmaps ? WebGLRenderingContextBase.LINEAR_MIPMAP_LINEAR : WebGLRenderingContextBase.LINEAR));
                    GraphicsExtensions.CheckGLError();
                    gl.TexParameteri(target, WebGLRenderingContextBase.TEXTURE_MAG_FILTER, (int)WebGLRenderingContextBase.LINEAR);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.PointMipLinear:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        gl.TexParameteri(target, TextureParameterNameTextureMaxAnisotropy, 1);
                        GraphicsExtensions.CheckGLError();
                    }
                    gl.TexParameteri(target, WebGLRenderingContextBase.TEXTURE_MIN_FILTER, (int)(useMipmaps ? WebGLRenderingContextBase.NEAREST_MIPMAP_LINEAR : WebGLRenderingContextBase.NEAREST));
                    GraphicsExtensions.CheckGLError();
                    gl.TexParameteri(target, WebGLRenderingContextBase.TEXTURE_MAG_FILTER, (int)WebGLRenderingContextBase.NEAREST);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.LinearMipPoint:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        gl.TexParameteri(target, TextureParameterNameTextureMaxAnisotropy, 1);
                        GraphicsExtensions.CheckGLError();
                    }
                    gl.TexParameteri(target, WebGLRenderingContextBase.TEXTURE_MIN_FILTER, (int)(useMipmaps ? WebGLRenderingContextBase.LINEAR_MIPMAP_NEAREST : WebGLRenderingContextBase.LINEAR));
                    GraphicsExtensions.CheckGLError();
                    gl.TexParameteri(target, WebGLRenderingContextBase.TEXTURE_MAG_FILTER, (int)WebGLRenderingContextBase.LINEAR);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.MinLinearMagPointMipLinear:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        gl.TexParameteri(target, TextureParameterNameTextureMaxAnisotropy, 1);
                        GraphicsExtensions.CheckGLError();
                    }
                    gl.TexParameteri(target, WebGLRenderingContextBase.TEXTURE_MIN_FILTER, (int)(useMipmaps ? WebGLRenderingContextBase.LINEAR_MIPMAP_LINEAR : WebGLRenderingContextBase.LINEAR));
                    GraphicsExtensions.CheckGLError();
                    gl.TexParameteri(target, WebGLRenderingContextBase.TEXTURE_MAG_FILTER, (int)WebGLRenderingContextBase.NEAREST);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.MinLinearMagPointMipPoint:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        gl.TexParameteri(target, TextureParameterNameTextureMaxAnisotropy, 1);
                        GraphicsExtensions.CheckGLError();
                    }
                    gl.TexParameteri(target, WebGLRenderingContextBase.TEXTURE_MIN_FILTER, (int)(useMipmaps ? WebGLRenderingContextBase.LINEAR_MIPMAP_NEAREST : WebGLRenderingContextBase.LINEAR));
                    GraphicsExtensions.CheckGLError();
                    gl.TexParameteri(target, WebGLRenderingContextBase.TEXTURE_MAG_FILTER, (int)WebGLRenderingContextBase.NEAREST);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.MinPointMagLinearMipLinear:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        gl.TexParameteri(target, TextureParameterNameTextureMaxAnisotropy, 1);
                        GraphicsExtensions.CheckGLError();
                    }
                    gl.TexParameteri(target, WebGLRenderingContextBase.TEXTURE_MIN_FILTER, (int)(useMipmaps ? WebGLRenderingContextBase.NEAREST_MIPMAP_LINEAR : WebGLRenderingContextBase.NEAREST));
                    GraphicsExtensions.CheckGLError();
                    gl.TexParameteri(target, WebGLRenderingContextBase.TEXTURE_MAG_FILTER, (int)WebGLRenderingContextBase.LINEAR);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.MinPointMagLinearMipPoint:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        gl.TexParameteri(target, TextureParameterNameTextureMaxAnisotropy, 1);
                        GraphicsExtensions.CheckGLError();
                    }
                    gl.TexParameteri(target, WebGLRenderingContextBase.TEXTURE_MIN_FILTER, (int)(useMipmaps ? WebGLRenderingContextBase.NEAREST_MIPMAP_NEAREST : WebGLRenderingContextBase.NEAREST));
                    GraphicsExtensions.CheckGLError();
                    gl.TexParameteri(target, WebGLRenderingContextBase.TEXTURE_MAG_FILTER, (int)WebGLRenderingContextBase.LINEAR);
                    GraphicsExtensions.CheckGLError();
                    break;
                default:
                    throw new NotSupportedException();
            }

            // Set up texture addressing.
            gl.TexParameteri(target, WebGLRenderingContextBase.TEXTURE_WRAP_S, (int)GetWrapMode(AddressU));
            GraphicsExtensions.CheckGLError();
            gl.TexParameteri(target, WebGLRenderingContextBase.TEXTURE_WRAP_T, (int)GetWrapMode(AddressV));
            GraphicsExtensions.CheckGLError();
        }

        private double GetWrapMode(TextureAddressMode textureAddressMode)
        {
            switch (textureAddressMode)
            {
                case TextureAddressMode.Clamp:
                    return WebGLRenderingContextBase.CLAMP_TO_EDGE;
                case TextureAddressMode.Wrap:
                    return WebGLRenderingContextBase.REPEAT;
                case TextureAddressMode.Mirror:
                    return WebGLRenderingContextBase.MIRRORED_REPEAT;
                default:
                    throw new ArgumentException("No support for " + textureAddressMode);
            }
        }
    }
}
