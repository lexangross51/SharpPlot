#version 460 core

uniform vec4 lineColor;
out vec4 fragColor;

void main(){
    fragColor = lineColor;
}