namespace Engine {
    public class Shader {
        private readonly string _path;
        public string Path {
            get {
                return _path;
            }
        }

        public Shader(string path) {
            _path = path;
        }
    }
}
