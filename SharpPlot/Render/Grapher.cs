using OpenTK.Graphics.OpenGL4;
using SharpPlot.Camera;
using SharpPlot.Core.Algorithms;
using SharpPlot.Objects;
using SharpPlot.Shaders;
using SharpPlot.Viewport;
using SharpPlot.Wrappers;

namespace SharpPlot.Render;

public class BaseGraphic2D
{
    private readonly ShaderProgram _shader;
    private readonly int[] _viewport;
    private RenderSettings _renderSettings;
    private VertexArrayObject _vao;
    private VertexBufferObject<float>? _vbo;
    private ElementBufferObject _ebo;
    private uint[] _indices;
    private float[] _data;
    
    public Camera2D Camera { get; }
    
    public BaseGraphic2D(RenderSettings renderSettings, Camera2D camera)
    {
        _shader = ShaderCollection.LineShader();
        _viewport = new[]
        {
            (int)renderSettings.Indent.Left,
            (int)renderSettings.Indent.Bottom,
            (int)(renderSettings.ScreenSize.Width - renderSettings.Indent.Left),
            (int)(renderSettings.ScreenSize.Height - renderSettings.Indent.Bottom)
        };
        
        _renderSettings = renderSettings;
        Camera = camera;
        
        var delaunay = new DelaunayTriangulation();
        // Debugger.ReadData("Htop.dat", out var points, out var values);
        //
        // for (var i = 0; i < points.Count; i++)
        // {
        //     var point = points[i];
        //     point.X -= 2139000;
        //     point.Y -= 6540000;
        //     points[i] = point;
        // }
        
        var points = Debugger.GenerateRandomPoints(2000);
        var mesh = delaunay.Triangulate(points);
        // var isolineBuilder = new IsolineBuilder(mesh, values.ToArray());
        // isolineBuilder.BuildIsolines(20);
        var data = new float[3 * points.Count];
        //var data = new float[3 * isolineBuilder.Isolines.Count * 2];
        _indices = new uint[3 * mesh.ElementsCount];

        var index = 0;
        foreach (var p in points)
        {
            data[index++] = (float)p.X;
            data[index++] = (float)p.Y;
            data[index++] = (float)p.Z;
        }
        // foreach (var iso in isolineBuilder.Isolines)
        // {
        //     data[index++] = (float)iso.Start.X;
        //     data[index++] = (float)iso.Start.Y;
        //     data[index++] = 0.0f;
        //     data[index++] = (float)iso.End.X;
        //     data[index++] = (float)iso.End.Y;
        //     data[index++] = 0.0f;
        // }

        _data = data;
        
        index = 0;
        for (int i = 0; i < mesh.ElementsCount; i++)
        {
            _indices[index++] = (uint)mesh.Element(i).Nodes[0];
            _indices[index++] = (uint)mesh.Element(i).Nodes[1];
            _indices[index++] = (uint)mesh.Element(i).Nodes[2];
        }
        
        _vao = new VertexArrayObject();
        _vbo = new VertexBufferObject<float>(data);
        _ebo = new ElementBufferObject(_indices);
        _shader.Use();
        _shader.GetAttribLocation("position", out var location);
        _vao.SetAttributePointer(location, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
    }
    
    public int[] GetNewViewport(ScreenSize newScreenSize)
    {
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
        _shader.SetUniform("model", Camera.GetModelMatrix());
        _shader.SetUniform("view", Camera.GetViewMatrix());
        _shader.SetUniform("projection", Camera.GetProjectionMatrix());
    }
    
    public void DrawObject()
    {
        _vao.Bind();
        _shader.Use();
        
        UpdateView();
        
        _shader.GetUniformLocation("lineColor", out var location);
        _shader.SetUniform(location, 0.0f, 0.0f, 0.0f, 1.0f);
        
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        
        // GL.DrawArrays(PrimitiveType.Lines, 0, _data.Length / 3);
    }
    
    public void Clear()
    {
    }
}