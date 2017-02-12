using System;
using System.IO;
using System.Runtime.InteropServices;
using Engine.Interfaces;
using Interop.Bindings;
using Interop.Exceptions.OpenGL;

namespace Engine {
    public class DemoRenderer : IRenderer {
        private uint _program;
        private uint _vertexArray;
        private uint _vertexBuffer;
        private IntPtr _window;

        public DemoRenderer(IntPtr window) {
            _window = window;
        }

        public void Cleanup() {
            // delete vertex array
            OpenGL.DeleteVertexArrays(1, ref _vertexArray);
            // delete vertex buffer
            OpenGL.DeleteBuffers(1, ref _vertexBuffer);
        }

        public void DrawScene() {
            // red, green, and blue values range from 0 (0) - 255 (1)
            OpenGL.ClearColor(1/255.0f, 113/255.0f, 187/255.0f, 1.0f);
            OpenGL.Clear(OpenGL.GL_COLOR_BUFFER_BIT);

            OpenGL.UseProgram(_program);
            OpenGL.BindVertexArray(_vertexArray);
            OpenGL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 3);
            OpenGL.BindVertexArray(0);

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

        public void SetupDrawing(float[] vertices) {
            _vertexArray = loadVertexArrayObject();
            _vertexBuffer = loadVertexBufferObject();

            loadVertices(vertices);

            // divide by 3 since each vertex is made up of 3 points
            setupDrawing(vertices.Length / 3);

            unbindBuffers();
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

        // create the OpenGL Vertex Array Object (VAO)
        private static uint loadVertexArrayObject() {
            uint vertexArray = 0;
            OpenGL.GenVertexArrays(1, ref vertexArray);
            // bind VAO first, then bind and set vertex buffer(s) and attribute pointer(s).
            OpenGL.BindVertexArray(vertexArray);
            return vertexArray;
        }

        // create the OpenGL Vertex Buffer Object (VBO)
        private static uint loadVertexBufferObject() {
            uint vertexBuffer = 0;
            OpenGL.GenBuffers(1, ref vertexBuffer);
            OpenGL.BindBuffer(OpenGL.GL_ARRAY_BUFFER, vertexBuffer);
            return vertexBuffer;
        }

        // load given vertices into VBO
        private static void loadVertices(float[] vertices) {
            var verticesSize = new IntPtr(sizeof(float) * vertices.Length);
            IntPtr verticesHandle = Marshal.AllocHGlobal(vertices.Length);
            Marshal.Copy(vertices, 0, verticesHandle, vertices.Length);
            OpenGL.BufferData(OpenGL.GL_ARRAY_BUFFER, verticesSize, verticesHandle, OpenGL.GL_STATIC_DRAW);
            Marshal.FreeHGlobal(verticesHandle);
        }

        // setup drawing vertices in currently bound VAO
        private static void setupDrawing(int totalVertices) {
            var bufferSize = sizeof(float) * totalVertices;
            OpenGL.VertexAttribPointer(0, totalVertices, OpenGL.GL_FLOAT, false, bufferSize, IntPtr.Zero);
            OpenGL.EnableVertexAttribArray(0);
        }

        // unbind OpenGL Vertex Array Object and OpenGL Vertex Buffer Object
        private static void unbindBuffers() {
            OpenGL.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);
            OpenGL.BindVertexArray(0);
        }
    }
}
