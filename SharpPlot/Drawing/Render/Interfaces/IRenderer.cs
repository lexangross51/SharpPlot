namespace SharpPlot.Drawing.Render.Interfaces;

public interface IRenderer
{
    void AddRenderable(IRenderStrategy renderable);
    void RemoveRenderable(IRenderStrategy renderable);
    void Render();
}