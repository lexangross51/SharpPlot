using System.Collections.Generic;
using System.Drawing;
using SharpPlot.Geometry.Interfaces;

namespace SharpPlot.Geometry.Implementations;

public class Quadrilateral : IElement
{
    public RectangleF Bounds { get; }
    public bool Contains(double x, double y)
    {
        throw new System.NotImplementedException();
    }

    public ElementType Type => ElementType.Quadrilateral;
    public int Id { get; set; }
    public Point3D[] Points { get; }
    public Edge[] Edges { get; }
    public IList<IElement?> Neighbors { get; }
}