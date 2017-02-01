using System;
using System.Runtime.InteropServices;

namespace Interop.Bindings
{
    public static class OpenGL
    {
        // library location
        private const string OPENGL_LIB = "/System/Library/Frameworks/OpenGL.framework/Versions/A/Libraries/libGL.dylib";

        // OpenGL bindings
        [DllImport(OPENGL_LIB, EntryPoint="glDrawArrays")]
        public static extern void DrawArrays(int mode, int first, int count);

        // OpenGL pointers
        public delegate void glGenBuffers(int n, ref uint buffers);
        public delegate void glBindBuffer(uint target, uint buffer);
        public delegate void glBufferData(uint target, IntPtr size, float[] data, uint usage);
        public delegate void glEnableVertexAttribArray(int index);
        public delegate void glVertexAttribPointer(uint indx, int size, uint type, bool normalized, int stride, IntPtr ptr);
        public delegate void glGenVertexArrays(int n, ref uint arrays);
        public delegate void glBindVertexArray(uint array);
        public delegate void glClearColor(float r, float g, float b, float a);
        public delegate void glClear(int mask);
        public delegate uint glCreateProgram();
        public delegate void glLinkProgram(uint program);
        public delegate void glAttachShader(uint program, uint shader);
        public delegate void glCompileShader(uint shader);
        public delegate uint glCreateShader(uint type);
        public delegate void glShaderSource(uint shader, int count, ref string str, IntPtr length);
        public delegate int glGetUniformLocation(uint program, string name);
        public delegate int glGetAttribLocation(uint program, string name);
        public delegate void glViewport(int x, int y, int width, int height);
        public delegate void glUseProgram(uint program);
        public delegate void glDeleteShader(uint shader);
        public delegate void glGetShaderiv(uint shader, uint pname, ref int parameters);
        public delegate void glGetShaderInfoLog(uint shader, int bufSize, IntPtr length, string infoLog);
        public delegate void glGetProgramiv(uint program, uint pname, ref int parameters);
        public delegate void glGetProgramInfoLog(uint program, int bufSize, IntPtr length, string infoLog);
        public delegate void glDeleteVertexArrays(int n, ref uint arrays);
        public delegate void glDeleteBuffers(int n, ref uint buffers);
        public delegate void glDrawElements(int mode, int first, int count, IntPtr indices);

        // OpenGL Methods
        public static glGenBuffers GenBuffers = GLFW.GetMethod<glGenBuffers>();
        public static glBindBuffer BindBuffer = GLFW.GetMethod<glBindBuffer>();
        public static glBufferData BufferData = GLFW.GetMethod<glBufferData>();
        public static glEnableVertexAttribArray EnableVertexAttribArray = GLFW.GetMethod<glEnableVertexAttribArray>();
        public static glVertexAttribPointer VertexAttribPointer = GLFW.GetMethod<glVertexAttribPointer>();
        public static glGenVertexArrays GenVertexArrays = GLFW.GetMethod<glGenVertexArrays>();
        public static glBindVertexArray BindVertexArray = GLFW.GetMethod<glBindVertexArray>();
        public static glClearColor ClearColor = GLFW.GetMethod<glClearColor>();
        public static glClear Clear = GLFW.GetMethod<glClear>();
        public static glCreateProgram CreateProgram = GLFW.GetMethod<glCreateProgram>();
        public static glLinkProgram LinkProgram = GLFW.GetMethod<glLinkProgram>();
        public static glAttachShader AttachShader = GLFW.GetMethod<glAttachShader>();
        public static glCompileShader CompileShader = GLFW.GetMethod<glCompileShader>();
        public static glCreateShader CreateShader = GLFW.GetMethod<glCreateShader>();
        public static glShaderSource ShaderSource = GLFW.GetMethod<glShaderSource>();
        public static glGetUniformLocation GetUniformLocation = GLFW.GetMethod<glGetUniformLocation>();
        public static glGetAttribLocation GetAttribLocation = GLFW.GetMethod<glGetAttribLocation>();
        public static glViewport Viewport = GLFW.GetMethod<glViewport>();
        public static glUseProgram UseProgram = GLFW.GetMethod<glUseProgram>();
        public static glDeleteShader DeleteShader = GLFW.GetMethod<glDeleteShader>();
        public static glGetShaderiv GetShaderiv = GLFW.GetMethod<glGetShaderiv>();
        public static glGetShaderInfoLog GetShaderInfoLog = GLFW.GetMethod<glGetShaderInfoLog>();
        public static glGetProgramiv GetProgramiv = GLFW.GetMethod<glGetProgramiv>();
        public static glGetProgramInfoLog GetProgramInfoLog = GLFW.GetMethod<glGetProgramInfoLog>();
        public static glDeleteVertexArrays DeleteVertexArrays = GLFW.GetMethod<glDeleteVertexArrays>();
        public static glDeleteBuffers DeleteBuffers = GLFW.GetMethod<glDeleteBuffers>();
        public static glDrawElements DrawElements = GLFW.GetMethod<glDrawElements>();
    }

    public static class GLFW
    {
        // library location
        private const string GLFW_LIB = "libs/bin/libglfw";

        // GLFW supporting delegates
        public delegate void ErrorCallback(int error, string description);
        public delegate void KeyCallback(IntPtr window, int key, int scancode, int action, int mode);

        // GLFW bindings
        [DllImport(GLFW_LIB, EntryPoint="glfwInit")]
        public static extern bool Initialize();
        [DllImport(GLFW_LIB, EntryPoint="glfwWindowHint")]
        public static extern void WindowHint(int hint, int value);
        [DllImport(GLFW_LIB, EntryPoint="glfwCreateWindow")]
        public static extern IntPtr CreateWindow(int width, int height, string title, IntPtr monitor, IntPtr share);
        [DllImport(GLFW_LIB, EntryPoint="glfwMakeContextCurrent")]
        public static extern void MakeContextCurrent(IntPtr window);
        [DllImport(GLFW_LIB, EntryPoint="glfwSwapBuffers")]
        public static extern void SwapBuffers(IntPtr window);
        [DllImport(GLFW_LIB, EntryPoint="glfwPollEvents")]
        public static extern void PollEvents();
        [DllImport(GLFW_LIB, EntryPoint="glfwSetErrorCallback")]
        public static extern void SetErrorCallback(ErrorCallback callback);
        [DllImport(GLFW_LIB, EntryPoint="glfwSetKeyCallback")]
        public static extern void SetKeyCallback(IntPtr window, KeyCallback callback);
        [DllImport(GLFW_LIB, EntryPoint="glfwSetWindowShouldClose")]
        public static extern void SetWindowShouldClose(IntPtr window, bool value);
        [DllImport(GLFW_LIB, EntryPoint="glfwWindowShouldClose")]
        public static extern int WindowShouldClose(IntPtr window);
        [DllImport(GLFW_LIB, EntryPoint="glfwGetFramebufferSize")]
        public static extern void GetFramebufferSize(IntPtr window, ref int width, ref int size);
        [DllImport(GLFW_LIB, EntryPoint="glfwTerminate")]
        public static extern void Terminate();

        [DllImport(GLFW_LIB, EntryPoint="glfwGetProcAddress")]
        private static extern IntPtr GetProcAddress(string procname);

        public static T GetMethod<T>() {
            var pointer = GetProcAddress(typeof(T).Name);
            if (pointer == IntPtr.Zero) {
                Console.WriteLine($"Unable to load function pointer: {typeof(T).Name}");
                return default(T);
            }
            return Marshal.GetDelegateForFunctionPointer<T>(pointer);
        }
    }
}
