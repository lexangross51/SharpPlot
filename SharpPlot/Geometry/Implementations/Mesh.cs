using System.Collections.Generic;
using SharpPlot.Geometry.Interfaces;

namespace SharpPlot.Geometry.Implementations;

public class Mesh(IList<IElement> triangles, IList<Point3D> points) : IMesh
{
    public IList<IElement> Elements { get; } = triangles;
    public IList<Point3D> Points { get; } = points;
}