using System;
using System.Collections.Generic;

namespace SharpPlot.Objects.Axes;

public class Axis
{
    public const string TemplateCaption = "00e+00";
    public List<double> Points { get; set; }
    public string AxisName { get; set; }

    public Axis(string name = "")
    {
        Points = new List<double>();
        AxisName = name;
    }

    public void GeneratePoints(double start, double end, double step, double scale = 1.0)
    {
        Points.Clear();
        step *= scale;
        start *= scale;
        end *= scale;

        for (double fCur = Math.Floor(start / step) * step; fCur <= end; fCur += step)
        {
            Points.Add(Math.Abs(fCur) < step / 4 ? 0.0 : fCur);
        }
    }
}