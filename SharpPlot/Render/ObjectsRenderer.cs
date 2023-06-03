using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using SharpGL;
using SharpGL.Enumerations;
using SharpPlot.Objects;

namespace SharpPlot.Render;

public sealed class ObjectsRenderer : IRenderer
{
    public IBaseGraphic BaseGraphic { get; set; }
    private readonly List<IBaseObject> _renderableObjects;
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    public ObjectsRenderer(IBaseGraphic graphic)
    {
        _renderableObjects = new List<IBaseObject>();
        BaseGraphic = graphic;
    }

    public void AppendRenderable(IBaseObject obj)
    {
        _renderableObjects.Add(obj);
        UpdateProjection();
    }

    private void UpdateProjection()
    {
        var addedObj = _renderableObjects.Last();
        var (lb, rt) = addedObj.BoundingBox();

        BaseGraphic.Projection.SetProjection(new[] { lb.X, rt.X, lb.Y, rt.Y, -1.0, 1.0 });
    }
    
    public void Draw()
    {
        foreach (var obj in _renderableObjects)
        {
            BaseGraphic.GL.PointSize(obj.PointSize);
            BaseGraphic.GL.Enable(OpenGL.GL_POINT_SMOOTH);
            BaseGraphic.GL.Begin((BeginMode)obj.Type);

            for (int i = 0; i < obj.Points.Count; i++)
            {
                var point = obj.Points[i];
                var color = obj.Colors[i];
                
                BaseGraphic.GL.Color(color.R, color.G, color.B);
                BaseGraphic.GL.Vertex(point.X, point.Y);
            }
            
            BaseGraphic.GL.End();
            BaseGraphic.GL.Disable(OpenGL.GL_POINT_SMOOTH);
        }
    }
}