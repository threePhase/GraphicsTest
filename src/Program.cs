using System;
using System.Runtime.InteropServices;

// heavily inspired by: https://github.com/CameronAavik/dotnetcore-graphics-example
// as well as: https://learnopengl.com/#!Getting-started/Hello-Triangle
namespace GraphicsTest
{
    public class Program
    {
        // external libraries
        private const string OPENGL_LIB = "/System/Library/Frameworks/OpenGL.framework/Versions/A/Libraries/libGL.dylib";
        private const string GLFW_LIB = "libs/bin/libglfw";

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
               SetWindowShouldClose(window, true);
            }
        }

        private static void onError(int error, string description) {
            Console.WriteLine($"GLFW Error: {description}");
        }

        public static void Main(string[] args) {
            Console.WriteLine("This is a simple drawing test using GLFW and OpenGL 3.3!");

            SetErrorCallback(onError);

            if (!Initialize()) {
                Console.WriteLine($"Failed to initialize GLFW.");
                return;
            }

            WindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
            WindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
            WindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
            // required for getting OpenGL context >= 3.2 on MacOS >= OS X 10.7
            // http://stackoverflow.com/a/9017716
            WindowHint(GLFW_OPENGL_FORWARD_COMPAT, 1);
            WindowHint(GLFW_RESIZABLE, 0);

            var window = CreateWindow(640, 480, "GLFW Drawing Test", IntPtr.Zero, IntPtr.Zero);
            if (window == IntPtr.Zero) {
                // TODO: Tear down GLFW here (since we initialized above)?
                Console.WriteLine($"Unable to load create window.");
                return;
            }

            MakeContextCurrent(window);

            SetKeyCallback(window, keyBindings);

            int width = 0,
                height = 0;
            GetFramebufferSize(window, ref width, ref height);
            Viewport(0, 0, width, height);

            // create the OpenGL Vertex Shader Object
            uint vertexShader = CreateShader(GL_VERTEX_SHADER);
            ShaderSource(vertexShader, 1, ref _vertexShaderText, IntPtr.Zero);
            CompileShader(vertexShader);
            int success = 0;
            string infoLog = "";
            // check for shader compile errors
            GetShaderiv(vertexShader, GL_COMPILE_STATUS, ref success);
            if (success == 0) {
                GetShaderInfoLog(vertexShader, 512, IntPtr.Zero, infoLog);
                Console.WriteLine($"Shader Compile Error: \n{infoLog}");
            }

            // create the OpenGL Fragment Shader Object
            uint fragmentShader = CreateShader(GL_FRAGMENT_SHADER);
            ShaderSource(fragmentShader, 1, ref _fragmentShaderText, IntPtr.Zero);
            CompileShader(fragmentShader);
            // check for shader compile errors
            GetShaderiv(fragmentShader, GL_COMPILE_STATUS, ref success);
            if (success == 0) {
                GetShaderInfoLog(fragmentShader, 512, IntPtr.Zero, infoLog);
                Console.WriteLine($"Shader Compile Error: \n{infoLog}");
            }

            var program = CreateProgram();
            AttachShader(program, vertexShader);
            AttachShader(program, fragmentShader);
            LinkProgram(program);
            // check for linking errors
            GetProgramiv(program, GL_LINK_STATUS, ref success);
            if (success == 0) {
                GetProgramInfoLog(program, 512, IntPtr.Zero, infoLog);
                Console.WriteLine($"Linking Error: \n{infoLog}");
            }

            DeleteShader(vertexShader);
            DeleteShader(fragmentShader);

            float[] vertices = {
                0.0f,  0.5f, 0.0f, // top right
                0.5f, -0.5f, 0.0f, // bottom right
               -0.5f, -0.5f, 0.0f // bottom left
            };
            /*
            float[] vertices = {
                 0.5f,  0.5f, 0.0f, // top right
                 0.5f, -0.5f, 0.0f, // bottom right
                -0.5f, -0.5f, 0.0f, // bottom left
                -0.5f,  0.5f, 0.0f // top left
            };

            uint[] indices = {
                0, 1, 3, // first triangle
                1, 2, 3  // second triangle
            };
            */

            // create the OpenGL Vertex Array Object (VAO)
            uint vertexArray = 0;
            GenVertexArrays(1, ref vertexArray);
            // create the OpenGL Vertex Buffer Object (VBO)
            uint vertexBuffer = 0;
            GenBuffers(1, ref vertexBuffer);

            // bind VAO first, then bind and set vertex buffer(s) and attribute pointer(s).
            BindVertexArray(vertexArray);

            BindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
            var verticesSize = new IntPtr(sizeof(float) * vertices.Length);
            BufferData(GL_ARRAY_BUFFER, verticesSize, vertices, GL_STATIC_DRAW);

            // create the OpenGL Element Buffer Object
            // uint elementBuffer = 0;
            // GenBuffers(1, ref elementBuffer);
            // BindBuffer(GL_ELEMENT_ARRAY_BUFFER, elementBuffer);
            // var indicesSize = new IntPtr(sizeof(uint) * indices.Length);
            // BufferData(GL_ELEMENT_ARRAY_BUFFER, indicesSize, indices, GL_STATIC_DRAW);

            var bufferSize = sizeof(float) * 3;
            VertexAttribPointer(0, 3, GL_FLOAT, false, bufferSize, IntPtr.Zero);
            EnableVertexAttribArray(0);

            // // unbind OpenGL Vertex Buffer Object
            BindBuffer(GL_ARRAY_BUFFER, 0);
            // // unbind OpenGL Vertex Array Object
            BindVertexArray(0);

            while (WindowShouldClose(window) == 0) {
                PollEvents();

                // red, green, and blue values range from 0 (0) - 255 (1)
                ClearColor(1/255.0f, 113/255.0f, 187/255.0f, 1.0f);
                Clear(GL_COLOR_BUFFER_BIT);

                UseProgram(program);
                BindVertexArray(vertexArray);
                DrawArrays(GL_TRIANGLES, 0, 3);
                // DrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, IntPtr.Zero);
                BindVertexArray(0);

                SwapBuffers(window);
            }
            // delete vertex array
            DeleteVertexArrays(1, ref vertexArray);
            // delete vertex buffer
            DeleteBuffers(1, ref vertexBuffer);
            // delete element buffer
            // DeleteBuffers(1, ref elementBuffer);

            // terminate GLFW, clearing any resources allocated by GLFW
            Terminate();
        }

        // OpenGL bindings
        [DllImport(OPENGL_LIB, EntryPoint="glDrawArrays")]
        private static extern void DrawArrays(int mode, int first, int count);
        // OpenGL pointers
        private delegate void glGenBuffers(int n, ref uint buffers);
        private delegate void glBindBuffer(uint target, uint buffer);
        private delegate void glBufferData(uint target, IntPtr size, float[] data, uint usage);
        // private delegate void glBufferData(uint target, IntPtr size, uint[] data, uint usage);
        private delegate void glEnableVertexAttribArray(int index);
        private delegate void glVertexAttribPointer(uint indx, int size, uint type, bool normalized, int stride, IntPtr ptr);
        private delegate void glGenVertexArrays(int n, ref uint arrays);
        private delegate void glBindVertexArray(uint array);
        private delegate void glClearColor(float r, float g, float b, float a);
        private delegate void glClear(int mask);
        private delegate uint glCreateProgram();
        private delegate void glLinkProgram(uint program);
        private delegate void glAttachShader(uint program, uint shader);
        private delegate void glCompileShader(uint shader);
        private delegate uint glCreateShader(uint type);
        private delegate void glShaderSource(uint shader, int count, ref string str, IntPtr length);
        private delegate int glGetUniformLocation(uint program, string name);
        private delegate int glGetAttribLocation(uint program, string name);
        private delegate void glViewport(int x, int y, int width, int height);
        private delegate void glUseProgram(uint program);
        private delegate void glDeleteShader(uint shader);
        private delegate void glGetShaderiv(uint shader, uint pname, ref int parameters);
        private delegate void glGetShaderInfoLog(uint shader, int bufSize, IntPtr length, string infoLog);
        private delegate void glGetProgramiv(uint program, uint pname, ref int parameters);
        private delegate void glGetProgramInfoLog(uint program, int bufSize, IntPtr length, string infoLog);
        private delegate void glDeleteVertexArrays(int n, ref uint arrays);
        private delegate void glDeleteBuffers(int n, ref uint buffers);
        private delegate void glDrawElements(int mode, int first, int count, IntPtr indices);


        // OpenGL Methods
        private static glGenBuffers GenBuffers = GetMethod<glGenBuffers>();
        private static glBindBuffer BindBuffer = GetMethod<glBindBuffer>();
        private static glBufferData BufferData = GetMethod<glBufferData>();
        private static glEnableVertexAttribArray EnableVertexAttribArray = GetMethod<glEnableVertexAttribArray>();
        private static glVertexAttribPointer VertexAttribPointer = GetMethod<glVertexAttribPointer>();
        private static glGenVertexArrays GenVertexArrays = GetMethod<glGenVertexArrays>();
        private static glBindVertexArray BindVertexArray = GetMethod<glBindVertexArray>();
        private static glClearColor ClearColor = GetMethod<glClearColor>();
        private static glClear Clear = GetMethod<glClear>();
        private static glCreateProgram CreateProgram = GetMethod<glCreateProgram>();
        private static glLinkProgram LinkProgram = GetMethod<glLinkProgram>();
        private static glAttachShader AttachShader = GetMethod<glAttachShader>();
        private static glCompileShader CompileShader = GetMethod<glCompileShader>();
        private static glCreateShader CreateShader = GetMethod<glCreateShader>();
        private static glShaderSource ShaderSource = GetMethod<glShaderSource>();
        private static glGetUniformLocation GetUniformLocation = GetMethod<glGetUniformLocation>();
        private static glGetAttribLocation GetAttribLocation = GetMethod<glGetAttribLocation>();
        private static glViewport Viewport = GetMethod<glViewport>();
        private static glUseProgram UseProgram = GetMethod<glUseProgram>();
        private static glDeleteShader DeleteShader = GetMethod<glDeleteShader>();
        private static glGetShaderiv GetShaderiv = GetMethod<glGetShaderiv>();
        private static glGetShaderInfoLog GetShaderInfoLog = GetMethod<glGetShaderInfoLog>();
        private static glGetProgramiv GetProgramiv = GetMethod<glGetProgramiv>();
        private static glGetProgramInfoLog GetProgramInfoLog = GetMethod<glGetProgramInfoLog>();
        private static glDeleteVertexArrays DeleteVertexArrays = GetMethod<glDeleteVertexArrays>();
        private static glDeleteBuffers DeleteBuffers = GetMethod<glDeleteBuffers>();
        private static glDrawElements DrawElements = GetMethod<glDrawElements>();

        // GLFW bindings
        [DllImport(GLFW_LIB, EntryPoint="glfwInit")]
        private static extern bool Initialize();
        [DllImport(GLFW_LIB, EntryPoint="glfwWindowHint")]
        private static extern void WindowHint(int hint, int value);
        [DllImport(GLFW_LIB, EntryPoint="glfwCreateWindow")]
        private static extern IntPtr CreateWindow(int width, int height, string title, IntPtr monitor, IntPtr share);
        [DllImport(GLFW_LIB, EntryPoint="glfwMakeContextCurrent")]
        private static extern void MakeContextCurrent(IntPtr window);
        [DllImport(GLFW_LIB, EntryPoint="glfwSwapBuffers")]
        private static extern void SwapBuffers(IntPtr window);
        [DllImport(GLFW_LIB, EntryPoint="glfwGetProcAddress")]
        private static extern IntPtr GetProcAddress(string procname);
        [DllImport(GLFW_LIB, EntryPoint="glfwPollEvents")]
        private static extern void PollEvents();
        [DllImport(GLFW_LIB, EntryPoint="glfwSetErrorCallback")]
        private static extern void SetErrorCallback(ErrorCallback callback);
        [DllImport(GLFW_LIB, EntryPoint="glfwSetKeyCallback")]
        private static extern void SetKeyCallback(IntPtr window, KeyCallback callback);
        [DllImport(GLFW_LIB, EntryPoint="glfwSetWindowShouldClose")]
        private static extern void SetWindowShouldClose(IntPtr window, bool value);
        [DllImport(GLFW_LIB, EntryPoint="glfwWindowShouldClose")]
        private static extern int WindowShouldClose(IntPtr window);
        [DllImport(GLFW_LIB, EntryPoint="glfwGetFramebufferSize")]
        private static extern void GetFramebufferSize(IntPtr window, ref int width, ref int size);
        [DllImport(GLFW_LIB, EntryPoint="glfwTerminate")]
        private static extern void Terminate();

        // GLFW supporting delegates
        private delegate void ErrorCallback(int error, string description);
        private delegate void KeyCallback(IntPtr window, int key, int scancode, int action, int mode);

        private static T GetMethod<T>() {
            var pointer = GetProcAddress(typeof(T).Name);
            if (pointer == IntPtr.Zero) {
                Console.WriteLine($"Unable to load function pointer: {typeof(T).Name}");
                return default(T);
            }
            return Marshal.GetDelegateForFunctionPointer<T>(pointer);
        }
    }
}
