using System;
using OpenTK.Graphics.OpenGL4;
using SharpPlot.Drawing.Buffers;
using SharpPlot.Drawing.Projection.Implementations;
using SharpPlot.Drawing.Shaders;

namespace SharpPlot.Drawing.Render;

public class AxesRenderer2D
{
    private readonly ShaderProgram _shaderAxes, _shaderBox;
    private readonly VertexArrayObject _vao;
    private readonly VertexBufferObject<float> _vbo;
    private readonly float[] _vertices;
    private readonly OrthographicProjection _projection;
    private RenderSettings _settings;

    public AxesRenderer2D(OrthographicProjection projection, RenderSettings settings)
    {
        _projection = projection;
        _settings = settings;
        _vertices = new float[6];
        
        _shaderAxes = new ShaderProgram(
            "Drawing/Shaders/Sources/Axes/AxesShader.vert",
            "Drawing/Shaders/Sources/Axes/AxesShader.frag",
            "Drawing/Shaders/Sources/Axes/AxesShader.geom");

        _shaderBox = new ShaderProgram("Drawing/Shaders/Sources/Axes/AxesShader.vert",
            "Drawing/Shaders/Sources/Axes/AxesShader.frag",
            "Drawing/Shaders/Sources/Axes/BoxShader.geom");
        
        _vao = new VertexArrayObject();
        _vbo = new VertexBufferObject<float>(_vertices, BufferUsageHint.DynamicDraw);
        _shaderAxes.Use();
        _shaderAxes.GetAttributeLocation("position", out var location);
        _vao.SetAttributePointer(location, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        
        _vao.Unbind();
    }
    
    public void Render()
    {
        RenderHorizontalAxis();
        RenderVerticalAxis();
        RenderBox();
    }

    public void UpdateViewPort(RenderSettings settings) 
        => _settings = settings;
    
    private void RenderHorizontalAxis()
    {
        var proj = _projection.ToArray();
        
        var step = 0.5f;
        double start = Math.Floor(proj[0] / step) * step;
        double end = Math.Ceiling(proj[1] / step) * step;
        var vRatio = (proj[3] - proj[2]) / _settings.ScreenHeight;
        
        _vertices[0] = Math.Abs(start) < step / 4 ? 0.0f : (float)start;
        _vertices[1] = (float)(proj[2] + _settings.Margin * vRatio);
        _vertices[2] = 0.0f;
        _vertices[3] = Math.Abs(end) < step / 4 ? 0.0f : (float)end;
        _vertices[4] = (float)(proj[2] + _settings.Margin * vRatio);
        _vertices[5] = 0.0f;

        GL.Viewport((int)_settings.Margin, 0, (int)(_settings.ScreenWidth - _settings.Margin),
            (int)_settings.ScreenHeight);
        _vbo.Bind();
        _vbo.UpdateData(_vertices);
        _vao.Bind();
        _shaderAxes.Use();
        _shaderAxes.SetUniform("isHorizontal", 1);
        _shaderAxes.SetUniform("projection", _projection.ProjectionMatrix);
        _shaderAxes.SetUniform("stepX", step);
        _shaderAxes.SetUniform("vRatio", (float)vRatio);

        GL.DrawArrays(PrimitiveType.Lines, 0, 2);
    }

    private void RenderVerticalAxis()
    {
        var proj = _projection.ToArray();
        
        var step = 0.5f;
        double start = Math.Floor(proj[2] / step) * step;
        double end = Math.Ceiling(proj[3] / step) * step;
        var hRatio = (proj[1] - proj[0]) / _settings.ScreenWidth;
        
        _vertices[0] = (float)(proj[0] + _settings.Margin * hRatio);
        _vertices[1] = Math.Abs(start) < step / 4 ? 0.0f : (float)start;
        _vertices[2] = 0.0f;
        _vertices[3] = (float)(proj[0] + _settings.Margin * hRatio);
        _vertices[4] = Math.Abs(end) < step / 4 ? 0.0f : (float)end;
        _vertices[5] = 0.0f;

        GL.Viewport(0, (int)_settings.Margin, (int)_settings.ScreenWidth,
            (int)(_settings.ScreenHeight - _settings.Margin));
        _vbo.Bind();
        _vbo.UpdateData(_vertices);
        _vao.Bind();
        _shaderAxes.Use();
        _shaderAxes.SetUniform("isHorizontal", 0);
        _shaderAxes.SetUniform("projection", _projection.ProjectionMatrix);
        _shaderAxes.SetUniform("stepY", step);
        _shaderAxes.SetUniform("hRatio", (float)hRatio);

        GL.DrawArrays(PrimitiveType.Lines, 0, 2);
    }

    private void RenderBox()
    {
        GL.Viewport((int)_settings.Margin, (int)_settings.Margin,
            (int)(_settings.ScreenWidth - _settings.Margin),
            (int)(_settings.ScreenHeight - _settings.Margin));
        
        _shaderBox.Use();
        
        GL.DrawArrays(PrimitiveType.Points, 0, 1);
    }
}