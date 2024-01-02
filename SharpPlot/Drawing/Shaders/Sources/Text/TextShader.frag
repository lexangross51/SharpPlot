#version 460 core

in vec2 texCoord;
out vec4 fragmentColor;

uniform sampler2D texture0;

void main() {
    fragmentColor = texture(texture0, texCoord);
}