using System;
using System.Collections.Generic;
using System.Drawing;
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
    private readonly ShaderProgram _borderShader, _axesShader;
    private RenderSettings _renderSettings;
    private readonly Camera2D _camera;
    private readonly int[] _viewport;
    private readonly double[] _multipliers = { 1, 2, 5, 10 };
    private readonly Axis _horizontalAxis;
    private readonly Axis _verticalAxis;
    private readonly List<float> _points;
    private readonly VertexArrayObject _vaoBorder, _vaoTicks;
    private readonly VertexBufferObject<float> _vboTicks;

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
    public bool DrawingGrid { get; set; } = true;

    public Camera2D GetCamera() => _camera;

    public RenderSettings GetRenderSettings() => _renderSettings;

    public Viewport2DRenderer(RenderSettings renderSettings, Camera2D camera)
    {
        _borderShader = ShaderCollection.LineShader();
        _axesShader = ShaderCollection.LineShader();

        _horizontalAxis = new Axis("X");
        _verticalAxis = new Axis("Y");

        _renderSettings = renderSettings;
        _camera = camera;
        _viewport = new int[4];
        _viewport = GetNewViewport(_renderSettings.ScreenSize);

        _points = new List<float>();

        Font = new SharpPlotFont
        {
            Color = Color.Black
        };

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

        _axesShader.SetUniform("model", _camera.GetModelMatrix());
        _axesShader.SetUniform("view", _camera.GetViewMatrix());
        _axesShader.SetUniform("projection", _camera.GetProjectionMatrix());
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

    private double CalculateStepAxis(double begin, double end, double measure)
    {
        var dH = end - begin;
        var hh = measure;

        var fontSize = /*TextPrinter.TextMeasure(Axis.TemplateCaption, Font).Width **/ 50 * dH / hh;
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
        var vRatio = (projection[3] - projection[2]) /
                     (_renderSettings.ScreenSize.Height - _renderSettings.Indent.Bottom);
        
        // var minDrawLetter = projection[0];
        // var maxDrawLetter = projection[1] - textWidth * hRatio;
        //
        // foreach (var it in _horizontalAxis.Points)
        // {
        //     double fVal = it;
        //     var msVal = fVal.ToString("G10", CultureInfo.InvariantCulture);
        //     var stringSize = TextPrinter.TextMeasure(msVal, Font).Width;
        //     var stringPositionL = fVal - stringSize * 0.5 * hRatio;
        //     var stringPositionR = fVal + stringSize * 0.5 * hRatio;
        //
        //     if (stringPositionL >= minDrawLetter && stringPositionR <= maxDrawLetter)
        //     {
        //         var color = Math.Abs(fVal) < 1E-15 ? Color.Red : Color.Black;
        //         var sharpPlotFont = Font;
        //         sharpPlotFont.Color = color;
        //         
        //         //TextPrinter.DrawText(BaseGraphic, msVal, stringPositionL, projection[2], sharpPlotFont);
        //     }
        // }

        const double dy = 6;
        foreach (var it in _horizontalAxis.Points)
        {
            _points.Add((float)it);
            _points.Add((float)(projection[2] + (heightText - dy) * vRatio));
            _points.Add(0.0f);
            _points.Add((float)it);
            _points.Add((float)(projection[2] + (heightText) * vRatio));
            _points.Add(0.0f);
        }

        _vboTicks.Bind();
        _vboTicks.UpdateData(_points.ToArray());
        _vaoTicks.Bind();
        _axesShader.Use();
        _axesShader.GetUniformLocation("lineColor", out var location);
        _axesShader.SetUniform(location, 0.0f, 0.0f, 0.0f, 1.0f);
        _axesShader.SetUniform("projection", _camera.GetProjectionMatrix());
        GL.Viewport((int)_renderSettings.Indent.Left, 0,
            (int)(_renderSettings.ScreenSize.Width - _renderSettings.Indent.Left),
            (int)_renderSettings.ScreenSize.Height);
        GL.DrawArrays(PrimitiveType.Lines, 0, _points.Count / 3);

        _vboTicks.Unbind();
        _vaoTicks.Unbind();
        
        // if (textWidth != 0)
        // {
        //     BaseGraphic.GL.Color(Font.Color.R, Font.Color.G, Font.Color.B);
        //     TextPrinter.DrawText(BaseGraphic, HorizontalAxis.AxisName, projection[1] - textWidth * hRatio, projection[2],
        //         Font);
        // }
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

        var hRatio = (projection[1] - projection[0]) / (_renderSettings.ScreenSize.Width - _renderSettings.Indent.Left);
        var vRatio = (projection[3] - projection[2]) /
                     (_renderSettings.ScreenSize.Height - _renderSettings.Indent.Bottom);

        // double minDrawLetter = projection[2];
        // double maxDrawLetter = projection[3] - textWidth * vRatio;
        //
        // foreach (var it in _verticalAxis.Points)
        // {
        //     double fVal = it;
        //     var msVal = fVal.ToString("G10", CultureInfo.InvariantCulture);
        //     var stringSize = TextPrinter.TextMeasure(msVal, Font).Width;
        //     var stringPositionL = fVal - stringSize * 0.5 * vRatio;
        //     var stringPositionR = fVal + stringSize * 0.5 * vRatio;
        //
        //     if (stringPositionL >= minDrawLetter && stringPositionR <= maxDrawLetter)
        //     {
        //         var sharpPlotFont = Font;
        //         sharpPlotFont.Color = Math.Abs(fVal) < 1E-15 ? Color.Red : Color.Black;
        //
        //         TextPrinter.DrawText(BaseGraphic, msVal, projection[0], stringPositionL, sharpPlotFont,
        //             TextOrientation.Vertical);
        //     }
        // }

        const double dx = 6.0;
        foreach (var it in _verticalAxis.Points)
        {
            _points.Add((float)(projection[0] + heightText * hRatio));
            _points.Add((float)it);
            _points.Add(0.0f);
            _points.Add((float)(projection[0] + (heightText - dx) * hRatio));
            _points.Add((float)it);
            _points.Add(0.0f);
        }

        _vboTicks.Bind();
        _vboTicks.UpdateData(_points.ToArray());
        _vaoTicks.Bind();
        _axesShader.Use();
        _axesShader.GetUniformLocation("lineColor", out var location);
        _axesShader.SetUniform(location, 0.0f, 0.0f, 0.0f, 1.0f);
        _axesShader.SetUniform("projection", _camera.GetProjectionMatrix());
        GL.Viewport(0, (int)_renderSettings.Indent.Bottom, (int)_renderSettings.ScreenSize.Width,
            (int)(_renderSettings.ScreenSize.Height - _renderSettings.Indent.Bottom));
        GL.DrawArrays(PrimitiveType.Lines, 0, _points.Count / 3);

        _vboTicks.Unbind();
        _vaoTicks.Unbind();
        
        // if (textWidth != 0)
        // {
        //     BaseGraphic.GL.Color(Font.Color.R, Font.Color.G, Font.Color.B);
        //     TextPrinter.DrawText(BaseGraphic, VerticalAxis.AxisName, projection[0], projection[3] - textWidth * vRatio,
        //         Font, TextOrientation.Vertical);
        // }
    }

    private void DrawGrid()
    {
        _points.Clear();

        UpdateView();
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
        _axesShader.GetUniformLocation("lineColor", out var location);
        _axesShader.SetUniform(location, 0.7f, 0.7f, 0.7f, 1.0f);

        GL.DrawArrays(PrimitiveType.Lines, 0, _points.Count / 3);
        
        _vboTicks.Unbind();
        _vaoTicks.Unbind();
    }

    public void Clear()
    {
    }
}