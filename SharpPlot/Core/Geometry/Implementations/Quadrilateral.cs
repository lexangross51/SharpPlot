using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SharpPlot.Core.Geometry.Interfaces;

namespace SharpPlot.Core.Geometry.Implementations;

public class Quadrilateral : IElement
{
    public RectangleF Bounds { get; private set; }
    public ElementType Type => ElementType.Quadrilateral;
    public int Id { get; set; }
    public Point3D[] Points { get; }
    public Edge[] Edges { get; }
    public IList<IElement?> Neighbors { get; }
    
    public Quadrilateral(Point3D[] points)
    {
        Points = points;
        Edges = new Edge[4];
        Neighbors = new List<IElement?>(4);
        Bounds = new RectangleF();

        MakeEdges();
        BuildBounds();
    }

    private void MakeEdges()
    {
        var p1 = Points[0];
        var p2 = Points[1];
        var p3 = Points[2];
        var p4 = Points[3];

        (Edges[0].P1, Edges[0].P2) = (p1, p2);
        (Edges[1].P1, Edges[1].P2) = (p2, p3);
        (Edges[2].P1, Edges[2].P2) = (p3, p4);
        (Edges[3].P1, Edges[3].P2) = (p1, p4);
    }

    private void BuildBounds()
    {
        double minX = Points.Min(p => p.X);
        double minY = Points.Min(p => p.Y);
        double maxX = Points.Min(p => p.X);
        double maxY = Points.Min(p => p.Y);
        
        Bounds = new RectangleF((float)minX, (float)minY, (float)(maxX - minX), (float)(maxY - minY));
    }
    
    public bool Contains(double x, double y)
    {
        var p1 = Points[0];
        var p2 = Points[1];
        var p3 = Points[2];
        var p4 = Points[3];

        var a = (p1.X - x) * (p2.Y - p1.Y) - (p2.X - p1.X) * (p1.Y - y);
        var b = (p2.X - x) * (p3.Y - p2.Y) - (p3.X - p2.X) * (p2.Y - y);
        var c = (p3.X - x) * (p4.Y - p3.Y) - (p4.X - p3.X) * (p3.Y - y);
        var d = (p4.X - x) * (p1.Y - p4.Y) - (p1.X - p4.X) * (p4.Y - y);

        return (a >= 0 && b >= 0 && c >= 0 && d >= 0) || (a <= 0 && b <= 0 && c <= 0 && d <= 0);
    }    
}