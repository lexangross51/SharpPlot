using System.Collections.Generic;
using SharpPlot.Algorithms.Tree;

namespace SharpPlot.Geometry.Interfaces;

public interface ITriangle : IQuadStorable
{
    int Id { get; set; }
    Point3D[] Points { get; }
    Edge[] Edges { get; }
    IList<ITriangle?> Neighbors { get; }
}