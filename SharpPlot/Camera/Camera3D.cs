using System;
using OpenTK.Mathematics;
using SharpPlot.Viewport;

namespace SharpPlot.Camera;

public class Camera3D : AbstractCamera
{
    private Vector3 _position;
    private readonly Vector3 _target;
    private double _fov;
    private float _yaw;
    private float _pitch;
    private float _lastX, _lastY;
    private readonly Vector3 _worldUp;
    private Vector3 _right;
    private Vector3 _front;
    private Vector3 _up;
    private float _distance;

    public bool FirstMouse { get; set; }
    public float Sensitivity { get; set; }
    
    public Camera3D(IProjection projection, Vector3 pos, Vector3 target) : base(projection)
    {
        _fov = 45.0;
        _position = pos;
        _target = target;
        _front = new Vector3(target.X - pos.X, target.Y - pos.Y, target.Z - pos.Z);
        _worldUp = new Vector3(0.0f, 1.0f, 0.0f);
        _up = new Vector3(0.0f, 1.0f, 0.0f);
        _yaw = -90.0f;
        _pitch = 0.0f;
        _lastX = _lastY = 0.0f;
        _distance = Vector3.Distance(_target, _position);

        FirstMouse = true;
        Sensitivity = 0.3f;

        UpdateVectors();
    }
    
    public override Matrix4 GetModelMatrix()
    {
        return Matrix4.CreateRotationX(MathHelper.PiOver2);
    }

    public override Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(_position.X, _position.Y, _position.Z, 
            _position.X + _front.X, _position.Y + _front.Y, _position.Z + _front.Z, 
            _up.X, _up.Y, _up.Z);
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

    public override void Move(double xPos, double yPos)
    {        
        if (Math.Abs(_lastX - xPos) < 1E-05 && Math.Abs(_lastY - yPos) < 1E-05) return;

        if (FirstMouse)
        {
            _lastX = (float)xPos;
            _lastY = (float)yPos;
            FirstMouse = false;
        }

        var offsetX = xPos - _lastX;
        var offsetY = _lastY - yPos;

        _lastX = (float)xPos;
        _lastY = (float)yPos;

        _yaw += (float)offsetX * Sensitivity;
        _pitch += (float)offsetY * Sensitivity;

        if (_pitch > 89.0f) _pitch = 89.0f;
        if (_pitch < -89.0f) _pitch = -89.0f;

        UpdateVectors();
    }
    
    private void UpdateVectors()
    {
        var radYaw = MathHelper.DegreesToRadians(_yaw);
        var radPitch = MathHelper.DegreesToRadians(_pitch);

        var cosYaw = Math.Cos(radYaw);
        var sinYaw = Math.Sin(radYaw);
        var cosPitch = Math.Cos(radPitch);
        var sinPitch = Math.Sin(radPitch);

        var front = new Vector3((float)(cosYaw * cosPitch), (float)sinPitch, (float)(sinYaw * cosPitch));
        
        _front = Vector3.Normalize(front);
        _right = Vector3.Normalize(Vector3.Cross(_front, _worldUp));
        _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        
        _position.X = _target.X - _distance * _front.X;
        _position.Y = _target.Y - _distance * _front.Y;
        _position.Z = _target.Z - _distance * _front.Z;
    }
}