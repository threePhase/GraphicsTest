using System;
using System.Runtime.InteropServices;
using Engine.Extensions;
using Interop.Bindings;

namespace Engine {
    public class Geometry {
        public uint[] Indices {get;}
        public float[] Vertices {get;}

        private uint _elementBuffer;
        private DrawingMode _mode;
        private uint _vertexArray;
        private uint _vertexBuffer;

        public Geometry() : this(null, null) {}
        public Geometry(float[] vertices) : this(vertices, null) {}
        public Geometry(float[] vertices,
                        uint[] indices,
                        DrawingMode mode = DrawingMode.Fill) {
            Indices = indices;
            Vertices = vertices;
            _mode = mode;
            setup();
        }

        public void Cleanup() {
            // delete vertex array
            OpenGL.DeleteVertexArrays(1, ref _vertexArray);
            // delete vertex buffer
            OpenGL.DeleteBuffers(1, ref _vertexBuffer);
        }

        public void Draw() {
            OpenGL.BindVertexArray(_vertexArray);
            if (_elementBuffer != 0) {
                OpenGL.DrawElements(OpenGL.GL_TRIANGLES, 6, OpenGL.GL_UNSIGNED_INT, IntPtr.Zero);
            } else {
                OpenGL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 3);
            }
            OpenGL.BindVertexArray(0);
        }

        private void setup() {
            _vertexArray = loadVertexArrayObject();
            _vertexBuffer = loadVertexBufferObject();

            loadVertices(Vertices);

            if (Indices != null) {
                _elementBuffer = loadElementBufferObject();
                loadIndices(Indices);
            }

            // number of components per vertex = 3
            setupDrawing(3);

            setDrawingMode(_mode);

            unbindBuffers();
        }

        // create the OpenGL Vertex Array Object (VAO)
        private uint loadVertexArrayObject() {
            uint vertexArray = 0;
            OpenGL.GenVertexArrays(1, ref vertexArray);
            // bind VAO first, then bind and set vertex buffer(s) and attribute pointer(s).
            OpenGL.BindVertexArray(vertexArray);
            return vertexArray;
        }

        // create the OpenGL Vertex Buffer Object (VBO)
        private uint loadVertexBufferObject() {
            uint vertexBuffer = 0;
            OpenGL.GenBuffers(1, ref vertexBuffer);
            OpenGL.BindBuffer(OpenGL.GL_ARRAY_BUFFER, vertexBuffer);
            return vertexBuffer;
        }

        // load given vertices into VBO
        private void loadVertices(float[] vertices) {
            int size = Marshal.SizeOf(vertices[0]) * vertices.Length;
            IntPtr handle = Marshal.AllocHGlobal(size);
            try {
                Marshal.Copy(vertices, 0, handle, vertices.Length);
                var unmanagedSize = new IntPtr(size);
                OpenGL.BufferData(OpenGL.GL_ARRAY_BUFFER, unmanagedSize, handle, OpenGL.GL_STATIC_DRAW);
            }
            finally {
                Marshal.FreeHGlobal(handle);
            }
        }

        // create the OpenGL Element Buffer Object (EBO)
        private uint loadElementBufferObject() {
            uint elementBuffer = 0;
            OpenGL.GenBuffers(1, ref elementBuffer);
            OpenGL.BindBuffer(OpenGL.GL_ELEMENT_ARRAY_BUFFER, elementBuffer);
            return elementBuffer;
        }

        // load given indices into EBO
        private void loadIndices(uint[] indices) {
            byte[] data = indices.ToByteArray();
            int size = Marshal.SizeOf(data[0]) * data.Length;
            IntPtr handle = Marshal.AllocHGlobal(size);
            try {
                Marshal.Copy(data, 0, handle, data.Length);
                var unmanagedSize = new IntPtr(size);
                OpenGL.BufferData(OpenGL.GL_ELEMENT_ARRAY_BUFFER, unmanagedSize, handle, OpenGL.GL_STATIC_DRAW);
            }
            finally {
                Marshal.FreeHGlobal(handle);
            }
        }

        // setup drawing vertices in currently bound VAO
        private void setupDrawing(int vertex_component_count) {
            var bufferSize = sizeof(float) * vertex_component_count;
            OpenGL.VertexAttribPointer(0, vertex_component_count, OpenGL.GL_FLOAT, false, bufferSize, IntPtr.Zero);
            OpenGL.EnableVertexAttribArray(0);
        }

        private void setDrawingMode(DrawingMode mode) {
            switch (mode) {
                case DrawingMode.Wireframe:
                    OpenGL.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_LINE);
                    break;
                case DrawingMode.Fill:
                    OpenGL.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_FILL);
                    break;
                default:
                    throw new ArgumentException($"Unknown DrawingMode: {mode}");
            }
        }

        // unbind OpenGL Vertex Array Object and OpenGL Vertex Buffer Object
        private void unbindBuffers() {
            OpenGL.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);
            OpenGL.BindVertexArray(0);
        }
    }
}
