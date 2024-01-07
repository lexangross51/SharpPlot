namespace SharpPlot.Geometry;

public struct Point3D(double x, double y, double z)
{
    public int Id { get; set; }
    public double X { get; set; } = x;
    public double Y { get; set; } = y;
    public double Z { get; set; } = z;
}