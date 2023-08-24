using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace SharpPlot.Objects;

public interface IBaseObject
{
    PrimitiveType ObjectType { get; }
    int PointSize { get; }
    Point[] Points { get; }
    Color4[] Colors { get; }
    uint[]? Indices { get; }

    void BoundingBox(out Point leftBottom, out Point rightTop);
}