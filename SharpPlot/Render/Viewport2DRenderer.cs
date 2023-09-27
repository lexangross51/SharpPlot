using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SharpPlot.Camera;
using SharpPlot.Objects.Axis;
using SharpPlot.Shaders;
using SharpPlot.Text;
using SharpPlot.Viewport;
using SharpPlot.Wrappers;

namespace SharpPlot.Render;

public class Viewport2DRenderer
{
    private ShaderProgram _borderShader = null!, _axesShader = null!;
    private RenderSettings _renderSettings;
    private readonly Camera2D _camera;
    private readonly int[] _viewport;
    private readonly double[] _multipliers = { 1, 2, 5, 10 };
    private readonly Axis _horizontalAxis;
    private readonly Axis _verticalAxis;
    private List<float> _points = null!;
    private VertexArrayObject _vaoBorder = null!, _vaoTicks = null!;
    private VertexBufferObject<float> _vboTicks = null!;

    public string HAxisName
    {
        get => _horizontalAxis.Name;
        set => _horizontalAxis.Name = value;
    }

    public string VAxisName
    {
        get => _verticalAxis.Name;
        set => _verticalAxis.Name = value;
    }

    public SharpPlotFont Font { get; set; }
    public bool DrawingGrid { get; set; }

    public Camera2D GetCamera() => _camera;

    public RenderSettings GetRenderSettings() => _renderSettings;

    public Viewport2DRenderer(RenderSettings renderSettings, Camera2D camera)
    {
        _renderSettings = renderSettings;
        _camera = camera;
        
        Font = new SharpPlotFont
        {
            Color = Color.Black,
        };
        
        _horizontalAxis = new Axis("X");
        _verticalAxis = new Axis("Y");
        
        _viewport = new int[4];
        _viewport = GetNewViewport(_renderSettings.ScreenSize);
        
        InitShaders();
    }

