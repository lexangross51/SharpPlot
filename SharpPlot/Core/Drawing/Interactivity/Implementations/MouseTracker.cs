using SharpPlot.Core.Drawing.Interactivity.Interfaces;
using SharpPlot.Core.Drawing.Projection.Interfaces;
using SharpPlot.Core.Drawing.Render;

namespace SharpPlot.Core.Drawing.Interactivity.Implementations;

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