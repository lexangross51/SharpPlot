using System;
using OpenTK.Mathematics;
using SharpPlot.Core.Drawing.Projection.Interfaces;
using SharpPlot.Core.Drawing.Render;

namespace SharpPlot.Core.Drawing.Camera;

public class ArcBallCamera(IProjection projection, FrameSettings settings) : ICamera
{
    private Vector3 _rotationAxisTmp, _rotationAxis;
    private Quaternion _prevQuaternion = new(Vector3.UnitX, 0.0f), _currQuaternion;
    private double _cosValue;
    
    public Matrix4 ViewMatrix { get; private set; } = Matrix4.Identity;
    
    public Vector3 ObjectPosition { get; set; }
    
    public double Radius { get; set; }
        
    public void Zoom(double pivotX, double pivotY, double delta)
    {
        
    }

    public void Move(Vector3d from, Vector3d to)
    {
        
    } 
    
    public void Rotate(Vector3d from, Vector3d to)
    {
        var from3D = GetNdcCoordinate(from);
        var to3D = GetNdcCoordinate(to);

        _currQuaternion.Xyz = Vector3.Cross(from3D, to3D);
        _currQuaternion.Xyz = Normalize(_currQuaternion.Xyz);
        
        from3D = Normalize(from3D);
        to3D = Normalize(to3D);
        var scalar = Vector3.Dot(from3D, to3D);

        if (scalar > 1.0f)
            scalar = 1.0f;

        var theta = Math.Acos(scalar) * 180.0 / Math.PI;
        _currQuaternion.W = (float)Math.Cos(theta * 0.5 * Math.PI / 180.0);
        _currQuaternion.Xyz *= (float)Math.Sin(theta * 0.5 * Math.PI / 180.0);

        _cosValue = _currQuaternion.W * _prevQuaternion.W - Vector3.Dot(_currQuaternion.Xyz, _prevQuaternion.Xyz);

        var axis = Vector3.Cross(_currQuaternion.Xyz, _prevQuaternion.Xyz);
        
        _rotationAxisTmp.X = _currQuaternion.W * _prevQuaternion.Xyz.X + _prevQuaternion.W * _currQuaternion.Xyz.X + axis.X;
        _rotationAxisTmp.Y = _currQuaternion.W * _prevQuaternion.Xyz.Y + _prevQuaternion.W * _currQuaternion.Xyz.Y + axis.Y;
        _rotationAxisTmp.Z = _currQuaternion.W * _prevQuaternion.Xyz.Z + _prevQuaternion.W * _currQuaternion.Xyz.Z + axis.Z;
        
        var angle = Math.Acos(_cosValue) * 180.0 / Math.PI * 2.0;
        
        _rotationAxis.X = _rotationAxisTmp.X / (float)Math.Sin(angle * 0.5 * Math.PI / 180.0);
        _rotationAxis.Y = _rotationAxisTmp.Y / (float)Math.Sin(angle * 0.5 * Math.PI / 180.0);
        _rotationAxis.Z = _rotationAxisTmp.Z / (float)Math.Sin(angle * 0.5 * Math.PI / 180.0);

        ViewMatrix = Matrix4.CreateTranslation(ObjectPosition);
        ViewMatrix *= Matrix4.CreateFromAxisAngle(_rotationAxis, (float)MathHelper.DegreesToRadians(angle));
    }

    public void StopRotate()
    {
        _prevQuaternion.W = (float)_cosValue;
        _prevQuaternion.Xyz = _rotationAxisTmp;
    }

    private Vector3 GetNdcCoordinate(Vector3d pos)
    {
        var w = settings.ScreenWidth;
        var h = settings.ScreenHeight;
        
        var ndcPos = new Vector2((float)(pos.X / w), (float)(pos.Y / h)) * 2.0f - Vector2.One;
        var lenSqr = ndcPos.LengthSquared;
        var z = lenSqr > Radius * Radius ? 0.0f : (float)Math.Sqrt(Radius * Radius - lenSqr);
        
        return new Vector3(ndcPos.X, ndcPos.Y, z);
    }

    private static Vector3 Normalize(Vector3 vector)
    {
        if (vector.Length > 0.0f)
            vector.Normalize();
        else
            vector = Vector3.Zero;

        return vector;
    }
}