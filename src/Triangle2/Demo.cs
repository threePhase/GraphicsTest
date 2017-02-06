using Engine;
using Engine.Interfaces;
using GraphicsTest.Interfaces;

namespace GraphicsTest.Triangle2 {
    public class Demo : IDemo {
        private IGameEngine _engine;
        private IRenderer _renderer;

        public void Run() {
            _engine = new DemoGameEngine();
            _renderer = new DemoRenderer(_engine.GetWindow());

            string vertexPath = "Triangle/triangle.vert";
            string fragmentPath = "Triangle/triangle.frag";
            _renderer.SetupShaders(vertexPath, fragmentPath);

            _renderer.SetupDrawing();

            while (_engine.IsRunning()) {
                _engine.PollEvents();
                _renderer.DrawScene();
            }

            _renderer.Cleanup();
            _engine.Cleanup();
        }
    }
}
