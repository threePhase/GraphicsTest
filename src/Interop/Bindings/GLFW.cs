using System;
using System.Runtime.InteropServices;

namespace Interop.Bindings
{
    public static class GLFW
    {
        // library location
        private const string GLFW_LIB = "libs/bin/libglfw";

        // GLFW - window constants
        public const int GLFW_RESIZABLE = 0x00020003;
        public const int GLFW_CONTEXT_VERSION_MAJOR = 0x00022002;
        public const int GLFW_CONTEXT_VERSION_MINOR = 0x00022003;
        public const int GLFW_OPENGL_FORWARD_COMPAT = 0x00022006;
        public const int GLFW_OPENGL_PROFILE = 0x00022008;
        public const int GLFW_OPENGL_CORE_PROFILE = 0x00032001;

        // GLFW - key bindings constants
        public const int GLFW_PRESS = 1;
        public const int GLFW_KEY_ESCAPE = 256;

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
