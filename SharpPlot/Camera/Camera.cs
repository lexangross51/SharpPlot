using System;
using System.Diagnostics;
using OpenTK.Mathematics;

namespace SharpPlot.Camera;

public enum CameraDirection
{
    Forward,
    Backward,
    Left,
    Right
}

public class DeltaTime
{
    private float _lastFrame;
    private float _deltaTime;
    private readonly Stopwatch _sw = new();

    public float Result()
    {
        Compute();
        return _deltaTime;
    }

    private void Compute()
    {
        _sw.Stop();

        float currentFrame = _sw.ElapsedMilliseconds / 1000.0f;
        _deltaTime = currentFrame - _lastFrame;
        _lastFrame = currentFrame;

        _sw.Start();
    }
}

public class Camera
{
    private float _xPrev, _yPrev;
    private float _pitch;
    private float _yaw;
    private float _fieldOfView;
    private Vector3 _position;
    private Vector3 _direction;
    private Vector3 _up;
    private Vector3 _right;
    public bool IsFirstMove = true; 

    public Vector3 Position => _position;
    public Vector3 Direction => _direction;
    public float AspectRation { private get; set; }
    public float Speed { get; set; }
    public float Sensitivity { get; set; }

    public float FieldOfView
    {
        get => MathHelper.RadiansToDegrees(_fieldOfView);
        set => _fieldOfView = MathHelper.DegreesToRadians(value);
    }

    public Camera(Vector3 position)
    {
        _position = position;
        _direction = -Vector3.UnitZ;
        FieldOfView = 45.0f;
        _up = Vector3.UnitY;
        _right = Vector3.UnitX;
        _pitch = 0.0f;
        _yaw = -MathHelper.PiOver2;

        Speed = 0.2f;
        Sensitivity = 0.01f;
    }

    public Matrix4 ViewMatrix() =>
        Matrix4.LookAt(_position, _position + _direction, _up);
    
    public Matrix4 ProjectionMatrix() =>
        Matrix4.CreatePerspectiveFieldOfView(_fieldOfView, AspectRation, 0.1f, 100.0f);

    public void LookAt(float x, float y)
    {
        if (Math.Abs(_xPrev - x) < 1E-05 && Math.Abs(_yPrev - y) < 1E-05) return;

        if (IsFirstMove)
        {
            _xPrev = x;
            _yPrev = y;
            IsFirstMove = false;
        }

        var dx = x - _xPrev;
        var dy = _yPrev - y;

        _xPrev = x;
        _yPrev = y;

        _yaw += dx * Sensitivity;
        _pitch += dy * Sensitivity;

        if (_pitch > 90.0f) _pitch = 90.0f;
        if (_pitch < -90.0f) _pitch = -90.0f;

        UpdateVectors();
    }
    
    public void Move(CameraDirection direction)
    {
        switch (direction)
        {
            case CameraDirection.Forward:
                _position += _direction * Speed;
                break;
            case CameraDirection.Backward:
                _position -= _direction * Speed;
                break;
            case CameraDirection.Left:
                _position -= _right * Speed;
                break;
            case CameraDirection.Right:
                _position += _right * Speed;
                break;
            default:
                return;
        }
    }
    
    private void UpdateVectors()
    {
        float sinYaw = MathF.Sin(_yaw);
        float cosYaw = MathF.Cos(_yaw);
        float sinPitch = MathF.Sin(_pitch);
        float cosPitch = MathF.Cos(_pitch);

        _direction.X = cosPitch * cosYaw;
        _direction.Y = sinPitch;
        _direction.Z = cosPitch * sinYaw;
        
        _direction = Vector3.Normalize(_direction);
        
        _right = Vector3.Normalize(Vector3.Cross(_direction, Vector3.UnitY));
        _up = Vector3.Normalize(Vector3.Cross(_right, _direction));
    }
}