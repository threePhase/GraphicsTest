using System;
using System.Runtime.InteropServices;
using Interop.Bindings;
using Interop.Exceptions.OpenGL;

namespace GraphicsTest
{
    public class Program
    {
        private static string _vertexShaderText =
            string.Join(Environment.NewLine, new string[] {
                            "#version 330 core",
                            "layout (location = 0) in vec3 position;",
                            "void main() {",
                            "    gl_Position = vec4(position.x, position.y, position.z, 1.0);",
                            "}"
            });

        private static string _fragmentShaderText =
            string.Join(Environment.NewLine, new string[] {
                            "#version 330 core",
                            "out vec4 color;",
                            "void main() {",
                            "    color = vec4(1.0f, 0.5f, 0.2f, 1.0f);",
                            "}"
            });


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

        public static void Main(string[] args) {
            Console.WriteLine("This is a simple drawing test using GLFW and OpenGL 3.3!");

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

            int width = 0,
                height = 0;
            GLFW.GetFramebufferSize(window, ref width, ref height);
            OpenGL.Viewport(0, 0, width, height);

            // create the OpenGL Vertex Shader Object
            uint vertexShader = OpenGL.CreateShader(OpenGL.GL_VERTEX_SHADER);
            OpenGL.ShaderSource(vertexShader, 1, ref _vertexShaderText, IntPtr.Zero);
            OpenGL.CompileShader(vertexShader);
            int success = 0;
            byte[] infoLog = new byte[512];
            // check for shader compile errors
            OpenGL.GetShaderiv(vertexShader, OpenGL.GL_COMPILE_STATUS, ref success);
            if (success == 0) {
                OpenGL.GetShaderInfoLog(vertexShader, infoLog.Length, IntPtr.Zero, infoLog);
                string log = System.Text.Encoding.UTF8.GetString(infoLog);
                Console.WriteLine($"Shader Compile Error: \n{log}");
            }

            // create the OpenGL Fragment Shader Object
            uint fragmentShader = OpenGL.CreateShader(OpenGL.GL_FRAGMENT_SHADER);
            OpenGL.ShaderSource(fragmentShader, 1, ref _fragmentShaderText, IntPtr.Zero);
            OpenGL.CompileShader(fragmentShader);
            // check for shader compile errors
            OpenGL.GetShaderiv(fragmentShader, OpenGL.GL_COMPILE_STATUS, ref success);
            if (success == 0) {
                OpenGL.GetShaderInfoLog(fragmentShader, infoLog.Length, IntPtr.Zero, infoLog);
                string log = System.Text.Encoding.UTF8.GetString(infoLog);
                Console.WriteLine($"Shader Compile Error: \n{log}");
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

            float[] vertices = {
                0.0f,  0.5f, 0.0f, // top right
                0.5f, -0.5f, 0.0f, // bottom right
               -0.5f, -0.5f, 0.0f // bottom left
            };

            // create the OpenGL Vertex Array Object (VAO)
            uint vertexArray = 0;
            OpenGL.GenVertexArrays(1, ref vertexArray);
            // create the OpenGL Vertex Buffer Object (VBO)
            uint vertexBuffer = 0;
            OpenGL.GenBuffers(1, ref vertexBuffer);

            // bind VAO first, then bind and set vertex buffer(s) and attribute pointer(s).
            OpenGL.BindVertexArray(vertexArray);

            OpenGL.BindBuffer(OpenGL.GL_ARRAY_BUFFER, vertexBuffer);
            var verticesSize = new IntPtr(sizeof(float) * vertices.Length);
            OpenGL.BufferData(OpenGL.GL_ARRAY_BUFFER, verticesSize, vertices, OpenGL.GL_STATIC_DRAW);

            var bufferSize = sizeof(float) * 3;
            OpenGL.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, bufferSize, IntPtr.Zero);
            OpenGL.EnableVertexAttribArray(0);

            // // unbind OpenGL Vertex Buffer Object
            OpenGL.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);
            // // unbind OpenGL Vertex Array Object
            OpenGL.BindVertexArray(0);

            while (GLFW.WindowShouldClose(window) == 0) {
                GLFW.PollEvents();

                // red, green, and blue values range from 0 (0) - 255 (1)
                OpenGL.ClearColor(1/255.0f, 113/255.0f, 187/255.0f, 1.0f);
                OpenGL.Clear(OpenGL.GL_COLOR_BUFFER_BIT);

                OpenGL.UseProgram(program);
                OpenGL.BindVertexArray(vertexArray);
                OpenGL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 3);
                // DrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, IntPtr.Zero);
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

        private static uint createProgram(uint vertexShader, uint fragmentShader) {
            uint program = OpenGL.CreateProgram();
            OpenGL.AttachShader(program, vertexShader);
            OpenGL.AttachShader(program, fragmentShader);
            OpenGL.LinkProgram(program);
            // check for linking errors
            int success = 0;
            string infoLog = "";
            OpenGL.GetProgramiv(program, OpenGL.GL_LINK_STATUS, ref success);
            if (success == 0) {
                OpenGL.GetProgramInfoLog(program, 512, IntPtr.Zero, infoLog);
                throw new LinkingException(infoLog);
            }
            return program;
        }
    }
}
