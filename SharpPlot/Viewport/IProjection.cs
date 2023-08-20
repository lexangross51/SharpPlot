using System.Drawing;

namespace SharpPlot.Viewport;

public struct ScreenSize
{
    public PointF StartPoint { get; init; }
    public double Width { get; init; }
    public double Height { get; init; }
}

public struct Indent
{
    public double Left { get; set; }
    public double Right { get; set; }
    public double Bottom { get; set; }
    public double Top { get; set; }
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
    void FromProjectionToWorld(double x, double y, ScreenSize screenSize, Indent indent, out double resX, out double resY);
    void FromWorldToProjection(double x, double y, ScreenSize screenSize, Indent indent, out double resX, out double resY);
    void Scale(double x, double y, double delta);
    void Translate(double h, double v);
    void Reset();
}