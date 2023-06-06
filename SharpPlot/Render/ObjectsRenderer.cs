using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using SharpPlot.Objects;

namespace SharpPlot.Render;

public sealed class ObjectsRenderer : IRenderer
{
    public IBaseGraphic BaseGraphic { get; set; }
    private readonly List<IRenderable> _renderableObjects;
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    public ObjectsRenderer(IBaseGraphic graphic)
    {
        _renderableObjects = new List<IRenderable>();
        BaseGraphic = graphic;
    }

    public void AppendRenderable(IRenderable obj)
    {
        _renderableObjects.Add(obj);
        UpdateProjection();
    }

    private void UpdateProjection()
    {
        var addedObj = _renderableObjects.Last();
        addedObj.BoundingBox(out var lb, out var rt);
        
        if (lb is null || rt is null) return;
        
        var dx = (rt.Value.X - lb.Value.X) * 0.1;
        var dy = (rt.Value.Y - lb.Value.Y) * 0.1;

        //BaseGraphic.Projection.SetProjection(new[] { lb.X - dx, rt.X + dx, lb.Y - dy, rt.Y + dy, -1.0, 1.0 });
        BaseGraphic.Projection.SetProjection(new[] { lb.Value.X, rt.Value.X, lb.Value.Y, rt.Value.Y, -1.0, 1.0 });
    }
    
    public void DrawObjects()
    {
        foreach (var obj in _renderableObjects)
        {
            obj.Render(BaseGraphic);
        }
    }
}