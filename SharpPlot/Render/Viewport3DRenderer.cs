using SharpPlot.Camera;
using SharpPlot.Text;
using SharpPlot.Viewport;

namespace SharpPlot.Render;

public class Viewport3DRenderer
{
    private RenderSettings _renderSettings;
    private readonly Camera3D _camera;
    private readonly int[] _viewport;

    public SharpPlotFont Font { get; set; }

    public Viewport3DRenderer(RenderSettings renderSettings, Camera3D camera)
    {
        _renderSettings = renderSettings;
        _camera = camera;
        _viewport = new[] { 0, 0, (int)_renderSettings.ScreenSize.Width, (int)_renderSettings.ScreenSize.Height };
    }

    public Camera3D GetCamera() => _camera;

    public void UpdateView()
    {
        
    }
    
    public int[] GetNewViewport(ScreenSize newScreenSize)
    {
        _camera.GetProjection().Ratio = newScreenSize.Height / newScreenSize.Width;
        _renderSettings.ScreenSize = newScreenSize;

        _viewport[0] = 0;
        _viewport[1] = 0;
        _viewport[2] = (int)_renderSettings.ScreenSize.Width;
        _viewport[3] = (int)_renderSettings.ScreenSize.Height;

        return _viewport;
    }
}