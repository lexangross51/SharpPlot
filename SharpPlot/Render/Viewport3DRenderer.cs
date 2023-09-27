using OpenTK.Mathematics;
using SharpPlot.Camera;
using SharpPlot.Shaders;
using SharpPlot.Text;
using SharpPlot.Viewport;
using SharpPlot.Wrappers;

namespace SharpPlot.Render;

public class Viewport3DRenderer
{
    private ShaderProgram _axesShader = null!;
    private RenderSettings _renderSettings;
    private readonly Camera3D _camera;
    private readonly int[] _viewport;

    public SharpPlotFont Font { get; set; }

    public Viewport3DRenderer(RenderSettings renderSettings, Camera3D camera)
    {
        _renderSettings = renderSettings;
        _camera = camera;
        _viewport = new[] { 0, 0, (int)_renderSettings.ScreenSize.Width, (int)_renderSettings.ScreenSize.Height };
        
        InitShaders();
    }

    private void InitShaders()
    {
        _axesShader = ShaderCollection.LineShader();
        _axesShader.Use();
        _axesShader.SetUniform("model", Matrix4.Identity);
        _axesShader.SetUniform("view", Matrix4.Identity);
        _axesShader.SetUniform("projection", Matrix4.Identity);
    }

    public Camera3D GetCamera() => _camera;

    public void UpdateView()
    {
        _axesShader.Use();
        _axesShader.SetUniform("view", _camera.GetViewMatrix());
        _axesShader.SetUniform("projection", _camera.GetProjectionMatrix());
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