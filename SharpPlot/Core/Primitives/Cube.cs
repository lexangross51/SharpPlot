using System.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SharpPlot.Objects;

namespace SharpPlot.Core.Primitives;

public class Cube : IBaseObject
{
    public PrimitiveType ObjectType => PrimitiveType.Triangles;
    public int PointSize => 1;
    public Point[] Points { get; } = 
    {
        new(-0.5, -0.5, -0.5),
        new(0.5, -0.5, -0.5),
        new(0.5, 0.5, -0.5),
        new(0.5, 0.5, -0.5),
        new(-0.5, 0.5, -0.5),
        new(-0.5, -0.5, -0.5),
        new(-0.5, -0.5, 0.5),
        new(0.5, -0.5, 0.5),
        new(0.5, 0.5, 0.5),
        new(0.5, 0.5, 0.5),
        new(-0.5, 0.5, 0.5),
        new(-0.5, -0.5, 0.5),
        new(-0.5, 0.5, 0.5),
        new(-0.5, 0.5, -0.5),
        new(-0.5, -0.5, -0.5),
        new(-0.5, -0.5, -0.5),
        new(-0.5, -0.5, 0.5),
        new(-0.5, 0.5, 0.5),
        new(0.5, 0.5, 0.5),
        new(0.5, 0.5, -0.5),
        new(0.5, -0.5, -0.5),
        new(0.5, -0.5, -0.5),
        new(0.5, -0.5, 0.5),
        new(0.5, 0.5, 0.5),
        new(-0.5, -0.5, -0.5),
        new(0.5, -0.5, -0.5),
        new(0.5, -0.5, 0.5),
        new(0.5, -0.5, 0.5),
        new(-0.5, -0.5, 0.5),
        new(-0.5, -0.5, -0.5),
        new(-0.5, 0.5, -0.5),
        new(0.5, 0.5, -0.5),
        new(0.5, 0.5, 0.5),
        new(0.5, 0.5, 0.5),
        new(-0.5, 0.5, 0.5),
        new(-0.5, 0.5, -0.5),
    };

    public Color4[] Colors { get; } = { Color4.Black };
    public uint[]? Indices => null;
    
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