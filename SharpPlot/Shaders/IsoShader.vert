#version 460 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in float aValue;
out float value;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    gl_Position = vec4(aPos, 1.0) * model * view * projection;
    value = aValue;
}