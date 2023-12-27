using OpenTK.Mathematics;

namespace SharpPlot.Camera;

public abstract class BaseCamera
{
    public Matrix4 ViewMatrix { get; }
    
    public abstract void Zoom(double pivotX, double pivotY, double delta);

    public abstract void Move(double dx, double dy);
}