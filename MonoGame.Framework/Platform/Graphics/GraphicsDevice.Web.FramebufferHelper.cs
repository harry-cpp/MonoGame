// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using WebGLDotNET;
using static WebHelper;

namespace Microsoft.Xna.Framework.Graphics
{
    // ARB_framebuffer_object implementation
    partial class GraphicsDevice
    {
        internal class FramebufferHelper
        {
            private static FramebufferHelper _instance;

            public static FramebufferHelper Create(GraphicsDevice gd)
            {
                if (gd.GraphicsCapabilities.SupportsFramebufferObjectARB || gd.GraphicsCapabilities.SupportsFramebufferObjectEXT)
                {
                    _instance = new FramebufferHelper(gd);
                }
                else
                {
                    throw new PlatformNotSupportedException(
                        "MonoGame requires either ARB_framebuffer_object or EXT_framebuffer_object." +
                        "Try updating your graphics drivers.");
                }

                return _instance;
            }

            public static FramebufferHelper Get()
            {
                if (_instance == null)
                    throw new InvalidOperationException("The FramebufferHelper has not been created yet!");
                return _instance;
            }

            public bool SupportsInvalidateFramebuffer { get; private set; }

            public bool SupportsBlitFramebuffer { get; private set; }

            internal FramebufferHelper(GraphicsDevice graphicsDevice)
            {
                this.SupportsBlitFramebuffer = true;
                this.SupportsInvalidateFramebuffer = true;
            }

            internal virtual void GenRenderbuffer(out WebGLRenderbuffer renderbuffer)
            {
                renderbuffer = gl.CreateRenderbuffer();
                GraphicsExtensions.CheckGLError();
            }

            internal virtual void BindRenderbuffer(WebGLRenderbuffer renderbuffer)
            {
                gl.BindRenderbuffer(WebGL2RenderingContextBase.RENDERBUFFER, renderbuffer);
                GraphicsExtensions.CheckGLError();
            }

            internal virtual void DeleteRenderbuffer(WebGLRenderbuffer renderbuffer)
            {
                gl.DeleteRenderbuffer(renderbuffer);
                GraphicsExtensions.CheckGLError();
            }

            internal virtual void RenderbufferStorageMultisample(int samples, uint internalFormat, int width, int height)
            {
                gl.RenderbufferStorage(WebGL2RenderingContextBase.RENDERBUFFER, internalFormat, width, height);
                GraphicsExtensions.CheckGLError();
            }

            internal virtual void GenFramebuffer(out WebGLFramebuffer framebuffer)
            {
                framebuffer = gl.CreateFramebuffer();
                GraphicsExtensions.CheckGLError();
            }

            internal virtual void BindFramebuffer(WebGLFramebuffer framebuffer)
            {
                gl.BindFramebuffer(WebGL2RenderingContextBase.FRAMEBUFFER, framebuffer);
                GraphicsExtensions.CheckGLError();
            }

            internal virtual void BindReadFramebuffer(WebGLFramebuffer readFramebuffer)
            {
                gl.BindFramebuffer(WebGL2RenderingContextBase.READ_FRAMEBUFFER, readFramebuffer);
                GraphicsExtensions.CheckGLError();
            }

            static readonly uint[] FramebufferAttachements = {
                WebGL2RenderingContextBase.COLOR_ATTACHMENT0,
                WebGL2RenderingContextBase.DEPTH_ATTACHMENT,
                WebGL2RenderingContextBase.STENCIL_ATTACHMENT,
            };

            internal virtual void InvalidateDrawFramebuffer()
            {
                Debug.Assert(this.SupportsInvalidateFramebuffer);
                gl.InvalidateFramebuffer(WebGL2RenderingContextBase.FRAMEBUFFER, FramebufferAttachements);
            }

            internal virtual void InvalidateReadFramebuffer()
            {
                Debug.Assert(this.SupportsInvalidateFramebuffer);
                gl.InvalidateFramebuffer(WebGL2RenderingContextBase.FRAMEBUFFER, FramebufferAttachements);
            }

            internal virtual void DeleteFramebuffer(WebGLFramebuffer framebuffer)
            {
                gl.DeleteFramebuffer(framebuffer);
                GraphicsExtensions.CheckGLError();
            }

            internal virtual void FramebufferTexture2D(uint attachement, uint target, WebGLTexture texture, int level = 0, double samples = 0)
            {
                gl.FramebufferTexture2D(WebGL2RenderingContextBase.FRAMEBUFFER, attachement, target, texture, level);
                GraphicsExtensions.CheckGLError();
            }

            internal virtual void FramebufferRenderbuffer(uint attachement, WebGLRenderbuffer renderbuffer, double level = 0)
            {
                gl.FramebufferRenderbuffer(WebGL2RenderingContextBase.FRAMEBUFFER, attachement, WebGL2RenderingContextBase.RENDERBUFFER, renderbuffer);
                GraphicsExtensions.CheckGLError();
            }

            internal virtual void GenerateMipmap(uint target)
            {
                gl.GenerateMipmap(target);
                GraphicsExtensions.CheckGLError();

            }

            internal virtual void BlitFramebuffer(uint iColorAttachment, int width, int height)
            {
                gl.ReadBuffer(WebGL2RenderingContextBase.COLOR_ATTACHMENT0 + iColorAttachment);
                GraphicsExtensions.CheckGLError();
                gl.DrawBuffers(new[] { WebGL2RenderingContextBase.COLOR_ATTACHMENT0 + iColorAttachment });
                GraphicsExtensions.CheckGLError();
                gl.BlitFramebuffer(0, 0, width, height, 0, 0, width, height, WebGL2RenderingContextBase.COLOR_BUFFER_BIT, WebGL2RenderingContextBase.NEAREST);
                GraphicsExtensions.CheckGLError();

            }

            /*internal virtual void CheckFramebufferStatus()
            {
                var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
                if (status != FramebufferErrorCode.FramebufferComplete)
                {
                    string message = "Framebuffer Incomplete.";
                    switch (status)
                    {
                        case FramebufferErrorCode.FramebufferIncompleteAttachment: message = "Not all framebuffer attachment points are framebuffer attachment complete."; break;
                        case FramebufferErrorCode.FramebufferIncompleteMissingAttachment: message = "No images are attached to the framebuffer."; break;
                        case FramebufferErrorCode.FramebufferUnsupported: message = "The combination of internal formats of the attached images violates an implementation-dependent set of restrictions."; break;
                        case FramebufferErrorCode.FramebufferIncompleteMultisample: message = "Not all attached images have the same number of samples."; break;
                    }
                    throw new InvalidOperationException(message);
                }
            }*/
        }
    }
}
