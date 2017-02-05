using System;
using System.Runtime.InteropServices;

namespace Interop.Bindings
{
    public static class OpenGL
    {
        // library location
        private const string OPENGL_LIB = "/System/Library/Frameworks/OpenGL.framework/Versions/A/Libraries/libGL.dylib";

        // OpenGL constants
        // TODO: Source these from deps/glad/glad.h
        public const int GL_TRIANGLES = 0x0004;
        public const int GL_FLOAT = 0x1406;
        public const int GL_COLOR_BUFFER_BIT = 0x4000;
        public const int GL_ARRAY_BUFFER = 0x8892;
        public const int GL_STATIC_DRAW = 0x88e4;
        public const int GL_FRAGMENT_SHADER = 0x8b30;
        public const int GL_VERTEX_SHADER = 0x8b31;
        public const int GL_COMPILE_STATUS = 0x8b81;
        public const int GL_LINK_STATUS = 0x8b82;

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
        public delegate void glViewport(int x, int y, int width, int height);
        public delegate void glUseProgram(uint program);
        public delegate void glDeleteShader(uint shader);
        public delegate void glGetShaderiv(uint shader, uint pname, ref int parameters);
        public delegate void glGetShaderInfoLog(uint shader, int maxSize, IntPtr length, byte[] infoLog);
        public delegate void glGetProgramiv(uint program, uint pname, ref int parameters);
        public delegate void glGetProgramInfoLog(uint program, int bufSize, IntPtr length, byte[] infoLog);
        public delegate void glDeleteVertexArrays(int n, ref uint arrays);
        public delegate void glDeleteBuffers(int n, ref uint buffers);

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
        public static glViewport Viewport = GLFW.GetMethod<glViewport>();
        public static glUseProgram UseProgram = GLFW.GetMethod<glUseProgram>();
        public static glDeleteShader DeleteShader = GLFW.GetMethod<glDeleteShader>();
        public static glGetShaderiv GetShaderiv = GLFW.GetMethod<glGetShaderiv>();
        public static glGetShaderInfoLog GetShaderInfoLog = GLFW.GetMethod<glGetShaderInfoLog>();
        public static glGetProgramiv GetProgramiv = GLFW.GetMethod<glGetProgramiv>();
        public static glGetProgramInfoLog GetProgramInfoLog = GLFW.GetMethod<glGetProgramInfoLog>();
        public static glDeleteVertexArrays DeleteVertexArrays = GLFW.GetMethod<glDeleteVertexArrays>();
        public static glDeleteBuffers DeleteBuffers = GLFW.GetMethod<glDeleteBuffers>();
    }
}
