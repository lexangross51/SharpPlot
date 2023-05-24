using System.Collections.Generic;
using System.Drawing;
using SharpPlot.Text;
using SharpPlot.Viewport;

namespace SharpPlot.Objects.Axes;

public abstract class Axis
{
    public static readonly Caption TemplateCaption = new() { Text = "Y,00e+00" };
    public List<double> Points { get; set; }
    public Caption AxisName { get; set; }

    protected Axis()
    {
        Points = new List<double>();
        AxisName = new Caption()
        {
            Font = new SharpPlotFont()
            {
                Color = Color.Blue,
            }
        };
    }

    public abstract void GeneratePoints(double start, double end, double step);
}