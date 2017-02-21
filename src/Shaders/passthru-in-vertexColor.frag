#version 330 core
// input variable from the vertex shader (must match name/type)
in vec4 vertexColor;

out vec4 color;

void main() {
    color = vertexColor;
}