    private void InitShaders()
    {
        _borderShader = ShaderCollection.LineShader();
        _axesShader = ShaderCollection.LineShader();
        _points = new List<float>();

        // Border settings will only be set once
        _vaoBorder = new VertexArrayObject();
        var vbo = new VertexBufferObject<float>(new float[]
        {
            -1, -1, 0,
            1, -1, 0,
            1, 1, 0,
            -1, 1, 0,
        });

        _borderShader.Use();
        _borderShader.SetUniform("model", Matrix4.Identity);
        _borderShader.SetUniform("view", Matrix4.Identity);
        _borderShader.SetUniform("projection", Matrix4.CreateOrthographicOffCenter(-1, 1, -1, 1, -1, 1));
        _borderShader.GetAttribLocation("position", out var location);
        _vaoBorder.SetAttributePointer(location, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        vbo.Unbind();
        _vaoBorder.Unbind();

        // Settings for axes ticks (will be updated very frequently)
        for (int i = 0; i < 200; i++)
        {
            _points.Add(0.0f);
        }

        _axesShader.Use();
        _axesShader.SetUniform("model", Matrix4.Identity);
        _axesShader.SetUniform("view", Matrix4.Identity);
        _axesShader.SetUniform("projection", Matrix4.Identity);
        _vaoTicks = new VertexArrayObject();
        _vboTicks = new VertexBufferObject<float>(_points.ToArray(), BufferUsageHint.DynamicDraw);
        _vaoTicks.SetAttributePointer(location, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        _vboTicks.Unbind();
        _vaoTicks.Unbind();
    }

    public void RenderAxis()
    {
        if (DrawingGrid)
            DrawGrid();
        
        DrawBorder();
        DrawHorizontalTicks();
        DrawVerticalAxis();
    }

    public void UpdateView()
    {
        GL.Viewport(_viewport[0], _viewport[1], _viewport[2], _viewport[3]);
        
        _axesShader.SetUniform("projection", _camera.GetProjectionMatrix());
    }

    public int[] GetNewViewport(ScreenSize newScreenSize)
    {
        _camera.GetProjection().Ratio = newScreenSize.Height / newScreenSize.Width;
        _renderSettings.ScreenSize = newScreenSize;

        _viewport[0] = (int)_renderSettings.Indent.Left;
        _viewport[1] = (int)_renderSettings.Indent.Bottom;
        _viewport[2] = (int)(_renderSettings.ScreenSize.Width - _renderSettings.Indent.Left);
        _viewport[3] = (int)(_renderSettings.ScreenSize.Height - _renderSettings.Indent.Bottom);

        return _viewport;
    }

    private double CalculateStepAxis(double begin, double end, double measure)
    {
        var dH = end - begin;
        var hh = measure;

        var fontSize = TextPrinter.TextMeasure(Axis.TemplateCaption, Font).Width * dH / hh;
        var dTiles = Math.Floor(dH / fontSize);

        var dStep = dH / dTiles;
        var dMul = Math.Pow(10, Math.Floor(Math.Log10(dStep)));

        int i;
        for (i = 1; i < _multipliers.Length - 1; ++i)
        {
            if (dMul * _multipliers[i] > dStep) break;
        }

        dStep = _multipliers[i] * dMul;
        return dStep;
    }

    private void DrawBorder()
    {
        GL.Viewport(_viewport[0], _viewport[1], _viewport[2], _viewport[3]);
        
        _vaoBorder.Bind();
        _borderShader.Use();
        _borderShader.SetUniform("projection", Matrix4.CreateOrthographicOffCenter(-1, 1, -1, 1, -1, 1));
        _borderShader.GetUniformLocation("lineColor", out var location);
        _borderShader.SetUniform(location, 0.0f, 0.0f, 0.0f, 1.0f);

        GL.LineWidth(2);
        GL.DrawArrays(PrimitiveType.LineLoop, 0, 4);
        GL.LineWidth(1);

        _vaoBorder.Unbind();
    }

    private void DrawHorizontalTicks()
    {
        _points.Clear();

        var projection = _camera.GetProjection().GetProjection();
        var step = CalculateStepAxis(projection[0], projection[1], _renderSettings.ScreenSize.Width);
        _horizontalAxis.GenerateTicks(projection[0], projection[1], step);

        var textWidth = TextPrinter.TextMeasure(_horizontalAxis.Name, Font).Width;
        var heightText = _horizontalAxis.Name == ""
            ? TextPrinter.TextMeasure("0", Font).Height
            : TextPrinter.TextMeasure(_horizontalAxis.Name, Font).Height;

        var hRatio = (projection[1] - projection[0]) / (_renderSettings.ScreenSize.Width - _renderSettings.Indent.Left);
        // var vRatio = (projection[3] - projection[2]) / (_renderSettings.ScreenSize.Height - _renderSettings.Indent.Bottom);
        var vRatio = (projection[3] - projection[2]) / _renderSettings.ScreenSize.Height;
        
        var minDrawLetter = projection[0];
        var maxDrawLetter = projection[1] - textWidth * hRatio;
        
        const double dy = 3;
        foreach (var it in _horizontalAxis.Points)
        {
            _points.Add((float)it);
            _points.Add((float)(projection[2] + (heightText - dy) * vRatio));
            _points.Add(0.0f);
            _points.Add((float)it);
            _points.Add((float)(projection[2] + (heightText + dy) * vRatio));
            _points.Add(0.0f);
        }

        GL.Viewport((int)_renderSettings.Indent.Left, 0,
            (int)(_renderSettings.ScreenSize.Width - _renderSettings.Indent.Left),
            (int)_renderSettings.ScreenSize.Height);
        _vboTicks.Bind();
        _vboTicks.UpdateData(_points.ToArray());
        _vaoTicks.Bind();
        _axesShader.Use();
        _axesShader.SetUniform("projection", _camera.GetProjectionMatrix());
        _axesShader.GetUniformLocation("lineColor", out var location);
        _axesShader.SetUniform(location, 0.0f, 0.0f, 0.0f, 1.0f);

        GL.LineWidth(2);
        GL.DrawArrays(PrimitiveType.Lines, 0, _points.Count / 3);
        GL.LineWidth(1);

        _vboTicks.Unbind();
        _vaoTicks.Unbind();
        
        foreach (var it in _horizontalAxis.Points)
        {
            var msVal = it.ToString("G10", CultureInfo.InvariantCulture);
            var stringSize = TextPrinter.TextMeasure(msVal, Font).Width;
            var stringPositionL = it - stringSize * 0.5 * hRatio;
            var stringPositionR = it + stringSize * 0.5 * hRatio;

            if (stringPositionL < minDrawLetter || stringPositionR > maxDrawLetter) continue;
            
            var color = Math.Abs(it) < 1E-15 ? Color.Red : Color.Black;
            var sharpPlotFont = Font;
            sharpPlotFont.Color = color;

            TextPrinter.DrawText(this, msVal, stringPositionL, projection[2], sharpPlotFont);
        }
        
        if (textWidth != 0)
        {
            TextPrinter.DrawText(this, HAxisName, projection[1] - textWidth * hRatio, projection[2], Font);
        }
    }

    private void DrawVerticalAxis()
    {
        _points.Clear();

        var projection = _camera.GetProjection().GetProjection();
        var step = CalculateStepAxis(projection[2], projection[3], _renderSettings.ScreenSize.Height);
        _verticalAxis.GenerateTicks(projection[2], projection[3], step);

        var textWidth = TextPrinter.TextMeasure(_verticalAxis.Name, Font).Width;
        int heightText = _verticalAxis.Name == ""
            ? TextPrinter.TextMeasure("0", Font).Height
            : TextPrinter.TextMeasure(_verticalAxis.Name, Font).Height;

        // var hRatio = (projection[1] - projection[0]) / (_renderSettings.ScreenSize.Width - _renderSettings.Indent.Left);
        var hRatio = (projection[1] - projection[0]) / _renderSettings.ScreenSize.Width;
        var vRatio = (projection[3] - projection[2]) / (_renderSettings.ScreenSize.Height - _renderSettings.Indent.Bottom);

        var minDrawLetter = projection[2];
        var maxDrawLetter = projection[3] - textWidth * vRatio;
        
        const double dx = 3.0;
        foreach (var it in _verticalAxis.Points)
        {
            _points.Add((float)(projection[0] + (heightText + dx) * hRatio));
            _points.Add((float)it);
            _points.Add(0.0f);
            _points.Add((float)(projection[0] + (heightText - dx) * hRatio));
            _points.Add((float)it);
            _points.Add(0.0f);
        }
        
        GL.Viewport(0, (int)_renderSettings.Indent.Bottom, (int)_renderSettings.ScreenSize.Width,
            (int)(_renderSettings.ScreenSize.Height - _renderSettings.Indent.Bottom));
        _vboTicks.Bind();
        _vboTicks.UpdateData(_points.ToArray());
        _vaoTicks.Bind();
        _axesShader.Use();
        _axesShader.SetUniform("projection", _camera.GetProjectionMatrix());
        _axesShader.GetUniformLocation("lineColor", out var location);
        _axesShader.SetUniform(location, 0.0f, 0.0f, 0.0f, 1.0f);

        GL.LineWidth(2);
        GL.DrawArrays(PrimitiveType.Lines, 0, _points.Count / 3);
        GL.LineWidth(1);

        _vboTicks.Unbind();
        _vaoTicks.Unbind();
        
        foreach (var it in _verticalAxis.Points)
        {
            var msVal = it.ToString("G10", CultureInfo.InvariantCulture);
            var stringSize = TextPrinter.TextMeasure(msVal, Font).Width;
            var stringPositionL = it - stringSize * 0.5 * vRatio;
            var stringPositionR = it + stringSize * 0.5 * vRatio;

            if (stringPositionL < minDrawLetter || stringPositionR > maxDrawLetter) continue;
            
            var sharpPlotFont = Font;
            sharpPlotFont.Color = Math.Abs(it) < 1E-15 ? Color.Red : Color.Black;

            TextPrinter.DrawText(this, msVal, projection[0], stringPositionL, sharpPlotFont,
                TextOrientation.Vertical);
        }
        
        if (textWidth != 0)
        {
            TextPrinter.DrawText(this, VAxisName, projection[0], projection[3] - textWidth * vRatio,
                Font, TextOrientation.Vertical);
        }
    }

    private void DrawGrid()
    {
        _points.Clear();
        
        var projection = _camera.GetProjection().GetProjection();

        foreach (var it in _horizontalAxis.Points)
        {
            _points.Add((float)it);
            _points.Add((float)projection[2]);
            _points.Add(0.0f);
            _points.Add((float)it);
            _points.Add((float)projection[3]);
            _points.Add(0.0f);
        }

        foreach (var it in _verticalAxis.Points)
        {
            _points.Add((float)projection[0]);
            _points.Add((float)it);
            _points.Add(0.0f);
            _points.Add((float)projection[1]);
            _points.Add((float)it);
            _points.Add(0.0f);
        }

        _vboTicks.Bind();
        _vboTicks.UpdateData(_points.ToArray());
        _vaoTicks.Bind();
        _axesShader.Use();
        
        UpdateView();
        
        _axesShader.GetUniformLocation("lineColor", out var location);
        _axesShader.SetUniform(location, 0.7f, 0.7f, 0.7f, 1.0f);

        GL.DrawArrays(PrimitiveType.Lines, 0, _points.Count / 3);
        
        _vboTicks.Unbind();
        _vaoTicks.Unbind();
    }
}