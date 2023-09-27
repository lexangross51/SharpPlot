using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SharpPlot.Objects;

namespace SharpPlot.Core;

public class Scatter : IBaseObject
{
    public PrimitiveType ObjectType { get; }
    public int PointSize { get; }
    public Point[] Points { get; }
    public Color4[] Colors { get; }
    public uint[]? Indices { get; }

    public Scatter(IEnumerable<Point> pointCollection, int size)
    {
        ObjectType = PrimitiveType.Points;
        PointSize = size;
        Points = pointCollection.ToArray();
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