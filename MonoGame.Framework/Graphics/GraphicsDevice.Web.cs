// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;
using System.Diagnostics;
using System.Linq;
using static Retyped.dom;
using static WebHelper;
using static Retyped.es5;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class GraphicsDevice
    {
        enum ResourceType
        {
            Texture,
            Buffer,
            Shader,
            Program,
            Query,
            Framebuffer
        }

        struct ResourceHandle
        {
            public ResourceType type;
            public object handle;

            public static ResourceHandle Texture(object handle)
            {
                return new ResourceHandle() { type = ResourceType.Texture, handle = handle };
            }

            public static ResourceHandle Buffer(object handle)
            {
                return new ResourceHandle() { type = ResourceType.Buffer, handle = handle };
            }

            public static ResourceHandle Shader(object handle)
            {
                return new ResourceHandle() { type = ResourceType.Shader, handle = handle };
            }

            public static ResourceHandle Program(object handle)
            {
                return new ResourceHandle() { type = ResourceType.Program, handle = handle };
            }

            public static ResourceHandle Query(object handle)
            {
                return new ResourceHandle() { type = ResourceType.Query, handle = handle };
            }

            public static ResourceHandle Framebuffer(object handle)
            {
                return new ResourceHandle() { type = ResourceType.Framebuffer, handle = handle };
            }

            public void Free()
            {
                switch (type)
                {
                    case ResourceType.Texture:
                        gl.deleteTexture(handle.As<WebGLTexture>());
                        break;
                    case ResourceType.Buffer:
                        gl.deleteBuffer(handle.As<WebGLBuffer>());
                        break;
                    case ResourceType.Shader:
                        if (gl.isShader(handle.As<WebGLShader>()))
                            gl.deleteShader(handle.As<WebGLShader>());
                        break;
                    case ResourceType.Program:
                        if (gl.isProgram(handle.As<WebGLProgram>()))
                            gl.deleteProgram(handle.As<WebGLProgram>());
                        break;
                    case ResourceType.Framebuffer:
                        gl.deleteFramebuffer(handle.As<WebGLFramebuffer>());
                        break;
                }
                GraphicsExtensions.CheckGLError();
            }
        }

        List<ResourceHandle> _disposeThisFrame = new List<ResourceHandle>();
        List<ResourceHandle> _disposeNextFrame = new List<ResourceHandle>();
        object _disposeActionsLock = new object();

        static List<IntPtr> _disposeContexts = new List<IntPtr>();
        static object _disposeContextsLock = new object();

        private ShaderProgramCache _programCache;
        private ShaderProgram _shaderProgram = null;

        static readonly float[] _posFixup = new float[4];

        private static BufferBindingInfo[] _bufferBindingInfos;
        private static bool[] _newEnabledVertexAttributes;
        internal static readonly Dictionary<int, bool> _enabledVertexAttributes = new Dictionary<int, bool>();
        internal static bool _attribsDirty;

        internal FramebufferHelper framebufferHelper;

        internal WebGLFramebuffer glFramebuffer = null;
        internal int MaxVertexAttributes;
        internal int _maxTextureSize = 0;

        // Keeps track of last applied state to avoid redundant OpenGL calls
        internal bool _lastBlendEnable = false;
        internal BlendState _lastBlendState = new BlendState();
        internal DepthStencilState _lastDepthStencilState = new DepthStencilState();
        internal RasterizerState _lastRasterizerState = new RasterizerState();
        private Vector4 _lastClearColor = Vector4.Zero;
        private float _lastClearDepth = 1.0f;
        private int _lastClearStencil = 0;
        private DepthStencilState clearDepthStencilState = new DepthStencilState { StencilEnable = true };
        
        private double[] _drawBuffers;
        private WEBGL_draw_buffers _drawBuffersExtension;

        // Get a hashed value based on the currently bound shaders
        // throws an exception if no shaders are bound
        private int ShaderProgramHash
        {
            get
            {
                if (_vertexShader == null && _pixelShader == null)
                    throw new InvalidOperationException("There is no shader bound!");
                if (_vertexShader == null)
                    return _pixelShader.HashKey;
                if (_pixelShader == null)
                    return _vertexShader.HashKey;
                return _vertexShader.HashKey ^ _pixelShader.HashKey;
            }
        }

        internal void SetVertexAttributeArray(bool[] attrs)
        {
            for (var x = 0; x < attrs.Length; x++)
            {
                if (attrs[x] && !_enabledVertexAttributes.ContainsKey(x))
                {
                    _enabledVertexAttributes[x] = true;
                    gl.enableVertexAttribArray(x);
                    GraphicsExtensions.CheckGLError();
                }
                else if (!attrs[x] && _enabledVertexAttributes.ContainsKey(x))
                {
                    _enabledVertexAttributes.Remove(x);
                    gl.disableVertexAttribArray(x);
                    GraphicsExtensions.CheckGLError();
                }
            }
        }

        private void PlatformSetup()
        {
            _programCache = new ShaderProgramCache(this);

            MaxTextureSlots = (int)gl.getParameter(gl.MAX_TEXTURE_IMAGE_UNITS);
            GraphicsExtensions.CheckGLError();

            _maxTextureSize = (int)gl.getParameter(gl.MAX_TEXTURE_SIZE);
            GraphicsExtensions.CheckGLError();

            MaxVertexAttributes = (int)gl.getParameter(gl.MAX_VERTEX_ATTRIBS);
            GraphicsExtensions.CheckGLError();

            _maxVertexBufferSlots = MaxVertexAttributes;
            _newEnabledVertexAttributes = new bool[MaxVertexAttributes];

            if (gl.getSupportedExtensions().Contains("WEBGL_draw_buffers"))
            {
                _drawBuffersExtension = gl.getExtension("WEBGL_draw_buffers").As<WEBGL_draw_buffers>();
                var maxDrawBuffers = _drawBuffersExtension.MAX_DRAW_BUFFERS_WEBGL.As<int>();

                _drawBuffers = new double[maxDrawBuffers];
                for (int i = 0; i < maxDrawBuffers; i++)
                    _drawBuffers[i] = _drawBuffersExtension.COLOR_ATTACHMENT0_WEBGL + i;
            }
        }

        private void PlatformInitialize()
        {
            _viewport = new Viewport(0, 0, PresentationParameters.BackBufferWidth, PresentationParameters.BackBufferHeight);

            // Ensure the vertex attributes are reset
            _enabledVertexAttributes.Clear();

            // Free all the cached shader programs. 
            _programCache.Clear();
            _shaderProgram = null;

            framebufferHelper = FramebufferHelper.Create(this);

            // Force resetting states
            this.PlatformApplyBlend(true);
            this.DepthStencilState.PlatformApplyState(this, true);
            this.RasterizerState.PlatformApplyState(this, true);

            _bufferBindingInfos = new BufferBindingInfo[_maxVertexBufferSlots];
            for (int i = 0; i < _bufferBindingInfos.Length; i++)
                _bufferBindingInfos[i] = new BufferBindingInfo(null, 0, 0, -1);
        }

        public void PlatformClear(ClearOptions options, Vector4 color, float depth, int stencil)
        {
            // TODO: We need to figure out how to detect if we have a
            // depth stencil buffer or not, and clear options relating
            // to them if not attached.

            // Unlike with XNA and DirectX...  GL.Clear() obeys several
            // different render states:
            //
            //  - The color write flags.
            //  - The scissor rectangle.
            //  - The depth/stencil state.
            //
            // So overwrite these states with what is needed to perform
            // the clear correctly and restore it afterwards.
            //
		    var prevScissorRect = ScissorRectangle;
		    var prevDepthStencilState = DepthStencilState;
            var prevBlendState = BlendState;
            ScissorRectangle = _viewport.Bounds;
            // DepthStencilState.Default has the Stencil Test disabled; 
            // make sure stencil test is enabled before we clear since
            // some drivers won't clear with stencil test disabled
            DepthStencilState = this.clearDepthStencilState;
		    BlendState = BlendState.Opaque;
            ApplyState(false);

            int bufferMask = 0;
            if ((options & ClearOptions.Target) == ClearOptions.Target)
            {
                if (color != _lastClearColor)
                {
                    gl.clearColor(color.X, color.Y, color.Z, color.W);
                    GraphicsExtensions.CheckGLError();
                    _lastClearColor = color;
                }

                bufferMask = bufferMask | (int)gl.COLOR_BUFFER_BIT;
            }
			if ((options & ClearOptions.Stencil) == ClearOptions.Stencil)
            {
                if (stencil != _lastClearStencil)
                {
				    gl.clearStencil(stencil);
                    GraphicsExtensions.CheckGLError();
                    _lastClearStencil = stencil;
                }
                bufferMask = bufferMask | (int)gl.STENCIL_BUFFER_BIT;
			}

			if ((options & ClearOptions.DepthBuffer) == ClearOptions.DepthBuffer) 
            {
                if (depth != _lastClearDepth)
                {
                    gl.clearDepth(depth);
                    GraphicsExtensions.CheckGLError();
                    _lastClearDepth = depth;
                }
				bufferMask = bufferMask | (int)gl.DEPTH_BUFFER_BIT;
			}

            gl.clear(bufferMask);
            GraphicsExtensions.CheckGLError();
           		
            // Restore the previous render state.
		    ScissorRectangle = prevScissorRect;
		    DepthStencilState = prevDepthStencilState;
		    BlendState = prevBlendState;
        }

        private void PlatformDispose()
        {
            _programCache.Dispose();
        }

        internal void DisposeTexture(WebGLTexture handle)
        {
            if (!_isDisposed)
            {
                lock (_disposeActionsLock)
                {
                    _disposeNextFrame.Add(ResourceHandle.Texture(handle));
                }
            }
        }

        internal void DisposeBuffer(WebGLBuffer handle)
        {
            if (!_isDisposed)
            {
                lock (_disposeActionsLock)
                {
                    _disposeNextFrame.Add(ResourceHandle.Buffer(handle));
                }
            }
        }

        internal void DisposeShader(WebGLShader handle)
        {
            if (!_isDisposed)
            {
                lock (_disposeActionsLock)
                {
                    _disposeNextFrame.Add(ResourceHandle.Shader(handle));
                }
            }
        }

        internal void DisposeProgram(WebGLProgram handle)
        {
            if (!_isDisposed)
            {
                lock (_disposeActionsLock)
                {
                    _disposeNextFrame.Add(ResourceHandle.Program(handle));
                }
            }
        }

        internal void DisposeFramebuffer(WebGLFramebuffer handle)
        {
            if (!_isDisposed)
            {
                lock (_disposeActionsLock)
                {
                    _disposeNextFrame.Add(ResourceHandle.Framebuffer(handle));
                }
            }
        }

        public void PlatformPresent()
        {
            // Dispose of any GL resources that were disposed in another thread
            int count = _disposeThisFrame.Count;
            for (int i = 0; i < count; ++i)
                _disposeThisFrame[i].Free();
            _disposeThisFrame.Clear();

            lock (_disposeActionsLock)
            {
                // Swap lists so resources added during this draw will be released after the next draw
                var temp = _disposeThisFrame;
                _disposeThisFrame = _disposeNextFrame;
                _disposeNextFrame = temp;
            }
        }

        private void PlatformSetViewport(ref Viewport value)
        {
            if (IsRenderTargetBound)
                gl.viewport(value.X, value.Y, value.Width, value.Height);
            else
                gl.viewport(value.X, PresentationParameters.BackBufferHeight - value.Y - value.Height, value.Width, value.Height);
            GraphicsExtensions.LogGLError("GraphicsDevice.Viewport_set() GL.Viewport");

            gl.depthRange(value.MinDepth, value.MaxDepth);
            GraphicsExtensions.LogGLError("GraphicsDevice.Viewport_set() GL.DepthRange");
                
            // In OpenGL we have to re-apply the special "posFixup"
            // vertex shader uniform if the viewport changes.
            _vertexShaderDirty = true;
        }

        private void PlatformApplyDefaultRenderTarget()
        {
            this.framebufferHelper.BindFramebuffer(this.glFramebuffer);

            // Reset the raster state because we flip vertices
            // when rendering offscreen and hence the cull direction.
            _rasterizerStateDirty = true;

            // Textures will need to be rebound to render correctly in the new render target.
            Textures.Dirty();
        }

        private class RenderTargetBindingArrayComparer : IEqualityComparer<RenderTargetBinding[]>
        {
            public bool Equals(RenderTargetBinding[] first, RenderTargetBinding[] second)
            {
                if (object.ReferenceEquals(first, second))
                    return true;

                if (first == null || second == null)
                    return false;

                if (first.Length != second.Length)
                    return false;

                for (var i = 0; i < first.Length; ++i)
                {
                    if ((first[i].RenderTarget != second[i].RenderTarget) || (first[i].ArraySlice != second[i].ArraySlice))
                    {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(RenderTargetBinding[] array)
            {
                if (array != null)
                {
                    unchecked
                    {
                        int hash = 17;
                        foreach (var item in array)
                        {
                            if (item.RenderTarget != null)
                                hash = hash * 23 + item.RenderTarget.GetHashCode();
                            hash = hash * 23 + item.ArraySlice.GetHashCode();
                        }
                        return hash;
                    }
                }
                return 0;
            }
        }

        // FBO cache, we create 1 FBO per RenderTargetBinding combination
        private Dictionary<RenderTargetBinding[], WebGLFramebuffer> glFramebuffers = new Dictionary<RenderTargetBinding[], WebGLFramebuffer>(new RenderTargetBindingArrayComparer());
        // FBO cache used to resolve MSAA rendertargets, we create 1 FBO per RenderTargetBinding combination
        private Dictionary<RenderTargetBinding[], WebGLFramebuffer> glResolveFramebuffers = new Dictionary<RenderTargetBinding[], WebGLFramebuffer>(new RenderTargetBindingArrayComparer());

        internal void PlatformCreateRenderTarget(IRenderTarget renderTarget, int width, int height, bool mipMap, SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage)
        {
            WebGLRenderbuffer color = null;
            WebGLRenderbuffer depth = null;
            WebGLRenderbuffer stencil = null;
            
            this.framebufferHelper.GenRenderbuffer(out color);
            this.framebufferHelper.BindRenderbuffer(color);
            this.framebufferHelper.RenderbufferStorageMultisample(preferredMultiSampleCount, gl.RGBA4, width, height);

            if (preferredDepthFormat != DepthFormat.None)
            {
                var depthInternalFormat = gl.DEPTH_COMPONENT16;
                var stencilInternalFormat = gl.NEVER;

                if (depthInternalFormat != 0)
                {
                    this.framebufferHelper.GenRenderbuffer(out depth);
                    this.framebufferHelper.BindRenderbuffer(depth);
                    this.framebufferHelper.RenderbufferStorageMultisample(preferredMultiSampleCount, depthInternalFormat, width, height);
                    if (preferredDepthFormat == DepthFormat.Depth24Stencil8)
                    {
                        stencil = depth;
                        if (stencilInternalFormat != 0)
                        {
                            this.framebufferHelper.GenRenderbuffer(out stencil);
                            this.framebufferHelper.BindRenderbuffer(stencil);
                            this.framebufferHelper.RenderbufferStorageMultisample(preferredMultiSampleCount, stencilInternalFormat, width, height);
                        }
                    }
                }
            }

            renderTarget.GLColorBuffer = color;
            renderTarget.GLDepthBuffer = depth;
            renderTarget.GLStencilBuffer = stencil;
        }

        internal void PlatformDeleteRenderTarget(IRenderTarget renderTarget)
        {
            var color = renderTarget.GLColorBuffer;
            var depth = renderTarget.GLDepthBuffer;
            var stencil = renderTarget.GLStencilBuffer;

            if (color != null)
            {
                this.framebufferHelper.DeleteRenderbuffer(color);

                if (stencil != null && stencil != depth)
                    this.framebufferHelper.DeleteRenderbuffer(stencil);
                if (depth != null)
                    this.framebufferHelper.DeleteRenderbuffer(depth);

                var bindingsToDelete = new List<RenderTargetBinding[]>();
                foreach (var bindings in this.glFramebuffers.Keys)
                {
                    foreach (var binding in bindings)
                    {
                        if (binding.RenderTarget == renderTarget)
                        {
                            bindingsToDelete.Add(bindings);
                            break;
                        }
                    }
                }

                foreach (var bindings in bindingsToDelete)
                {
                    WebGLFramebuffer fbo = null;
                    if (this.glFramebuffers.TryGetValue(bindings, out fbo))
                    {
                        this.framebufferHelper.DeleteFramebuffer(fbo);
                        this.glFramebuffers.Remove(bindings);
                    }
                    if (this.glResolveFramebuffers.TryGetValue(bindings, out fbo))
                    {
                        this.framebufferHelper.DeleteFramebuffer(fbo);
                        this.glResolveFramebuffers.Remove(bindings);
                    }
                }
            }
        }

        internal void PlatformResolveRenderTargets()
        {
            if (this._currentRenderTargetCount == 0)
                return;

            var renderTargetBinding = this._currentRenderTargetBindings[0];
            var renderTarget = renderTargetBinding.RenderTarget as IRenderTarget;
            if (renderTarget.MultiSampleCount > 0 && this.framebufferHelper.SupportsBlitFramebuffer)
            {
                WebGLFramebuffer glResolveFramebuffer = null;
                if (!this.glResolveFramebuffers.TryGetValue(this._currentRenderTargetBindings, out glResolveFramebuffer))
                {
                    this.framebufferHelper.GenFramebuffer(out glResolveFramebuffer);
                    this.framebufferHelper.BindFramebuffer(glResolveFramebuffer);
                    for (var i = 0; i < this._currentRenderTargetCount; ++i)
                    {
                        var rt = this._currentRenderTargetBindings[i].RenderTarget as IRenderTarget;
                        this.framebufferHelper.FramebufferTexture2D((int)(gl.COLOR_ATTACHMENT0 + i), (int) rt.GetFramebufferTarget(renderTargetBinding), rt.GLTexture);
                    }
                    this.glResolveFramebuffers.Add((RenderTargetBinding[])this._currentRenderTargetBindings.Clone(), glResolveFramebuffer);
                }
                else
                {
                    this.framebufferHelper.BindFramebuffer(glResolveFramebuffer);
                }
                // The only fragment operations which affect the resolve are the pixel ownership test, the scissor test, and dithering.
                if (this._lastRasterizerState.ScissorTestEnable)
                {
                    gl.disable(gl.SCISSOR_TEST);
                    GraphicsExtensions.CheckGLError();
                }
                var glFramebuffer = this.glFramebuffers[this._currentRenderTargetBindings];
                this.framebufferHelper.BindReadFramebuffer(glFramebuffer);
                for (var i = 0; i < this._currentRenderTargetCount; ++i)
                {
                    renderTargetBinding = this._currentRenderTargetBindings[i];
                    renderTarget = renderTargetBinding.RenderTarget as IRenderTarget;
                    this.framebufferHelper.BlitFramebuffer(i, renderTarget.Width, renderTarget.Height);
                }
                if (renderTarget.RenderTargetUsage == RenderTargetUsage.DiscardContents && this.framebufferHelper.SupportsInvalidateFramebuffer)
                    this.framebufferHelper.InvalidateReadFramebuffer();
                if (this._lastRasterizerState.ScissorTestEnable)
                {
                    gl.enable(gl.SCISSOR_TEST);
                    GraphicsExtensions.CheckGLError();
                }
            }
            for (var i = 0; i < this._currentRenderTargetCount; ++i)
            {
                renderTargetBinding = this._currentRenderTargetBindings[i];
                renderTarget = renderTargetBinding.RenderTarget as IRenderTarget;
                if (renderTarget.LevelCount > 1)
                {
                    gl.bindTexture(renderTarget.GLTarget, renderTarget.GLTexture);
                    GraphicsExtensions.CheckGLError();
                    this.framebufferHelper.GenerateMipmap((int)renderTarget.GLTarget);
                }
            }
        }

        private IRenderTarget PlatformApplyRenderTargets()
        {
            if (!this.glFramebuffers.TryGetValue(this._currentRenderTargetBindings, out WebGLFramebuffer glFramebuffer))
            {
                this.framebufferHelper.GenFramebuffer(out glFramebuffer);
                this.framebufferHelper.BindFramebuffer(glFramebuffer);
                var renderTargetBinding = this._currentRenderTargetBindings[0];
                var renderTarget = renderTargetBinding.RenderTarget as IRenderTarget;
                this.framebufferHelper.FramebufferRenderbuffer(gl.DEPTH_ATTACHMENT, renderTarget.GLDepthBuffer, 0);
                this.framebufferHelper.FramebufferRenderbuffer(gl.STENCIL_ATTACHMENT, renderTarget.GLStencilBuffer, 0);
                for (var i = 0; i < this._currentRenderTargetCount; ++i)
                {
                    renderTargetBinding = this._currentRenderTargetBindings[i];
                    renderTarget = renderTargetBinding.RenderTarget as IRenderTarget;
                    var attachement = gl.COLOR_ATTACHMENT0 + i;

                    this.framebufferHelper.FramebufferTexture2D(attachement, (int)renderTarget.GetFramebufferTarget(renderTargetBinding), renderTarget.GLTexture, 0, renderTarget.MultiSampleCount);
                }

                this.glFramebuffers.Add((RenderTargetBinding[])_currentRenderTargetBindings.Clone(), glFramebuffer);
            }
            else
            {
                this.framebufferHelper.BindFramebuffer(glFramebuffer);
            }

            if (_drawBuffersExtension != null)
            {
                var tmp = new double[(int)_currentRenderTargetCount];
                for (int i = 0; i < this._currentRenderTargetCount; i++)
                    tmp[i] = _drawBuffers[i];
                _drawBuffersExtension.drawBuffersWEBGL(tmp);
            }

            // Reset the raster state because we flip vertices
            // when rendering offscreen and hence the cull direction.
            _rasterizerStateDirty = true;

            // Textures will need to be rebound to render correctly in the new render target.
            Textures.Dirty();

            return _currentRenderTargetBindings[0].RenderTarget as IRenderTarget;
        }
        
        internal void OnPresentationChanged()
        {
            ApplyRenderTargets(null);
        }

        // Holds information for caching
        private class BufferBindingInfo
        {
            public VertexDeclaration.VertexDeclarationAttributeInfo AttributeInfo;
            public int VertexOffset;
            public int InstanceFrequency;
            public int Vbo;

            public BufferBindingInfo(VertexDeclaration.VertexDeclarationAttributeInfo attributeInfo, int vertexOffset, int instanceFrequency, int vbo)
            {
                AttributeInfo = attributeInfo;
                VertexOffset = vertexOffset;
                InstanceFrequency = instanceFrequency;
                Vbo = vbo;
            }
        }

        private void ActivateShaderProgram()
        {
            // Lookup the shader program.
            var shaderProgram = _programCache.GetProgram(VertexShader, PixelShader);
            if (shaderProgram.Program == null)
                return;
            // Set the new program if it has changed.
            if (_shaderProgram != shaderProgram)
            {
                gl.useProgram(shaderProgram.Program);
                GraphicsExtensions.CheckGLError();
                _shaderProgram = shaderProgram;
            }

            var posFixupLoc = shaderProgram.GetUniformLocation("posFixup");
            if (posFixupLoc == null)
                return;

            // Apply vertex shader fix:
            // The following two lines are appended to the end of vertex shaders
            // to account for rendering differences between OpenGL and DirectX:
            //
            // gl_Position.y = gl_Position.y * posFixup.y;
            // gl_Position.xy += posFixup.zw * gl_Position.ww;
            //
            // (the following paraphrased from wine, wined3d/state.c and wined3d/glsl_shader.c)
            //
            // - We need to flip along the y-axis in case of offscreen rendering.
            // - D3D coordinates refer to pixel centers while GL coordinates refer
            //   to pixel corners.
            // - D3D has a top-left filling convention. We need to maintain this
            //   even after the y-flip mentioned above.
            // In order to handle the last two points, we translate by
            // (63.0 / 128.0) / VPw and (63.0 / 128.0) / VPh. This is equivalent to
            // translating slightly less than half a pixel. We want the difference to
            // be large enough that it doesn't get lost due to rounding inside the
            // driver, but small enough to prevent it from interfering with any
            // anti-aliasing.
            //
            // OpenGL coordinates specify the center of the pixel while d3d coords specify
            // the corner. The offsets are stored in z and w in posFixup. posFixup.y contains
            // 1.0 or -1.0 to turn the rendering upside down for offscreen rendering. PosFixup.x
            // contains 1.0 to allow a mad.

            _posFixup[0] = 1.0f;
            _posFixup[1] = 1.0f;
            _posFixup[2] = (63.0f/64.0f)/Viewport.Width;
            _posFixup[3] = -(63.0f/64.0f)/Viewport.Height;

            //If we have a render target bound (rendering offscreen)
            if (IsRenderTargetBound)
            {
                //flip vertically
                _posFixup[1] *= -1.0f;
                _posFixup[3] *= -1.0f;
            }

            gl.uniform4f(posFixupLoc, _posFixup[0], _posFixup[1], _posFixup[2], _posFixup[3]);
            GraphicsExtensions.CheckGLError();
        }
		
        internal void PlatformBeginApplyState()
        {
        }

        private void PlatformApplyBlend(bool force = false)
        {
            _actualBlendState.PlatformApplyState(this, force);

            if (force || BlendFactor != _lastBlendState.BlendFactor)
            {
                gl.blendColor(
                    this.BlendFactor.R/255.0f,
                    this.BlendFactor.G/255.0f,
                    this.BlendFactor.B/255.0f,
                    this.BlendFactor.A/255.0f);
                GraphicsExtensions.CheckGLError();
                _lastBlendState.BlendFactor = this.BlendFactor;
            }
        }

        internal void PlatformApplyState(bool applyShaders)
        {
            if ( _scissorRectangleDirty )
	        {
                var scissorRect = _scissorRectangle;
                if (!IsRenderTargetBound)
                    scissorRect.Y = PresentationParameters.BackBufferHeight - (scissorRect.Y + scissorRect.Height);
                gl.scissor(scissorRect.X, scissorRect.Y, scissorRect.Width, scissorRect.Height);
                GraphicsExtensions.CheckGLError();
	            _scissorRectangleDirty = false;
	        }

            // If we're not applying shaders then early out now.
            if (!applyShaders)
                return;

            if (_indexBufferDirty)
            {
                if (_indexBuffer != null)
                {
                    gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, _indexBuffer.ibo);
                    GraphicsExtensions.CheckGLError();
                }
                _indexBufferDirty = false;
            }

            if (_vertexShader == null)
                throw new InvalidOperationException("A vertex shader must be set!");
            if (_pixelShader == null)
                throw new InvalidOperationException("A pixel shader must be set!");

            if (_vertexShaderDirty || _pixelShaderDirty)
            {
                ActivateShaderProgram();

                if (_vertexShaderDirty)
                {
                    unchecked
                    {
                        _graphicsMetrics._vertexShaderCount++;
                    }
                }

                if (_pixelShaderDirty)
                {
                    unchecked
                    {
                        _graphicsMetrics._pixelShaderCount++;
                    }
                }

                _vertexShaderDirty = _pixelShaderDirty = false;
            }

            _vertexConstantBuffers.SetConstantBuffers(this, _shaderProgram);
            _pixelConstantBuffers.SetConstantBuffers(this, _shaderProgram);

            Textures.SetTextures(this);
            SamplerStates.PlatformSetSamplers(this);
        }

        private void PlatformDrawIndexedPrimitives(PrimitiveType primitiveType, int baseVertex, int startIndex, int primitiveCount)
        {
            throw new NotImplementedException();
        }

        private void PlatformDrawUserPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, VertexDeclaration vertexDeclaration, int vertexCount) where T : struct
        {
            ApplyState(true);

            var vertexArrayBuffer = ConvertVertices(vertexData, vertexOffset, vertexCount, vertexDeclaration);

            var buffer = gl.createBuffer();
            GraphicsExtensions.CheckGLError();
            gl.bindBuffer(gl.ARRAY_BUFFER, buffer);
            GraphicsExtensions.CheckGLError();
            gl.bufferData(gl.ARRAY_BUFFER, vertexArrayBuffer, gl.STATIC_DRAW);
            GraphicsExtensions.CheckGLError();

            var mode = (uint) GraphicsExtensions.GetPrimitiveTypeGL(primitiveType);
            vertexDeclaration.GraphicsDevice = this;
            vertexDeclaration.Apply(_vertexShader, vertexOffset, ShaderProgramHash);

            GraphicsExtensions.CheckGLError();
            gl.drawArrays(mode, 0, vertexCount);
            GraphicsExtensions.CheckGLError();

            gl.bindBuffer(gl.ARRAY_BUFFER, null);
            GraphicsExtensions.CheckGLError();
            gl.deleteBuffer(buffer);
            GraphicsExtensions.CheckGLError();
        }

        private void PlatformDrawPrimitives(PrimitiveType primitiveType, int vertexStart, int vertexCount)
        {
            throw new NotImplementedException();
        }

        private void PlatformDrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, short[] indexData, int indexOffset, int primitiveCount, VertexDeclaration vertexDeclaration) where T : struct
        {
            ApplyState(true);

            var count = GetElementCountArray(primitiveType, primitiveCount);

            var vertexArrayBuffer = ConvertVertices(vertexData, vertexOffset, numVertices, vertexDeclaration);
            var uintIndexData = new Uint16Array(count.As<uint>());//indexData.As<Uint16Array>();
            for (uint i = 0; i < uintIndexData.length; i++)
                uintIndexData[i] = indexData[indexOffset + i].As<ushort>();

            var vertexBuffer = gl.createBuffer();
            var indexBuffer = gl.createBuffer();

            gl.bindBuffer(gl.ARRAY_BUFFER, vertexBuffer);
            GraphicsExtensions.CheckGLError();
            gl.bufferData(gl.ARRAY_BUFFER, vertexArrayBuffer, gl.STATIC_DRAW);
            GraphicsExtensions.CheckGLError();
            gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, indexBuffer);
            GraphicsExtensions.CheckGLError();
            gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, uintIndexData, gl.STATIC_DRAW);
            GraphicsExtensions.CheckGLError();

            _vertexBuffersDirty = true;
            _indexBufferDirty = true;

            var mode = (uint) GraphicsExtensions.GetPrimitiveTypeGL(primitiveType);
            vertexDeclaration.GraphicsDevice = this;
            vertexDeclaration.Apply(_vertexShader, vertexOffset, ShaderProgramHash);

            GraphicsExtensions.CheckGLError();
            gl.drawElements(mode, count, gl.UNSIGNED_SHORT, 0);
            GraphicsExtensions.CheckGLError();

            gl.bindBuffer(gl.ARRAY_BUFFER, null);
            gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, null);
            gl.deleteBuffer(vertexBuffer);
            gl.deleteBuffer(indexBuffer);
        }

        private void PlatformDrawUserIndexedPrimitives(PrimitiveType primitiveType, ArrayBuffer vertexData, int vertexOffset, int numVertices, short[] indexData, int indexOffset, int primitiveCount, VertexDeclaration vertexDeclaration)
        {
            ApplyState(true);

            var count = GetElementCountArray(primitiveType, primitiveCount);

            var vertexBuffer = gl.createBuffer();
            var indexBuffer = gl.createBuffer();

            gl.bindBuffer(gl.ARRAY_BUFFER, vertexBuffer);
            GraphicsExtensions.CheckGLError();
            gl.bufferData(gl.ARRAY_BUFFER, new Uint8Array(vertexData, (vertexOffset * 4 * 6).As<uint>(), (numVertices * 4 * 6).As<uint>()), gl.STATIC_DRAW);
            GraphicsExtensions.CheckGLError();
            gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, indexBuffer);
            GraphicsExtensions.CheckGLError();
            gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, indexData.As<Int16Array>().subarray(indexOffset.As<uint>()), gl.STATIC_DRAW);
            GraphicsExtensions.CheckGLError();

            _vertexBuffersDirty = true;
            _indexBufferDirty = true;

            var mode = GraphicsExtensions.GetPrimitiveTypeGL(primitiveType);
            vertexDeclaration.GraphicsDevice = this;
            vertexDeclaration.Apply(_vertexShader, vertexOffset, ShaderProgramHash);

            GraphicsExtensions.CheckGLError();
            gl.drawElements(mode, count, gl.UNSIGNED_SHORT, 0);
            GraphicsExtensions.CheckGLError();

            gl.bindBuffer(gl.ARRAY_BUFFER, null);
            gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, null);
            gl.deleteBuffer(vertexBuffer);
            gl.deleteBuffer(indexBuffer);
        }

        private void PlatformDrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, int[] indexData, int indexOffset, int primitiveCount, VertexDeclaration vertexDeclaration) where T : struct
        {
            throw new NotImplementedException();
        }

        private void PlatformDrawInstancedPrimitives(PrimitiveType primitiveType, int baseVertex, int startIndex, int primitiveCount, int instanceCount)
        {
            throw new NotImplementedException();
        }

        private void PlatformGetBackBufferData<T>(Rectangle? rect, T[] data, int startIndex, int count) where T : struct
        {
            throw new NotImplementedException();
        }

        private static Rectangle PlatformGetTitleSafeArea(int x, int y, int width, int height)
        {
            return new Rectangle(x, y, width, height);
        }
        
        internal void PlatformSetMultiSamplingToMaximum(PresentationParameters presentationParameters, out int quality)
        {
            presentationParameters.MultiSampleCount = 4;
            quality = 0;
        }

        private ArrayBuffer ConvertVertices<T>(T[] vertices, int startVertex, int numVertices, VertexDeclaration decl)
        {
            var result =  new ArrayBuffer(numVertices * decl.VertexStride);
            var fa = new Float32Array(result);
            var fi = new Uint32Array(result);
            uint pos = 0;

            if (vertices[0] is VertexPositionColorTexture) // hardcoding types gives us a bit of performance
            {
                var tmp = vertices.As<VertexPositionColorTexture[]>();
                for (uint i = 0; i < numVertices; i++)
                {
                    fa[pos + 0] = tmp[i].Position.X;
                    fa[pos + 1] = tmp[i].Position.Y;
                    fa[pos + 2] = tmp[i].Position.Z;
                    fi[pos + 3] = tmp[i].Color.PackedValue;
                    fa[pos + 4] = tmp[i].TextureCoordinate.X;
                    fa[pos + 5] = tmp[i].TextureCoordinate.Y;

                    pos += 6;
                }
            }
            else
            {
                var props = object.GetOwnPropertyNames(vertices[0]).Where(p => p[0] != '$').ToArray();
                var propertyIndex = 0;

                foreach (var el in decl.InternalVertexElements)
                {
                    var prop = props[propertyIndex++];
                    CopyVertexElement(vertices, startVertex, numVertices, result, prop, el.Offset, el.VertexElementFormat, decl.VertexStride);
                }
            }

            return result;
        }

        private void CopyVertexElement<T>(T[] vertices, int startVertex, int numVertices, ArrayBuffer buf, string property, int offset, VertexElementFormat format, int stride)
        {
            // Note that this implementation assumes vertex element format matches the actual struct implementation
            // E.g. Vector4 element must map to a Vector4 field
            // TODO: This could be made more robust by trying to guess how many bytes each property is
            // E.g. if we have Short4 format and 2 fields, assume both are 32-bit instead of 4 16-bit fields
            switch (format)
            {
                case VertexElementFormat.Byte4:
                    // assumes struct with 4 byte fields
                    var props = object.GetOwnPropertyNames(vertices[0][property]).Where(p => p[0] != '$').ToArray();
                    for (var i = 0; i < numVertices; i++)
                    {
                        var byte4 = vertices[startVertex + i][property];
                        var startIndex = offset + i * stride;
                        var byteView = new Uint8Array(buf, (uint) startIndex);
                        byteView[0] = (byte) byte4[props[0]];
                        byteView[1] = (byte) byte4[props[1]];
                        byteView[2] = (byte) byte4[props[2]];
                        byteView[3] = (byte) byte4[props[3]];
                    }
                    break;
                case VertexElementFormat.Color:
                    // assumes struct field with uint field
                    props = object.GetOwnPropertyNames(vertices[0][property]).Where(p => p[0] != '$').ToArray();
                    for (var i = 0; i < numVertices; i++)
                    {
                        var color = vertices[startVertex + i][property];
                        var startIndex = offset + i * stride;
                        var uintView = new Uint32Array(buf, (uint) startIndex);
                        uintView[0] = (uint) color[props[0]];
                    }
                    break;
                case VertexElementFormat.HalfVector2:
                case VertexElementFormat.HalfVector4:
                    // TODO
                    throw new NotImplementedException();
                case VertexElementFormat.NormalizedShort2:
                case VertexElementFormat.Short2:
                case VertexElementFormat.NormalizedShort4:
                case VertexElementFormat.Short4:
                    // assumes struct field with n short fields
                    props = object.GetOwnPropertyNames(vertices[0][property]).Where(p => p[0] != '$').ToArray();
                    for (var i = 0; i < numVertices; i++)
                    {
                        var shorts = vertices[startVertex + i][property];
                        var startIndex = offset + i * stride;
                        var shortView = new Int16Array(buf, (uint) startIndex);
                        for (var ci = 0; ci < props.Length; ci++)
                            shortView[(uint) ci] = (short) shorts[props[ci]];
                    }
                    break;
                case VertexElementFormat.Single:
                    // assumes direct float field
                    props = object.GetOwnPropertyNames(vertices[0][property]).Where(p => p[0] != '$').ToArray();
                    for (var i = 0; i < numVertices; i++)
                    {
                        var floatVal = vertices[startVertex + i][property];
                        var startIndex = offset + i * stride;
                        var floatView = new Float32Array(buf, (uint) startIndex);
                        floatView[0] = (float) floatVal;
                    }
                    break;
                case VertexElementFormat.Vector2:
                case VertexElementFormat.Vector3:
                case VertexElementFormat.Vector4:
                    // assumes struct field with n float fields
                    props = object.GetOwnPropertyNames(vertices[0][property]).Where(p => p[0] != '$').ToArray();
                    for (var i = 0; i < numVertices; i++)
                    {
                        var startIndex = offset + i * stride;
                        var floatView = new Float32Array(buf, (uint) startIndex);
                        var vec = vertices[startVertex + i][property];
                        for (var ci = 0; ci < props.Length; ci++)
                            floatView[(uint) ci] = (float) vec[props[ci]];
                    }

                    break;
            }
        }
    }
}
