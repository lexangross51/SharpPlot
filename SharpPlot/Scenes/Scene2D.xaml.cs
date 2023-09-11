using System;
using System.Windows.Input;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Wpf;
using SharpPlot.Camera;
using SharpPlot.Core.Algorithms;
using SharpPlot.Core.Isolines;
using SharpPlot.Core.Mesh;
using SharpPlot.Core.Palette;
using SharpPlot.Objects;
using SharpPlot.Render;
using SharpPlot.Text;
using SharpPlot.Viewport;
using Color = System.Drawing.Color;

namespace SharpPlot.Scenes;

public sealed partial class Scene2D
{
    private readonly Viewport2DRenderer _viewPortRenderer;
    private readonly IRenderContext _baseGraphic;
    private bool _isMouseDown;
    private double _mouseXPrevious, _mouseYPrevious;

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

        var font = new SharpPlotFont
        {
            Color = Color.Black,
            Size = 14,
            FontFamily = "Times New Roman"
        };
        var indent = TextPrinter.TextMeasure("0", font).Height;
        var renderSettings = new RenderSettings
        {
            ScreenSize = new ScreenSize
            {
                Width = width,
                Height = height,
            },
            Indent = new Indent
            {
                Left = indent,
                Bottom = indent
            }
        };

        var camera = new Camera2D(new OrthographicProjection(new double[] { -1, 1, -1, 1, -1, 1 }, height / width));

        _viewPortRenderer = new Viewport2DRenderer(renderSettings, camera) { Font = font };
        _baseGraphic = new BaseGraphic2D(renderSettings, camera);

        GL.ClearColor(Color.White);
        
        // Debugger.ReadData("points", out var points);
        // Debugger.ReadData("points", "values", out var points, out var values);
        // Debugger.ReadData("spline", out var points, out var values);
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
        // _baseGraphic.AddObject(new ContourF(points, values, Palette.RainbowReverse, 20));
        // // _baseGraphic.AddObject(mesh);
        // _baseGraphic.AddObject(new ColorMap(mesh, values, Palette.Rainbow));
        // _baseGraphic.AddObject(new Contour(points, values, 30));
    }

    private void OnRender(TimeSpan obj)
    {
        GL.Disable(EnableCap.DepthTest);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        // GL.Clear(ClearBufferMask.DepthBufferBit);

        _baseGraphic.DrawObjects();
        _viewPortRenderer.RenderAxis();
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
        _viewPortRenderer.GetNewViewport(newSize);
        _baseGraphic.GetNewViewport(newSize);
        _viewPortRenderer.UpdateView();
        
        Width = newSize.Width;
        Height = newSize.Height;
        GlControl.InvalidateArrange();
    }

    public void AddObject(IBaseObject obj)
    {
        _baseGraphic.AddObject(obj);
    }
}