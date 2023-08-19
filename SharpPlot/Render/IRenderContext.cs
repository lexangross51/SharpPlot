using SharpPlot.Viewport;

namespace SharpPlot.Render;

public interface IRenderSettings
{
    ScreenSize ScreenSize { get; set; }
    Indent Indent { get; set; }
}

public interface IRenderContext
{
    IRenderSettings Settings { get; set; }

    void DrawPoints();
    void DrawLines();
    void DrawTriangles();
    void DrawQuadrilaterals();
    
    void Clear();
}