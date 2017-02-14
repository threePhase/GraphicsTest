using System;
using System.Collections.Generic;
using System.IO;
using Engine.Interfaces;
using Interop.Bindings;
using Interop.Exceptions.OpenGL;

namespace Engine {
    public class DemoRenderer : IRenderer {
        private uint _program;
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

            OpenGL.UseProgram(_program);
            foreach(var triangle in _triangles) {
                triangle.Draw();
            }

            GLFW.SwapBuffers(_window);
        }

        public void SetupShaders(Shader vertexShader,
                                 Shader fragmentShader) {
            SetupShaders(vertexShader.Path, fragmentShader.Path);
        }

        public void SetupShaders(string vertexShaderPath,
                                 string fragmentShaderPath) {
            string vertexShaderText = loadShaderFile(vertexShaderPath);
            // create the OpenGL Vertex Shader Object
            uint vertexShader = 0;
            try {
                vertexShader = createShader(OpenGL.GL_VERTEX_SHADER,
                                            vertexShaderText);
            }
            catch (ShaderCompileException e) {
                Console.WriteLine(e.Message);
                GLFW.Terminate();
                return;
            }

            string fragmentShaderText = loadShaderFile(fragmentShaderPath);
            // create the OpenGL Fragment Shader Object
            uint fragmentShader = 0;
            try {
                fragmentShader = createShader(OpenGL.GL_FRAGMENT_SHADER,
                                              fragmentShaderText);
            }
            catch (ShaderCompileException e) {
                Console.WriteLine(e.Message);
                OpenGL.DeleteShader(vertexShader);
                GLFW.Terminate();
                return;
            }

            try {
                _program = createProgram(vertexShader, fragmentShader);
            }
            catch (LinkingException e) {
                Console.WriteLine(e.Message);
                OpenGL.DeleteShader(vertexShader);
                OpenGL.DeleteShader(fragmentShader);
                GLFW.Terminate();
                return;
            }

            OpenGL.DeleteShader(vertexShader);
            OpenGL.DeleteShader(fragmentShader);
        }

        public void SetupDrawing(float[] vertices, uint[] indices = null, DrawingMode mode = DrawingMode.Fill) {
            var geometry = new List<Geometry>{
                new Geometry(vertices, indices)
            };
            SetupDrawing(geometry);
        }

        public void SetupDrawing(IEnumerable<Geometry> geometry) {
            _triangles = geometry;
        }

        private string loadShaderFile(string shaderPath) {
            var location = System.Reflection.Assembly.GetEntryAssembly().Location;
            string path = Path.GetDirectoryName(location);
            return File.ReadAllText($"{path}/{shaderPath}");
        }

        private uint createShader(uint type, string shaderText) {
            uint shader = OpenGL.CreateShader(type);
            OpenGL.ShaderSource(shader, 1, ref shaderText, IntPtr.Zero);
            OpenGL.CompileShader(shader);
            int success = 0;
            byte[] infoLog = new byte[512];
            // check for shader compile errors
            OpenGL.GetShaderiv(shader, OpenGL.GL_COMPILE_STATUS, ref success);
            if (success == 0) {
                OpenGL.GetShaderInfoLog(shader, infoLog.Length, IntPtr.Zero, infoLog);
                string log = System.Text.Encoding.UTF8.GetString(infoLog);
                throw new ShaderCompileException(log);
            }
            return shader;
        }

        // TODO: rename to createShaderProgram
        private uint createProgram(uint vertexShader, uint fragmentShader) {
            uint program = OpenGL.CreateProgram();
            OpenGL.AttachShader(program, vertexShader);
            OpenGL.AttachShader(program, fragmentShader);
            OpenGL.LinkProgram(program);
            // check for linking errors
            int success = 0;
            byte[] infoLog = new byte[512];
            OpenGL.GetProgramiv(program, OpenGL.GL_LINK_STATUS, ref success);
            if (success == 0) {
                OpenGL.GetProgramInfoLog(program, infoLog.Length, IntPtr.Zero, infoLog);
                string log = System.Text.Encoding.UTF8.GetString(infoLog);
                throw new LinkingException(log);
            }
            return program;
        }
    }
}
