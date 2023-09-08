using System;
using System.Drawing;
using System.Windows.Input;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Wpf;
using SharpPlot.Camera;
using SharpPlot.Core.Primitives;
using SharpPlot.Render;
using SharpPlot.Text;
using SharpPlot.Viewport;
using Point = SharpPlot.Objects.Point;

namespace SharpPlot.Scenes;

public partial class Scene3D
{
    private readonly Viewport3DRenderer _viewPortRenderer;
    private readonly IRenderContext _baseGraphic;
    private bool _isMouseDown;
    private bool _isXClicked, _isYClicked, _isZClicked;
    
    public Scene3D(double width, double height)
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
        var renderSettings = new RenderSettings
        {
            ScreenSize = new ScreenSize
            {
                Width = width,
                Height = height,
            }
        };

        var camera = new Camera3D(new PerspectiveProjection(width / height), new Point(0, 0, -3), new Point(0, 0));

        _viewPortRenderer = new Viewport3DRenderer(renderSettings, camera) { Font = font };
        _baseGraphic = new BaseGraphic3D(renderSettings, camera);

        GL.ClearColor(Color.White);
        GL.Enable(EnableCap.DepthTest);
        
        _baseGraphic.AddObject(new Cube());
    }

    private void OnRender(TimeSpan obj)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _baseGraphic.DrawObjects();
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {        
        _viewPortRenderer.GetCamera().Zoom(0, 0, e.Delta);
        _viewPortRenderer.UpdateView();
        GlControl.InvalidateVisual();
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isMouseDown = true;
    }
    
    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isMouseDown = false;
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        
    }
    
    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.X:
                _isXClicked = true;
                e.Handled = true;
                break;
            
            case Key.Y:
                _isYClicked = true;
                e.Handled = true;
                break;
            
            case Key.Z:
                _isZClicked = true;
                e.Handled = true;
                break;
        }
    }
    
    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.X:
                _isXClicked = false;
                break;
            
            case Key.Y:
                _isYClicked = false;
                break;
            
            case Key.Z:
                _isZClicked = false;
                break;
        }
        
        e.Handled = true;
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
}