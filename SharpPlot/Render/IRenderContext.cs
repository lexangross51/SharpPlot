using SharpPlot.Viewport;

namespace SharpPlot.Render;

public struct RenderSettings
{
    public ScreenSize ScreenSize { get; set; }
    public Indent Indent { get; set; }
}

public interface IRenderContext
{
    void DrawPoints();
    void DrawLines();
    void DrawTriangles();
    void DrawQuadrilaterals();
    
    void Clear();
}