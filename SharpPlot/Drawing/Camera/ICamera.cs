using OpenTK.Mathematics;

namespace SharpPlot.Drawing.Camera;

public interface ICamera
{
    public abstract Matrix4 ViewMatrix { get; }
    
    public abstract void Zoom(double pivotX, double pivotY, double delta);
    
    public abstract void Move(Vector3d from, Vector3d to);
}