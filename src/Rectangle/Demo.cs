using Engine;
using Engine.Interfaces;
using GraphicsTest.Interfaces;

namespace GraphicsTest.Rectangle {
    public class Demo : IDemo {
        private IGameEngine _engine;
        private IRenderer _renderer;

        public string Name {
            get {
                return "Rectangle Demo (using Engine)";
            }
        }

        public void Run() {
            _engine = new DemoGameEngine();
            _renderer = new DemoRenderer(_engine.GetWindow());

            var fragmentShader = new Shader("Rectangle/Rectangle.frag");
            var vertexShader = new Shader("Rectangle/Rectangle.vert");

            _renderer.SetupShaders(vertexShader, fragmentShader);

            // (x, y, z) coordinate pairs
            float[] vertices = {
                 0.5f,  0.5f, 0.0f, // top right
                 0.5f, -0.5f, 0.0f, // bottom right
                -0.5f, -0.5f, 0.0f,  // bottom left
                -0.5f,  0.5f, 0.0f  // top left
            };

            // specify vertex drawing order
            uint[] indices = {
                0, 1, 3, // first triangle
                1, 2, 3
            };

            _renderer.SetupDrawing(vertices, indices);

            while (_engine.IsRunning()) {
                _engine.PollEvents();
                _renderer.DrawScene();
            }

            _renderer.Cleanup();
            _engine.Cleanup();
        }
    }
}
