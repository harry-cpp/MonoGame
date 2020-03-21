// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using WebGLDotNET;
using static WebHelper;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class DepthStencilState
    {
        internal void PlatformApplyState(GraphicsDevice device, bool force = false)
        {
            if (force || this.DepthBufferEnable != device._lastDepthStencilState.DepthBufferEnable)
            {
                if (!DepthBufferEnable)
                {
                    gl.Disable(WebGL2RenderingContextBase.DEPTH_TEST);
                    GraphicsExtensions.CheckGLError();
                }
                else
                {
                    // enable Depth Buffer
                    gl.Enable(WebGL2RenderingContextBase.DEPTH_TEST);
                    GraphicsExtensions.CheckGLError();
                }
                device._lastDepthStencilState.DepthBufferEnable = this.DepthBufferEnable;
            }

            if (force || this.DepthBufferFunction != device._lastDepthStencilState.DepthBufferFunction)
            {
                gl.DepthFunc(DepthBufferFunction.GetDepthFunction());
                GraphicsExtensions.CheckGLError();
                device._lastDepthStencilState.DepthBufferFunction = this.DepthBufferFunction;
            }

            if (force || this.DepthBufferWriteEnable != device._lastDepthStencilState.DepthBufferWriteEnable)
            {
                gl.DepthMask(DepthBufferWriteEnable);
                GraphicsExtensions.CheckGLError();
                device._lastDepthStencilState.DepthBufferWriteEnable = this.DepthBufferWriteEnable;
            }

            if (force || this.StencilEnable != device._lastDepthStencilState.StencilEnable)
            {
                if (!StencilEnable)
                {
                    gl.Disable(WebGL2RenderingContextBase.STENCIL_TEST);
                    GraphicsExtensions.CheckGLError();
                }
                else
                {
                    // enable Stencil
                    gl.Enable(WebGL2RenderingContextBase.STENCIL_TEST);
                    GraphicsExtensions.CheckGLError();
                }
                device._lastDepthStencilState.StencilEnable = this.StencilEnable;
            }

            // set function
            if (this.TwoSidedStencilMode)
            {
                var cullFaceModeFront = WebGL2RenderingContextBase.FRONT;
                var cullFaceModeBack = WebGL2RenderingContextBase.BACK;
                var stencilFaceFront = WebGL2RenderingContextBase.FRONT;
                var stencilFaceBack = WebGL2RenderingContextBase.BACK;

                if (force ||
					this.TwoSidedStencilMode != device._lastDepthStencilState.TwoSidedStencilMode ||
					this.StencilFunction != device._lastDepthStencilState.StencilFunction ||
					this.ReferenceStencil != device._lastDepthStencilState.ReferenceStencil ||
					this.StencilMask != device._lastDepthStencilState.StencilMask)
				{
                    gl.StencilFuncSeparate(cullFaceModeFront, GetStencilFunc(this.StencilFunction),
                                           this.ReferenceStencil, (uint)this.StencilMask);
                    GraphicsExtensions.CheckGLError();
                    device._lastDepthStencilState.StencilFunction = this.StencilFunction;
                    device._lastDepthStencilState.ReferenceStencil = this.ReferenceStencil;
                    device._lastDepthStencilState.StencilMask = this.StencilMask;
                }

                if (force ||
                    this.TwoSidedStencilMode != device._lastDepthStencilState.TwoSidedStencilMode ||
                    this.CounterClockwiseStencilFunction != device._lastDepthStencilState.CounterClockwiseStencilFunction ||
                    this.ReferenceStencil != device._lastDepthStencilState.ReferenceStencil ||
                    this.StencilMask != device._lastDepthStencilState.StencilMask)
			    {
                    gl.StencilFuncSeparate(cullFaceModeBack, GetStencilFunc(this.CounterClockwiseStencilFunction),
                                           this.ReferenceStencil, (uint)this.StencilMask);
                    GraphicsExtensions.CheckGLError();
                    device._lastDepthStencilState.CounterClockwiseStencilFunction = this.CounterClockwiseStencilFunction;
                    device._lastDepthStencilState.ReferenceStencil = this.ReferenceStencil;
                    device._lastDepthStencilState.StencilMask = this.StencilMask;
                }

                
                if (force ||
					this.TwoSidedStencilMode != device._lastDepthStencilState.TwoSidedStencilMode ||
					this.StencilFail != device._lastDepthStencilState.StencilFail ||
					this.StencilDepthBufferFail != device._lastDepthStencilState.StencilDepthBufferFail ||
					this.StencilPass != device._lastDepthStencilState.StencilPass)
                {
                    gl.StencilOpSeparate(stencilFaceFront, GetStencilOp(this.StencilFail),
                                         GetStencilOp(this.StencilDepthBufferFail),
                                         GetStencilOp(this.StencilPass));
                    GraphicsExtensions.CheckGLError();
                    device._lastDepthStencilState.StencilFail = this.StencilFail;
                    device._lastDepthStencilState.StencilDepthBufferFail = this.StencilDepthBufferFail;
                    device._lastDepthStencilState.StencilPass = this.StencilPass;
                }

                if (force ||
                    this.TwoSidedStencilMode != device._lastDepthStencilState.TwoSidedStencilMode ||
                    this.CounterClockwiseStencilFail != device._lastDepthStencilState.CounterClockwiseStencilFail ||
                    this.CounterClockwiseStencilDepthBufferFail != device._lastDepthStencilState.CounterClockwiseStencilDepthBufferFail ||
                    this.CounterClockwiseStencilPass != device._lastDepthStencilState.CounterClockwiseStencilPass)
			    {
                    gl.StencilOpSeparate(stencilFaceBack, GetStencilOp(this.CounterClockwiseStencilFail),
                                         GetStencilOp(this.CounterClockwiseStencilDepthBufferFail),
                                         GetStencilOp(this.CounterClockwiseStencilPass));
                    GraphicsExtensions.CheckGLError();
                    device._lastDepthStencilState.CounterClockwiseStencilFail = this.CounterClockwiseStencilFail;
                    device._lastDepthStencilState.CounterClockwiseStencilDepthBufferFail = this.CounterClockwiseStencilDepthBufferFail;
                    device._lastDepthStencilState.CounterClockwiseStencilPass = this.CounterClockwiseStencilPass;
                }
            }
            else
            {
                if (force ||
					this.TwoSidedStencilMode != device._lastDepthStencilState.TwoSidedStencilMode ||
					this.StencilFunction != device._lastDepthStencilState.StencilFunction ||
					this.ReferenceStencil != device._lastDepthStencilState.ReferenceStencil ||
					this.StencilMask != device._lastDepthStencilState.StencilMask)
				{
                    gl.StencilFunc(GetStencilFunc(this.StencilFunction), ReferenceStencil, (uint)StencilMask);
                    GraphicsExtensions.CheckGLError();
                    device._lastDepthStencilState.StencilFunction = this.StencilFunction;
                    device._lastDepthStencilState.ReferenceStencil = this.ReferenceStencil;
                    device._lastDepthStencilState.StencilMask = this.StencilMask;
                }

                if (force ||
                    this.TwoSidedStencilMode != device._lastDepthStencilState.TwoSidedStencilMode ||
                    this.StencilFail != device._lastDepthStencilState.StencilFail ||
                    this.StencilDepthBufferFail != device._lastDepthStencilState.StencilDepthBufferFail ||
                    this.StencilPass != device._lastDepthStencilState.StencilPass)
                {
                    gl.StencilOp(GetStencilOp(StencilFail),
                                 GetStencilOp(StencilDepthBufferFail),
                                 GetStencilOp(StencilPass));
                    GraphicsExtensions.CheckGLError();
                    device._lastDepthStencilState.StencilFail = this.StencilFail;
                    device._lastDepthStencilState.StencilDepthBufferFail = this.StencilDepthBufferFail;
                    device._lastDepthStencilState.StencilPass = this.StencilPass;
                }
            }

            device._lastDepthStencilState.TwoSidedStencilMode = this.TwoSidedStencilMode;

            if (force || this.StencilWriteMask != device._lastDepthStencilState.StencilWriteMask)
            {
                gl.StencilMask((uint)this.StencilWriteMask);
                GraphicsExtensions.CheckGLError();
                device._lastDepthStencilState.StencilWriteMask = this.StencilWriteMask;
            }
        }

        private static uint GetStencilFunc(CompareFunction function)
        {
            switch (function)
            {
                case CompareFunction.Always: 
                    return WebGL2RenderingContextBase.ALWAYS;
                case CompareFunction.Equal:
                    return WebGL2RenderingContextBase.EQUAL;
                case CompareFunction.Greater:
                    return WebGL2RenderingContextBase.GREATER;
                case CompareFunction.GreaterEqual:
                    return WebGL2RenderingContextBase.GEQUAL;
                case CompareFunction.Less:
                    return WebGL2RenderingContextBase.LESS;
                case CompareFunction.LessEqual:
                    return WebGL2RenderingContextBase.LEQUAL;
                case CompareFunction.Never:
                    return WebGL2RenderingContextBase.NEVER;
                case CompareFunction.NotEqual:
                    return WebGL2RenderingContextBase.NOTEQUAL;
                default:
                    return WebGL2RenderingContextBase.ALWAYS;
            }
        }

        private static uint GetStencilOp(StencilOperation operation)
        {
            switch (operation)
            {
                case StencilOperation.Keep:
                    return WebGL2RenderingContextBase.KEEP;
                case StencilOperation.Decrement:
                    return WebGL2RenderingContextBase.DECR_WRAP;
                case StencilOperation.DecrementSaturation:
                    return WebGL2RenderingContextBase.DECR;
                case StencilOperation.IncrementSaturation:
                    return WebGL2RenderingContextBase.INCR;
                case StencilOperation.Increment:
                    return WebGL2RenderingContextBase.INCR_WRAP;
                case StencilOperation.Invert:
                    return WebGL2RenderingContextBase.INVERT;
                case StencilOperation.Replace:
                    return WebGL2RenderingContextBase.REPLACE;
                case StencilOperation.Zero:
                    return WebGL2RenderingContextBase.ZERO;
                default:
                    return WebGL2RenderingContextBase.KEEP;
            }
        }
    }
}

