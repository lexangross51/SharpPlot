﻿using OpenTK.Mathematics;
using SharpPlot.Core.Drawing.Projection.Implementations;
using SharpPlot.Core.Drawing.Render;

namespace SharpPlot.Core.Drawing.Camera;

public class Camera2D(OrthographicProjection projection, FrameSettings settings) : ICamera
{
    public Matrix4 ViewMatrix => Matrix4.Identity;

    public void Zoom(double pivotX, double pivotY, double delta)
    {
        var current = projection.FromWorldToProjection(pivotX, pivotY, settings);
        projection.Scale(current.X, current.Y, delta);
    }

    public void Rotate(Vector3d from, Vector3d to)
    {
        var previous = projection.FromWorldToProjection(from.X, from.Y, settings);
        var current = projection.FromWorldToProjection(to.X, to.Y, settings);
        
        projection.Translate(-current.X + previous.X, -current.Y + previous.Y);
    }
}