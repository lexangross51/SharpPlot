using System;
using System.Collections.Generic;
using System.Drawing;
using SharpPlot.Geometry.Interfaces;

namespace SharpPlot.Geometry;

public class Triangle : ITriangle
{
    private double _circumcircleX, _circumcircleY, _circumcircleR;
    
    public int Id { get; set; }
    public Point3D[] Points { get; }
    public Edge[] Edges { get; }
    public IList<ITriangle?> Neighbors { get; }
    public RectangleF Bounds { get; private set; }
    
    private Triangle()
    {
        Points = new Point3D[3];
        Edges = new Edge[3];
        Neighbors = new List<ITriangle?>(3);
    }
    
    public Triangle(Point3D a, Point3D b, Point3D c) : this()
    {
        Points[0] = a;
        Points[1] = b;
        Points[2] = c;
        
        MakeEdges();
        BuildCircumcircle();
        BuildBounds();
    }

    private void MakeEdges()
    {
        var p1 = Points[0];
        var p2 = Points[1];
        var p3 = Points[2];

        (p1, p2) = p1.Id > p2.Id ? (p2, p1) : (p1, p2);
        (p2, p3) = p2.Id > p3.Id ? (p3, p2) : (p2, p3);
        (p1, p2) = p1.Id > p2.Id ? (p2, p1) : (p1, p2);

        (Edges[0].P1, Edges[0].P2) = (p1, p2);
        (Edges[1].P1, Edges[1].P2) = (p2, p3);
        (Edges[2].P1, Edges[2].P2) = (p1, p3);
    }
    private void BuildCircumcircle()
    {
        var a = Points[0];
        var b = Points[1];
        var c = Points[2];
        
        double ab = Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y)), 
            ac = Math.Sqrt((a.X - c.X) * (a.X - c.X) + (a.Y - c.Y) * (a.Y - c.Y)), 
            bc = Math.Sqrt((b.X - c.X) * (b.X - c.X) + (b.Y - c.Y) * (b.Y - c.Y)), 
            p = 0.5 * (ab + ac + bc);
        double xa2 = a.X * a.X, 
            xb2 = b.X * b.X, 
            xc2 = c.X * c.X, 
            ya2 = a.Y * a.Y, 
            yb2 = b.Y * b.Y, 
            yc2 = c.Y * c.Y;

        _circumcircleR = 0.25 * ab * ac * bc / Math.Sqrt(p * (p - ab) * (p - ac) * (p - bc));
        _circumcircleX = -0.5 * (a.Y * (xb2 + yb2 - xc2 - yc2) + b.Y * (xc2 + yc2 - xa2 - ya2) + c.Y * (xa2 + ya2 - xb2 - yb2))
                   / (a.X * (b.Y - c.Y) + b.X * (c.Y - a.Y) + c.X * (a.Y - b.Y));
        _circumcircleY = 0.5 * (a.X * (xb2 + yb2 - xc2 - yc2) + b.X * (xc2 + yc2 - xa2 - ya2) + c.X * (xa2 + ya2 - xb2 - yb2))
                   / (a.X * (b.Y - c.Y) + b.X * (c.Y - a.Y) + c.X * (a.Y - b.Y));
    }
    private void BuildBounds()
    {
        Bounds = new RectangleF((float)(_circumcircleX - _circumcircleR), (float)(_circumcircleY - _circumcircleR),
            2.0f * (float)_circumcircleR, 2.0f * (float)_circumcircleR);
    }
    
    public bool Contains(double x, double y)
    {
        double distance = Math.Sqrt((x - _circumcircleX) * (x - _circumcircleX) 
                                    + (y - _circumcircleY) * (y - _circumcircleY));
        return distance - _circumcircleR <= 1E-14;
    }
}