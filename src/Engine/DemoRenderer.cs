using System;
using System.Collections.Generic;
using System.IO;
using Engine.Interfaces;
using Interop.Bindings;
using Interop.Exceptions.OpenGL;

namespace Engine {
    public class DemoRenderer : IRenderer {
        private IEnumerable<Geometry> _triangles;
        private IntPtr _window;

        public DemoRenderer(IntPtr window) {
            _window = window;
        }

        public void Cleanup() {
            foreach(var triangle in _triangles) {
                triangle.Cleanup();
            }
        }

        public void DrawScene() {
            // red, green, and blue values range from 0 (0) - 255 (1)
            OpenGL.ClearColor(1/255.0f, 113/255.0f, 187/255.0f, 1.0f);
            OpenGL.Clear(OpenGL.GL_COLOR_BUFFER_BIT);

            foreach(var triangle in _triangles) {
                triangle.Draw();
            }

            GLFW.SwapBuffers(_window);
        }

        public void SetupGeometry(IEnumerable<Geometry> geometry) {
            _triangles = geometry;
        }
    }
}
