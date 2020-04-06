// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using static WebHelper;
using WebGLDotNET;
using System.Diagnostics;

namespace Microsoft.Xna.Framework.Graphics
{
    internal static partial class GraphicsExtensions
    {
        [Conditional("DEBUG")]
        [DebuggerHidden]
        public static void CheckGLError()
        {
            var error = gl.GetError();

            if (error != WebGLRenderingContextBase.NO_ERROR)
                throw new MonoGameGLException("GL.GetError() returned " + error);
        }

        [Conditional("DEBUG")]
        public static void LogGLError(string location)
        {
            try
            {
                GraphicsExtensions.CheckGLError();
            }
            catch (MonoGameGLException ex)
            {
                Debug.WriteLine("MonoGameGLException at " + location + " - " + ex.Message);
            }
        }

        public static int OpenGLNumberOfElements(this VertexElementFormat elementFormat)
        {
            switch (elementFormat)
            {
                case VertexElementFormat.Single:
                    return 1;

                case VertexElementFormat.Vector2:
                    return 2;

                case VertexElementFormat.Vector3:
                    return 3;

                case VertexElementFormat.Vector4:
                    return 4;

                case VertexElementFormat.Color:
                    return 4;

                case VertexElementFormat.Byte4:
                    return 4;

                case VertexElementFormat.Short2:
                    return 2;

                case VertexElementFormat.Short4:
                    return 4;

                case VertexElementFormat.NormalizedShort2:
                    return 2;

                case VertexElementFormat.NormalizedShort4:
                    return 4;

                case VertexElementFormat.HalfVector2:
                    return 2;

                case VertexElementFormat.HalfVector4:
                    return 4;
            }

            throw new ArgumentException();
        }

        public static int OpenGLVertexAttribPointerType(this VertexElementFormat elementFormat)
        {
            switch (elementFormat)
            {
                case VertexElementFormat.Single:
                case VertexElementFormat.Vector2:
                case VertexElementFormat.Vector3:
                case VertexElementFormat.Vector4:
                    return (int)WebGLRenderingContextBase.FLOAT;

                case VertexElementFormat.Color:
                case VertexElementFormat.Byte4:
                    return (int)WebGLRenderingContextBase.UNSIGNED_BYTE;

                case VertexElementFormat.Short2:
                case VertexElementFormat.Short4:
                case VertexElementFormat.NormalizedShort2:
                case VertexElementFormat.NormalizedShort4:
                    return (int)WebGLRenderingContextBase.SHORT;
            }

            throw new ArgumentException();
        }

        public static bool OpenGLVertexAttribNormalized(this VertexElement element)
        {
            // TODO: This may or may not be the right behavor.  
            //
            // For instance the VertexElementFormat.Byte4 format is not supposed
            // to be normalized, but this line makes it so.
            //
            // The question is in MS XNA are types normalized based on usage or
            // normalized based to their format?
            //
            if (element.VertexElementUsage == VertexElementUsage.Color)
                return true;

            switch (element.VertexElementFormat)
            {
                case VertexElementFormat.NormalizedShort2:
                case VertexElementFormat.NormalizedShort4:
                    return true;

                default:
                    return false;
            }
        }

        public static uint GetBlendEquationMode(this BlendFunction function)
        {
            switch (function)
            {
                case BlendFunction.Add:
                    return WebGLRenderingContextBase.FUNC_ADD;
                case BlendFunction.ReverseSubtract:
                    return WebGLRenderingContextBase.FUNC_REVERSE_SUBTRACT;
                case BlendFunction.Subtract:
                    return WebGLRenderingContextBase.FUNC_SUBTRACT;

                default:
                    throw new ArgumentException();
            }
        }

        public static uint GetBlendFactorSrc(this Blend blend)
        {
            switch (blend)
            {
                case Blend.BlendFactor:
                    return WebGLRenderingContextBase.CONSTANT_COLOR;
                case Blend.DestinationAlpha:
                    return WebGLRenderingContextBase.DST_ALPHA;
                case Blend.DestinationColor:
                    return WebGLRenderingContextBase.DST_COLOR;
                case Blend.InverseBlendFactor:
                    return WebGLRenderingContextBase.ONE_MINUS_CONSTANT_COLOR;
                case Blend.InverseDestinationAlpha:
                    return WebGLRenderingContextBase.ONE_MINUS_DST_ALPHA;
                case Blend.InverseDestinationColor:
                    return WebGLRenderingContextBase.ONE_MINUS_DST_COLOR;
                case Blend.InverseSourceAlpha:
                    return WebGLRenderingContextBase.ONE_MINUS_SRC_ALPHA;
                case Blend.InverseSourceColor:
                    return WebGLRenderingContextBase.ONE_MINUS_SRC_COLOR;
                case Blend.One:
                    return WebGLRenderingContextBase.ONE;
                case Blend.SourceAlpha:
                    return WebGLRenderingContextBase.SRC_ALPHA;
                case Blend.SourceAlphaSaturation:
                    return WebGLRenderingContextBase.SRC_ALPHA_SATURATE;
                case Blend.SourceColor:
                    return WebGLRenderingContextBase.SRC_COLOR;
                case Blend.Zero:
                    return WebGLRenderingContextBase.ZERO;
                default:
                    throw new ArgumentOutOfRangeException("blend", "The specified blend function is not implemented.");
            }

        }

