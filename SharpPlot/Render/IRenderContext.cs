using SharpPlot.Objects;
using SharpPlot.Viewport;

namespace SharpPlot.Render;

public struct RenderSettings
{
    public ScreenSize ScreenSize { get; set; }
    public Indent Indent { get; set; }
}

public interface IRenderContext
{
    void AddObject(IBaseObject obj);
    void DrawObjects();
    void UpdateView();
    int[] GetNewViewport(ScreenSize newScreenSize);
    void Clear();
}