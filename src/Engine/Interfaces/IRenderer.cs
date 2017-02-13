using System;
using System.Collections.Generic;

namespace Engine.Interfaces {
    public interface IRenderer {
        void Cleanup();
        void DrawScene();
        void SetupShaders(string vertexShaderPath, string fragmentShaderPath);
        void SetupShaders(Shader vertexShader, Shader fragmentShader);
        // TODO: Not sure about this name
        void SetupDrawing(float[] vertices, uint[] indices = null, DrawingMode mode = DrawingMode.Fill);
        void SetupDrawing(IEnumerable<Geometry> geometry);
    }
}
