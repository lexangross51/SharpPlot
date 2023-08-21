using System;
using System.Drawing;

namespace SharpPlot.Objects.Vertices;

public enum VertexType
{
    Position,
    PositionColor,
    PositionTexture,
    PositionNormal,
    PositionColorTexture,
    PositionColorNormal
}

// public struct BaseVertex
// {
//     public VertexType VertexType { get; } = VertexType.Position;
//     public Point Position { get; set; }
//     public Color Color { get; set; }
//     public Point Normal { get; set; }
//     public Point TextureCoord { get; set; }
//
//     public int AttributesCount
//     {
//         get
//         {
//             return VertexType switch
//             {
//                 VertexType.Position => 3,
//                 VertexType.PositionColor => 6,
//                 VertexType.PositionTexture => 5,
//                 VertexType.PositionNormal => 6,
//                 VertexType.PositionColorTexture => 8,
//                 VertexType.PositionColorNormal => 9,
//                 _ => throw new ArgumentOutOfRangeException()
//             };
//         }
//     }
// }