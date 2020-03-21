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

            if (error != WebGL2RenderingContextBase.NO_ERROR)
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
                    return (int)WebGL2RenderingContextBase.FLOAT;

                case VertexElementFormat.Color:
                case VertexElementFormat.Byte4:
                    return (int)WebGL2RenderingContextBase.UNSIGNED_BYTE;

                case VertexElementFormat.Short2:
                case VertexElementFormat.Short4:
                case VertexElementFormat.NormalizedShort2:
                case VertexElementFormat.NormalizedShort4:
                    return (int)WebGL2RenderingContextBase.SHORT;
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
                    return WebGL2RenderingContextBase.FUNC_ADD;
                case BlendFunction.ReverseSubtract:
                    return WebGL2RenderingContextBase.FUNC_REVERSE_SUBTRACT;
                case BlendFunction.Subtract:
                    return WebGL2RenderingContextBase.FUNC_SUBTRACT;

                default:
                    throw new ArgumentException();
            }
        }

        public static uint GetBlendFactorSrc(this Blend blend)
        {
            switch (blend)
            {
                case Blend.BlendFactor:
                    return WebGL2RenderingContextBase.CONSTANT_COLOR;
                case Blend.DestinationAlpha:
                    return WebGL2RenderingContextBase.DST_ALPHA;
                case Blend.DestinationColor:
                    return WebGL2RenderingContextBase.DST_COLOR;
                case Blend.InverseBlendFactor:
                    return WebGL2RenderingContextBase.ONE_MINUS_CONSTANT_COLOR;
                case Blend.InverseDestinationAlpha:
                    return WebGL2RenderingContextBase.ONE_MINUS_DST_ALPHA;
                case Blend.InverseDestinationColor:
                    return WebGL2RenderingContextBase.ONE_MINUS_DST_COLOR;
                case Blend.InverseSourceAlpha:
                    return WebGL2RenderingContextBase.ONE_MINUS_SRC_ALPHA;
                case Blend.InverseSourceColor:
                    return WebGL2RenderingContextBase.ONE_MINUS_SRC_COLOR;
                case Blend.One:
                    return WebGL2RenderingContextBase.ONE;
                case Blend.SourceAlpha:
                    return WebGL2RenderingContextBase.SRC_ALPHA;
                case Blend.SourceAlphaSaturation:
                    return WebGL2RenderingContextBase.SRC_ALPHA_SATURATE;
                case Blend.SourceColor:
                    return WebGL2RenderingContextBase.SRC_COLOR;
                case Blend.Zero:
                    return WebGL2RenderingContextBase.ZERO;
                default:
                    throw new ArgumentOutOfRangeException("blend", "The specified blend function is not implemented.");
            }

        }

        public static uint GetBlendFactorDest(this Blend blend)
        {
            switch (blend)
            {
                case Blend.BlendFactor:
                    return WebGL2RenderingContextBase.CONSTANT_COLOR;
                case Blend.DestinationAlpha:
                    return WebGL2RenderingContextBase.DST_ALPHA;
                case Blend.DestinationColor:
                    return WebGL2RenderingContextBase.DST_COLOR;
                case Blend.InverseBlendFactor:
                    return WebGL2RenderingContextBase.ONE_MINUS_CONSTANT_COLOR;
                case Blend.InverseDestinationAlpha:
                    return WebGL2RenderingContextBase.ONE_MINUS_DST_ALPHA;
                case Blend.InverseDestinationColor:
                    return WebGL2RenderingContextBase.ONE_MINUS_DST_COLOR;
                case Blend.InverseSourceAlpha:
                    return WebGL2RenderingContextBase.ONE_MINUS_SRC_ALPHA;
                case Blend.InverseSourceColor:
                    return WebGL2RenderingContextBase.ONE_MINUS_SRC_COLOR;
                case Blend.One:
                    return WebGL2RenderingContextBase.ONE;
                case Blend.SourceAlpha:
                    return WebGL2RenderingContextBase.SRC_ALPHA;
                case Blend.SourceAlphaSaturation:
                    return WebGL2RenderingContextBase.SRC_ALPHA_SATURATE;
                case Blend.SourceColor:
                    return WebGL2RenderingContextBase.SRC_COLOR;
                case Blend.Zero:
                    return WebGL2RenderingContextBase.ZERO;
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
            }
        }

        public static void GetGLFormat(this SurfaceFormat format,
            GraphicsDevice graphicsDevice,
            out uint glInternalFormat,
            out uint glFormat,
            out uint glType)
        {
            glInternalFormat = (int)WebGL2RenderingContextBase.RGBA;
            glFormat = (int)WebGL2RenderingContextBase.RGBA;
            glType = (int)WebGL2RenderingContextBase.UNSIGNED_BYTE;

            var supportsSRgb = graphicsDevice.GraphicsCapabilities.SupportsSRgb;

            switch (format)
            {
                case SurfaceFormat.Color:
                    glInternalFormat = WebGL2RenderingContextBase.RGBA;
                    glFormat = WebGL2RenderingContextBase.RGBA;
                    glType = WebGL2RenderingContextBase.UNSIGNED_BYTE;
                    break;
                case SurfaceFormat.ColorSRgb:
                    glInternalFormat = WebGL2RenderingContextBase.SRGB;
                    glFormat = WebGL2RenderingContextBase.RGBA;
                    glType = WebGL2RenderingContextBase.UNSIGNED_BYTE;
                    break;
                case SurfaceFormat.Bgr565:
                    glInternalFormat = WebGL2RenderingContextBase.RGB;
                    glFormat = WebGL2RenderingContextBase.RGB;
                    glType = WebGL2RenderingContextBase.UNSIGNED_SHORT_5_6_5;
                    break;
                case SurfaceFormat.Bgra4444:
                    glInternalFormat = WebGL2RenderingContextBase.RGBA;
                    glFormat = WebGL2RenderingContextBase.RGBA;
                    glType = WebGL2RenderingContextBase.UNSIGNED_SHORT_4_4_4_4;
                    break;
                case SurfaceFormat.Bgra5551:
                    glInternalFormat = WebGL2RenderingContextBase.RGBA;
                    glFormat = WebGL2RenderingContextBase.RGBA;
                    glType = WebGL2RenderingContextBase.UNSIGNED_SHORT_5_5_5_1;
                    break;
                case SurfaceFormat.Alpha8:
                    glInternalFormat = WebGL2RenderingContextBase.LUMINANCE;
                    glFormat = WebGL2RenderingContextBase.LUMINANCE;
                    glType = WebGL2RenderingContextBase.UNSIGNED_BYTE;
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
                    return (int)WebGL2RenderingContextBase.LINES;
                case PrimitiveType.LineStrip:
                    return (int)WebGL2RenderingContextBase.LINE_STRIP;
                case PrimitiveType.TriangleList:
                    return (int)WebGL2RenderingContextBase.TRIANGLES;
                case PrimitiveType.TriangleStrip:
                    return (int)WebGL2RenderingContextBase.TRIANGLE_STRIP;
            }

            throw new ArgumentException();
        }

        public static WebGLTexture GetBoundTexture2D()
        {
            var ret = gl.GetParameter(WebGL2RenderingContextBase.TEXTURE_BINDING_2D);
            GraphicsExtensions.CheckGLError();

            return ret as WebGLTexture;
        }
    }
}