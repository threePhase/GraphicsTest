using System;
using System.IO;
using Engine.Interfaces;
using Interop.Bindings;
using Interop.Exceptions.OpenGL;

namespace Engine {
    public class Shader {
        private readonly uint _program;

        public uint Program {
            get {
                return _program;
            }
        }

        public Shader(string vertexShaderPath, string fragmentShaderPath) {
            _program = createProgram(vertexShaderPath, fragmentShaderPath);
        }

        private uint createProgram(string vertexShaderPath, string fragmentShaderPath) {
            string vertexShaderText = loadShaderFile(vertexShaderPath);
            // create the OpenGL Vertex Shader Object
            uint vertexShader =
                createShader(OpenGL.GL_VERTEX_SHADER, vertexShaderText);

            string fragmentShaderText = loadShaderFile(fragmentShaderPath);
            // create the OpenGL Fragment Shader Object
            uint fragmentShader = 0;
            try {
                fragmentShader = createShader(OpenGL.GL_FRAGMENT_SHADER,
                                              fragmentShaderText);
            }
            catch (ShaderCompileException e) {
                // clean up vertex shader before passing exception to client
                OpenGL.DeleteShader(vertexShader);
                // Console.WriteLine(e.Message);
                throw e;
            }

            uint program = 0;
            try {
                program = createProgram(vertexShader, fragmentShader);
            }
            catch (LinkingException e) {
                // clean up vertex and fragment shader before passing exception to client
                OpenGL.DeleteShader(vertexShader);
                OpenGL.DeleteShader(fragmentShader);
                // Console.WriteLine(e.Message);
                throw e;
            }

            OpenGL.DeleteShader(vertexShader);
            OpenGL.DeleteShader(fragmentShader);

            return program;
        }

        private string loadShaderFile(string shaderPath) {
            var location = System.Reflection.Assembly.GetEntryAssembly().Location;
            string path = Path.GetDirectoryName(location);
            return File.ReadAllText($"{path}/{shaderPath}");
        }

        private uint createShader(uint type, string shaderText) {
            uint shader = OpenGL.CreateShader(type);
            OpenGL.ShaderSource(shader, 1, ref shaderText, IntPtr.Zero);
            OpenGL.CompileShader(shader);
            int success = 0;
            byte[] infoLog = new byte[512];
            // check for shader compile errors
            OpenGL.GetShaderiv(shader, OpenGL.GL_COMPILE_STATUS, ref success);
            if (success == 0) {
                OpenGL.GetShaderInfoLog(shader, infoLog.Length, IntPtr.Zero, infoLog);
                string log = System.Text.Encoding.UTF8.GetString(infoLog);
                throw new ShaderCompileException(log);
            }
            return shader;
        }

        // TODO: rename to createShaderProgram
        private uint createProgram(uint vertexShader, uint fragmentShader) {
            uint program = OpenGL.CreateProgram();
            OpenGL.AttachShader(program, vertexShader);
            OpenGL.AttachShader(program, fragmentShader);
            OpenGL.LinkProgram(program);
            // check for linking errors
            int success = 0;
            byte[] infoLog = new byte[512];
            OpenGL.GetProgramiv(program, OpenGL.GL_LINK_STATUS, ref success);
            if (success == 0) {
                OpenGL.GetProgramInfoLog(program, infoLog.Length, IntPtr.Zero, infoLog);
                string log = System.Text.Encoding.UTF8.GetString(infoLog);
                throw new LinkingException(log);
            }
            return program;
        }
    }
}
