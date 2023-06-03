using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Point = System.Windows.Point;

namespace SharpPlot.Objects.Plots;

public class Plot : IBaseObject
{
    public PrimitiveType Type { get; set; } = PrimitiveType.LineStrip;
    public int PointSize { get; set; } = 1;
    public List<Point> Points { get; set; }
    public List<int>? VertexIndices { get; set; }
    public List<Color> Colors { get; set; }
    
    public (Point LeftBottom, Point RightTop) BoundingBox()
    {
        var minX = Points.MinBy(p => p.X).X;
        var maxX = Points.MaxBy(p => p.X).X;
        var minY = Points.MinBy(p => p.Y).Y;
        var maxY = Points.MaxBy(p => p.Y).Y;

        return (new Point(minX, minY), new Point(maxX, maxY));
    }

    public Plot(IEnumerable<double> args, IEnumerable<double> values, Color color)
    {
        var argsArray = args as double[] ?? args.ToArray();
        var valuesArray = values as double[] ?? values.ToArray();
        
        if (argsArray.Length != valuesArray.Length) throw new Exception("Arrays must be the same size");

        Points = new List<Point>(argsArray.Length);
        Colors = new List<Color>(argsArray.Length);

        for (int i = 0; i < argsArray.Length; i++)
        {
            Points.Add(new Point(argsArray[i], valuesArray[i]));
            Colors.Add(color);
        }
    }
}