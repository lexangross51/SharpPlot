using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using SharpPlot.Core.Mesh;
using SharpPlot.Objects;

namespace SharpPlot.Core.Algorithms;

public class DelaunayTriangulation
{
    private List<Point> _triPoints = default!;
    private List<Element> _triangles = default!;
    private List<int> _triIndices = default!;
    private List<Edge> _uniqueEdges = default!;

    private void Prepare(Point[] points)
    {
        _triPoints = new List<Point>(points.Length);
        _triangles = new List<Element>(points.Length);
        _triIndices = new List<int>(points.Length);
        _uniqueEdges = new List<Edge>();
    }
    
    public Mesh.Mesh Triangulate(IEnumerable<Point> pointsCollection)
    {
        var points = pointsCollection.ToArray();

        Prepare(points);
        
        var minX = points.Min(p => p.X);
        var maxX = points.Max(p => p.X);
        var minY = points.Min(p => p.Y);
        var maxY = points.Max(p => p.Y);
        var dx = maxX - minX;
        var dy = maxY - minY;

        // Make super triangle that includes all points
        var superTriangle = new Element(new[] { 0, 1, 2 });
        _triangles.Add(superTriangle);
        _triPoints.Add(new Point { X = minX - 1.5 * dx, Y = minY - 1.5 * dy });
        _triPoints.Add(new Point { X = maxX + 1.5 * dx, Y = minY - 1.5 * dy });
        _triPoints.Add(new Point { X = (minX + maxX) * 0.5, Y = maxY + 3.0 * dy });
        
        foreach (var p in points)
        {
            AddPoint(p);
        }

        DeleteSuperTriangle(superTriangle);
        RenumberNodes();
        
        return new Mesh.Mesh(_triPoints, _triangles);
    }
    
    private void AddPoint(Point point)
    {
        _triPoints.Add(point);
        _triIndices.Clear();

        for (int i = 0; i < _triangles.Count; i++)
        {
            var tri = _triangles[i];
            
            // Build circumcircle
            var a = _triPoints[tri.Nodes[0]];
            var b = _triPoints[tri.Nodes[1]];
            var c = _triPoints[tri.Nodes[2]];
            var d = 2 * (a.X * (b.Y - c.Y) + b.X * (c.Y - a.Y) + c.X * (a.Y - b.Y));
            var cx = ((a.X * a.X + a.Y * a.Y) * (b.Y - c.Y) + (b.X * b.X + b.Y * b.Y) * (c.Y - a.Y) + (c.X * c.X + c.Y * c.Y) * (a.Y - b.Y)) / d;
            var cy = ((a.X * a.X + a.Y * a.Y) * (c.X - b.X) + (b.X * b.X + b.Y * b.Y) * (a.X - c.X) + (c.X * c.X + c.Y * c.Y) * (b.X - a.X)) / d;
            var radius = Math.Sqrt((a.X - cx) * (a.X - cx) + (a.Y - cy) * (a.Y - cy));

            var distance = MathHelper.Distance2D(point.X, point.Y, cx, cy);
            if (distance >= radius + 1E-04) continue;
            
            _triIndices.Add(i);
        }
        
        Flip(point);
    }
    
