using SharpPlot.Render;

namespace SharpPlot.Viewport;

public class PerspectiveProjection : IProjection
{
    public double HorizontalCenter { get; set; }
    public double VerticalCenter { get; set; }
    public double Width { get; }
    public double Height { get; }
    public double Ratio { get; set; }
    public double ZBuffer { get; set; }

    public PerspectiveProjection(double ratio)
    {
        Width = Height = 0;
        Ratio = ratio;
    }
    
    public void SetProjection(double[] projection)
    {
        throw new System.NotImplementedException();
    }

    public double[] GetProjection()
    {
        throw new System.NotImplementedException();
    }

    public void FromProjectionToWorld(double x, double y, RenderSettings renderSettings, out double resX, out double resY)
    {
        throw new System.NotImplementedException();
    }

    public void FromWorldToProjection(double x, double y, RenderSettings renderSettings, out double resX, out double resY)
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