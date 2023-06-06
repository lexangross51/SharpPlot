using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using SharpGL;
using SharpGL.Enumerations;
using SharpPlot.Render;

namespace SharpPlot.Objects;

public enum ColorbarLocation
{
    BottomLeft,
    BottomRight,
    TopLeft,
    TopRight
}

public class Colorbar : IRenderable
{
    private const int MaxValuesCount = 8;
    private readonly List<double> _values;
    private readonly List<Color> _colors;
    private ColorInterpolation _interpolation;

    public ColorbarLocation Location { get; set; } = ColorbarLocation.BottomRight;

    public Colorbar(IEnumerable<double> values, Palette palette,
        ColorInterpolation interpolation = ColorInterpolation.Constant)
    {
        _values = new List<double>();
        _colors = new List<Color>();
        _interpolation = interpolation;

        var valuesList = values.ToList();
        double max = valuesList.Max();
        double min = valuesList.Min();
        int colorsCount = palette.ColorsCount;

        for (int i = 0; i < colorsCount; i++)
        {
            _colors.Add(palette[i]);
        }

        var (valueStep, valuesCount) = colorsCount < MaxValuesCount - 1
            ? ((max - min) / (colorsCount + 1), colorsCount + 1)
            : ((max - min) / MaxValuesCount, MaxValuesCount);

        for (int i = 0; i <= valuesCount; i++)
        {
            _values.Add(min + i * valueStep);
        }
    }
    
    public PrimitiveType Type { get; set; } = PrimitiveType.Quads;
    public int PointSize { get; set; } = 1;
    public List<Point> Points { get; set; } = null!;
    public List<Color> Colors { get; set; } = null!;

    public void BoundingBox(out Point? leftBottom, out Point? rightTop)
    {
        leftBottom = null;
        rightTop = null;
    }

    public void Render(IBaseGraphic graphic)
    {
        var xStart = graphic.ScreenSize.Width - 100;
        
        graphic.GL.MatrixMode(MatrixMode.Projection);
        graphic.GL.PushMatrix();
        graphic.GL.LoadIdentity();
        graphic.GL.Viewport((int)xStart, (int)graphic.Indent.Vertical, 
            (int)graphic.ScreenSize.Width, (int)graphic.Indent.Vertical + 100);
        graphic.GL.Ortho(0, 1, 0,  1, -1, 1);
        graphic.GL.MatrixMode(MatrixMode.Modelview);
        graphic.GL.PushMatrix();
        graphic.GL.LoadIdentity();

        graphic.GL.Color(1f, 1f, 1f);
        graphic.GL.Begin(OpenGL.GL_QUADS);
        graphic.GL.Vertex(0, 0);
        graphic.GL.Vertex(1, 0);
        graphic.GL.Vertex(1, 1);
        graphic.GL.Vertex(0, 1);
        graphic.GL.End();
        
        graphic.GL.Color(0f, 0f, 0f);
        graphic.GL.Begin(OpenGL.GL_LINE_LOOP);
        graphic.GL.Vertex(0, 0);
        graphic.GL.Vertex(1, 0);
        graphic.GL.Vertex(1, 1);
        graphic.GL.Vertex(0, 1);
        graphic.GL.End();
        
        graphic.GL.PopMatrix();
        graphic.GL.MatrixMode(MatrixMode.Projection);
        graphic.GL.PopMatrix();
        graphic.GL.MatrixMode(MatrixMode.Modelview);
    }
}