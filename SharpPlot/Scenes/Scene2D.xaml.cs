using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Wpf;
using SharpPlot.Camera;
using SharpPlot.Core.Algorithms;
using SharpPlot.Core.Colorbar;
using SharpPlot.Core.Isolines;
using SharpPlot.Core.Mesh;
using SharpPlot.Core.Palette;
using SharpPlot.Objects;
using SharpPlot.Render;
using SharpPlot.Shaders;
using SharpPlot.Text;
using SharpPlot.Viewport;
using SharpPlot.Wrappers;
using Color = System.Drawing.Color;

namespace SharpPlot.Scenes;

public sealed partial class Scene2D
{
    private readonly Viewport2DRenderer _viewPortRenderer;
    private readonly IRenderContext _baseGraphic;
    private bool _isMouseDown;
    private double _mouseXPrevious, _mouseYPrevious;

    private readonly ShaderProgram _program;
    private VertexArrayObject _vao;
    private VertexBufferObject<float> _vbo;
    private ElementBufferObject _ebo;

    private int _splits = 70;
    private readonly float[] _vertices;
    private readonly uint[] _indices;
    private readonly float[] _palette;
    
    public Scene2D(double width, double height)
    {
        InitializeComponent();
        
        GlControl.Start(new GLWpfControlSettings
        {
            MajorVersion = 2,
            MinorVersion = 1,
            RenderContinuously = false
        });

        Width = width;
        Height = height;

        // var font = new SharpPlotFont
        // {
        //     Color = Color.Black,
        //     Size = 14,
        //     FontFamily = "Times New Roman"
        // };
        // var indent = TextPrinter.TextMeasure("0", font).Height;
        // var renderSettings = new RenderSettings
        // {
        //     ScreenSize = new ScreenSize
        //     {
        //         Width = width,
        //         Height = height,
        //     },
        //     Indent = new Indent
        //     {
        //         Left = indent,
        //         Bottom = indent
        //     }
        // };
        //
        // var camera = new Camera2D(new OrthographicProjection(new double[] { -1, 1, -1, 1, -1, 1 }, height / width));
        //
        // _viewPortRenderer = new Viewport2DRenderer(renderSettings, camera) { Font = font };
        // _baseGraphic = new BaseGraphic2D(renderSettings, camera);

        GL.ClearColor(Color.White);
        
        Debugger.ReadData("solution18000", out var points, out var values);
        // Debugger.ReadData("points", out var points);
        // Debugger.ReadData("points", "values", out var points, out var values);
        // Debugger.ReadData("data.txt", out var points, out var values);
        // for (var i = 0; i < points.Count; i++)
        // {
        //     var point = points[i];
        //     point.X -= 2139000;
        //     point.Y -= 6540000;
        //     points[i] = point;
        // }

        // var points = Debugger.GenerateRandomPoints(2000);
        // var values = Debugger.GenerateRandomData(2000);
        // var delaunay = new DelaunayTriangulation();
        // var mesh = delaunay.Triangulate(points);
        // _baseGraphic.AddObject(new Mesh(points, elements));
        // _baseGraphic.AddObject(new ColorMap(mesh, values, Palette.RainbowReverse));
        // _baseGraphic.AddObject(new Contour(points, values, 40));
        // var colorbar = new Colorbar(values, Palette.Rainbow)
        // {
        //     VerticalAlignment = VerticalAlignment.Bottom,
        //     HorizontalAlignment = HorizontalAlignment.Right
        // };
        // MainGrid.Children.Add(colorbar);
        
        // _baseGraphic.AddObject(new Mesh(points, elements));

        //Debugger.ReadData("points", "values", out var points, out var values);
        
        var vals = new List<float>();
        _vertices = new float[(_splits + 1) * (_splits + 1) * 4];
        for (int i = 0, k = 0; i < _splits + 1; i++)
        {
            for (int j = 0; j < _splits + 1; j++)
            {
                int index = (i * (_splits + 1) + j) * 4;
                
                _vertices[index + 0] = (float)points[k].X;
                _vertices[index + 1] = (float)points[k].Y;
                _vertices[index + 2] = 0.0f;
                _vertices[index + 3] = (float)values[k];
                vals.Add((float)values[k]);
                k++;
            }
        }
        
        _indices = new uint[(_splits + 1) * (_splits + 1) * 6];
        for (uint y = 0; y < _splits; y++)
        {
            for (uint x = 0; x < _splits; x++)
            {
                uint index = (uint)(y * _splits + x) * 6;
                _indices[index + 0] = (uint)(y * (_splits + 1) + x);
                _indices[index + 1] = (uint)(y * (_splits + 1) + x + 1);
                _indices[index + 2] = (uint)((y + 1) * (_splits + 1) + x);
                _indices[index + 3] = (uint)(y * (_splits + 1) + x + 1);
                _indices[index + 4] = (uint)((y + 1) * (_splits + 1) + x);
                _indices[index + 5] = (uint)((y + 1) * (_splits + 1) + x + 1);
            }
        }

        var myPalette = Palette.Autumn;
        _palette = new float[myPalette.ColorsCount * 4];
        for (int i = 0; i < myPalette.ColorsCount; i++)
        {
            _palette[4 * i + 0] = myPalette[i].R;
            _palette[4 * i + 1] = myPalette[i].G;
            _palette[4 * i + 2] = myPalette[i].B;
            _palette[4 * i + 3] = myPalette[i].A;
        }
        
        _program = ShaderCollection.IsolineShader();
        
        _vao = new VertexArrayObject();
        _vbo = new VertexBufferObject<float>(_vertices);
        _ebo = new ElementBufferObject(_indices);
            
        _program.Use();
        _program.GetAttribLocation("aPos", out var location);
        _vao.SetAttributePointer(location, 3, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
        _program.GetAttribLocation("aValue", out location);
        _vao.SetAttributePointer(location, 1, VertexAttribPointerType.Float, false, 4 * sizeof(float), 3 * sizeof(float));
        
        _program.SetUniform("model", Matrix4.Identity);
        _program.SetUniform("view", Matrix4.Identity);
        _program.SetUniform("projection", Matrix4.CreateOrthographicOffCenter(-0.01f, 0.11f, -0.01f, 0.11f, 0.0f, 1.0f));
        _program.SetUniform("minValue", vals.Min());
        _program.SetUniform("maxValue", vals.Max());
        _program.SetUniform("valuesRangeCount", 50);
        _program.SetUniform("palette[0]", _palette);
        _program.SetUniform("colorsCount", _palette.Length / 4);
        
        _vbo.Unbind();
        _vao.Unbind();
    }

    private void OnRender(TimeSpan obj)
    {
        GL.ClearColor(Color.White);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        OpenTK.Graphics.OpenGL.GL.Ortho(-0.1, 0.5, -0.1, 0.5, 0, 1);
        
        // _baseGraphic.DrawObjects();
        // _viewPortRenderer.RenderAxis();
        
        _vao.Bind();
        _vbo.Bind();
        _program.Use();
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {        
        var pos = e.GetPosition(this);
        var projection = _viewPortRenderer.GetCamera().GetProjection();
        var renderSettings = _viewPortRenderer.GetRenderSettings();
        
        projection.FromWorldToProjection(pos.X, pos.Y, renderSettings, out var x, out var y);
        
        _viewPortRenderer.GetCamera().Zoom(x, y, e.Delta);
        _viewPortRenderer.UpdateView();
        GlControl.InvalidateVisual();
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isMouseDown = true;
        var mousePosition = e.GetPosition(this);
        _mouseXPrevious = mousePosition.X;
        _mouseYPrevious = mousePosition.Y;
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isMouseDown = false;
    }
    
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (!_isMouseDown) return;

        var projection = _viewPortRenderer.GetCamera().GetProjection();
        var renderSettings = _viewPortRenderer.GetRenderSettings();
        
        var mousePosition = e.GetPosition(this);
        projection.FromWorldToProjection(_mouseXPrevious, _mouseYPrevious, renderSettings, out var xPrevious, out var yPrevious);
        projection.FromWorldToProjection(mousePosition.X, mousePosition.Y, renderSettings, out var xCurrent, out var yCurrent);
        
        _viewPortRenderer.GetCamera().Move(-xCurrent + xPrevious, -yCurrent + yPrevious);

        _mouseXPrevious = mousePosition.X;
        _mouseYPrevious = mousePosition.Y;
        
        _viewPortRenderer.UpdateView();
        GlControl.InvalidateVisual();
    }

    private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        _viewPortRenderer.DrawingGrid = !_viewPortRenderer.DrawingGrid;
        GlControl.InvalidateVisual();
    }
    
    public void OnChangeSize(ScreenSize newSize)
    {
        // _viewPortRenderer.GetNewViewport(newSize);
        // _baseGraphic.GetNewViewport(newSize);
        // _viewPortRenderer.UpdateView();
        //
        // Width = newSize.Width;
        // Height = newSize.Height;
        // GlControl.InvalidateArrange();
    }

    public void AddObject(IBaseObject obj)
    {
        _baseGraphic.AddObject(obj);
    }
}