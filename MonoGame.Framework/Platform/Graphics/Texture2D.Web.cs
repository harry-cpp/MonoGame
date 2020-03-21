// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using static WebHelper;
using WebGLDotNET;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class Texture2D : Texture
    {
        private void PlatformConstruct(int width, int height, bool mipmap, SurfaceFormat format, SurfaceType type, bool shared)
        {
            this.glTarget = WebGL2RenderingContextBase.TEXTURE_2D;
            format.GetGLFormat(GraphicsDevice, out glInternalFormat, out glFormat, out glType);

            GenerateGLTextureIfRequired();
            int w = width;
            int h = height;
            int level = 0;

            while (true)
            {
                if (glFormat == WebGL2RenderingContextBase.COMPRESSED_TEXTURE_FORMATS)
                {
                    int imageSize = 0;
                    // PVRTC has explicit calculations for imageSize
                    // https://www.khronos.org/registry/OpenGL/extensions/IMG/IMG_texture_compression_pvrtc.txt
                    if (format == SurfaceFormat.RgbPvrtc2Bpp || format == SurfaceFormat.RgbaPvrtc2Bpp)
                    {
                        imageSize = (Math.Max(w, 16) * Math.Max(h, 8) * 2 + 7) / 8;
                    }
                    else if (format == SurfaceFormat.RgbPvrtc4Bpp || format == SurfaceFormat.RgbaPvrtc4Bpp)
                    {
                        imageSize = (Math.Max(w, 8) * Math.Max(h, 8) * 4 + 7) / 8;
                    }
                    else
                    {
                        int blockSize = format.GetSize();
                        int blockWidth, blockHeight;
                        format.GetBlockSize(out blockWidth, out blockHeight);
                        int wBlocks = (w + (blockWidth - 1)) / blockWidth;
                        int hBlocks = (h + (blockHeight - 1)) / blockHeight;
                        imageSize = wBlocks * hBlocks * blockSize;
                    }
                }

                var imageData = new ImageData(new byte[4] { 0, 0, 0, 0 }, 1, 1);

                gl.TexImage2D(WebGL2RenderingContextBase.TEXTURE_2D, level, glInternalFormat, glFormat, glType, imageData);
                GraphicsExtensions.CheckGLError();

                if ((w == 1 && h == 1) || !mipmap)
                    break;
                if (w > 1)
                    w = w / 2;
                if (h > 1)
                    h = h / 2;
                ++level;
            }
        }

        private void GenerateGLTextureIfRequired()
        {
            if (this.glTexture == null)
            {
                glTexture = gl.CreateTexture();
                GraphicsExtensions.CheckGLError();

                // For best compatibility and to keep the default wrap mode of XNA, only set ClampToEdge if either
                // dimension is not a power of two.
                var wrap = WebGL2RenderingContextBase.REPEAT;
                if (((width & (width - 1)) != 0) || ((height & (height - 1)) != 0))
                    wrap = WebGL2RenderingContextBase.CLAMP_TO_EDGE;

                gl.BindTexture(WebGL2RenderingContextBase.TEXTURE_2D, glTexture);
                GraphicsExtensions.CheckGLError();
                gl.TexParameteri(WebGL2RenderingContextBase.TEXTURE_2D, WebGL2RenderingContextBase.TEXTURE_MIN_FILTER, (int)((_levelCount > 1) ? WebGL2RenderingContextBase.LINEAR_MIPMAP_LINEAR : WebGL2RenderingContextBase.LINEAR));
                GraphicsExtensions.CheckGLError();
                gl.TexParameteri(WebGL2RenderingContextBase.TEXTURE_2D, WebGL2RenderingContextBase.TEXTURE_MAG_FILTER, (int)WebGL2RenderingContextBase.LINEAR);
                GraphicsExtensions.CheckGLError();
                gl.TexParameteri(WebGL2RenderingContextBase.TEXTURE_2D, WebGL2RenderingContextBase.TEXTURE_WRAP_S, (int)wrap);
                GraphicsExtensions.CheckGLError();
                gl.TexParameteri(WebGL2RenderingContextBase.TEXTURE_2D, WebGL2RenderingContextBase.TEXTURE_WRAP_T, (int)wrap);
                GraphicsExtensions.CheckGLError();
            }
        }

        private void PlatformSetData<T>(int level, T[] data, int startIndex, int elementCount) where T : struct
        {
            int w, h;
            GetSizeForLevel(Width, Height, level, out w, out h);

            // Store the current bound texture.
            var prevTexture = GraphicsExtensions.GetBoundTexture2D();

            if (prevTexture != glTexture)
            {
                gl.BindTexture(WebGL2RenderingContextBase.TEXTURE_2D, glTexture);
                GraphicsExtensions.CheckGLError();
            }

            GenerateGLTextureIfRequired();
            gl.PixelStorei(WebGL2RenderingContextBase.UNPACK_ALIGNMENT, Math.Min(_format.GetSize(), 8));

            if (glFormat == WebGL2RenderingContextBase.COMPRESSED_TEXTURE_FORMATS)
            {
                throw new NotImplementedException();
            }
            else
            {
                gl.TexImage2D(WebGL2RenderingContextBase.TEXTURE_2D, level, (int)glInternalFormat, w, h, 0, glFormat, glType, data);
            }

            GraphicsExtensions.CheckGLError();

            // Restore the bound texture.
            if (prevTexture != glTexture)
            {
                gl.BindTexture(WebGL2RenderingContextBase.TEXTURE_2D, prevTexture);
                GraphicsExtensions.CheckGLError();
            }
        }

        private void PlatformSetData<T>(int level, int arraySlice, Rectangle rect, T[] data, int startIndex, int elementCount) where T : struct
        {
            throw new NotImplementedException();
        }

        private void PlatformGetData<T>(int level, int arraySlice, Rectangle rect, T[] data, int startIndex, int elementCount) where T : struct
        {
            throw new NotImplementedException();
        }

        private static Texture2D PlatformFromStream(GraphicsDevice graphicsDevice, Stream stream)
        {
            throw new NotImplementedException();
        }

        private void PlatformSaveAsJpeg(Stream stream, int width, int height)
        {
            throw new NotImplementedException();
        }

        private void PlatformSaveAsPng(Stream stream, int width, int height)
        {
            throw new NotImplementedException();
        }

        private void PlatformReload(Stream textureStream)
        {
            
        }
	}
}

