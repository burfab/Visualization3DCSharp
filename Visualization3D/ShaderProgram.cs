namespace Visualization3D
{
    public class ShaderProgram
    {
        public ShaderProgram(string key, string vertexShaderFile, string fragmentShaderFile, int val)
        {
            Key = key;
            VertexShaderFile = vertexShaderFile;
            FragmentShaderFile = fragmentShaderFile;
            Val = val;
        }
        public string Key { get; private set; }
        public string VertexShaderFile { get; private set; }
        public string FragmentShaderFile { get; private set; }
        public int Val { get; private set; }

    }
}

