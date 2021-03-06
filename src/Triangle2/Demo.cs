using System.Collections.Generic;
using Engine;
using Engine.Interfaces;
using GraphicsTest.Interfaces;

namespace GraphicsTest.Triangle2 {
    public class Demo : IDemo {
        private IGameEngine _engine;
        private IRenderer _renderer;
        private const string _fragmentShaderPath = "Shaders/orange.frag";
        private const string _vertexShaderPath = "Shaders/triangle.vert";

        public string Name {
            get {
                return "Triangle Demo (using Engine)";
            }
        }

        public void Run() {
            _engine = new DemoGameEngine();
            _renderer = new DemoRenderer(_engine.GetWindow());

            var shader = new Shader(_vertexShaderPath, _fragmentShaderPath);

            // (x, y, z) coordinate pairs
            float[] vertices = {
                0.0f,  0.5f, 0.0f, // top
                0.5f, -0.5f, 0.0f, // bottom right
                -0.5f, -0.5f, 0.0f  // bottom left
            };


            var geometry = new List<Geometry> {
                new Geometry(vertices, shader)
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
