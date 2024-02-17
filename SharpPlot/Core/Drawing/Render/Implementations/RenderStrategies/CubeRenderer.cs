using System.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SharpPlot.Core.Drawing.Buffers;
using SharpPlot.Core.Drawing.Camera;
using SharpPlot.Core.Drawing.Projection.Interfaces;
using SharpPlot.Core.Drawing.Render.Interfaces;
using SharpPlot.Core.Drawing.Shaders;
using SharpPlot.Core.Geometry.Implementations;
using SharpPlot.Core.Helpers;

namespace SharpPlot.Core.Drawing.Render.Implementations.RenderStrategies;

public class CubeRenderer : IRenderStrategy
{
    private const double Shift = 0.7;
    private ShaderProgram _shader = null!;
    private VertexArrayObject _vao = null!;
    private float[] _vertices = null!;
    
    public IProjection Projection { get; set; }

    public ICamera Camera { get; set; }

    public Color4 CubeColor { get; set; } = Color4.Black;

    public CubeRenderer(IProjection projection, ICamera camera, Cube cube)
    {
        ThrowHelper.ThrowIfNull(cube, nameof(cube));
        Projection = projection;
        Camera = camera;

        MakeDate(cube);
        InitShaderProgram();
    }

    private void MakeDate(Cube cube)
    {
        UpdateView(cube);
        
        _vertices = new float[cube.Points.Length * 3];
        
        for (int i = 0; i < cube.Points.Length; i++)
        {
            _vertices[3 * i + 0] = (float)cube.Points[i].X;
            _vertices[3 * i + 1] = (float)cube.Points[i].Y;
            _vertices[3 * i + 2] = (float)cube.Points[i].Z;
        }
    }
    
    private void InitShaderProgram()
    {
        _shader = new ShaderProgram(
            "Core/Drawing/Shaders/Sources/Objects/SimpleVertexShader.vert",
            "Core/Drawing/Shaders/Sources/Objects/SimpleFragmentShader.frag"
        );

        _vao = new VertexArrayObject();
        _ = new VertexBufferObject<float>(_vertices);
        
        _shader.Use();
        _shader.GetAttributeLocation("position", out var location);
        _vao.SetAttributePointer(location, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        _vao.Unbind();
    }

    public void Render()
    {
        _vao.Bind();
        _shader.Use();
        _shader.SetUniform("color", CubeColor);
        _shader.SetUniform("modelView", Camera.ViewMatrix);
        _shader.SetUniform("projection", Projection.ProjectionMatrix);
        
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
        GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Length / 3);
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        
        _vao.Unbind();
    }

    private void UpdateView(Cube cube)
    {
        var points = cube.Points;

        double minX = points.Min(p => p.X);
        double maxX = points.Max(p => p.X);
        double minY = points.Min(p => p.Y);
        double maxY = points.Max(p => p.Y);
        double minZ = points.Min(p => p.Z);
        double maxZ = points.Max(p => p.Z);

        double dx = maxX - minX;
        double dy = maxY - minY;
        double dz = maxZ - minZ;

        minX -= dx * Shift;
        maxX += dx * Shift;
        
        minY -= dy * Shift;
        maxY += dy * Shift;
        
        minZ -= dz * Shift;
        maxZ += dz * Shift;

        Projection.SetProjection([minX, maxX, minY, maxY, minZ, maxZ]);
    }
}