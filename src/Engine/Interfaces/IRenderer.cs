using System;

namespace Engine.Interfaces {
    public interface IRenderer {
        void Cleanup();
        void DrawScene();
        void SetupShaders(string vertexShaderPath, string fragmentShaderPath);
        // TODO: Not sure about this name
        void SetupDrawing();
    }
}
