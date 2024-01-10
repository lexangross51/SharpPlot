using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SharpPlot.Algorithms.Meshing;
using SharpPlot.Drawing.Buffers;
using SharpPlot.Drawing.Projection.Interfaces;
using SharpPlot.Drawing.Render.Interfaces;
using SharpPlot.Drawing.Shaders;
using SharpPlot.Geometry;
using SharpPlot.Geometry.Implementations;
using SharpPlot.Geometry.Interfaces;
using SharpPlot.Helpers;

namespace SharpPlot.Drawing.Render.Implementations.RenderStrategies;

public class Mesh2DRenderer : IRenderStrategy
{
    private readonly IProjection _projection;
    private ShaderProgram _shader = null!;
    private VertexArrayObject _vao = null!;
    private IncrementalDelaunay? _delaunay;
    private float[] _vertices = null!;
    private uint[] _indices = null!;
    private ElementType _elementType;
    
    public Color4 MeshColor { get; set; } = Color4.Blue;

    public Mesh2DRenderer(IProjection projection, Mesh mesh)
    {
        ThrowHelper.ThrowIfNull(mesh, nameof(mesh));
        _projection = projection;
        
        MakeData(mesh);
        InitShaderProgram();
    }

    public Mesh2DRenderer(IProjection projection, IEnumerable<Point3D> points)
    {
        ThrowHelper.ThrowIfNull(points, nameof(points));
        _projection = projection;
        
        MakeData(points);
        InitShaderProgram();
    }

    private void InitShaderProgram()
    {
        _shader = new ShaderProgram(
            "Drawing/Shaders/Sources/Objects/SimpleVertexShader.vert",
            "Drawing/Shaders/Sources/Objects/SimpleFragmentShader.frag"
        );

        _vao = new VertexArrayObject();
        _ = new VertexBufferObject<float>(_vertices);
        _ = new ElementBufferObject(_indices);
        
        _shader.Use();
        _shader.GetAttributeLocation("position", out var location);
        _vao.SetAttributePointer(location, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        _vao.Unbind();
    }

    private void MakeData(IMesh mesh)
    {
        UpdateView(mesh);
        
        var points = mesh.Points;
        var elements = mesh.Elements;

        if (elements.Count == 0 || points.Count == 0) return;

        _elementType = elements[0].Type == ElementType.Triangle ? ElementType.Triangle : ElementType.Quadrilateral; 
        
        if (_elementType == ElementType.Triangle)
        {
            _vertices = new float[points.Count * 3];
            _indices = new uint[elements.Count * 3];

            for (int i = 0; i < points.Count; i++)
            {
                _vertices[3 * i + 0] = (float)points[i].X;
                _vertices[3 * i + 1] = (float)points[i].Y;
                _vertices[3 * i + 2] = (float)points[i].Z;
            }

            for (int i = 0; i < elements.Count; i++)
            {
                _indices[3 * i + 0] = (uint)elements[i].Points[0].Id;
                _indices[3 * i + 1] = (uint)elements[i].Points[1].Id;
                _indices[3 * i + 2] = (uint)elements[i].Points[2].Id;
            }
        }
        else
        {
            _vertices = new float[points.Count * 3];
            _indices = new uint[elements.Count * 4];

            for (int i = 0; i < points.Count; i++)
            {
                _vertices[3 * i + 0] = (float)points[i].X;
                _vertices[3 * i + 1] = (float)points[i].Y;
                _vertices[3 * i + 2] = (float)points[i].Z;
            }

            for (int i = 0; i < elements.Count; i++)
            {
                _indices[3 * i + 0] = (uint)elements[i].Points[0].Id;
                _indices[3 * i + 1] = (uint)elements[i].Points[1].Id;
                _indices[3 * i + 2] = (uint)elements[i].Points[2].Id;
                _indices[3 * i + 3] = (uint)elements[i].Points[4].Id;
            }
        }
    }

    private void MakeData(IEnumerable<Point3D> points)
    {
        _delaunay ??= new IncrementalDelaunay();
        var mesh = _delaunay.Triangulate(points);
        
        MakeData(mesh);
    }
    
    public void Render()
    {
        _vao.Bind();
        _shader.Use();
        _shader.SetUniform("color", MeshColor);
        _shader.SetUniform("modelView", Matrix4.Identity);
        _shader.SetUniform("projection", _projection.ProjectionMatrix);
        
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
        GL.DrawElements(_elementType == ElementType.Triangle ? PrimitiveType.Triangles : PrimitiveType.Quads,
            _indices.Length, DrawElementsType.UnsignedInt, 0);
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        
        _vao.Unbind();
    }

    private void UpdateView(IMesh mesh)
    {
        var points = mesh.Points;

        double minX = points.Min(p => p.X);
        double maxX = points.Max(p => p.X);
        double minY = points.Min(p => p.Y);
        double maxY = points.Max(p => p.Y);

        _projection.SetProjection([minX, maxX, minY, maxY, -1.0, 1.0]);
    }
}