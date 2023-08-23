using System;
using System.Windows.Input;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Wpf;
using SharpPlot.Camera;
using SharpPlot.Core.Algorithms;
using SharpPlot.Render;
using SharpPlot.Text;
using SharpPlot.Viewport;
using Color = System.Drawing.Color;

namespace SharpPlot.Scenes;

public partial class Scene2D
{
    private readonly Viewport2DRenderer _viewPortRenderer;
    private readonly BaseGraphic2D _baseGraphic;
    private bool _isMouseDown;
    private double _mouseXPrevious, _mouseYPrevious;

    public Scene2D()
    {
        InitializeComponent();
        
        GlControl.Start(new GLWpfControlSettings
        {
            MajorVersion = 4,
            MinorVersion = 6,
            RenderContinuously = false
        });

        Width = 900;
        Height = 900;

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
                Width = Width,
                Height = Height,
            },
            Indent = new Indent
            {
                Left = indent,
                Bottom = indent
            }
        };

        var camera = new Camera2D(new OrthographicProjection(new double[] { -1, 1, -1, 1, -1, 1 }, 1));

        _viewPortRenderer = new Viewport2DRenderer(renderSettings, camera) { Font = font };
        _baseGraphic = new BaseGraphic2D(renderSettings, camera);

        GL.ClearColor(Color.White);
    }

    private void OnRender(TimeSpan obj)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.Clear(ClearBufferMask.DepthBufferBit);

        _viewPortRenderer.RenderAxis();
        _baseGraphic.DrawObject();
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
}