    private void Flip(Point point)
    {
        if (_triIndices.Count == 1)
        {
            var tri = _triangles[_triIndices.First()];
            
            // If the point is inside a triangle -> 3 new triangles
            if (MathHelper.IsPointInsidePolygon(point, _triPoints[tri.Nodes[0]], _triPoints[tri.Nodes[1]],
                    _triPoints[tri.Nodes[2]]))
            {
                _triangles.Add(new Element(new[] { tri.Nodes[0], tri.Nodes[1], _triPoints.Count - 1 }));
                _triangles.Add(new Element(new[] { tri.Nodes[0], tri.Nodes[2], _triPoints.Count - 1 }));
                _triangles.Add(new Element(new[] { tri.Nodes[1], tri.Nodes[2], _triPoints.Count - 1 }));
            }
            // If the point is outside the triangle -> 2 new triangles
            else
            {
                // Find the edge nearest to the new point
                var edgeIndex = 0;
                var dist = double.MaxValue;

                for (int k = 0; k < tri.Edges.Length; k++)
                {
                    var a = _triPoints[tri.Edges[k].Node1];
                    var b = _triPoints[tri.Edges[k].Node2];
                    double tmpDistance;

                    if (!MathHelper.CanDropPerpendicular(point, a, b, out var intersection) ||
                        (tmpDistance = MathHelper.Distance2D(point.X, point.Y, intersection.X, intersection.Y)) >= dist)
                        continue;

                    edgeIndex = k;
                    dist = tmpDistance;
                }

                // Rewrite the edges of the old triangle for the new ones
                for (int k = 0; k < tri.Edges.Length; k++)
                {
                    if (k == edgeIndex) continue;

                    _triangles.Add(new Element(new[]
                        { tri.Edges[k].Node1, tri.Edges[k].Node2, _triPoints.Count - 1 }));
                }
            }
        }
        else
        {
            // Delete non unique edges
            _uniqueEdges.Clear();

            foreach (var edge in _triIndices.SelectMany(index => _triangles[index].Edges))
            {
                _uniqueEdges.Add(edge);
                _uniqueEdges.Add(edge.Flip());
            }
            
            _uniqueEdges = _uniqueEdges.GroupBy(num => num)
                .Where(group => group.Count() == 1)
                .SelectMany(group => group)
                .Where((_, index) => (index & 1) == 1)
                .ToList();

            // Rewrite the edges of the old triangle for the new ones
            foreach (var edge in _uniqueEdges)
            {
                _triangles.Add(new Element(new[] { edge.Node1, edge.Node2, _triPoints.Count - 1 }));
            }
        }

        for (var j = 0; j < _triIndices.Count; j++)
        {
            _triangles.RemoveAt(_triIndices[j]);

            for (int k = 0; k < _triIndices.Count; k++)
            {
                _triIndices[k]--;
            }
        }
    }

    private void DeleteSuperTriangle(Element superTriangle)
    {
        var nodes = superTriangle.Nodes;

        foreach (var node in nodes)
        {
            for (int i = 0; i < _triangles.Count; i++)
            {
                if (!_triangles[i].Nodes.Contains(node)) continue;
                _triangles.RemoveAt(i);
                i--;
            }
        }
        
        _triPoints.RemoveAt(0);
        _triPoints.RemoveAt(0);
        _triPoints.RemoveAt(0);
    }

    private void RenumberNodes()
    {
        foreach (var tri in _triangles)
        {
            for (int i = 0; i < tri.Nodes.Length; i++)
            {
                tri.Nodes[i] -= 3;
            }
            
            for (int i = 0; i < tri.Edges.Length; i++)
            {
                tri.Edges[i].Node1 -= 3;
                tri.Edges[i].Node2 -= 3;
            }
        }
    }
}

public static class Debugger
{
    private static int _fileIndex;
    
    public static void WriteMesh(List<Point> points, List<Element> triangles)
    {
        var sw = new StreamWriter($"C://Users//lexan//source//repos//Python//triangulation//points{_fileIndex}");
        foreach (var p in points)
        {
            sw.WriteLine($"{p.X} {p.Y}", CultureInfo.InvariantCulture);
        }
        sw.Close();
        
        sw = new StreamWriter($"C://Users//lexan//source//repos//Python//triangulation//triangles{_fileIndex}");
        foreach (var p in triangles)
        {
            sw.WriteLine($"{p.Nodes[0]} {p.Nodes[1]} {p.Nodes[2]}", CultureInfo.InvariantCulture);
        }
        sw.Close();

        _fileIndex++;
    }
    
    public static void WritePoints(List<Point> points)
    {
        var sw = new StreamWriter($"C://Users//lexan//source//repos//Python//triangulation//area");
        foreach (var p in points)
        {
            sw.WriteLine($"{p.X} {p.Y}", CultureInfo.InvariantCulture);
        }
        sw.Close();
    }
    
    public static void ReadData(string filename, out List<Point> points, out List<double> values)
    {
        points = new List<Point>();
        values = new List<double>();
        
        var lines = File.ReadAllLines(filename);

        foreach (var line in lines)
        {
            var words = line.Split().Select(val => double.Parse(val, CultureInfo.InvariantCulture)).ToArray();
            points.Add(new Point
            {
                X = words[0],
                Y = words[1],
            });
            values.Add(words[2]);
        }
    }
    
    public static List<Point> GenerateRandomPoints(int pointsCount)
    {
        var random = new Random();
        var points = new HashSet<Point>();

        while (points.Count < pointsCount)
        {
            points.Add(new Point { X = random.NextDouble() * 1000.0, Y = random.NextDouble() * 1000.0 });
        }

        return points.ToList();
    }
    
    public static List<double> GenerateRandomData(int pointsCount)
    {
        var random = new Random();
        var points = new List<double>();

        while (points.Count < pointsCount)
        {
            points.Add(random.NextDouble() * 1000.0);
        }

        return points;
    }
}