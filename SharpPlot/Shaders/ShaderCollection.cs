using SharpPlot.Wrappers;

namespace SharpPlot.Shaders;

public static class ShaderCollection
{
    public static ShaderProgram LineShader() 
        => new("Shaders//LineShader.vert", "Shaders//LineShader.frag");
}