namespace Engine {
    public class Geometry {
        public uint[] Indices {get;}
        public float[] Vertices {get;}

        public Geometry() : this(null, null) {}
        public Geometry(float[] vertices) : this(vertices, null) {}
        public Geometry(float[] vertices, uint[] indices) {
            Indices = indices;
            Vertices = vertices;
        }
    }
}
