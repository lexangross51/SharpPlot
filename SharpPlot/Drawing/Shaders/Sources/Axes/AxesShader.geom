#version 460 core

layout(lines) in;
layout(line_strip, max_vertices = 80) out;

uniform float stepX, stepY, hRatio, vRatio;
uniform int isHorizontal;

const int shortIntervals = 5;
const int longTickSize = 4;
const int shortTickSize = 2;

void drawHorizontalTicks(){
    float shortStep = stepX / shortIntervals;

    for (float i = gl_in[0].gl_Position.x; i < gl_in[1].gl_Position.x; i += stepX) {
        gl_Position = vec4(i, gl_in[0].gl_Position.y - longTickSize * vRatio, 0.0, 1.0);
        EmitVertex();
        gl_Position = vec4(i, gl_in[0].gl_Position.y + longTickSize * vRatio, 0.0, 1.0);
        EmitVertex();
        EndPrimitive();
        
        for (float j = i + shortStep; j < i + stepX; j += shortStep) {
            gl_Position = vec4(j, gl_in[0].gl_Position.y - shortTickSize * vRatio, 0.0, 1.0);
            EmitVertex();
            gl_Position = vec4(j, gl_in[0].gl_Position.y + shortTickSize * vRatio, 0.0, 1.0);
            EmitVertex();
            EndPrimitive();
        }
    }
}

void drawVerticalTicks() {
    float shortStep = stepY / shortIntervals;

    for (float i = gl_in[0].gl_Position.y; i < gl_in[1].gl_Position.y; i += stepY) {
        gl_Position = vec4(gl_in[0].gl_Position.x - longTickSize * hRatio, i, 0.0, 1.0);
        EmitVertex();
        gl_Position = vec4(gl_in[0].gl_Position.x + longTickSize * hRatio, i, 0.0, 1.0);
        EmitVertex();
        EndPrimitive();
        
        for (float j = i + shortStep; j < i + stepY; j += shortStep) {
            gl_Position = vec4(gl_in[0].gl_Position.x - shortTickSize * hRatio, j, 0.0, 1.0);
            EmitVertex();
            gl_Position = vec4(gl_in[0].gl_Position.x + shortTickSize * hRatio, j, 0.0, 1.0);
            EmitVertex();
            EndPrimitive();
        }
    }
}

void main() {
    if (isHorizontal == 1) {
        drawHorizontalTicks();
        return;
    }
    
    drawVerticalTicks();
}