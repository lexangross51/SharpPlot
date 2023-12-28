using OpenTK.Mathematics;
using SharpPlot.Drawing.Projection.Interfaces;

namespace SharpPlot.Drawing.Camera;

public class Camera2D(IProjection projection) : BaseCamera(projection)
{
    public override Matrix4 ViewMatrix => Matrix4.Identity;

    public override void Zoom(double pivotX, double pivotY, double delta)
        => Projection.Scale(pivotX, pivotY, delta);

    public override void Move(double dx, double dy) 
        => Projection.Translate(dx, dy);
}