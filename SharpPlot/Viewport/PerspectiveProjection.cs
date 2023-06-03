namespace SharpPlot.Viewport;

public class PerspectiveProjection : IProjection
{
    public double HorizontalCenter { get; set; }
    public double VerticalCenter { get; set; }
    public double Width { get; }
    public double Height { get; }
    public double ZBuffer { get; set; }
    public double Scaling { get; set; }
    public double Ratio { get; set; }
    public void SetProjection(double[] projection)
    {
        throw new System.NotImplementedException();
    }

    public void GetProjection(out double[] projection)
    {
        projection = new[] { 60.0, Ratio, -1, 100 };
    }

    public void FromProjectionToWorld(double x, double y, ScreenSize screenSize, Indent indent, out double resX, out double resY)
    {
        throw new System.NotImplementedException();
    }

    public void FromWorldToProjection(double x, double y, ScreenSize screenSize, Indent indent, out double resX, out double resY)
    {
        throw new System.NotImplementedException();
    }

    public void Scale(double x, double y, double delta)
    {
        throw new System.NotImplementedException();
    }

    public void Translate(double h, double v)
    {
        throw new System.NotImplementedException();
    }

    public void Reset()
    {
        throw new System.NotImplementedException();
    }
}