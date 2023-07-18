using System;
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
    private readonly int[,] _vertexIndices;
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

    public ContourF(IEnumerable<Point> points, IEnumerable<double> values, Palette palette, int levels)
    {
        Points = points.ToList();
        Colors = new List<Color>();
        var nx = Points.Select(p => p.X).Distinct().Count();
        var ny = Points.Select(p => p.Y).Distinct().Count();
        _pointsPerLevels = new List<List<Point>>();
        _vertexIndices = new int[(nx - 1) * (ny - 1), 4];
        var valuesArray = values.ToArray();
        double minValue = valuesArray.Min(); minValue = Math.Floor(minValue);
        double maxValue = valuesArray.Max(); maxValue = Math.Ceiling(maxValue);
        double levelsStep = (maxValue - minValue) / (levels + 1);
        double valueStep = (maxValue - minValue) / palette.ColorsCount;
        var valuesByPalette = new double[palette.ColorsCount + 1];
        var levelsValues = new double[levels + 1];

        for (int i = 0; i < palette.ColorsCount + 1; i++) valuesByPalette[i] = minValue + i * valueStep;
        for (int i = 0; i < levels + 1; i++) levelsValues[i] = minValue + (i + 1) * levelsStep;
        for (int l = 0; l < levels + 1; l++) _pointsPerLevels.Add(new List<Point>());
        
        for (int i = 0; i < ny - 1; i++)
        {
            for (int j = 0; j < nx - 1; j++)
            {
                _vertexIndices[i * (nx - 1) + j, 0] = i * nx + j; 
                _vertexIndices[i * (nx - 1) + j, 1] = i * nx + j + 1; 
                _vertexIndices[i * (nx - 1) + j, 2] = (i + 1) * nx + j + 1; 
                _vertexIndices[i * (nx - 1) + j, 3] = (i + 1) * nx + j;
            }
        }

        // Forming points
        for (int i = 0; i < levels + 1; i++)
        {
            var lower = minValue + i * levelsStep;
            var upper = minValue + (i + 1) * levelsStep;
            var interpolated = ColorInterpolator.InterpolateColor(valuesByPalette, (lower + upper) / 2.0, palette,
                ColorInterpolation.Linear);
            
            Colors.Add(interpolated);
            
            for (int k = 0; k < Points.Count; k++)
            {
                if (valuesArray[k] >= lower /*&& valuesArray[k] < upper*/)
                {
                    _pointsPerLevels[i].Add(Points[k]);
                }
            }
        }

        for (int i = 0; i < ny - 1; i++)
        {
            for (int j = 0; j < nx - 1; j++)
            {
                int icell = i * (nx - 1) + j;

                for (int k = 0; k < 4; k++)
                {
                    var v1 = valuesArray[_vertexIndices[icell, k]];
                    var v2 = valuesArray[_vertexIndices[icell, (k + 1) % 4]];
                    var p1 = Points[_vertexIndices[icell, k]];
                    var p2 = Points[_vertexIndices[icell, (k + 1) % 4]];

                    for (int l = 0; l < levelsValues.Length; l++)
                    {
                        double t = (levelsValues[l] - v1) / (v2 - v1);

                        if (Math.Abs(t) <= 1.0)
                        {
                            double x = p1.X + t * (p2.X - p1.X);
                            double y = p1.Y + t * (p2.Y - p1.Y);
                            var intersected = new Point(x, y);
                            
                            if (!IsPointInside(icell, intersected)) continue;

                            _pointsPerLevels[l].Add(intersected);
                        }
                    }
                }
            }
        }

        for (int i = 0; i < _pointsPerLevels.Count; i++)
        {
            var pointsPerLevel = _pointsPerLevels[i].Distinct().ToList();
            
            if (pointsPerLevel.Count > 2)
                Geometry.BuildContourHull(ref pointsPerLevel);
            _pointsPerLevels[i] = pointsPerLevel;
        }
    }

    private bool IsPointInside(int icell, Point point)
    {
        var p1 = Points[_vertexIndices[icell, 0]];
        var p2 = Points[_vertexIndices[icell, 2]];

        if (point.X >= p1.X && point.X <= p2.X &&
            point.Y >= p1.Y && point.Y <= p2.Y)
            return true;
        return false;
    }
    
    public void Render(IBaseGraphic graphic)
    {
        for (var i = 0; i < _pointsPerLevels.Count; i++)
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