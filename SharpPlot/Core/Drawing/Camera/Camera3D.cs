using System;
using OpenTK.Mathematics;
using SharpPlot.Core.Drawing.Projection.Interfaces;
using SharpPlot.Core.Drawing.Render;

namespace SharpPlot.Core.Drawing.Camera;

public class Camera3D(IProjection projection, FrameSettings settings) : ICamera
{
    public Matrix4 ViewMatrix { get; private set; } = Matrix4.Identity;
        
    public void Zoom(double pivotX, double pivotY, double delta)
    {
        
    }

    public void Move(Vector3d from, Vector3d to)
    {
        var w = settings.ScreenWidth;
        var h = settings.ScreenHeight;
        
        var fromNdc = new Vector2((float)(from.X / w), (float)((h - from.Y) / h)) * 2.0f - Vector2.One;
        var toNdc = new Vector2((float)(to.X / w), (float)((h - to.Y) / h)) * 2.0f - Vector2.One;
        
        var fromLengthSq = fromNdc.LengthSquared;
        var toLengthSq = toNdc.LengthSquared;
        
        var fromZ = fromLengthSq > 1.0f ? 0.0f : (float)Math.Sqrt(1.0f - fromLengthSq);
        var toZ = toLengthSq > 1.0f ? 0.0f : (float)Math.Sqrt(1.0f - toLengthSq);
        
        var from3D = new Vector3(fromNdc.X, fromNdc.Y, fromZ);
        var to3D = new Vector3(toNdc.X, toNdc.Y, toZ);
        
        from3D.Normalize();
        to3D.Normalize();
        
        var rotation = Quaternion.FromAxisAngle(Vector3.Cross(from3D, to3D), (float)Math.Acos(Math.Clamp(Vector3.Dot(from3D, to3D), -1.0f, 1.0f)));

        var objectPosition = Vector3.Zero;
        var eye = objectPosition + Vector3.TransformPosition(new Vector3(0, 0, -1f), CreateFromQuaternion(rotation));
        // var eye = objectPosition + rotation * new Vector3(0, 0, -1f);
        var up = rotation * Vector3.UnitY;
        ViewMatrix *= Matrix4.LookAt(eye, objectPosition, up);
    }
    
    private static Matrix4 CreateFromQuaternion(Quaternion q)
    {
        q.ToAxisAngle(out var axis, out var angle);
        return Matrix4.CreateFromAxisAngle(axis, angle);
    }
}