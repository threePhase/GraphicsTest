using System.Collections.Generic;
using Engine;
using Engine.Interfaces;
using GraphicsTest.Interfaces;

namespace GraphicsTest.TwoTriangles {
    public class Demo : IDemo {
        private IGameEngine _engine;
        private IRenderer _renderer;

        public string Name {
            get {
                return "Two Triangles Demo with Single VAO/VBO (using Engine)";
            }
        }

        public void Run() {
            _engine = new DemoGameEngine();
            _renderer = new DemoRenderer(_engine.GetWindow());

            var shader = new Shader("Shaders/triangle.vert",
                                    "Shaders/orange.frag");

            // (x, y, z) coordinate pairs
            float[] vertices = {
                -1.0f, -0.5f, 0.0f, // left triangle - left point
                -0.5f,  0.5f, 0.0f, // left triangle - top
                 0.0f, -0.5f, 0.0f, // center
                 0.5f,  0.5f, 0.0f, // right triangle - top
                 1.0f, -0.5f, 0.0f, // right triangle - right point
            };

            // specify vertex drawing order
            uint[] indices = {
                0, 1, 2,
                2, 3, 4
            };

            var triangles = new Geometry(vertices, indices, shader);
            _renderer.SetupGeometry(new List<Geometry>{ triangles });

            while (_engine.IsRunning()) {
                _engine.PollEvents();
                _renderer.DrawScene();
            }

            _renderer.Cleanup();
            _engine.Cleanup();
        }
    }
}
