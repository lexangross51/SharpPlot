#version 460 core

layout (location = 0) in vec3 position;

out vec4 vertexColor;

uniform vec4 color;
uniform mat4 modelView;
uniform mat4 projection;

void main() {
    vertexColor = color;
    gl_Position = vec4(position, 1.0f) * modelView * projection;
}