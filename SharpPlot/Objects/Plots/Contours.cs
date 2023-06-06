using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using SharpGL;
using SharpGL.Enumerations;
using SharpPlot.Render;
using Geometry = SharpPlot.Mathematics.Geometry;

namespace SharpPlot.Objects.Plots;

public class ContourF : IRenderable
{
    private readonly List<List<Point>> _pointsPerLevels;
    public PrimitiveType Type { get; set; } = PrimitiveType.Polygon;
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

    public ContourF(IEnumerable<Point> points, IEnumerable<double> values, PaletteType paletteType, int levels)
    {
        Points = points.ToList();
        Colors = new List<Color>();
        _pointsPerLevels = new List<List<Point>>();
        var valuesArray = values.ToArray();
        var palette = Palette.Create(paletteType);
        var maxValue = valuesArray.Max();
        var minValue = valuesArray.Min();
        var levelsStep = (maxValue - minValue) / (levels + 1);
        var valueStep = (maxValue - minValue) / palette.ColorsCount;
        var valuesRanges = new double[palette.ColorsCount + 1];
        
        int j = 0;
        for (double value = maxValue; value >= minValue; value -= valueStep)
        {
            valuesRanges[j++] = value;
        }
        valuesRanges[^1] = minValue;

        for (int i = 0; i < levels + 1; i++)
        {
            var lower = minValue + i * levelsStep;
            var upper = minValue + (i + 1) * levelsStep;
            var interpolated = InterpolateColor((lower + upper) / 2.0, valuesRanges, palette);
            
            _pointsPerLevels.Add(new List<Point>());
            Colors.Add(interpolated);
            
            for (int k = 0; k < Points.Count; k++)
            {
                if (valuesArray[k] >= lower && valuesArray[k] < upper)
                {
                    _pointsPerLevels[i].Add(Points[k]);
                }
            }

            var pointsPerLevel = _pointsPerLevels[i];
            Geometry.BuildContourHull(ref pointsPerLevel);
            _pointsPerLevels[i] = pointsPerLevel;
        }
    }

    private Color InterpolateColor(double value, double[] range, Palette palette)
    {
        int rangeNumber = 0; 

        for (int i = 0; i < range.Length - 1; i++)
        {
            if (!(value <= range[i]) || !(value >= range[i + 1])) continue;
            rangeNumber = i;
            break;
        }

        var start = palette[rangeNumber];
        var end = rangeNumber == palette.ColorsCount - 1 ? palette[rangeNumber] : palette[rangeNumber + 1];

        var t = (value - range[rangeNumber + 1]) / (range[rangeNumber] - range[rangeNumber + 1]);
        var r = end.R + t * (start.R - end.R);
        var g = end.G + t * (start.G - end.G);
        var b = end.B + t * (start.B - end.B);
        return Color.FromRgb((byte)r, (byte)g, (byte)b);
    }
    
    public void Render(IBaseGraphic graphic)
    {
        for (var i = _pointsPerLevels.Count - 1; i >= 0; i--)
        {
            var color = Colors[i];
            graphic.GL.Color(color.R, color.G, color.B);
            graphic.GL.Begin((BeginMode)Type);

            foreach (var point in _pointsPerLevels[i])
            {
                graphic.GL.Vertex(point.X, point.Y);
            }
            
            graphic.GL.End();
        }
    }
}

public class Contour : IRenderable
{
    private readonly List<List<Point>> _pointsPerLevels;
    public PrimitiveType Type { get; set; } = PrimitiveType.Polygon;
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

    public Contour(IEnumerable<Point> points, IEnumerable<double> values, int levels)
    {
        Points = points.ToList();
        Colors = new List<Color>();
        _pointsPerLevels = new List<List<Point>>();
        var valuesArray = values.ToArray();
        var maxValue = valuesArray.Max();
        var minValue = valuesArray.Min();
        var levelsStep = (maxValue - minValue) / (levels + 1);

        for (int i = 0; i < levels + 1; i++)
        {
            var lower = minValue + i * levelsStep;
            var upper = minValue + (i + 1) * levelsStep;

            _pointsPerLevels.Add(new List<Point>());

            for (int k = 0; k < Points.Count; k++)
            {
                if (valuesArray[k] >= lower && valuesArray[k] < upper)
                {
                    _pointsPerLevels[i].Add(Points[k]);
                }
            }

            var pointsPerLevel = _pointsPerLevels[i];
            Geometry.BuildContourHull(ref pointsPerLevel);
            _pointsPerLevels[i] = pointsPerLevel;
        }
    }
    
    public void Render(IBaseGraphic graphic)
    {
        for (var i = _pointsPerLevels.Count - 1; i >= 0; i--)
        {
            graphic.GL.Color(0, 0, 0);
            graphic.GL.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_LINE);
            graphic.GL.Begin((BeginMode)Type);

            foreach (var point in _pointsPerLevels[i])
            {
                graphic.GL.Vertex(point.X, point.Y);
            }
            
            graphic.GL.End();
            graphic.GL.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_FILL);
        }
    }
}