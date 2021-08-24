using System;
using OpenTK.Graphics.OpenGL4;


namespace Visualization3D
{
    public class PCL : RenderObject
    {

        public PCL(string vertexShaderFile, string fragmentShaderFile) : base(vertexShaderFile, fragmentShaderFile)
        { }

        private float[] points_;
        private float[] colors_;
        private int vertexBufferObject_;
        private int colorBufferObject_;
        private int VertexArrayObject;

        private int PointsBufferLength = 0;
        private int ColorsBufferLength = 0;

        private bool setBufferLength(ref int l, int newl)
        {
            if (l > 2 * newl) { l = newl; return true; }
            if (newl > l) { l = newl; return true; }

            return false;
        }
        public void shrinkBufferLengthToFit()
        {
            PointsBufferLength = points_?.Length ?? 0;
            ColorsBufferLength = colors_?.Length ?? 0;
            updateBuffers(points_, colors_);
        }
        public void updateBuffers(float[] points, float[] colors)
        {
            if(points != null)
            {
                points_ = points;
                var pointsLen = points.Length * sizeof(float);

                bool useBufferData = setBufferLength(ref PointsBufferLength, pointsLen);
                // Bind the VBO and copy the vertex data into it.
                GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject_);
                if (!useBufferData)
                    GL.BufferSubData<float>(BufferTarget.ArrayBuffer, new IntPtr(0), pointsLen, points_);
                else
                    GL.BufferData(BufferTarget.ArrayBuffer, pointsLen, points_, BufferUsageHint.DynamicDraw);
            }
            if (colors != null)
            {
                colors_ = colors;
                var colorLen = colors.Length * sizeof(float);
                
                bool useBufferData = setBufferLength(ref ColorsBufferLength, colorLen);
                // Bind the VBO and copy the color data into it.
                GL.BindBuffer(BufferTarget.ArrayBuffer, colorBufferObject_);
                
                if (!useBufferData)
                    GL.BufferSubData<float>(BufferTarget.ArrayBuffer, new IntPtr(0), colorLen, colors_);
                else
                    GL.BufferData(BufferTarget.ArrayBuffer, colorLen, colors_, BufferUsageHint.DynamicDraw);
            }
        }

        protected override void initBuffers()
        {
            // Create the vertex buffer object (VBO) for the vertex data.
            vertexBufferObject_ = GL.GenBuffer();
            // Create the color buffer object (VBO) for the color data.
            colorBufferObject_ = GL.GenBuffer();

            
            updateBuffers(null, null);


            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject_);
            // Retrive the position location from the program.
            var positionLocation = GL.GetAttribLocation(ShaderProgram.Val, "position");

            
            // Create the vertex array object (VAO) for the program.
            VertexArrayObject = GL.GenVertexArray();
            // Bind the VAO and setup the position attribute.
            GL.BindVertexArray(VertexArrayObject);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(positionLocation);

            GL.BindBuffer(BufferTarget.ArrayBuffer, colorBufferObject_);
            // Retrive the position location from the program.
            var colorLocation = GL.GetAttribLocation(ShaderProgram.Val, "color");

            GL.VertexAttribPointer(colorLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(colorLocation);
        }
        public override void render()
        {
            GL.BindVertexArray(VertexArrayObject);
            GL.DrawArrays(PrimitiveType.Points, 0, points_.Length / 3);
        }

    }
}

