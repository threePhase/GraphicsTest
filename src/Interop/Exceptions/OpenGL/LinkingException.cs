using System;

namespace Interop.Exceptions.OpenGL {
    // Exception for OpenGL Shader Compilation Error
    public class LinkingException : Exception {
        public LinkingException() {}
        public LinkingException(string infoLog)
            : base($"Linking Error: \n{infoLog}") {}
        public LinkingException(string message, Exception innerException)
            : base(message, innerException) {}
    }
}
