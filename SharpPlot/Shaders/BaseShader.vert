#version 460 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 inTexCoord;

out vec2 texCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(){
    texCoord = inTexCoord;
    gl_Position = vec4(position, 1.0) * model * view * projection;
}