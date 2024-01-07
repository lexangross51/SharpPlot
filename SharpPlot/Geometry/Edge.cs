namespace SharpPlot.Geometry;

public struct Edge(Point3D p1, Point3D p2)
{
    public Point3D P1 { get; set;} = p1;
    public Point3D P2 { get; set;} = p2;

    public override bool Equals(object? obj)
    {
        if (obj is not Edge e) return false;
        return e.P1.Id == P1.Id && e.P2.Id == P2.Id || e.P1.Id == P2.Id && e.P2.Id == P1.Id;
    }

    public override int GetHashCode() => P1.GetHashCode() ^ P2.GetHashCode();

    public static bool operator ==(Edge left, Edge right) => left.Equals(right);

    public static bool operator !=(Edge left, Edge right) => !(left == right);
}