using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SharpPlot.Camera;
using SharpPlot.Objects;
using SharpPlot.Shaders;
using SharpPlot.Viewport;
using SharpPlot.Wrappers;

namespace SharpPlot.Render;

public class BaseGraphic2D : IRenderContext
{
    private readonly ShaderProgram _lineShader;
    private readonly ShaderProgram _fieldShader;
    
    private readonly int[] _viewport;
    private readonly Camera2D _camera;
    private RenderSettings _renderSettings;
    
    private readonly Dictionary<IBaseObject, VertexArrayObject> _context;

    
    public BaseGraphic2D(RenderSettings renderSettings, Camera2D camera)
    {
        _lineShader = ShaderCollection.LineShader();
        _fieldShader = ShaderCollection.FieldShader();
        _context = new Dictionary<IBaseObject, VertexArrayObject>();
        
        _viewport = new[]
        {
            (int)renderSettings.Indent.Left,
            (int)renderSettings.Indent.Bottom,
            (int)(renderSettings.ScreenSize.Width - renderSettings.Indent.Left),
            (int)(renderSettings.ScreenSize.Height - renderSettings.Indent.Bottom)
        };
        
        _renderSettings = renderSettings;
        _camera = camera;
        
        _lineShader.Use();
        _lineShader.SetUniform("model", Matrix4.Identity);
        _lineShader.SetUniform("view", Matrix4.Identity);
        _lineShader.SetUniform("projection", Matrix4.Identity);
        
        _fieldShader.Use();
        _fieldShader.SetUniform("model", Matrix4.Identity);
        _fieldShader.SetUniform("view", Matrix4.Identity);
        _fieldShader.SetUniform("projection", Matrix4.Identity);
    }
    
    public int[] GetNewViewport(ScreenSize newScreenSize)
    {
        _camera.GetProjection().Ratio = newScreenSize.Height / newScreenSize.Width;
        _renderSettings.ScreenSize = newScreenSize;
        
        _viewport[0] = (int)_renderSettings.Indent.Left;
        _viewport[1] = (int)_renderSettings.Indent.Bottom;
        _viewport[2] = (int)(_renderSettings.ScreenSize.Width - _renderSettings.Indent.Left);
        _viewport[3] = (int)(_renderSettings.ScreenSize.Height - _renderSettings.Indent.Bottom);

        return _viewport;
    }

    public void UpdateView()
    {
        GL.Viewport(_viewport[0], _viewport[1], _viewport[2], _viewport[3]);
        
        _lineShader.SetUniform("view", Matrix4.Identity);
        _lineShader.SetUniform("projection", _camera.GetProjectionMatrix());
        _fieldShader.SetUniform("view", Matrix4.Identity);
        _fieldShader.SetUniform("projection", _camera.GetProjectionMatrix());
    }
    
    public void AddObject(IBaseObject obj)
    {
        var points = obj.Points;
        var colors = obj.Colors;
        var indices = obj.Indices;
        float[] data;
        
        if (colors.Length == 1)
        {
            data = new float[points.Length * 3];
            uint index = 0;
            foreach (var p in points)
            {
                data[index++] = (float)p.X;
                data[index++] = (float)p.Y;
                data[index++] = 0.0f/*(float)p.Z*/;
            }
            
            var vao = new VertexArrayObject();
            var vbo = new VertexBufferObject<float>(data);
            
            _lineShader.Use();
            _lineShader.GetAttribLocation("position", out var location);
            vao.SetAttributePointer(location, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            if (indices is not null)
            {
                _ = new ElementBufferObject(indices);
            }
            
            vbo.Unbind();
            vao.Unbind();
            _context.Add(obj, vao);
        }
        else
        {
            data = new float[points.Length * 6];
            uint index = 0;
            for (var i = 0; i < points.Length; i++)
            {
                data[index++] = (float)points[i].X;
                data[index++] = (float)points[i].Y;
                data[index++] = 0.0f/*(float)points[i].Z*/;
                data[index++] = colors[i].R;
                data[index++] = colors[i].G;
                data[index++] = colors[i].B;
            }
                
            var vao = new VertexArrayObject();
            var vbo = new VertexBufferObject<float>(data);
            
            _fieldShader.Use();
            _fieldShader.GetAttribLocation("position", out var location);
            vao.SetAttributePointer(location, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            _fieldShader.GetAttribLocation("color", out location);
            vao.SetAttributePointer(location, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            
            if (indices is not null)
            {
                _ = new ElementBufferObject(indices);
            }
            
            vbo.Unbind();
            vao.Unbind();
            _context.Add(obj, vao);
        }
        
        // Update projection
        obj.BoundingBox(out var lb, out var rt);
        
        // var dx = (rt.X - lb.X) * 0.1;
        // var dy = (rt.Y - lb.Y) * 0.1;

        _camera.GetProjection().SetProjection(new[] { lb.X, rt.X, lb.Y, rt.Y, -1.0, 1.0 });
    }

    public void DrawObjects()
    {
        foreach (var (obj, buffer) in _context)
        {
            GL.PointSize(obj.PointSize);
            buffer.Bind();
            
            if (obj.Indices is null)
            {
                if (obj.Colors.Length == 1)
                {
                    _lineShader.Use();
                    _lineShader.GetUniformLocation("lineColor", out var location);
                    _lineShader.SetUniform(location, obj.Colors[0].R, obj.Colors[0].G, obj.Colors[0].B, 1.0f);
                    
                    UpdateView();
                    
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                    GL.DrawArrays(obj.ObjectType, 0, obj.Points.Length);
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                }
                else
                {
                    _fieldShader.Use();
                    
                    UpdateView();
                    
                    GL.DrawArrays(obj.ObjectType, 0, obj.Points.Length);
                }
            }
            else
            {
                if (obj.Colors.Length == 1)
                {
                    _lineShader.Use();
                    _lineShader.GetUniformLocation("lineColor", out var location);
                    _lineShader.SetUniform(location, obj.Colors[0].R, obj.Colors[0].G, obj.Colors[0].B, 1.0f);
                    
                    UpdateView();
                    
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                    GL.DrawElements(obj.ObjectType, obj.Indices.Length, DrawElementsType.UnsignedInt, 0);   
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                }
                else
                {
                    _fieldShader.Use();
                    
                    UpdateView();
                    
                    GL.DrawElements(obj.ObjectType, obj.Indices.Length, DrawElementsType.UnsignedInt, 0);
                }
            }
            buffer.Unbind();
        }
        
        GL.PointSize(1);
    }
    
    public void Clear()
    {
    }
}