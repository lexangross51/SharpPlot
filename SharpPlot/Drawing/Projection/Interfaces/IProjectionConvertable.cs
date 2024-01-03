using OpenTK.Mathematics;
using SharpPlot.Drawing.Render;

namespace SharpPlot.Drawing.Projection.Interfaces;

public interface IProjectionConvertable
{
    Vector3d FromWorldToProjection(double sx, double sy, FrameSettings settings);
    Vector3d FromProjectionToWorld(double px, double py, FrameSettings settings);
}