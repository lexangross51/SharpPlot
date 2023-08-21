using OpenTK.Mathematics;
using System.Collections.Generic;

namespace SharpPlot.Objects;

public interface IBaseObject
{
    List<Vector3> Points { get; }
    List<Color4> Colors { get; }
    List<uint>? Indices { get; }
}