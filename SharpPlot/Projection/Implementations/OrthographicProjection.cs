using OpenTK.Mathematics;
using SharpPlot.Projection.Interfaces;
using SharpPlot.Render;

namespace SharpPlot.Projection.Implementations;

public class OrthographicProjection : IProjection, IProjectionConvertable
{
    private double _hCenter, _vCenter, _halfHStep, _halfVStep;
    
    public Matrix4 ProjectionMatrix { get; }
    
    public void Scale(double pivotX, double pivotY, double delta)
    {
        throw new System.NotImplementedException();
    }

    public void Translate(double dx, double dy)
    {
        throw new System.NotImplementedException();
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