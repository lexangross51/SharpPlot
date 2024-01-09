using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using SharpPlot.Drawing.Buffers;
using SharpPlot.Drawing.Projection.Implementations;
using SharpPlot.Drawing.Shaders;
using SharpPlot.Drawing.Text;

namespace SharpPlot.Drawing.Render;

public class AxesRenderer2D
{
    private const int LongTickSize = 5;
    private const int ShortTickSize = 2;
    private const int ShortIntervals = 5;
    private const double TextWidth = 100.0;
    
    private readonly ShaderProgram _shaderAxes, _shaderBox;
    private readonly VertexArrayObject _vao;
    private readonly VertexBufferObject<float> _vbo;
    private readonly OrthographicProjection _projection;
    private readonly FrameSettings _settings;
    private readonly double[] _multipliers = [1, 2, 5, 10];
    private readonly float[] _vertices = new float[1000];

    public bool DrawLongTicks { get; set; } = true;
    public bool DrawShortTicks { get; set; } = true;
    public SharpPlotFont Font { get; set; } = new();

    public string HorizontalAxisName { get; set; } = "X";
    public string VerticalAxisName { get; set; } = "Y";
    
    public AxesRenderer2D(OrthographicProjection projection, FrameSettings settings)
    {
        _projection = projection;
        _settings = settings;

        if (_settings.Margin == 0.0)
        {
            TextRenderer.TextMeasure("0", Font, out _, out var height);
            _settings.Margin = height;
        }
        
        TextRenderer.Instance.Projection = _projection;
        TextRenderer.Instance.Settings = _settings;
        
        _shaderAxes = new ShaderProgram(
            "Drawing/Shaders/Sources/Axes/AxesShader.vert",
            "Drawing/Shaders/Sources/Axes/AxesShader.frag");

        _shaderBox = new ShaderProgram("Drawing/Shaders/Sources/Axes/AxesShader.vert",
            "Drawing/Shaders/Sources/Axes/AxesShader.frag",
            "Drawing/Shaders/Sources/Axes/BoxShader.geom");
        
        _vao = new VertexArrayObject();
        _vbo = new VertexBufferObject<float>(_vertices);
        _shaderAxes.Use();
        _shaderAxes.GetAttributeLocation("position", out var location);
        _vao.SetAttributePointer(location, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        _vao.Unbind();
    }
    
    public void Render()
    {
        RenderHorizontalAxis();
        RenderVerticalAxis();
        RenderBox();
    }

    public void UpdateViewPort(FrameSettings settings)
    {
        _settings.ScreenWidth = settings.ScreenWidth;
        _settings.ScreenHeight = settings.ScreenHeight;
    }
    
    private double CalculateStepAxis(double begin, double end, double measure, double textWidth)
    {
        var dH = end - begin;

        var fontSize = textWidth * dH / measure;
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
    
    private void RenderHorizontalAxis()
    {
        Array.Clear(_vertices);
        
        var proj = _projection.ToArray();
        var step = CalculateStepAxis(proj[0], proj[1], _settings.ScreenWidth - _settings.Margin, TextWidth);
        var shortStep = step / ShortIntervals;
        var hRatio = (proj[1] - proj[0]) / (_settings.ScreenWidth - _settings.Margin);
        var vRatio = (proj[3] - proj[2]) / _settings.ScreenHeight;
        int index = 0;
        
        GL.Viewport((int)_settings.Margin, 0, (int)(_settings.ScreenWidth - _settings.Margin),
            (int)_settings.ScreenHeight);
        
        if (DrawLongTicks)
        {
            for (double curr = Math.Floor(proj[0] / step) * step; curr < proj[1]; curr += step)
            {
                _vertices[index++] = (float)curr;
                _vertices[index++] = (float)(proj[2] + (_settings.Margin - LongTickSize) * vRatio);
                _vertices[index++] = (float)curr;
                _vertices[index++] = (float)(proj[2] + (_settings.Margin + LongTickSize) * vRatio);

                if (!DrawShortTicks) continue;

                for (double j = curr + shortStep; j < curr + step; j += shortStep)
                {
                    _vertices[index++] = (float)j;
                    _vertices[index++] = (float)(proj[2] + (_settings.Margin - ShortTickSize) * vRatio);
                    _vertices[index++] = (float)j;
                    _vertices[index++] = (float)(proj[2] + (_settings.Margin + ShortTickSize) * vRatio);
                }
            }
            
            _vbo.Bind();
            _vbo.UpdateData(_vertices);
            _vao.Bind();
            _shaderAxes.Use();
            _shaderAxes.SetUniform("projection", _projection.ProjectionMatrix);

            GL.DrawArrays(PrimitiveType.Lines, 0, _vertices.Length / 2);
        }

        var minDrawLetter = proj[0];
        var maxDrawLetter = proj[1];

        if (HorizontalAxisName != string.Empty)
        {
            TextRenderer.TextMeasure(HorizontalAxisName, Font, out var width, out _);
            maxDrawLetter -= (width + 1) * hRatio;
        }
        
        for (double curr = Math.Floor(proj[0] / step) * step; curr <= proj[1]; curr += step)
        {
            var msVal = Math.Abs(curr) < 1E-15 ? "0" : curr.ToString("G7");
            TextRenderer.TextMeasure(msVal, Font, out var stringSize, out _);
            var stringPositionL = curr - stringSize * 0.5 * hRatio;
            var stringPositionR = curr + stringSize * 0.5 * hRatio;
            var color = Math.Abs(curr) < 1E-15 ? Color.Red : Color.Black;
            
            if (stringPositionL < minDrawLetter || stringPositionR > maxDrawLetter) continue;

            Font.Print(stringPositionL, proj[2], 0, msVal, color);
        }
        
        if (HorizontalAxisName != string.Empty)
        {
            Font.Print(maxDrawLetter, proj[2], 0.0, HorizontalAxisName, Color.Black);
        }
    }

    private void RenderVerticalAxis()
    {
        Array.Clear(_vertices);
        
        var proj = _projection.ToArray();
        var step = CalculateStepAxis(proj[2], proj[3], _settings.ScreenHeight - _settings.Margin, 80.0);
        var shortStep = step / ShortIntervals;
        var hRatio = (proj[1] - proj[0]) / _settings.ScreenWidth;
        var vRatio = (proj[3] - proj[2]) / (_settings.ScreenHeight - _settings.Margin);
        int index = 0;

        GL.Viewport(0, (int)_settings.Margin, (int)_settings.ScreenWidth,
            (int)(_settings.ScreenHeight - _settings.Margin));
        
        if (DrawLongTicks)
        {
            for (double curr = Math.Floor(proj[2] / step) * step; curr < proj[3]; curr += step)
            {
                _vertices[index++] = (float)(proj[0] + (_settings.Margin - LongTickSize) * hRatio);
                _vertices[index++] = (float)curr;
                _vertices[index++] = (float)(proj[0] + (_settings.Margin + LongTickSize) * hRatio);
                _vertices[index++] = (float)curr;

                if (!DrawShortTicks) continue;

                for (double j = curr + shortStep; j < curr + step; j += shortStep)
                {
                    _vertices[index++] = (float)(proj[0] + (_settings.Margin - ShortTickSize) * hRatio);
                    _vertices[index++] = (float)j;
                    _vertices[index++] = (float)(proj[0] + (_settings.Margin + ShortTickSize) * hRatio);
                    _vertices[index++] = (float)j;
                }
            }
            
            _vbo.Bind();
            _vbo.UpdateData(_vertices);
            _vao.Bind();
            _shaderAxes.Use();
            _shaderAxes.SetUniform("projection", _projection.ProjectionMatrix);

            GL.DrawArrays(PrimitiveType.Lines, 0, _vertices.Length / 2);
        }
        
        var minDrawLetter = proj[2];
        var maxDrawLetter = proj[3];
        
        if (VerticalAxisName != string.Empty)
        {
            TextRenderer.TextMeasure(HorizontalAxisName, Font, out var width, out _);
            maxDrawLetter -= (width + 1) * vRatio;
        }
        
        for (double curr = Math.Floor(proj[2] / step) * step; curr <= proj[3]; curr += step)
        {
            var msVal = Math.Abs(curr) < 1E-15 ? "0" : curr.ToString("G7");
            TextRenderer.TextMeasure(msVal, Font, out var stringSize, out _);
            var stringPositionL = curr - stringSize * 0.5 * vRatio;
            var stringPositionR = curr + stringSize * 0.5 * vRatio;
            var color = Math.Abs(curr) < 1E-15 ? Color.Red : Color.Black;

            if (stringPositionL < minDrawLetter || stringPositionR > maxDrawLetter) continue;
            
            Font.Print(proj[0], stringPositionL, 0, msVal, color, TextRenderOrientation.Vertical);
        }

        if (VerticalAxisName != string.Empty)
        {
            Font.Print(proj[0], maxDrawLetter, 0.0, VerticalAxisName, Color.Black, TextRenderOrientation.Vertical);
        }
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