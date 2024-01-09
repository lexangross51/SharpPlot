using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using SharpPlot.Drawing.Projection.Interfaces;
using SharpPlot.Drawing.Render.Interfaces;

namespace SharpPlot.Drawing.Render.Implementations;

public class ObjectsRenderer2D(IProjection projection, FrameSettings settings) : IRenderer
{
    private readonly List<IRenderStrategy> _objects = [];
    
    public void AddRenderable(IRenderStrategy renderable)
    {
        if (_objects.Contains(renderable)) return;
        _objects.Add(renderable);
    }

    public void RemoveRenderable(IRenderStrategy renderable)
    {
        if (!_objects.Contains(renderable)) return;
        _objects.Remove(renderable);
    }

    public void Render()
    {
        GL.Viewport((int)settings.Margin, (int)settings.Margin, (int)(settings.ScreenWidth - settings.Margin),
            (int)(settings.ScreenHeight - settings.Margin));
        
        foreach (var renderable in _objects)
        {
            renderable.Render();
        }
    }
}