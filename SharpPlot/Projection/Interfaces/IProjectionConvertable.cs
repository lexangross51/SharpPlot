using OpenTK.Mathematics;
using SharpPlot.Render;

namespace SharpPlot.Projection.Interfaces;

public interface IProjectionConvertable
{
    Vector3d FromWorldToProjection(double sx, double sy, RenderSettings settings);
    Vector3d FromProjectionToWorld(double px, double py, RenderSettings settings);
}