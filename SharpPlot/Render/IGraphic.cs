﻿using SharpPlot.Viewport;

namespace SharpPlot.Render;

public interface IBaseGraphic
{
    ScreenSize ScreenSize { get; set; }
    Indent Indent { get; set; }
    IProjection Projection { get; set; }
    double[] GetNewViewPort(ScreenSize newScreenSize);
    void UpdateViewMatrix();

}

public interface IViewable
{
    void Draw(IBaseGraphic graph);
}