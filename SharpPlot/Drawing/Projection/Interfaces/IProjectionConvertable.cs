using OpenTK.Mathematics;
using SharpPlot.Drawing.Render;

namespace SharpPlot.Drawing.Projection.Interfaces;

public interface IProjectionConvertable
{
    Vector3d FromWorldToProjection(double sx, double sy, FrameSettings settings);
    Vector2d FromProjectionToWorld(double px, double py, double pz, FrameSettings settings);
}