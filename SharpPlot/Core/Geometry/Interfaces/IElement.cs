using System.Collections.Generic;
using SharpPlot.Core.Algorithms.Tree;

namespace SharpPlot.Core.Geometry.Interfaces;

public enum ElementType
{
    Triangle,
    Quadrilateral
}

public interface IElement : IQuadStorable
{
    ElementType Type { get; }
    int Id { get; set; }
    Point3D[] Points { get; }
    Edge[] Edges { get; }
    IList<IElement?> Neighbors { get; }
}