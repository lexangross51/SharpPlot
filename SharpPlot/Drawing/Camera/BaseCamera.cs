using OpenTK.Mathematics;
using SharpPlot.Drawing.Projection.Interfaces;

namespace SharpPlot.Drawing.Camera;

public abstract class BaseCamera(IProjection projection)
{
    protected readonly IProjection Projection = projection;

    public abstract Matrix4 ViewMatrix { get; }
    
    public abstract void Zoom(double pivotX, double pivotY, double delta);

    public abstract void Move(double dx, double dy);
}