using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using SharpGL;
using SharpGL.Enumerations;
using SharpPlot.Render;
using SharpPlot.Text;
using SharpPlot.Viewport;
using FontStyle = System.Drawing.FontStyle;

namespace SharpPlot.Objects;

public enum ColorbarLocation
{
    BottomLeft,
    BottomRight,
    TopLeft,
    TopRight
}

public class Colorbar : IRenderable
{
    private const int MaxValuesCount = 8;
    private readonly List<double> _values;
    private readonly SharpPlotFont _font;
    private readonly ColorInterpolation _interpolation;
    private IBaseGraphic? _barGraphic;
    
    public PrimitiveType Type { get; set; } = PrimitiveType.Quads;
    public int PointSize { get; set; } = 1;
    public List<Point> Points { get; set; } = null!;
    public List<Color> Colors { get; set; }
    public ColorbarLocation Location { get; set; } = ColorbarLocation.BottomRight;

    private Colorbar()
    {
        _values = new List<double>();
        Colors = new List<Color>();
        _font = new SharpPlotFont()
        {
            Size = 14,
            Style = FontStyle.Regular
        };
    }
    
    public Colorbar(IEnumerable<double> values, Palette palette,
        ColorInterpolation interpolation = ColorInterpolation.Constant) : this()
    {
        _interpolation = interpolation;

        var valuesList = values.ToList();
        double max = valuesList.Max();
        double min = valuesList.Min();
        int colorsCount = palette.ColorsCount;

        for (int i = 0; i < colorsCount; i++)
        {
            Colors.Add(palette[i]);
        }

        var (valueStep, valuesCount) = colorsCount < MaxValuesCount - 1
            ? ((max - min) / colorsCount, colorsCount + 1)
            : ((max - min) / (MaxValuesCount - 1), MaxValuesCount);

        for (int i = 0; i < valuesCount; i++)
        {
            _values.Add(double.Round(min + i * valueStep, 4));
        }
    }

    public void BoundingBox(out Point? leftBottom, out Point? rightTop)
    {
        leftBottom = null;
        rightTop = null;
    }

    public void Render(IBaseGraphic graphic)
    {
        var textMes = TextPrinter.TextMeasure(_values[0].ToString(CultureInfo.InvariantCulture), _font);
        var width = textMes.Width + 30;
        var height = _values.Count * textMes.Height + 25;
        var xStart = graphic.ScreenSize.Width - width - 2;

        _barGraphic ??= new BaseGraphic2D(graphic.GL, new ScreenSize(width, height),
            new OrthographicProjection(new[] { -1.2, 1.2, -1.2, 1.2, -1.0, 1.0 }, 1.0), graphic.Indent);

        graphic.GL.MatrixMode(MatrixMode.Projection);
        graphic.GL.PushMatrix();
        graphic.GL.LoadIdentity();
        graphic.GL.Viewport((int)xStart, (int)graphic.Indent.Vertical + 2, (int)(width + graphic.Indent.Horizontal), height);
        graphic.GL.Ortho(-1.2, 1.2, -1.2, 1.2, -1, 1);
        graphic.GL.MatrixMode(MatrixMode.Modelview);
        graphic.GL.PushMatrix();
        graphic.GL.LoadIdentity();

        // White rectangle
        graphic.GL.Color(1f, 1f, 1f);
        graphic.GL.Begin(OpenGL.GL_QUADS);
        graphic.GL.Vertex(-1.2, -1.2);
        graphic.GL.Vertex(1.2, -1.2);
        graphic.GL.Vertex(1.2, 1.2);
        graphic.GL.Vertex(-1.2, 1.2);
        graphic.GL.End();
        
        // Border
        graphic.GL.Color(0f, 0f, 0f);
        graphic.GL.Begin(OpenGL.GL_LINE_LOOP);
        graphic.GL.Vertex(-1.19, -1.19);
        graphic.GL.Vertex(1.19, -1.19);
        graphic.GL.Vertex(1.19, 1.19);
        graphic.GL.Vertex(-1.19, 1.19);
        graphic.GL.End();
        
        // Colored quads
        int quadsCount = Colors.Count;
        double step = 2.0 / quadsCount;

        if (_interpolation == ColorInterpolation.Constant)
        {
            for (int i = 0; i < quadsCount; i++)
            {
                var color = Colors[^(i + 1)];
            
                graphic.GL.Color(color.R, color.G, color.B);
                graphic.GL.Begin(BeginMode.Quads);
                graphic.GL.Vertex(-1.0, -1.0 + i * step);
                graphic.GL.Vertex(-0.8, -1.0 + i * step);
                graphic.GL.Vertex(-0.8, -1.0 + (i + 1) * step);
                graphic.GL.Vertex(-1.0, -1.0 + (i + 1) * step);
                graphic.GL.End();
            }
        }
        else
        {
            for (int i = 0; i < quadsCount; i++)
            {
                var color = Colors[^(i + 1)];
            
                graphic.GL.Color(color.R, color.G, color.B);
                graphic.GL.Begin(BeginMode.Quads);
                graphic.GL.Vertex(-1.0, -1.0 + i * step);
                graphic.GL.Vertex(-0.8, -1.0 + i * step);

                color = i == quadsCount - 1 ? Colors[^(i + 1)] : Colors[^(i + 2)];

                graphic.GL.Color(color.R, color.G, color.B);
                graphic.GL.Vertex(-0.8, -1.0 + (i + 1) * step);
                graphic.GL.Vertex(-1.0, -1.0 + (i + 1) * step);
                graphic.GL.End();
            }
        }

        
        // Border for colorbar
        graphic.GL.Color(0f, 0f, 0f);
        graphic.GL.Begin(BeginMode.LineLoop);
        graphic.GL.Vertex(-1.0, -1.0);
        graphic.GL.Vertex(-0.8, -1.0);
        graphic.GL.Vertex(-0.8, 1.0);
        graphic.GL.Vertex(-1.0, 1.0);
        graphic.GL.End();
        
        // Lines for colorbar
        graphic.GL.Begin(BeginMode.Lines);

        for (int i = 0; i < quadsCount + 1; i++)
        {
            graphic.GL.Vertex(-0.8, -1.0 + i * step);
            graphic.GL.Vertex(-0.7, -1.0 + i * step);
        }
        
        graphic.GL.End();
        
        // Values
        for (int i = 0; i < quadsCount + 1; i++)
        {
            var value = _values[i].ToString(CultureInfo.InvariantCulture);
            TextPrinter.DrawText(_barGraphic, value, -0.65, -1.1 + i * step, _font);
        }

        graphic.GL.PopMatrix();
        graphic.GL.MatrixMode(MatrixMode.Projection);
        graphic.GL.PopMatrix();
        graphic.GL.MatrixMode(MatrixMode.Modelview);
    }
}