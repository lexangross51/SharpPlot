using System.Collections.Generic;
using SharpPlot.Geometry.Interfaces;

namespace SharpPlot.Geometry;

public class Mesh(IList<ITriangle> triangles, IList<Point3D> points) : IMesh
{
    public IList<ITriangle> Triangles { get; } = triangles;
    public IList<Point3D> Points { get; } = points;
}