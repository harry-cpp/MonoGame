// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using WebGLDotNET;
using static WebHelper;

namespace Microsoft.Xna.Framework.Graphics
{
    internal partial class Shader
    {
        // The shader handle.
        private WebGLShader _shaderHandle;

        // We keep this around for recompiling on context lost and debugging.
        private string _glslCode;
        
        private static int PlatformProfile()
        {
            return 0;
        }

        private void PlatformConstruct(ShaderStage stage, byte[] shaderBytecode)
        {
            _glslCode = System.Text.Encoding.ASCII.GetString(shaderBytecode);

            HashKey = MonoGame.Utilities.Hash.ComputeHash(shaderBytecode);
        }

        internal WebGLShader GetShaderHandle()
        {
            // If the shader has already been created then return it.
            if (gl.IsShader(_shaderHandle))
                return _shaderHandle;

            _shaderHandle = gl.CreateShader(Stage == ShaderStage.Vertex ? WebGL2RenderingContextBase.VERTEX_SHADER : WebGL2RenderingContextBase.FRAGMENT_SHADER);
            GraphicsExtensions.CheckGLError();
            gl.ShaderSource(_shaderHandle, _glslCode);
            GraphicsExtensions.CheckGLError();
            gl.CompileShader(_shaderHandle);
            GraphicsExtensions.CheckGLError();
            var compiled = (bool)gl.GetShaderParameter(_shaderHandle, WebGL2RenderingContextBase.COMPILE_STATUS);
            GraphicsExtensions.CheckGLError();
            if (!compiled)
            {
                var log = gl.GetShaderInfoLog(_shaderHandle);
                Debug.WriteLine(log);

                GraphicsDevice.DisposeShader(_shaderHandle);

                throw new InvalidOperationException("Shader Compilation Failed");
            }

            return _shaderHandle;
        }

        internal void GetVertexAttributeLocations(WebGLProgram program)
        {
            for (int i = 0; i < Attributes.Length; ++i)
            {
                Attributes[i].location = (int)gl.GetAttribLocation(program, Attributes[i].name);
                GraphicsExtensions.CheckGLError();
            }
        }

        internal int GetAttribLocation(VertexElementUsage usage, int index)
        {
            for (int i = 0; i < Attributes.Length; ++i)
            {
                if ((Attributes[i].usage == usage) && (Attributes[i].index == index))
                    return Attributes[i].location;
            }
            return -1;
        }

        internal void ApplySamplerTextureUnits(WebGLProgram program)
        {
            // Assign the texture unit index to the sampler uniforms.
            foreach (var sampler in Samplers)
            {
                var loc = gl.GetUniformLocation(program, sampler.name);
                GraphicsExtensions.CheckGLError();
                
                gl.Uniform1i(loc, sampler.textureSlot);
                GraphicsExtensions.CheckGLError();
            }
        }

        private void PlatformGraphicsDeviceResetting()
        {
            if (gl.IsShader(_shaderHandle))
                GraphicsDevice.DisposeShader(_shaderHandle);
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && gl.IsShader(_shaderHandle))
                GraphicsDevice.DisposeShader(_shaderHandle);

            base.Dispose(disposing);
        }
    }
}
