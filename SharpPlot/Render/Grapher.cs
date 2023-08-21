using OpenTK.Graphics.OpenGL4;
using SharpPlot.Camera;
using SharpPlot.Objects;
using SharpPlot.Shaders;
using SharpPlot.Viewport;
using SharpPlot.Wrappers;

namespace SharpPlot.Render;

public class BaseGraphic2D : IRenderContext
{
    private readonly ShaderProgram _shader;
    private readonly int[] _viewport;
    private RenderSettings _renderSettings;
    private VertexArrayObject? _vao;
    private VertexBufferObject<float>? _vbo;
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
        
        UpdateView();

        var points = new Point[] { new(-1, -1), new(1, 1) };
        var data = new float[3 * points.Length];

        var index = 0;
        foreach (var p in points)
        {
            data[index++] = (float)p.X;
            data[index++] = (float)p.Y;
            data[index++] = (float)p.Z;
        }
        _vao = new VertexArrayObject();
        _vbo = new VertexBufferObject<float>(data);
        _shader.Use();
        _shader.GetAttribLocation("position", out var location);
        _vao.SetAttributePointer(location, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
    }
    
    public int[] GetNewViewport(ScreenSize newScreenSize)
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
        _shader.SetUniform("model", Camera.GetModelMatrix());
        _shader.SetUniform("view", Camera.GetViewMatrix());
        _shader.SetUniform("projection", Camera.GetProjectionMatrix());
    }
    
    public void DrawObject(IBaseObject obj)
    {
        _vao!.Bind();
        _shader.Use();
        
        UpdateView();
        
        GL.PointSize(4);
        GL.DrawArrays(PrimitiveType.Lines, 0, 2);
    }
    
    public void Clear()
    {
    }
}