using System;
using Interop.Bindings;
using Engine.Interfaces;

namespace Engine {
    public class DemoGameEngine : IGameEngine {
        private IntPtr _window;

        public DemoGameEngine() {
            Initialize();
        }

        public void Cleanup() {
            // terminate GLFW, clearing any resources allocated by GLFW
            GLFW.Terminate();
        }

        public IntPtr GetWindow() {
            return _window;
        }

        public void Initialize() {
            GLFW.SetErrorCallback(OnError);

            if (!GLFW.Initialize()) {
                Console.WriteLine($"Failed to initialize GLFW.");
                return;
            }

            InitializeWindow();

            GLFW.SetKeyCallback(_window, KeyPressCallback);
        }

        public bool IsRunning() {
            return GLFW.WindowShouldClose(_window) == 0;
        }

        public void InitializeWindow() {
            var possibleWindow = createWindow("GLFW Drawing Test");
            if (!possibleWindow.HasValue) {
                Console.WriteLine($"Unable to load create window.");
                GLFW.Terminate();
                return;
            }

            _window = possibleWindow.Value;

            GLFW.MakeContextCurrent(_window);
            setupViewport(_window);
        }

        public void KeyPressCallback(IntPtr window,
                                     int key,
                                     int scancode,
                                     int action,
                                     int mode) {
            if (key == GLFW.GLFW_KEY_ESCAPE && action == GLFW.GLFW_PRESS) {
                GLFW.SetWindowShouldClose(window, true);
            }
        }

        public void OnError(int error, string description) {
            Console.WriteLine($"GLFW Error: {description}");
        }

        public void PollEvents() {
            GLFW.PollEvents();
        }

        private IntPtr? createWindow(string windowName) {
            GLFW.WindowHint(GLFW.GLFW_CONTEXT_VERSION_MAJOR, 3);
            GLFW.WindowHint(GLFW.GLFW_CONTEXT_VERSION_MINOR, 3);
            GLFW.WindowHint(GLFW.GLFW_OPENGL_PROFILE, GLFW.GLFW_OPENGL_CORE_PROFILE);
            // required for getting OpenGL context >= 3.2 on MacOS >= OS X 10.7
            // http://stackoverflow.com/a/9017716
            GLFW.WindowHint(GLFW.GLFW_OPENGL_FORWARD_COMPAT, 1);
            GLFW.WindowHint(GLFW.GLFW_RESIZABLE, 0);

            _window = GLFW.CreateWindow(640, 480, windowName, IntPtr.Zero, IntPtr.Zero);
            if (_window == IntPtr.Zero) {
                return null;
            }

            return _window;
        }

        private void setupViewport(IntPtr window) {
            int width = 0,
                height = 0;
            GLFW.GetFramebufferSize(window, ref width, ref height);
            OpenGL.Viewport(0, 0, width, height);
        }
    }
}
