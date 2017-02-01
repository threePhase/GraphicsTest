using System;
using System.Runtime.InteropServices;
using Interop.Bindings;

// heavily inspired by: https://github.com/CameronAavik/dotnetcore-graphics-example
// as well as: https://learnopengl.com/#!Getting-started/Hello-Triangle
namespace GraphicsTest
{
    public class Program
    {
        // OpenGL constants
        // TODO: Source these from deps/glad/glad.h
        private const int GL_TRIANGLES = 0x0004;
        private const int GL_UNSIGNED_INT = 0x1405;
        private const int GL_FLOAT = 0x1406;
        private const int GL_COLOR_BUFFER_BIT = 0x4000;
        private const int GL_ARRAY_BUFFER = 0x8892;
        private const int GL_ELEMENT_ARRAY_BUFFER = 0x8893;
        private const int GL_STATIC_DRAW = 0x88e4;
        private const int GL_FRAGMENT_SHADER = 0x8b30;
        private const int GL_VERTEX_SHADER = 0x8b31;
        private const int GL_COMPILE_STATUS = 0x8b81;
        private const int GL_LINK_STATUS = 0x8b82;

        // GLFW - window constants
        private const int GLFW_RESIZABLE = 0x00020003;
        private const int GLFW_CONTEXT_VERSION_MAJOR = 0x00022002;
        private const int GLFW_CONTEXT_VERSION_MINOR = 0x00022003;
        private const int GLFW_OPENGL_FORWARD_COMPAT = 0x00022006;
        private const int GLFW_OPENGL_PROFILE = 0x00022008;
        private const int GLFW_OPENGL_CORE_PROFILE = 0x00032001;

        // GLFW - key bindings constants
        private const int GLFW_PRESS = 1;
        private const int GLFW_KEY_ESCAPE = 256;

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
            if (key == GLFW_KEY_ESCAPE && action == GLFW_PRESS) {
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

            GLFW.WindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
            GLFW.WindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
            GLFW.WindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
            // required for getting OpenGL context >= 3.2 on MacOS >= OS X 10.7
            // http://stackoverflow.com/a/9017716
            GLFW.WindowHint(GLFW_OPENGL_FORWARD_COMPAT, 1);
            GLFW.WindowHint(GLFW_RESIZABLE, 0);

            var window = GLFW.CreateWindow(640, 480, "GLFW Drawing Test", IntPtr.Zero, IntPtr.Zero);
            if (window == IntPtr.Zero) {
                Console.WriteLine($"Unable to load create window.");
                GLFW.Terminate();
                return;
            }

            GLFW.MakeContextCurrent(window);

            GLFW.SetKeyCallback(window, keyBindings);

            int width = 0,
                height = 0;
            GLFW.GetFramebufferSize(window, ref width, ref height);
            OpenGL.Viewport(0, 0, width, height);

            // create the OpenGL Vertex Shader Object
            uint vertexShader = OpenGL.CreateShader(GL_VERTEX_SHADER);
            OpenGL.ShaderSource(vertexShader, 1, ref _vertexShaderText, IntPtr.Zero);
            OpenGL.CompileShader(vertexShader);
            int success = 0;
            string infoLog = "";
            // check for shader compile errors
            OpenGL.GetShaderiv(vertexShader, GL_COMPILE_STATUS, ref success);
            if (success == 0) {
                OpenGL.GetShaderInfoLog(vertexShader, 512, IntPtr.Zero, infoLog);
                Console.WriteLine($"Shader Compile Error: \n{infoLog}");
            }

            // create the OpenGL Fragment Shader Object
            uint fragmentShader = OpenGL.CreateShader(GL_FRAGMENT_SHADER);
            OpenGL.ShaderSource(fragmentShader, 1, ref _fragmentShaderText, IntPtr.Zero);
            OpenGL.CompileShader(fragmentShader);
            // check for shader compile errors
            OpenGL.GetShaderiv(fragmentShader, GL_COMPILE_STATUS, ref success);
            if (success == 0) {
                OpenGL.GetShaderInfoLog(fragmentShader, 512, IntPtr.Zero, infoLog);
                Console.WriteLine($"Shader Compile Error: \n{infoLog}");
            }

            var program = OpenGL.CreateProgram();
            OpenGL.AttachShader(program, vertexShader);
            OpenGL.AttachShader(program, fragmentShader);
            OpenGL.LinkProgram(program);
            // check for linking errors
            OpenGL.GetProgramiv(program, GL_LINK_STATUS, ref success);
            if (success == 0) {
                OpenGL.GetProgramInfoLog(program, 512, IntPtr.Zero, infoLog);
                Console.WriteLine($"Linking Error: \n{infoLog}");
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

            OpenGL.BindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
            var verticesSize = new IntPtr(sizeof(float) * vertices.Length);
            OpenGL.BufferData(GL_ARRAY_BUFFER, verticesSize, vertices, GL_STATIC_DRAW);

            var bufferSize = sizeof(float) * 3;
            OpenGL.VertexAttribPointer(0, 3, GL_FLOAT, false, bufferSize, IntPtr.Zero);
            OpenGL.EnableVertexAttribArray(0);

            // // unbind OpenGL Vertex Buffer Object
            OpenGL.BindBuffer(GL_ARRAY_BUFFER, 0);
            // // unbind OpenGL Vertex Array Object
            OpenGL.BindVertexArray(0);

            while (GLFW.WindowShouldClose(window) == 0) {
                GLFW.PollEvents();

                // red, green, and blue values range from 0 (0) - 255 (1)
                OpenGL.ClearColor(1/255.0f, 113/255.0f, 187/255.0f, 1.0f);
                OpenGL.Clear(GL_COLOR_BUFFER_BIT);

                OpenGL.UseProgram(program);
                OpenGL.BindVertexArray(vertexArray);
                OpenGL.DrawArrays(GL_TRIANGLES, 0, 3);
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
    }
}
