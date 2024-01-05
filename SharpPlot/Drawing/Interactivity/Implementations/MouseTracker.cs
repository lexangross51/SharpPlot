using SharpPlot.Drawing.Interactivity.Interfaces;
using SharpPlot.Drawing.Projection.Interfaces;
using SharpPlot.Drawing.Render;

namespace SharpPlot.Drawing.Interactivity.Implementations;

public class MouseTracker(IProjectionConvertable projection, FrameSettings settings) : IMouseTracker
{
    public double X { get; private set; }
    public double Y { get; private set; }
    
    public void Update(double x, double y)
    {
        var position = projection.FromWorldToProjection(x, y, settings);
        X = position.X;
        Y = position.Y;
    }
}