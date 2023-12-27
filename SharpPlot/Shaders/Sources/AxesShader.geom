#version 460 core

layout(points) in;
layout(line_strip, max_vertices = 40) out;

uniform float startX, endX, stepX;
uniform vec2 screenSize;
uniform int marginPixels;

const int tickSizePixels = 6;

void generateAxisX() {
    float indent = marginPixels / screenSize.x * (endX - startX);
    float tickSize = tickSizePixels / screenSize.x * (endX - startX);
    
    gl_Position = vec4(indent, indent, 0.0, 1.0);
    EmitVertex();
    gl_Position = vec4(endX, indent, 0.0, 1.0);
    EmitVertex();
    EndPrimitive();
    
    for (float i = startX + stepX; i < endX; i += stepX) {
        gl_Position = vec4(i, indent - tickSize, 0.0, 1.0);
        EmitVertex();
        gl_Position = vec4(i, indent + tickSize, 0.0, 1.0);
        EmitVertex();
        EndPrimitive();
    }
}

void main() {
    generateAxisX();
}