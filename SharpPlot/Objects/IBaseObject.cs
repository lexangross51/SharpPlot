using System.Collections.Generic;
using Point = System.Windows.Point;
using Color = System.Drawing.Color;

namespace SharpPlot.Objects;

public enum PrimitiveType
{
    Points,
    Lines,
    LineLoop,
    LineStrip,
    Triangles,
    TriangleString,
    TriangleFan,
    Quads,
    QuadStrip,
    Polygon
}

public interface IBaseObject
{
    PrimitiveType Type { get; set; }
    int PointSize { get; set; }
    List<Point> Points { get; set; }
    List<int>? VertexIndices { get; set; }
    List<Color> Colors { get; set; }
    (Point LeftBottom, Point RightTop) BoundingBox();
}