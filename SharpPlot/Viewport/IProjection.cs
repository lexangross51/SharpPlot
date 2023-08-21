using SharpPlot.Render;

namespace SharpPlot.Viewport;

public struct ScreenSize
{
    public double Width { get; init; }
    public double Height { get; init; }
}

public struct Indent
{
    public double Left { get; init; }
    public double Right { get; init; }
    public double Bottom { get; init; }
    public double Top { get; init; }
}

public interface IProjection
{
    double HorizontalCenter { get; set; }
    double VerticalCenter { get; set; }
    double Width { get; }
    double Height { get; }
    double Ratio { get; set; }
    double ZBuffer { get; set; }
    void SetProjection(double[] projection);
    double[] GetProjection();
    void FromProjectionToWorld(double x, double y, RenderSettings renderSettings, out double resX, out double resY);
    void FromWorldToProjection(double x, double y, RenderSettings renderSettings, out double resX, out double resY);
    void Scale(double x, double y, double delta);
    void Translate(double h, double v);
    void Reset();
}