        public static uint GetBlendFactorDest(this Blend blend)
        {
            switch (blend)
            {
                case Blend.BlendFactor:
                    return WebGLRenderingContextBase.CONSTANT_COLOR;
                case Blend.DestinationAlpha:
                    return WebGLRenderingContextBase.DST_ALPHA;
                case Blend.DestinationColor:
                    return WebGLRenderingContextBase.DST_COLOR;
                case Blend.InverseBlendFactor:
                    return WebGLRenderingContextBase.ONE_MINUS_CONSTANT_COLOR;
                case Blend.InverseDestinationAlpha:
                    return WebGLRenderingContextBase.ONE_MINUS_DST_ALPHA;
                case Blend.InverseDestinationColor:
                    return WebGLRenderingContextBase.ONE_MINUS_DST_COLOR;
                case Blend.InverseSourceAlpha:
                    return WebGLRenderingContextBase.ONE_MINUS_SRC_ALPHA;
                case Blend.InverseSourceColor:
                    return WebGLRenderingContextBase.ONE_MINUS_SRC_COLOR;
                case Blend.One:
                    return WebGLRenderingContextBase.ONE;
                case Blend.SourceAlpha:
                    return WebGLRenderingContextBase.SRC_ALPHA;
                case Blend.SourceAlphaSaturation:
                    return WebGLRenderingContextBase.SRC_ALPHA_SATURATE;
                case Blend.SourceColor:
                    return WebGLRenderingContextBase.SRC_COLOR;
                case Blend.Zero:
                    return WebGLRenderingContextBase.ZERO;
                default:
                    throw new ArgumentOutOfRangeException("blend", "The specified blend function is not implemented.");
            }
        }

        public static uint GetDepthFunction(this CompareFunction compare)
        {
            switch (compare)
            {
                default:
                case CompareFunction.Always:
                    return WebGLRenderingContextBase.ALWAYS;
                case CompareFunction.Equal:
                    return WebGLRenderingContextBase.EQUAL;
                case CompareFunction.Greater:
                    return WebGLRenderingContextBase.GREATER;
                case CompareFunction.GreaterEqual:
                    return WebGLRenderingContextBase.GEQUAL;
                case CompareFunction.Less:
                    return WebGLRenderingContextBase.LESS;
                case CompareFunction.LessEqual:
                    return WebGLRenderingContextBase.LEQUAL;
                case CompareFunction.Never:
                    return WebGLRenderingContextBase.NEVER;
                case CompareFunction.NotEqual:
                    return WebGLRenderingContextBase.NOTEQUAL;
            }
        }

        public static void GetGLFormat(this SurfaceFormat format,
            GraphicsDevice graphicsDevice,
            out uint glInternalFormat,
            out uint glFormat,
            out uint glType)
        {
            glInternalFormat = (int)WebGLRenderingContextBase.RGBA;
            glFormat = (int)WebGLRenderingContextBase.RGBA;
            glType = (int)WebGLRenderingContextBase.UNSIGNED_BYTE;

            var supportsSRgb = graphicsDevice.GraphicsCapabilities.SupportsSRgb;

            switch (format)
            {
                case SurfaceFormat.Color:
                    glInternalFormat = WebGLRenderingContextBase.RGBA;
                    glFormat = WebGLRenderingContextBase.RGBA;
                    glType = WebGLRenderingContextBase.UNSIGNED_BYTE;
                    break;
                case SurfaceFormat.Bgr565:
                    glInternalFormat = WebGLRenderingContextBase.RGB;
                    glFormat = WebGLRenderingContextBase.RGB;
                    glType = WebGLRenderingContextBase.UNSIGNED_SHORT_5_6_5;
                    break;
                case SurfaceFormat.Bgra4444:
                    glInternalFormat = WebGLRenderingContextBase.RGBA;
                    glFormat = WebGLRenderingContextBase.RGBA;
                    glType = WebGLRenderingContextBase.UNSIGNED_SHORT_4_4_4_4;
                    break;
                case SurfaceFormat.Bgra5551:
                    glInternalFormat = WebGLRenderingContextBase.RGBA;
                    glFormat = WebGLRenderingContextBase.RGBA;
                    glType = WebGLRenderingContextBase.UNSIGNED_SHORT_5_5_5_1;
                    break;
                case SurfaceFormat.Alpha8:
                    glInternalFormat = WebGLRenderingContextBase.LUMINANCE;
                    glFormat = WebGLRenderingContextBase.LUMINANCE;
                    glType = WebGLRenderingContextBase.UNSIGNED_BYTE;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public static int GetPrimitiveTypeGL(PrimitiveType primitiveType)
        {
            switch (primitiveType)
            {
                case PrimitiveType.LineList:
                    return (int)WebGLRenderingContextBase.LINES;
                case PrimitiveType.LineStrip:
                    return (int)WebGLRenderingContextBase.LINE_STRIP;
                case PrimitiveType.TriangleList:
                    return (int)WebGLRenderingContextBase.TRIANGLES;
                case PrimitiveType.TriangleStrip:
                    return (int)WebGLRenderingContextBase.TRIANGLE_STRIP;
            }

            throw new ArgumentException();
        }

        public static WebGLTexture GetBoundTexture2D()
        {
            var ret = gl.GetParameter(WebGLRenderingContextBase.TEXTURE_BINDING_2D);
            GraphicsExtensions.CheckGLError();

            return ret as WebGLTexture;
        }
    }
}