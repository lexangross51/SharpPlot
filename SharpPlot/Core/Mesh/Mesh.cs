using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SharpPlot.Objects;
using Point = SharpPlot.Objects.Point;

namespace SharpPlot.Core.Mesh;

public class Mesh : IBaseObject
{
    private readonly Element[] _elements;

    public PrimitiveType ObjectType { get; }
    public Point[] Points { get; }
    public Color4[] Colors { get; }
    public uint[]? Indices { get; }

    public int ElementsCount => _elements.Length;
    public int PointsCount => Points.Length;
    
    public Mesh(IEnumerable<Point> points, IEnumerable<Element> elements)
    {
        Points = points.ToArray();
        _elements = elements.ToArray();
        Colors = new[] { Color4.Black };
        Indices = new uint[_elements.Length * _elements[0].Nodes.Length];
        ObjectType = _elements.First().Nodes.Length == 3 ? PrimitiveType.Triangles : PrimitiveType.Quads;

        uint index = 0;
        foreach (var node in _elements.SelectMany(element => element.Nodes))
        {
            Indices[index++] = (uint)node;
        }
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

    public Point Point(int index) => Points[index];
    public Element Element(int index) => _elements[index];
}