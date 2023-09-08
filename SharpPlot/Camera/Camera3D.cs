using OpenTK.Mathematics;
using SharpPlot.Objects;
using SharpPlot.Viewport;

namespace SharpPlot.Camera;

public class Camera3D : AbstractCamera
{
    private Point _position;
    private Point _target;
    private double _fov;
    
    public Camera3D(IProjection projection, Point pos, Point target) : base(projection)
    {
        _fov = 45.0;
        _position = pos;
        _target = target;
    }

    public override Matrix4 GetModelMatrix()
    {
        return Matrix4.Identity;
    }

    public override Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(0f, 0f, 10, 0, 0, 0, 0, 1, 0 );
    }

    public override Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView((float)MathHelper.DegreesToRadians(_fov), 
            (float)Projection.Ratio, 0.01f, 100.0f);
    }

    public override void Zoom(double xPivot, double yPivot, double delta)
    {
        if (delta > 0) _fov *= 1.0 / 1.05;
        else _fov *= 1.05;
    }

    public override void Move(double dx, double dy)
    {
    }

    public override void Rotate(double ox, double oy, double oz, double angle)
    {
    }
}