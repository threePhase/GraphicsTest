﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using Interop.Bindings;
using Interop.Exceptions.OpenGL;

namespace GraphicsTest.Triangle
{
    public class Demo
    {
        private static void keyBindings(IntPtr window,
                                        int key,
                                        int scancode,
                                        int action,
                                        int mode) {
            if (key == GLFW.GLFW_KEY_ESCAPE && action == GLFW.GLFW_PRESS) {
               GLFW.SetWindowShouldClose(window, true);
            }
        }

        private static void onError(int error, string description) {
            Console.WriteLine($"GLFW Error: {description}");
        }

        public static void Run() {
            GLFW.SetErrorCallback(onError);

            if (!GLFW.Initialize()) {
                Console.WriteLine($"Failed to initialize GLFW.");
                return;
            }

            var possibleWindow = createWindow("GLFW Drawing Test");
            if (!possibleWindow.HasValue) {
                Console.WriteLine($"Unable to load create window.");
                GLFW.Terminate();
                return;
            }

            IntPtr window = possibleWindow.Value;

            GLFW.MakeContextCurrent(window);

            GLFW.SetKeyCallback(window, keyBindings);

            setupViewport(window);

            string vertexShaderText = loadShaderFile("Shaders/triangle.vert");
            // create the OpenGL Vertex Shader Object
            uint vertexShader = 0;
            try {
                vertexShader = createShader(OpenGL.GL_VERTEX_SHADER, vertexShaderText);
            }
            catch (ShaderCompileException e) {
                Console.WriteLine(e.Message);
                GLFW.Terminate();
                return;
            }

            string fragmentShaderText = loadShaderFile("Shaders/orange.frag");
            // create the OpenGL Fragment Shader Object
            uint fragmentShader = 0;
            try {
                fragmentShader = createShader(OpenGL.GL_FRAGMENT_SHADER, fragmentShaderText);
            }
            catch (ShaderCompileException e) {
                Console.WriteLine(e.Message);
                OpenGL.DeleteShader(vertexShader);
                GLFW.Terminate();
                return;
            }

            uint program = 0;
            try {
                program = createProgram(vertexShader, fragmentShader);
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

            uint vertexArray = loadVertexArrayObject();
            uint vertexBuffer = loadVertexBufferObject();

            // (x, y, z) coordinate pairs
            float[] vertices = {
                0.0f,  0.5f, 0.0f, // top
                0.5f, -0.5f, 0.0f, // bottom right
               -0.5f, -0.5f, 0.0f  // bottom left
            };
            loadVertices(vertices);

            // divide by 3 since each vertex is made up of 3 points
            setupDrawing(vertices.Length / 3);

            unbindBuffers();

            while (GLFW.WindowShouldClose(window) == 0) {
                GLFW.PollEvents();

                // red, green, and blue values range from 0 (0) - 255 (1)
                OpenGL.ClearColor(1/255.0f, 113/255.0f, 187/255.0f, 1.0f);
                OpenGL.Clear(OpenGL.GL_COLOR_BUFFER_BIT);

                OpenGL.UseProgram(program);
                OpenGL.BindVertexArray(vertexArray);
                OpenGL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 3);
                OpenGL.BindVertexArray(0);

                GLFW.SwapBuffers(window);
            }
            // delete vertex array
            OpenGL.DeleteVertexArrays(1, ref vertexArray);
            // delete vertex buffer
            OpenGL.DeleteBuffers(1, ref vertexBuffer);

            // terminate GLFW, clearing any resources allocated by GLFW
            GLFW.Terminate();
        }

        private static IntPtr? createWindow(string windowName) {
            GLFW.WindowHint(GLFW.GLFW_CONTEXT_VERSION_MAJOR, 3);
            GLFW.WindowHint(GLFW.GLFW_CONTEXT_VERSION_MINOR, 3);
            GLFW.WindowHint(GLFW.GLFW_OPENGL_PROFILE, GLFW.GLFW_OPENGL_CORE_PROFILE);
            // required for getting OpenGL context >= 3.2 on MacOS >= OS X 10.7
            // http://stackoverflow.com/a/9017716
            GLFW.WindowHint(GLFW.GLFW_OPENGL_FORWARD_COMPAT, 1);
            GLFW.WindowHint(GLFW.GLFW_RESIZABLE, 0);

            var window = GLFW.CreateWindow(640, 480, windowName, IntPtr.Zero, IntPtr.Zero);
            if (window == IntPtr.Zero) {
                return null;
            }

            return window;
        }

        private static string loadShaderFile(string shaderPath) {
            var location = System.Reflection.Assembly.GetEntryAssembly().Location;
            string path = Path.GetDirectoryName(location);
            return File.ReadAllText($"{path}/{shaderPath}");
        }

        private static uint createShader(uint type, string shaderText) {
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

        private static uint createProgram(uint vertexShader, uint fragmentShader) {
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

        private static void setupViewport(IntPtr window) {
            int width = 0,
                height = 0;
            GLFW.GetFramebufferSize(window, ref width, ref height);
            OpenGL.Viewport(0, 0, width, height);
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
            int size = Marshal.SizeOf(vertices[0]) * vertices.Length;
            IntPtr handle = Marshal.AllocHGlobal(size);
            try {
                Marshal.Copy(vertices, 0, handle, vertices.Length);
                var unmanagedSize = new IntPtr(size);
                OpenGL.BufferData(OpenGL.GL_ARRAY_BUFFER, unmanagedSize, handle, OpenGL.GL_STATIC_DRAW);
            }
            finally {
                Marshal.FreeHGlobal(handle);
            }
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
