using System;
using System.Collections.Generic;
using System.Linq;
using SharpPlot.Objects;

namespace SharpPlot.Core.Algorithms;

public static class MathHelper
{
    private static double Epsilon { get; } = 1E-14;

    public static double Distance2D(double ax, double ay, double bx, double by)
    {
        return Math.Sqrt((ax - bx) * (ax - bx) + (ay - by) * (ay - by));
    }
    
    public static double Distance3D(double ax, double ay, double az, double bx, double by, double bz)
    {
        return Math.Sqrt((ax - bx) * (ax - bx) + (ay - by) * (ay - by) + (az - bz) * (az - bz));
    }

    public static bool IsIntersected(Point a, Point b, Point c, Point d, out Point? intersection)
    {
        intersection = null;
        var denominator = (a.X - b.X) * (c.Y - d.Y) - (a.Y - b.Y) * (c.X - d.X);

        if (Math.Abs(denominator) < Epsilon) return false;

        var intersectX = (a.X * b.Y - a.Y * b.X) * (c.X - d.X) - (c.X * d.Y - c.Y * d.X) * (a.X - b.X);
        var intersectY = (a.X * b.Y - a.Y * b.X) * (c.Y - d.Y) - (c.X * d.Y - c.Y * d.X) * (a.Y - b.Y);
        intersectX /= denominator;
        intersectY /= denominator;

        if (intersectX < Math.Min(a.X, b.X) && intersectX > Math.Max(a.X, b.X) &&
            intersectX < Math.Min(c.X, d.X) && intersectX > Math.Max(c.X, d.X) &&
            intersectY < Math.Min(a.Y, b.Y) && intersectY > Math.Max(a.Y, b.Y) &&
            intersectY < Math.Min(c.Y, d.Y) && intersectY > Math.Max(c.Y, d.Y))
        
            return false;

        intersection = new Point(intersectX, intersectY);
        return true;
    }

    /// <summary>
    /// Segment - [a, b], point - p
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="p"></param>
    /// <returns></returns>
    public static bool IsPointOnSegment(Point a, Point b, Point p)
    {
        var d3 = Distance2D(a.X, a.Y, b.X, b.Y);
        var d1 = Distance2D(p.X, p.Y, a.X, a.Y) / d3;
        var d2 = Distance2D(p.X, p.Y, b.X, b.Y) / d3;

        return Math.Abs(d1 + d2 - 1.0) < Epsilon;
    }

    public static bool IsPointOnPolygon(IEnumerable<Point> pointsCollection, Point point)
    {
        var points = pointsCollection.ToArray();
        var pointsCount = points.Length;

        for (int i = 0; i < pointsCount; i++)
        {
            var p1 = points[i];
            var p2 = points[(i + 1) % pointsCount];

            if (IsPointOnSegment(p1, p2, point))
                return true;
        }

        return false;
    }

    public static bool IsPointInsidePolygon(Point point, params Point[] points)
    {
        if (IsPointOnPolygon(points, point)) return false;

        int intersections = 0;

        for (int i = 0; i < points.Length; i++)
        {
            var p1 = points[i];
            var p2 = points[(i + 1) % points.Length];

            if (!(point.Y > Math.Min(p1.Y, p2.Y)) ||
                !(point.Y <= Math.Max(p1.Y, p2.Y)) ||
                !(point.X <= Math.Max(p1.X, p2.X)) ||
                !(Math.Abs(p1.Y - p2.Y) > Epsilon)) continue;

            var xIntersection = (point.Y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y) + p1.X;

            if (Math.Abs(p1.X - p2.X) < Epsilon || point.X <= xIntersection)
            {
                intersections++;
            }
        }

        return intersections % 2 == 1;
    }

    public static bool CanDropPerpendicular(Point point, Point a, Point b, out Point pointProjection)
    {
        var dx = b.X - a.X;
        var dy = b.Y - a.Y;
        var lenghtSqr = dx * dx + dy * dy;
        var wx = point.X - a.X;
        var wy = point.Y - a.Y;
        var scalar = wx * dx + wy * dy;

        if (scalar > 0 && scalar < lenghtSqr)
        {
            var projX = scalar / lenghtSqr * dx;
            var projY = scalar / lenghtSqr * dy;

            pointProjection = new Point(a.X + projX, a.Y + projY);

            return true;
        }

        pointProjection = new Point();
        return false;
    }

    public static Point InterpolateByValue(Point p1, Point p2, double v1, double v2, double v)
    {
        var t = (v - v1) / (v2 - v1);
        var x = p1.X + t * (p2.X - p1.X);
        var y = p1.Y + t * (p2.Y - p1.Y);
        return new Point(x, y);
    }
    
    public static List<Point> MakeRegularPolygon(int verticesCount, Point center, double radius)
    {
        var polygon = new List<Point>();
        var angle = 2.0 * Math.PI / verticesCount;

        for (int i = 0; i < verticesCount; i++)
        {
            polygon.Add(new Point
            {
                X = center.X + radius * Math.Cos(i * angle),
                Y = center.Y + radius * Math.Sin(i * angle)
            });
        }
        
        return polygon;
    }
}