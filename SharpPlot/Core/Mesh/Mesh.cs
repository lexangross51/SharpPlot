using System.Collections.Generic;
using System.Linq;
using SharpPlot.Objects;

namespace SharpPlot.Core.Mesh;

public class Mesh
{
    private readonly Point[] _points;
    private readonly Element[] _elements;

    public int ElementsCount => _elements.Length;
    public int PointsCount => _points.Length;
    
    public Mesh(IEnumerable<Point> points, IEnumerable<Element> triangles)
    {
        _points = points.ToArray();
        _elements = triangles.ToArray();
    }

    public Point Point(int index) => _points[index];
    public Element Element(int index) => _elements[index];
}