using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;


namespace Visualization3D
{
    public sealed class Viewer3D : GameWindow
    {
        public List<RenderObject> RenderObjects = new List<RenderObject>();

        public Viewer3D(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        
        public Matrix4 ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(-1, 1, -1, 1, 0.001f, 1.0f); 
        public Matrix4 ViewMatrix = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);

        public Action<Viewer3D> OnSizeChanged = null;
        private bool backgroundChanged_ = false;
        private Vector3 background_ = Vector3.Zero;
        public Size ViewportSize;

        public Vector3 Background
        {
            get
            {
                return background_;
            }
            set
            {
                background_ = value;
                backgroundChanged_ = true;
            }
        }

        PCL genPCL(PCL pcl)
        {
            const double MIN = -0.8;
            const double MAX = 0.8;

            const int NUM_POINTS = 10000;
            Random rnd = new Random();

            var points = new float[NUM_POINTS * 3];
            var colors = new float[NUM_POINTS * 3];

            for (int i = 0; i < NUM_POINTS - 1; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var val = rnd.NextDouble();
                    val = val * (MAX - MIN);
                    val = val + MIN;
                    //if (j == 2)
                    //val *= -1;

                    points[3 * i + j] = (float)val;
                    colors[3 * i + j] = (float)rnd.NextDouble();
                }
            }
            //vertexBuffer_ = vertices;

            pcl.updateBuffers(points, colors);
            return pcl;
        }

        protected override void OnLoad()
        {
            initGl();

            GL.ClearColor(Background.X, Background.Y, Background.Z, 0.0f);

            base.OnLoad();
        }

        private void initGl()
        {
            GL.Enable(EnableCap.VertexProgramPointSize);
        }

        protected override void OnUnload()
        {
            // Unbind all the resources by binding the targets to 0/null.
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            RenderObjects.Clear();

            // Delete all the resources.
            base.OnUnload();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            this.ViewportSize = new System.Drawing.Size(e.Width, e.Height);
            if (OnSizeChanged != null)
                OnSizeChanged(this);
            GL.Viewport(0, 0, ViewportSize.Width, ViewportSize.Height);
            base.OnResize(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (backgroundChanged_)
            {
                GL.ClearColor(Background.X, Background.Y, Background.Z, 0.0f);
                backgroundChanged_ = false;
            }

            GL.Clear(ClearBufferMask.ColorBufferBit);

            foreach (var ro in RenderObjects)
            {
                if (ro is PCL pcl) genPCL(pcl);
                ro.UseProgram();
                ro.SetMatrix(ProjectionMatrix * ViewMatrix);
                ro.render();
            }

            // Swap the front/back buffers so what we just rendered to the back buffer is displayed in the window.
            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

    }
}

