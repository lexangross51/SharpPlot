using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SharpPlot.Objects;
using Point = SharpPlot.Objects.Point;

namespace SharpPlot.Core.Mesh;

public class ColorMap : IBaseObject
{
    public PrimitiveType ObjectType { get; }
    public int PointSize { get; }
    public Point[] Points { get; }
    public Color4[] Colors { get; }
    public uint[]? Indices { get; }
    
    public ColorMap(Mesh mesh, IEnumerable<double> valuesCollection, Palette.Palette palette, 
        ColorInterpolationType interpolation = ColorInterpolationType.Linear)
    {
        ObjectType = mesh.ObjectType;
        PointSize = 1;
        Points = mesh.Points;
        Indices = mesh.Indices;

        var values = valuesCollection.ToArray();
        Colors = new Color4[Points.Length];
        
        var colorsCount = palette.ColorsCount;
        var maxValue = values.Max();
        var minValue = values.Min();
        var valueStep = (maxValue - minValue) / colorsCount;
        var valuesRanges = new double[colorsCount + 1];

        for (int i = 0; i < colorsCount + 1; i++)
        {
            valuesRanges[i] = minValue + i * valueStep;
        }
        valuesRanges[^1] = maxValue;
        
        for (int j = 0; j < Points.Length; j++)
        {
            var valueAtPoint = values[j];
            var interpolated = ColorInterpolator.InterpolateColor(valuesRanges, valueAtPoint, palette, interpolation);
            
            Colors[j] = interpolated;
        }
    }
    
    public void BoundingBox(out Point leftBottom, out Point rightTop)
    {
        var minX = Points.MinBy(p => p.X).X;
        var maxX = Points.MaxBy(p => p.X).X;
        var minY = Points.MinBy(p => p.Y).Y;
        var maxY = Points.MaxBy(p => p.Y).Y;
        var minZ = Points.MinBy(p => p.Z).Z;
        var maxZ = Points.MaxBy(p => p.Z).Z;

        leftBottom = new Point(minX, minY, minZ);
        rightTop = new Point(maxX, maxY, maxZ);
    }
}