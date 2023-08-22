#version 460 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texPosition;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec2 texCoord;

void main(){
    texCoord = vec2(texPosition.x, 1.0f - texPosition.y);
    gl_Position = vec4(position, 1.0) * model * view * projection;
}