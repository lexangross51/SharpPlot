#version 460 core

in vec2 texCoord;
out vec4 fragColor;

uniform vec4 vertexColor;

uniform sampler2D texture0;
uniform sampler2D texture1;

void main(){
    fragColor = mix(texture(texture0, texCoord), texture(texture1, texCoord), 0.2f);
}