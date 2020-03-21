// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using WebGLDotNET;
using static WebHelper;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class IndexBuffer
    {
		internal WebGLBuffer ibo;

        private void PlatformConstruct(IndexElementSize indexElementSize, int indexCount)
        {
            GenerateIfRequired();
        }

        private void PlatformGraphicsDeviceResetting()
        {
            ibo = null;
        }

        /// <summary>
        /// If the IBO does not exist, create it.
        /// </summary>
        void GenerateIfRequired()
        {
            if (ibo == null)
            {
                var sizeInBytes = IndexCount * (this.IndexElementSize == IndexElementSize.SixteenBits ? 2 : 4);

                ibo = gl.CreateBuffer();
                GraphicsExtensions.CheckGLError();
                gl.BindBuffer(WebGL2RenderingContextBase.ELEMENT_ARRAY_BUFFER, ibo);
                GraphicsExtensions.CheckGLError();
                gl.BufferData(WebGL2RenderingContextBase.ELEMENT_ARRAY_BUFFER, 0, _isDynamic ? WebGL2RenderingContextBase.STREAM_DRAW : WebGL2RenderingContextBase.STATIC_DRAW);
                GraphicsExtensions.CheckGLError();
            }
        }

        private void PlatformGetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount) where T : struct
        {
            // Buffers are write-only on OpenGL ES 1.1 and 2.0.  See the GL_OES_mapbuffer extension for more information.
            // http://www.khronos.org/registry/gles/extensions/OES/OES_mapbuffer.txt
            throw new NotSupportedException("Index buffers are write-only on OpenGL ES platforms");
        }

        private void PlatformSetDataInternal<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, SetDataOptions options) where T : struct
        {
            BufferData(offsetInBytes, data, startIndex, elementCount, options);
        }

        private void BufferData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, SetDataOptions options) where T : struct
        {
            GenerateIfRequired();
            
            var elementSizeInByte = (IndexElementSize == IndexElementSize.SixteenBits ? 2 : 4);
            var bufferSize = IndexCount * elementSizeInByte;
            
            gl.BindBuffer(WebGL2RenderingContextBase.ELEMENT_ARRAY_BUFFER, ibo);
            GraphicsExtensions.CheckGLError();
            
            if (options == SetDataOptions.Discard)
            {
                // By assigning NULL data to the buffer this gives a hint
                // to the device to discard the previous content.
                gl.BufferData(WebGL2RenderingContextBase.ELEMENT_ARRAY_BUFFER, 0, _isDynamic ? WebGL2RenderingContextBase.STREAM_DRAW : WebGL2RenderingContextBase.STATIC_DRAW);
                GraphicsExtensions.CheckGLError();
            }
            
            if (elementSizeInByte == 2)
            {
                gl.BufferSubData(WebGL2RenderingContextBase.ELEMENT_ARRAY_BUFFER, (uint)offsetInBytes, data);
            }
            else
            {
                gl.BufferSubData(WebGL2RenderingContextBase.ELEMENT_ARRAY_BUFFER, (uint)offsetInBytes, data);
            }

            GraphicsExtensions.CheckGLError();
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
                GraphicsDevice.DisposeBuffer(ibo);
            base.Dispose(disposing);
        }
	}
}
