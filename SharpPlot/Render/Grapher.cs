using OpenTK.Graphics.OpenGL4;
using SharpPlot.Camera;
using SharpPlot.Shaders;
using SharpPlot.Viewport;
using SharpPlot.Wrappers;

namespace SharpPlot.Render;

public class BaseGraphic2D : IRenderContext
{
    private readonly ShaderProgram _shader;
    private readonly int[] _viewport;
    private RenderSettings _renderSettings;
    public Camera2D Camera { get; }
    
    public BaseGraphic2D(RenderSettings renderSettings, Camera2D camera)
    {
        _shader = ShaderCollection.LineShader();
        _viewport = new[]
        {
            (int)renderSettings.Indent.Left,
            (int)renderSettings.Indent.Bottom,
            (int)(renderSettings.ScreenSize.Width - renderSettings.Indent.Left),
            (int)(renderSettings.ScreenSize.Height - renderSettings.Indent.Bottom)
        };
        
        _renderSettings = renderSettings;
        Camera = camera;
    }
    
    public int[] GetNewViewPort(ScreenSize newScreenSize)
    {
        _renderSettings.ScreenSize = newScreenSize;
        
        _viewport[0] = (int)_renderSettings.Indent.Left;
        _viewport[1] = (int)_renderSettings.Indent.Bottom;
        _viewport[2] = (int)(_renderSettings.ScreenSize.Width - _renderSettings.Indent.Left);
        _viewport[3] = (int)(_renderSettings.ScreenSize.Height - _renderSettings.Indent.Bottom);

        return _viewport;
    }

    public void UpdateView()
    {
        GL.Viewport(_viewport[0], _viewport[1], _viewport[2], _viewport[3]);
        _shader.SetUniform("projection", Camera.GetProjectionMatrix());
    }

    public void DrawPoints()
    {
        throw new System.NotImplementedException();
    }

    public void DrawLines()
    {
        throw new System.NotImplementedException();
    }

    public void DrawTriangles()
    {
        throw new System.NotImplementedException();
    }

    public void DrawQuadrilaterals()
    {
        throw new System.NotImplementedException();
    }

    public void Clear()
    {
        throw new System.NotImplementedException();
    }
}