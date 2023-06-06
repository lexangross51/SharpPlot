using System.Collections.Generic;
using Point = System.Windows.Point;
using Color = System.Windows.Media.Color;

namespace SharpPlot.Objects;

public interface IBaseObject
{
    PrimitiveType Type { get; set; }
    int PointSize { get; set; }
    List<Point> Points { get; set; }
    List<Color> Colors { get; set; }
    void BoundingBox(out Point? leftBottom, out Point? rightTop);
}