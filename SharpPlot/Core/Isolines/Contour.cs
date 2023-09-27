using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SharpPlot.Core.Algorithms;
using SharpPlot.Objects;
using Point = SharpPlot.Objects.Point;

namespace SharpPlot.Core.Isolines;

public class Contour : IBaseObject
{
    private readonly DelaunayTriangulation _triangulation = new();
    public PrimitiveType ObjectType { get; }
    public int PointSize { get; }
    public Point[] Points { get; }
    public Color4[] Colors { get; }
    public uint[]? Indices { get; }

    public Contour(IEnumerable<Point> pointsCollection, IEnumerable<double> values, int levels = 5)
    {
        ObjectType = PrimitiveType.Lines;
        PointSize = 1;
        
        var mesh = _triangulation.Triangulate(pointsCollection);
        var isolineBuilder = new IsolineBuilder(mesh, values.ToArray());
        isolineBuilder.BuildIsolines(levels);

        Points = isolineBuilder.Points.ToArray();
        Colors = new[] { Color4.Black };
        Indices = null;
    }
    
    public void BoundingBox(out Point leftBottom, out Point rightTop)
    {
        var minX = Points.MinBy(p => p.X).X;
        var maxX = Points.MaxBy(p => p.X).X;
        var minY = Points.MinBy(p => p.Y).Y;
        var maxY = Points.MaxBy(p => p.Y).Y;
        var minZ = Points.MinBy(p => p.Z).Z;
        var maxZ = Points.MaxBy(p => p.Z).Z;

        leftBottom = new Point(minX, minY, minZ);
        rightTop = new Point(maxX, maxY, maxZ);
    }
}