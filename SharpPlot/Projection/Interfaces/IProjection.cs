using OpenTK.Mathematics;

namespace SharpPlot.Projection.Interfaces;

public interface IProjection
{
    Matrix4 ProjectionMatrix { get; }
    
    void Scale(double pivotX, double pivotY, double delta);
    void Translate(double dx, double dy);
}