using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SharpPlot.Camera;
using SharpPlot.Objects;
using SharpPlot.Shaders;
using SharpPlot.Viewport;
using SharpPlot.Wrappers;

namespace SharpPlot.Render;

public class BaseGraphic3D : IRenderContext
{
    private readonly ShaderProgram _lineShader;
    private readonly ShaderProgram _fieldShader;
    
    private readonly int[] _viewport;
    private readonly Camera3D _camera;
    private RenderSettings _renderSettings;
    
    private readonly Dictionary<IBaseObject, VertexArrayObject> _context;

    public BaseGraphic3D(RenderSettings renderSettings, Camera3D camera)
    {
        _lineShader = ShaderCollection.LineShader();
        _fieldShader = ShaderCollection.FieldShader();
        _context = new Dictionary<IBaseObject, VertexArrayObject>();
        _renderSettings = renderSettings;
        _camera = camera;
        
        _viewport = new[]
        {
            0, 0,
            (int)_renderSettings.ScreenSize.Width,
            (int)_renderSettings.ScreenSize.Height
        };
        
        _lineShader.Use();
        _lineShader.SetUniform("model", Matrix4.Identity);
        _fieldShader.SetUniform("model", Matrix4.Identity);
    }

    public void AddObject(IBaseObject obj)
    {
        var points = obj.Points;
        var colors = obj.Colors;
        var indices = obj.Indices;
        float[] data;
        
        // Update projection
        obj.BoundingBox(out var lb, out var rt);
        float dx = (float)(rt.X - lb.X);
        float dy = (float)(rt.Y - lb.Y);
        float dz = (float)(rt.Z - lb.Z);
        
        // _camera.GetProjection().SetProjection(new[] { lb.X - dx, rt.X + dx, lb.Y - dy, rt.Y + dy, lb.Z - dz, rt.Z + dz });
        _camera.GetProjection().SetProjection(new[] { -10.0, 10.0, -10.0, 10.0, -10.0, 10.0, });
        
        double xMean = (lb.X + rt.X) / 2.0;
        double yMean = (lb.Y + rt.Y) / 2.0;
        double zMean = (lb.Z + rt.Z) / 2.0;
        
        if (colors.Length == 1)
        {
            data = new float[points.Length * 3];
            uint index = 0;
            foreach (var p in points)
            {
                data[index++] = (float)(p.X - xMean);
                data[index++] = (float)(p.Y - yMean);
                data[index++] = (float)(p.Z - zMean);
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
                data[index++] = (float)(points[i].X - xMean);
                data[index++] = (float)(points[i].Y - yMean);
                data[index++] = (float)(points[i].Z - zMean);
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
        
        _camera.Position = new Vector3(0, 0, -1);
        _camera.Target = Vector3.Zero;
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

    public void UpdateView()
    {        
        GL.Viewport(_viewport[0], _viewport[1], _viewport[2], _viewport[3]);
        _lineShader.SetUniform("projection", _camera.GetProjectionMatrix());
        _lineShader.SetUniform("view", _camera.GetViewMatrix());
        _fieldShader.SetUniform("projection", _camera.GetProjectionMatrix());
        _fieldShader.SetUniform("view", _camera.GetViewMatrix());
    }

    public int[] GetNewViewport(ScreenSize newScreenSize)
    {
        _camera.GetProjection().Ratio = newScreenSize.Height / newScreenSize.Width;
        _renderSettings.ScreenSize = newScreenSize;
        
        _viewport[0] = 0;
        _viewport[1] = 0;
        _viewport[2] = (int)_renderSettings.ScreenSize.Width;
        _viewport[3] = (int)_renderSettings.ScreenSize.Height;

        return _viewport;
    }

    public void Clear()
    {
    }
}