using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;


namespace Visualization3D
{
    public static class ShaderManager
    {
        private static object lock_ = new object();

        private static Dictionary<string, int> shaders_ = new Dictionary<string, int>();
        private static Dictionary<string, int> shadersRefCnt_ = new Dictionary<string, int>();

        private static Dictionary<string, ShaderProgram> linkedPrograms_ = new Dictionary<string, ShaderProgram>();
        private static Dictionary<string, int> linkedProgramsRefCnt_ = new Dictionary<string, int>();


        public static ShaderProgram LoadShaderProgram(string vertexShader, string fragmentShader)
        {
            lock (lock_)
            {
                string key = buildProgramKey(vertexShader, fragmentShader);
                if (!linkedPrograms_.ContainsKey(key))
                {
                    var vertex = loadShader(vertexShader, ShaderType.VertexShader);
                    var frag = loadShader(fragmentShader, ShaderType.FragmentShader);

                    var prog = GL.CreateProgram();
                    GL.AttachShader(prog, vertex);
                    GL.AttachShader(prog, frag);
                    GL.LinkProgram(prog);

                    var program = new ShaderProgram(key, vertexShader, fragmentShader, prog);
                    linkedPrograms_.Add(key, program);
                    linkedProgramsRefCnt_.Add(key, 0);
                }
                linkedProgramsRefCnt_[key]++;
                return linkedPrograms_[key];
            }
        }
        public static void UnloadShaderProgram(ShaderProgram prog)
        {
            lock (lock_)
            {
                linkedProgramsRefCnt_[prog.Key]--;
                if(linkedProgramsRefCnt_[prog.Key] == 0)
                {
                    linkedProgramsRefCnt_.Remove(prog.Key);
                    linkedPrograms_.Remove(prog.Key);

                    unloadShader(prog.FragmentShaderFile);
                    unloadShader(prog.VertexShaderFile);

                    GL.DeleteProgram(prog.Val);
                }
            }
        }
        private static void unloadShader(string key)
        {
            shadersRefCnt_[key]--;
            if(shadersRefCnt_[key] == 0)
            {
                shadersRefCnt_.Remove(key);
                var val = shaders_[key];
                shaders_.Remove(key);
                
                GL.DeleteShader(val);
            }
        }

        private static string buildProgramKey(string vertexShader, string fragmentShader){
            return $"vs:{vertexShader};fs:{fragmentShader};";
        }
        private static int loadShader(string shaderfile, ShaderType type)
        {
            if (!shaders_.ContainsKey(shaderfile))
            {
                var source = System.IO.File.ReadAllText(shaderfile);
                var val = GL.CreateShader(type);
                GL.ShaderSource(val, source);
                GL.CompileShader(val);

                shadersRefCnt_.Add(shaderfile, 0);
                shaders_.Add(shaderfile, val);
            }
            shadersRefCnt_[shaderfile]++;

            return shaders_[shaderfile];
        }

    }
}

