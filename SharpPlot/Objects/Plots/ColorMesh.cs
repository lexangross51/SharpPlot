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

        var nx = Points.Select(p => p.X).Distinct().Count();
        var ny = Points.Select(p => p.Y).Distinct().Count();
        _vertexIndices = new List<int>(4 * (nx - 1) * (ny - 1));

        for (int i = 0; i < ny - 1; i++)
        {
            for (int j = 0; j < nx - 1; j++)
            {
                var index = i * nx + j;
                _vertexIndices.Add(index);
                index = i * nx + j + 1;
                _vertexIndices.Add(index);
                index = (i + 1) * nx + j + 1;
                _vertexIndices.Add(index);
                index = (i + 1) * nx + j;
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
            var interpolated = ColorInterpolator.InterpolateColor(valuesRanges, valueAtPoint, palette, interpolation);
            Colors.Add(interpolated);
        }

        Colors.Add(Colors.Last());
        Colors.Add(Colors.Last());
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