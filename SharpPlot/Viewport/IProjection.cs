using System.Windows;

namespace SharpPlot.Viewport;

public struct ScreenSize
{
    public double Width { get; set; }
    public double Height { get; set; }

    public ScreenSize(double width, double height)
        => (Width, Height) = (width, height);
}

public struct Indent
{
    public double Horizontal { get; set; }
    public double Vertical { get; set; }

    public Indent(double h, double v)
        => (Horizontal, Vertical) = (h, v);
}

public interface IProjection
{
    double HorizontalCenter { get; set; }
    double VerticalCenter { get; set; }
    double Width { get; }
    double Height { get; }
    double Scaling { get; set; }
    double RationHeightToWidth { get; set; }
    double ZBuffer { get; set; }
    void SetProjection(double[] projection);
    void GetProjection(out double[] projection);
    Point FromProjectionToWorld(Point point, ScreenSize screenSize, Indent indent);
    Point FromWorldToProjection(Point point, ScreenSize screenSize, Indent indent);
    void Scale(Point pivot, double delta);
    void Translate(double h, double v);
    void Reset();
}
