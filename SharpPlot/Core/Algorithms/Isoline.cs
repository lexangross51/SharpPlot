using SharpPlot.Objects;

namespace SharpPlot.Core.Algorithms;

public struct Isoline
{
    public Point Start { get; set; }
    public Point End { get; set; }
    
    public Isoline(Point start, Point end)
    {
        Start = start;
        End = end;
    }
}