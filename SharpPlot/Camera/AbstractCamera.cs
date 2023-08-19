using OpenTK.Mathematics;

namespace SharpPlot.Camera;

public abstract class AbstractCamera
{
    public Matrix4 ModelMatrix { get; }
    public Matrix4 ViewMatrix { get; }
    public Matrix4 ProjectionMatrix { get; }

    public abstract void Zoom(double xPivot, double yPivot, double delta);
    public abstract void Move(double dx, double dy);
}