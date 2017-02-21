using System;
using System.Collections.Generic;

namespace Engine.Interfaces {
    public interface IRenderer {
        void Cleanup();
        void DrawScene();
        void SetupGeometry(IEnumerable<Geometry> geometry);
    }
}
