using System.Collections.Generic;
using Engine;
using Engine.Interfaces;
using GraphicsTest.Interfaces;

namespace GraphicsTest.TwoTriangles2 {
    public class Demo : IDemo {
        private IGameEngine _engine;
        private IRenderer _renderer;

        public string Name {
            get {
                return "Two Triangles Demo VAO/VBOs per Triangle (using Engine)";
            }
        }

        public void Run() {
            _engine = new DemoGameEngine();
            _renderer = new DemoRenderer(_engine.GetWindow());

            var shader = new Shader("Shaders/triangle.vert",
                                    "Shaders/orange.frag");

            // (x, y, z) coordinate pairs
            var left_triangle = new Geometry(new float[] {
                -1.0f, -0.5f, 0.0f, // left triangle - left point
                -0.5f,  0.5f, 0.0f, // left triangle - top
                 0.0f, -0.5f, 0.0f  // center
            }, shader);

            var right_triangle = new Geometry(new float[] {
                0.5f,  0.5f, 0.0f, // right triangle - top
                1.0f, -0.5f, 0.0f, // right triangle - right point
                0.0f, -0.5f, 0.0f  // center
            }, shader);

            var geometry = new List<Geometry>{
                left_triangle,
                right_triangle
            };

            _renderer.SetupGeometry(geometry);

            while (_engine.IsRunning()) {
                _engine.PollEvents();
                _renderer.DrawScene();
            }

            _renderer.Cleanup();
            _engine.Cleanup();
        }
    }
}
