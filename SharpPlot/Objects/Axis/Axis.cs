using System;
using System.Collections.Generic;

namespace SharpPlot.Objects.Axis;

public class Axis
{
    public static readonly string TemplateCaption = "00e+00";
    public List<double> Points { get; }
    public string Name { get; set; }

    public Axis(string name = "")
    {
        Points = new List<double>();
        Name = name;
    }

    public void GenerateTicks(double start, double end, double step)
    {
        Points.Clear();

        for (double fCur = Math.Floor(start / step) * step; fCur <= end; fCur += step)
        {
            Points.Add(Math.Abs(fCur) < step / 4 ? 0.0 : fCur);
        }
    }
}