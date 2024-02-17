using OpenTK.Mathematics;
using SharpPlot.Core.Drawing.Camera;
using SharpPlot.Core.Drawing.Projection.Interfaces;

namespace SharpPlot.Core.Drawing.Render.Interfaces;

public interface IRenderStrategy
{
    IProjection Projection { get; set; }
    
    ICamera Camera { get; set; }
    
    void Render();
}