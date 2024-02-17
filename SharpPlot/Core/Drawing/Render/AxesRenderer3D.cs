using SharpPlot.Core.Drawing.Projection.Interfaces;

namespace SharpPlot.Core.Drawing.Render;

public class AxesRenderer3D
{
    private readonly IProjection _projection;
    private readonly FrameSettings _settings;
    
    public AxesRenderer3D(IProjection projection, FrameSettings settings)
    {
        _projection = projection;
        _settings = settings;
    }
    
    public void Render()
    {
        
    }
    
    public void UpdateViewPort(FrameSettings settings)
    {
        _settings.ScreenWidth = settings.ScreenWidth;
        _settings.ScreenHeight = settings.ScreenHeight;
    }
}