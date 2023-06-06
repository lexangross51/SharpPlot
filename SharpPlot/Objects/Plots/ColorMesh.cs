using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using SharpGL.Enumerations;
using SharpPlot.Render;

namespace SharpPlot.Objects.Plots;

public class ColorMesh : IRenderable
{
    private readonly List<int> _vertexIndices;
    public PrimitiveType Type { get; set; } = PrimitiveType.Quads;
    public int PointSize { get; set; } = 1;
    public List<Point> Points { get; set; }
    public List<Color> Colors { get; set; }
    
    public void BoundingBox(out Point? leftBottom, out Point? rightTop)
    {
        var minX = Points.MinBy(p => p.X).X;
        var maxX = Points.MaxBy(p => p.X).X;
        var minY = Points.MinBy(p => p.Y).Y;
        var maxY = Points.MaxBy(p => p.Y).Y;

        leftBottom = new Point(minX, minY);
        rightTop = new Point(maxX, maxY);
    }

    public ColorMesh(IEnumerable<Point> points, IEnumerable<double> values, PaletteType paletteType,
        ColorInterpolation interpolation)
    {
        Points = points.ToList();
        Colors = new List<Color>();

        var xPoints = Points.Select(p => p.X).Distinct().ToArray();
        var yPoints = Points.Select(p => p.Y).Distinct().ToArray();
        _vertexIndices = new List<int>(4 * (xPoints.Length - 1) * (yPoints.Length - 1));

        for (int i = 0; i < yPoints.Length - 1; i++)
        {
            for (int j = 0; j < xPoints.Length - 1; j++)
            {
                var index = i * xPoints.Length + j;
                _vertexIndices.Add(index);
                index = i * xPoints.Length + j + 1;
                _vertexIndices.Add(index);
                index = (i + 1) * xPoints.Length + j + 1;
                _vertexIndices.Add(index);
                index = (i + 1) * xPoints.Length + j;
                _vertexIndices.Add(index);
            }
        }

        MakeColorsByValues(values, paletteType, interpolation);
    }

    public ColorMesh(IEnumerable<Point> points, IEnumerable<int> vertexIndices, IEnumerable<double> values,
        PaletteType paletteType, ColorInterpolation interpolation)
    {
        Points = points.ToList();
        _vertexIndices = vertexIndices.ToList();
        Colors = new List<Color>();
        
        MakeColorsByValues(values, paletteType, interpolation);
    }

    private void MakeColorsByValues(IEnumerable<double> values, PaletteType paletteType,
        ColorInterpolation interpolation)
    {
        var palette = Palette.Create(paletteType);

        var colorsCount = palette.ColorsCount;
        var valuesArray = values.ToArray();
        var maxValue = valuesArray.Max();
        var minValue = valuesArray.Min();
        var valueStep = (maxValue - minValue) / colorsCount;
        var valuesRanges = new double[colorsCount + 1];

        int i = 0;
        for (double value = maxValue; value >= minValue; value -= valueStep)
        {
            valuesRanges[i++] = value;
        }
        valuesRanges[^1] = minValue;

        for (int j = 0; j < Points.Count; j++)
        {
            var valueAtPoint = valuesArray[j];

            for (int k = 0; k < valuesRanges.Length - 1; k++)
            {
                if (valueAtPoint <= valuesRanges[k] && valueAtPoint >= valuesRanges[k + 1])
                {
                    var colorStart = palette[k];
                    var colorEnd = k == colorsCount - 1 ? palette[k] : palette[k + 1];
                    
                    var interpolated = InterpolateColor(
                        valuesRanges[k],valuesRanges[k + 1], valueAtPoint,
                        colorStart, colorEnd, interpolation); 
                    Colors.Add(interpolated);
                    break;
                }
            }
        }

        Colors.Add(Colors.Last());
        Colors.Add(Colors.Last());
    }

    private Color InterpolateColor(double valueStart, double valueEnd, double value, Color start, Color end,
        ColorInterpolation interpolation)
    {
        switch (interpolation)
        {
            case ColorInterpolation.Constant:
                return start;
            case ColorInterpolation.Linear:
                var t = (value - valueEnd) / (valueStart - valueEnd);
                var r = end.R + t * (start.R - end.R);
                var g = end.G + t * (start.G - end.G);
                var b = end.B + t * (start.B - end.B);
                return Color.FromRgb((byte)r, (byte)g, (byte)b);
            case ColorInterpolation.Quadratic:
                return System.Windows.Media.Colors.Bisque;
            default:
                throw new ArgumentOutOfRangeException(nameof(interpolation), interpolation, null);
        }
    }
    
    public void Render(IBaseGraphic graphic)
    {
        graphic.GL.Begin((BeginMode)Type);

        foreach (var index in _vertexIndices)
        {
            var point = Points[index];
            var color = Colors[index];
            
            graphic.GL.Color(color.R, color.G, color.B, color.A);
            graphic.GL.Vertex(point.X, point.Y);
        }
        
        graphic.GL.End();
    }
}