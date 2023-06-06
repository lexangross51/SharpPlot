using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SharpPlot.Mathematics;

public static class Geometry
{
    public static void BuildContourHull(ref List<Point> points)
    {
        points.Sort((a, b) => a.X.CompareTo(b.X));

        List<Point> lowerHull = new();
        foreach (var p in points)
        {
            while (lowerHull.Count >= 2 && Cross(lowerHull[^2], lowerHull[^1], p) <= 0)
            {
                lowerHull.RemoveAt(lowerHull.Count - 1);
            }
            lowerHull.Add(new Point { X = p.X, Y = p.Y });
        }

        List<Point> upperHull = new();
        for (int i = points.Count - 1; i >= 0; i--)
        {
            var p = points[i];
            while (upperHull.Count >= 2 && Cross(upperHull[^2], upperHull[^1], p) <= 0)
            {
                upperHull.RemoveAt(upperHull.Count - 1);
            }
            upperHull.Add(new Point { X = p.X, Y = p.Y });
        }

        upperHull.RemoveAt(0);
        upperHull.RemoveAt(upperHull.Count - 1);
        lowerHull.AddRange(upperHull);
        points = lowerHull;
    }

    private static double Cross(Point o, Point a, Point b)
        => (a.X - o.X) * (b.Y - o.Y) - (a.Y - o.Y) * (b.X - o.X);
}