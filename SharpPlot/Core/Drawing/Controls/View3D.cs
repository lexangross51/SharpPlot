using System;
using System.Windows;
using System.Windows.Input;
using OpenTK.Wpf;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using SharpPlot.Core.Drawing.Camera;
using SharpPlot.Core.Drawing.Projection.Implementations;
using SharpPlot.Core.Drawing.Render;
using SharpPlot.Core.Drawing.Render.Implementations;
using SharpPlot.Core.Drawing.Render.Implementations.RenderStrategies;
using SharpPlot.Core.Drawing.Render.Interfaces;
using SharpPlot.Core.Geometry.Implementations;

namespace SharpPlot.Core.Drawing.Controls;

public class View3D : GLWpfControl
{
    private Vector3d _mousePreviousPosition, _mouseCurrentPosition;
    private FrameSettings _settings = default!;
    private AxesRenderer3D _axesRenderer = default!;
    private IRenderer _objectsRenderer = default!;
    private ArcBallCamera _camera = default!;
    
    public View3D()
    {
        Start(new GLWpfControlSettings
        {
            MajorVersion = 2,
            MinorVersion = 1,
            RenderContinuously = false,
        });
        
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _settings = new FrameSettings
        {
            ScreenWidth = ActualWidth,
            ScreenHeight = ActualHeight,
        };
        
        var projection = new OrthographicProjection(-1, 1, -1, 1, -1, 1);
        _camera = new ArcBallCamera(projection, _settings) { Radius = 1.0 };
        _axesRenderer = new AxesRenderer3D(projection, _settings);
        _objectsRenderer = new ObjectsRenderer3D(projection, _settings);

        Render += RenderScene;
        SizeChanged += OnSizeChanged;
        MouseMove += OnMouseMove;
        MouseWheel += OnMouseWheel;
        MouseLeftButtonDown += OnMouseLeftButtonDown;
        MouseLeftButtonUp += OnMouseLeftButtonUp;

        _objectsRenderer.AddRenderable(new CubeRenderer(projection, _camera, new Cube()));
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        var mousePosition = e.GetPosition(this);
        _mousePreviousPosition.X = mousePosition.X;
        _mousePreviousPosition.Y = mousePosition.Y;
        _camera.StopRotate();
    }


    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var mousePosition = e.GetPosition(this);
        _mousePreviousPosition.X = mousePosition.X;
        _mousePreviousPosition.Y = mousePosition.Y;
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var pos = e.GetPosition(this);
        
        _camera.Zoom(pos.X, pos.Y, e.Delta);
        InvalidateVisual();
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        var mousePosition = e.GetPosition(this); 
        _mouseCurrentPosition.X = mousePosition.X;
        _mouseCurrentPosition.Y = mousePosition.Y;
        
        if (Mouse.LeftButton != MouseButtonState.Pressed) return;
        
        _camera.Rotate(_mousePreviousPosition, _mouseCurrentPosition);
        
        InvalidateVisual();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Render -= RenderScene;
        SizeChanged -= OnSizeChanged;
        MouseLeftButtonUp -= OnMouseLeftButtonDown;
        MouseLeftButtonDown -= OnMouseLeftButtonDown;
        MouseMove -= OnMouseMove;
        MouseWheel -= OnMouseWheel;
        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;
    }

    private void RenderScene(TimeSpan obj)
    {
        GL.ClearColor(Color4.White);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        _objectsRenderer.Render();
        _axesRenderer.Render();
    }
    
    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _settings.ScreenWidth = ActualWidth;
        _settings.ScreenHeight = ActualHeight;
        _axesRenderer.UpdateViewPort(_settings);
        InvalidateVisual();
    }
}