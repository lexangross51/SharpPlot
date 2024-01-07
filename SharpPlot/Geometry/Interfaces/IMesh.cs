using System.Collections.Generic;

namespace SharpPlot.Geometry.Interfaces;

public interface IMesh
{
    IList<Point3D> Points { get; }
    IList<ITriangle> Triangles { get; }
}