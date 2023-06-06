using SharpPlot.Render;

namespace SharpPlot.Objects;

public interface IRenderable : IBaseObject
{
    void Render(IBaseGraphic graphic);
}