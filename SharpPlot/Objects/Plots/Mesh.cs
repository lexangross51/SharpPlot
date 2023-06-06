using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using SharpGL;
using SharpGL.Enumerations;
using SharpPlot.Render;

namespace SharpPlot.Objects.Plots;

public class Mesh : IRenderable
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

    public Mesh(IEnumerable<Point> points)
    {
        Points = points.ToList();
        Colors = new List<Color>() { Color.FromRgb(0, 0, 0) };
        
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
    }

    public Mesh(IEnumerable<Point> points, IEnumerable<int> vertexIndices)
    {
        Points = points.ToList();
        _vertexIndices = vertexIndices.ToList();
        Colors = new List<Color>() { Color.FromRgb(0, 0, 0) };
    }
    
    public void Render(IBaseGraphic graphic)
    {
        graphic.GL.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_LINE);
        graphic.GL.Begin((BeginMode)Type);
        graphic.GL.Color(Colors[0].R, Colors[0].G, Colors[0].B, Colors[0].A);
        
        foreach (var point in _vertexIndices.Select(index => Points[index]))
        {
            graphic.GL.Vertex(point.X, point.Y);
        }
        
        graphic.GL.End();
        graphic.GL.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_FILL);
    }
}