using System;

namespace Engine.Interfaces {
    public interface IRenderer {
        void Cleanup();
        void DrawScene();
        void SetupShaders(string vertexShaderPath, string fragmentShaderPath);
        void SetupShaders(Shader vertexShader, Shader fragmentShader);
        // TODO: Not sure about this name
        void SetupDrawing(float[] vertices, uint[] indices = null);
    }
}
