namespace SharpPlot.Objects;

public struct Point
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public Point(double x, double y, double z = 0.0)
    {
        X = x;
        Y = y;
        Z = z;
    }
}