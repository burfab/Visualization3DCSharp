using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Visualization3D
{
    public abstract class RenderObject : IDisposable
    {
        public RenderObject(string vertexShaderFile, string fragShaderFile)
        {
            ShaderProgram = ShaderManager.LoadShaderProgram(vertexShaderFile, fragShaderFile);
        }

        public ShaderProgram ShaderProgram { get; private set; }

        public abstract void render();

        public Matrix4 ModelMatrix = Matrix4.Identity;
        public void init()
        {
            GL.UseProgram(ShaderProgram.Val);
            initBuffers();
        }
        protected abstract void initBuffers();
        public void Dispose()
        {
            if(ShaderProgram != null)
            {
                ShaderManager.UnloadShaderProgram(ShaderProgram);
                ShaderProgram = null;
            }
        }

        public void SetMatrix(Matrix4 VP)
        {
            int loc = GL.GetUniformLocation(ShaderProgram.Val, "MVP");
            var mat = VP * ModelMatrix;
            GL.UniformMatrix4(loc, false, ref mat);
        }

        public void UseProgram()
        {
            GL.UseProgram(ShaderProgram.Val);
        }
    }
}

