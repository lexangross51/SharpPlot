#version 460 core

in float value;
out vec4 fragColor;

uniform float minValue;
uniform float maxValue;
uniform vec4 palette[40];
uniform int colorsCount;
uniform int valuesRangeCount;

void main()
{
    float stepV = (maxValue - minValue) / valuesRangeCount;
    float stepP = (maxValue - minValue) / colorsCount;
    
    for (int i = 0; i < valuesRangeCount; i++) {
        float lower = minValue + i * stepV;
        float upper = minValue + (i + 1) * stepV;
        float mean = (lower + upper) / 2.0f;
        
        if (value < upper) {
            for (int j = 0; j < colorsCount; j++) {
                vec4 color1 = palette[colorsCount - 1 - j];
                vec4 color2 = palette[j == colorsCount - 1 ? 0 : colorsCount - 2 - j];
                
                float pMin = minValue + j * stepP;
                float pMax = minValue + (j + 1) * stepP;
                
                if (mean <= pMax) {
                    float t = (mean - pMin) / (pMax - pMin);
                    float r = color1.r + (color2.r - color1.r) * t;
                    float g = color1.g + (color2.g - color1.g) * t;
                    float b = color1.b + (color2.b - color1.b) * t;
                    
                    fragColor = vec4(r, g, b, 1.0f);
                    return;
                }
            }
        }
    }
}