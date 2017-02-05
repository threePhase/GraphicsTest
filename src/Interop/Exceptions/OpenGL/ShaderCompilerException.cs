using System;

namespace Interop.Exceptions.OpenGL {
    // Exception for OpenGL Shader Compilation Error
    public class ShaderCompileException : Exception {
        public ShaderCompileException() {}
        public ShaderCompileException(string infoLog)
            : base($"Shader Compile Error: \n{infoLog}") {}
        public ShaderCompileException(string message, Exception innerException)
            : base(message, innerException) {}
    }
}
