using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SharpPlot.Algorithms.Tree;
using SharpPlot.Geometry;
using SharpPlot.Geometry.Interfaces;

namespace SharpPlot.Algorithms.Meshing;

public class IncrementalDelaunay
{
    private QuadTree<ITriangle> _tris = null!;
    private HashSet<ITriangle> _badTriangles = null!;
    private HashSet<Edge> _uniqueEdges = null!;
    private HashSet<Edge> _duplicates = null!;
    private int _pointId;

    public IMesh Triangulate(IEnumerable<Point3D> pointsCollection)
    {
        var points = pointsCollection.ToArray();
        var pointsCount = points.Length;
        
        _uniqueEdges = new HashSet<Edge>(2 * pointsCount);
        _duplicates = new HashSet<Edge>(2 * pointsCount);
        _badTriangles = new HashSet<ITriangle>(2 * pointsCount);

        double minX = points.Min(p => p.X);
        double minY = points.Min(p => p.Y);
        double maxX = points.Max(p => p.X);
        double maxY = points.Max(p => p.Y);

        double dx = maxX - minX;
        double dy = maxY - minY;
        double deltaMax = Math.Max(dx, dy) * 20.0;
        double midX = (minX + maxX) / 2.0;
        double midY = (minY + maxY) / 2.0;
        
        _tris = new QuadTree<ITriangle>(new RectangleF((float)(minX - 10.0 * deltaMax), (float)(minY - 10.0 * deltaMax), 
            20.0f * (float)deltaMax, 20.0f * (float)deltaMax));

        // Super triangle
        var p1 = new Point3D(midX - deltaMax, midY - deltaMax, 0.0) { Id = 0 };
        var p2 = new Point3D(midX, midY + deltaMax, 0.0) { Id = 1 };
        var p3 = new Point3D(midX + deltaMax, midY - deltaMax, 0.0) { Id = 2 };
        var t0 = new Triangle(p1, p2, p3);
        
        _tris.Insert(t0, t0.Bounds);
        _pointId = 3;
        
        for (var i = 0; i < points.Length; i++)
        {
            var p = points[i];
            p.Id = _pointId++;
            points[i] = p;
            
            AddPoint(p);
        }

        var triangles = _tris.CollectAll().ToList();
        DeleteSuperTriangle(triangles);

        foreach (var triangle in triangles)
        {
            for (var i = 0; i < triangle.Points.Length; i++)
            {
                var p = triangle.Points[i];
                p.Id -= 3;
                triangle.Points[i] = p;
            }
        }

        return new Mesh(triangles, points.ToList());
    }

    private void AddPoint(Point3D p)
    {
        _badTriangles.Clear();
        _uniqueEdges.Clear();
        _duplicates.Clear();

        _tris.Query(p, _badTriangles);

        // Take only unique edges
        foreach (var t in _badTriangles)
        {
            var edges = t.Edges;
            foreach (var e in edges)
            {
                if (_uniqueEdges.Add(e)) continue; 
                _duplicates.Add(e);
            }
        }
        
        _uniqueEdges.ExceptWith(_duplicates);

        // Remove bad triangles
        foreach (var t in _badTriangles)
        {
            _tris.Remove(t);
        }

        // Make new triangles
        foreach (var e in _uniqueEdges)
        {
            var t = new Triangle(e.P1, e.P2, p);
            // _triangles.Add(t);
            _tris.Insert(t, t.Bounds);
        }
    }
    
    private void DeleteSuperTriangle(List<ITriangle> triangles)
        => triangles.RemoveAll(t => t.Points.Any(p => p.Id is 0 or 1 or 2));
}