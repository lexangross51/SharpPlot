using SharpPlot.Wrappers;

namespace SharpPlot.Shaders;

public static class ShaderCollection
{
    public static ShaderProgram LineShader() 
        => new("Shaders//LineShader.vert", "Shaders//LineShader.frag");

    public static ShaderProgram TextShader()
        => new("Shaders//TextShader.vert", "Shaders//TextShader.frag");

    public static ShaderProgram FieldShader()
        => new("Shaders//FieldShader.vert", "Shaders//FieldShader.frag");
    
    public static ShaderProgram IsolineShader()
        => new("Shaders//IsoShader.vert", "Shaders//IsoShader.frag");
}