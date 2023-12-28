using OpenTK.Mathematics;

namespace SharpPlot.Drawing.Projection.Interfaces;

public interface IProjection
{
    Matrix4 ProjectionMatrix { get; }

    double[] ToArray();
    void Scale(double pivotX, double pivotY, double delta);
    void Translate(double dx, double dy);
}