using System;

namespace Engine.Interfaces {
    public interface IGameEngine {
        void Cleanup();
        IntPtr GetWindow();
        void Initialize();
        void InitializeWindow();
        bool IsRunning();
        void PollEvents();
    }
}
