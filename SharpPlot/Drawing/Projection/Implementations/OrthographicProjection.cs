using OpenTK.Mathematics;
using SharpPlot.Drawing.Projection.Interfaces;
using SharpPlot.Drawing.Render;

namespace SharpPlot.Drawing.Projection.Implementations;

public class OrthographicProjection : IProjection, IProjectionConvertable
{
    private readonly double[] _projectionArray = new double[6];
    private double _hCenter, _vCenter, _halfHStep, _halfVStep;

    public Matrix4 ProjectionMatrix => Matrix4.CreateOrthographicOffCenter(
            (float)(_hCenter - _halfHStep),
            (float)(_hCenter + _halfHStep),
            (float)(_vCenter - _halfVStep),
            (float)(_vCenter + _halfVStep),
            -1.0f, 1.0f);

    public OrthographicProjection(double left, double right, double bottom, double top, double near, double far)
    {
        _hCenter = 0.5 * (left + right);
        _vCenter = 0.5 * (bottom + top);
        _halfHStep = 0.5 * (right - left);
        _halfVStep = 0.5 * (top - bottom);
    }

    public double[] ToArray()
    {
        _projectionArray[0] = _hCenter - _halfHStep;
        _projectionArray[1] = _hCenter + _halfHStep;
        _projectionArray[2] = _vCenter - _halfVStep;
        _projectionArray[3] = _vCenter + _halfVStep;
        _projectionArray[4] = -1.0f;
        _projectionArray[5] = 1.0f;

        return _projectionArray;
    }

    public void Scale(double pivotX, double pivotY, double delta)
    {
        throw new System.NotImplementedException();
    }

    public void Translate(double dx, double dy)
    {
        _hCenter += dx;
        _vCenter += dy;
    }

    public Vector3d FromWorldToProjection(double sx, double sy, RenderSettings settings)
    {
        var result = new Vector3d();
        
        if (sx < settings.Margin)
        {
            result.X = _hCenter - _halfHStep;
        }
        else if (sx > settings.ScreenWidth)
        {
            result.X = _hCenter + _halfHStep;
        }
        else
        {
            double coefficient = (sx - settings.Margin) / (settings.ScreenWidth - settings.Margin);
            result.X = _hCenter + (2 * coefficient - 1) * _halfHStep;
        }

        if (sy < 0.0)
        {
            result.Y = _vCenter + _halfVStep;
        }
        else if (sy > settings.ScreenHeight)
        {
            result.Y = _vCenter - _halfVStep;
        }
        else
        {
            double coefficient = (settings.ScreenHeight - settings.Margin - sy) / (settings.ScreenHeight - settings.Margin);
            result.Y = _vCenter + (2 * coefficient - 1) * _halfVStep;
        }

        return result;
    }

    public Vector3d FromProjectionToWorld(double px, double py, RenderSettings settings)
    {
        var result = new Vector3d();
        
        var dx = px - (_hCenter - _halfHStep);
        var dy = py - (_vCenter - _halfVStep);
        var coefficient = dx / (2.0 * _halfHStep);
        result.X = coefficient * (settings.ScreenWidth - settings.Margin) + settings.Margin;

        coefficient = dy / (2.0 * _halfVStep);
        result.Y = coefficient * (settings.ScreenHeight - settings.Margin) + settings.Margin;

        return result;
    }
}