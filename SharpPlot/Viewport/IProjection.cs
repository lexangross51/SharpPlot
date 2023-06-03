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
    double Ratio { get; set; }
    double ZBuffer { get; set; }
    void SetProjection(double[] projection);
    void GetProjection(out double[] projection);
    void FromProjectionToWorld(double x, double y, ScreenSize screenSize, Indent indent, out double resX, out double resY);
    void FromWorldToProjection(double x, double y, ScreenSize screenSize, Indent indent, out double resX, out double resY);
    void Scale(double x, double y, double delta);
    void Translate(double h, double v);
    void Reset();
}
