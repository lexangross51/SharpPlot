using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Windows;
using SharpGL;
using SharpGL.Enumerations;
using SharpPlot.Render;

namespace SharpPlot.Objects.Plots;

public class Scatter : IRenderable
{
    public PrimitiveType Type { get; set; } = PrimitiveType.Points;
    public int PointSize { get; set; }
    public List<Point> Points { get; set; }
    public List<Color> Colors { get; set; }
    
    public void BoundingBox(out Point? leftBottom, out Point? rightTop)
    {
        var minX = Points.MinBy(p => p.X).X;
        var maxX = Points.MaxBy(p => p.X).X;
        var minY = Points.MinBy(p => p.Y).Y;
        var maxY = Points.MaxBy(p => p.Y).Y;

        leftBottom = new Point(minX, minY);
        rightTop = new Point(maxX, maxY);
    }

    public Scatter(IEnumerable<double> args, IEnumerable<double> values, Color color, int size = 5)
    {
        var argsArray = args as double[] ?? args.ToArray();
        var valuesArray = values as double[] ?? values.ToArray();
        
        Points = new List<Point>(argsArray.Length);
        Colors = new List<Color>(1) { color };
        PointSize = size;

        for (int i = 0; i < argsArray.Length; i++)
        {
            Points.Add(new Point(argsArray[i], valuesArray[i]));
        }
    }
    
    public void Render(IBaseGraphic graphic)
    {
        graphic.GL.PointSize(PointSize);
        graphic.GL.Enable(OpenGL.GL_POINT_SMOOTH);
        graphic.GL.Color(Colors[0].R, Colors[0].G, Colors[0].B, Colors[0].A);
        graphic.GL.Begin((BeginMode)Type);

        foreach (var point in Points)
        {
            graphic.GL.Vertex(point.X, point.Y);
        }
        
        graphic.GL.End();
        graphic.GL.Disable(OpenGL.GL_POINT_SMOOTH);
        graphic.GL.PointSize(1);
    }
}