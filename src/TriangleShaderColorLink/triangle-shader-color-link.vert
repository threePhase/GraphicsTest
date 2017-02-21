#version 330 core
// the position variable has attribute position 0
layout (location = 0) in vec3 position;

// specify a color to output to fragment shader
out vec4 vertexColor;

void main() {
    // leveraging constructor which uses vec3
    gl_Position = vec4(position, 1.0);
    // set the color to dark-red
    vertexColor = vec4(0.5f, 0.0f, 0.0f, 1.0f);
}