#version 460 core

layout(points) in;
layout(line_strip, max_vertices = 8) out;

uniform int isToDrawGrid;

void drawBox() {
    // bottom line
    gl_Position = vec4(-1.0, -0.9995, 0.0, 1.0);
    EmitVertex();
    gl_Position = vec4(1.0, -0.9995, 0.0, 1.0);
    EmitVertex();
    EndPrimitive();
    
    // top line
    gl_Position = vec4(-1.0, 1.0, 0.0, 1.0);
    EmitVertex();
    gl_Position = vec4(1.0, 1.0, 0.0, 1.0);
    EmitVertex();
    EndPrimitive();
    
    // left line
    gl_Position = vec4(-0.9995, -1.0, 0.0, 1.0);
    EmitVertex();
    gl_Position = vec4(-0.9995, 1.0, 0.0, 1.0);
    EmitVertex();
    EndPrimitive();
    
    // right line
    gl_Position = vec4(0.9999, -1.0, 0.0, 1.0);
    EmitVertex();
    gl_Position = vec4(0.9999, 1.0, 0.0, 1.0);
    EmitVertex();
    EndPrimitive();    
}

void drawGrid() {
    return;
}

void main() {
    drawBox();
    
    if (isToDrawGrid == 1) {
        drawGrid();
    }
}