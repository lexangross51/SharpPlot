using System.Drawing;

namespace SharpPlot.Core.Algorithms.Tree;

public interface IQuadStorable
{
    RectangleF Bounds { get; }
    bool Contains(double x, double y);
}