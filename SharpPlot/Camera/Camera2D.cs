using OpenTK.Mathematics;
using SharpPlot.Viewport;

namespace SharpPlot.Camera;

public class Camera2D : AbstractCamera
{
    public Camera2D(IProjection projection) : base(projection) {}
    
    public override Matrix4 GetModelMatrix() => Matrix4.Identity;

    public override Matrix4 GetViewMatrix() => Matrix4.Identity;

    public override Matrix4 GetProjectionMatrix()
    {
        var proj = Projection.GetProjection();
        return Matrix4.CreateOrthographicOffCenter(
            (float)proj[0], (float)proj[1],
            (float)proj[2], (float)proj[3],
            (float)proj[4], (float)proj[5]
        );
    }

    public override void Zoom(double xPivot, double yPivot, double delta) 
        => Projection.Scale(xPivot, yPivot, delta);

    public override void Move(double dx, double dy)
        => Projection.Translate(dx, dy);

    public override void Rotate(double ox, double oy, double oz, double angle) {}
}