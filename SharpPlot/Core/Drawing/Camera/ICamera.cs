using OpenTK.Mathematics;

namespace SharpPlot.Core.Drawing.Camera;

public interface ICamera
{
    public Matrix4 ViewMatrix { get; }
    
    public void Zoom(double pivotX, double pivotY, double delta);
    
    public void Rotate(Vector3d from, Vector3d to);
}