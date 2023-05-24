using System;
using System.Windows.Forms;
using SharpPlot.Text;
using SharpPlot.Viewport;

namespace SharpPlot.Objects.Axes;

public class HorizontalAxis : Axis
{
    public override void GeneratePoints(double start, double end, double step)
    {
        Points.Clear();

        for (double fCur = Math.Floor(start / step) * step; fCur <= end; fCur += step)
        {
            Points.Add(Math.Abs(fCur) < step / 4 ? 0.0 : fCur);
        }
    }
}