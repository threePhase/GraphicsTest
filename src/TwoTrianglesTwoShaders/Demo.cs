using System.Collections.Generic;
using Engine;
using Engine.Interfaces;
using GraphicsTest.Interfaces;

namespace GraphicsTest.TwoTrianglesTwoShaders {
    public class Demo : IDemo {
        private IGameEngine _engine;
        private IRenderer _renderer;

        public string Name {
            get {
                return "Two Triangles Two Shaders Demo (using Engine)";
            }
        }

        public void Run() {
            _engine = new DemoGameEngine();
            _renderer = new DemoRenderer(_engine.GetWindow());

            var leftTriangleShader =
                new Shader("Shaders/triangle.vert",
                           "Shaders/orange.frag");

            var rightTriangleShader =
                new Shader("Shaders/triangle.vert",
                           "Shaders/yellow.frag");

            // (x, y, z) coordinate pairs
            var left_triangle = new Geometry(new float[] {
                -1.0f, -0.5f, 0.0f, // left triangle - left point
                -0.5f,  0.5f, 0.0f, // left triangle - top
                 0.0f, -0.5f, 0.0f  // center
            }, shader: leftTriangleShader);

            var right_triangle = new Geometry(new float[] {
                0.5f,  0.5f, 0.0f, // right triangle - top
                1.0f, -0.5f, 0.0f, // right triangle - right point
                0.0f, -0.5f, 0.0f  // center
            }, shader: rightTriangleShader);

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
