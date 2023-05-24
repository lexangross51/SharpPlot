using System.Collections.Generic;
using Point = System.Windows.Point;
using Color = System.Drawing.Color;

namespace SharpPlot.Objects;

public interface IBaseObject
{
    List<Point> Points { get; set; }
    Color Color { get; set; }
}