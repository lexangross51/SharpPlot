using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SharpPlot.Core.Algorithms;
using SharpPlot.Objects;

namespace SharpPlot.Core.Isolines;

public class ContourF : IBaseObject
{
    private readonly DelaunayTriangulation _triangulation = new();
    public PrimitiveType ObjectType { get; }
    public int PointSize { get; }
    public Point[] Points { get; }
    public Color4[] Colors { get; }
    public uint[]? Indices { get; }

    public ContourF(IEnumerable<Point> pointsCollection, IEnumerable<double> values, Palette.Palette palette,
        int levels = 5)
    {
        ObjectType = PrimitiveType.Lines;
        PointSize = 1;
        
        var mesh = _triangulation.Triangulate(pointsCollection);
        var isobandBuilder = new IsobandBuilder(mesh, values.ToArray());
        isobandBuilder.BuildIsobands(levels, palette);

        Points = isobandBuilder.Points.ToArray();
        Colors = isobandBuilder.Colors.ToArray();
        Indices = null;
    }
    
    public void BoundingBox(out Point leftBottom, out Point rightTop)
    {
        var minX = Points.MinBy(p => p.X).X;
        var maxX = Points.MaxBy(p => p.X).X;
        var minY = Points.MinBy(p => p.Y).Y;
        var maxY = Points.MaxBy(p => p.Y).Y;

        leftBottom = new Point(minX, minY);
        rightTop = new Point(maxX, maxY);
    }
}