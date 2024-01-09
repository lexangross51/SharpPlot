using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Wpf;
using SharpPlot.Drawing.Camera;
using SharpPlot.Drawing.Interactivity.Implementations;
using SharpPlot.Drawing.Projection.Implementations;
using SharpPlot.Drawing.Render;
using SharpPlot.Drawing.Render.Implementations;
using SharpPlot.Drawing.Render.Implementations.RenderStrategies;
using SharpPlot.Drawing.Render.Interfaces;
using SharpPlot.Drawing.Text;
using SharpPlot.Helpers;

namespace SharpPlot.Drawing.Controls;

public class View2D : GLWpfControl
{
    private Vector3d _mousePreviousPosition, _mouseCurrentPosition;
    private FrameSettings _settings = null!;
    private Camera2D _camera = null!;
    private MouseTracker _mouseTracker = null!;
    private AxesRenderer2D _axesRenderer = null!;
    private IRenderer _objectsRenderer = null!;
    
    #region Dependency properties

    public static readonly DependencyProperty ObjectsSourceProperty = DependencyProperty.Register(
        nameof(ObjectsSource), typeof(IEnumerable<IRenderStrategy>), typeof(View2D),
        new PropertyMetadata(default(IEnumerable<IRenderStrategy>)));

    public IEnumerable<IRenderStrategy> ObjectsSource
    {
        get => (IEnumerable<IRenderStrategy>)GetValue(ObjectsSourceProperty);
        set
        {
            SetValue(ObjectsSourceProperty, value);

            foreach (var obj in value)
            {
                _objectsRenderer.AddRenderable(obj);
            }
        }
    }
    
    public static readonly DependencyProperty XPositionProperty = DependencyProperty.Register(
        nameof(XPosition), typeof(double), typeof(View2D), new PropertyMetadata(default(double)));

    public double XPosition
    {
        get => (double)GetValue(XPositionProperty);
        set => SetValue(XPositionProperty, value);
    }
    
    public static readonly DependencyProperty YPositionProperty = DependencyProperty.Register(
        nameof(YPosition), typeof(double), typeof(View2D), new PropertyMetadata(default(double)));

    public double YPosition
    {
        get => (double)GetValue(YPositionProperty);
        set => SetValue(YPositionProperty, value);
    }

    public static readonly DependencyProperty HorizontalAxisNameProperty = DependencyProperty.Register(
        nameof(HorizontalAxisName), typeof(string), typeof(View2D), new PropertyMetadata(default(string)));

    public string HorizontalAxisName
    {
        get => (string)GetValue(HorizontalAxisNameProperty);
        set
        {
            SetValue(HorizontalAxisNameProperty, value);
            _axesRenderer.HorizontalAxisName = value;
        }
    }

    public static readonly DependencyProperty VerticalAxisNameProperty = DependencyProperty.Register(
        nameof(VerticalAxisName), typeof(string), typeof(View2D), new PropertyMetadata(default(string)));

    public string VerticalAxisName
    {
        get => (string)GetValue(VerticalAxisNameProperty);
        set
        {
            SetValue(VerticalAxisNameProperty, value);
            _axesRenderer.VerticalAxisName = value;
        }
    }

    public static readonly DependencyProperty DrawLongTicksProperty = DependencyProperty.Register(
        nameof(DrawLongTicks), typeof(bool), typeof(View2D), new PropertyMetadata(default(bool)));

    public bool DrawLongTicks
    {
        get => (bool)GetValue(DrawLongTicksProperty);
        set
        {
            SetValue(DrawLongTicksProperty, value);
            _axesRenderer.DrawLongTicks = value;
        }
    }

    public static readonly DependencyProperty DrawShortTicksProperty = DependencyProperty.Register(
        nameof(DrawShortTicks), typeof(bool), typeof(View2D), new PropertyMetadata(default(bool)));

    public bool DrawShortTicks
    {
        get => (bool)GetValue(DrawShortTicksProperty);
        set
        {
            SetValue(DrawShortTicksProperty, value);
            _axesRenderer.DrawShortTicks = value;
        }
    }

    public static readonly DependencyProperty FontProperty = DependencyProperty.Register(
        nameof(Font), typeof(SharpPlotFont), typeof(View2D), new PropertyMetadata(default(SharpPlotFont)));

    public SharpPlotFont Font
    {
        get => (SharpPlotFont)GetValue(FontProperty);
        set => SetValue(FontProperty, value);
    }
    
    #endregion
    
    public View2D()
    {
        Start(new GLWpfControlSettings
        {
            MajorVersion = 2,
            MinorVersion = 1,
            RenderContinuously = false
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
        _camera = new Camera2D(projection, _settings);
        _axesRenderer = new AxesRenderer2D(projection, _settings);
        _mouseTracker = new MouseTracker(projection, _settings);
        _objectsRenderer = new ObjectsRenderer2D(projection, _settings);

        Utilities.ReadData("solution18000", out var points, out _);
        _objectsRenderer.AddRenderable(new MeshRenderer(projection, points));

        Render += RenderScene;
        SizeChanged += OnSizeChanged;
        MouseMove += OnMouseMove;
        MouseWheel += OnMouseWheel;
        MouseLeftButtonDown += OnMouseLeftButtonDown;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Render -= RenderScene;
        SizeChanged -= OnSizeChanged;
        MouseMove -= OnMouseMove;
        MouseWheel -= OnMouseWheel;
        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;
        MouseLeftButtonDown -= OnMouseLeftButtonDown;
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
    
    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var mousePosition = e.GetPosition(this);
        _mousePreviousPosition.X = mousePosition.X;
        _mousePreviousPosition.Y = mousePosition.Y;
    }
    
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        var mousePosition = e.GetPosition(this); 
        _mouseCurrentPosition.X = mousePosition.X;
        _mouseCurrentPosition.Y = mousePosition.Y;
        
        _mouseTracker.Update(mousePosition.X, mousePosition.Y);
        XPosition = _mouseTracker.X;
        YPosition = _mouseTracker.Y;
        
        if (Mouse.LeftButton != MouseButtonState.Pressed) return;
        
        _camera.Move(_mousePreviousPosition, _mouseCurrentPosition);
        _mousePreviousPosition.X = mousePosition.X;
        _mousePreviousPosition.Y = mousePosition.Y;
        
        InvalidateVisual();
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var pos = e.GetPosition(this);
        
        _camera.Zoom(pos.X, pos.Y, e.Delta);
        InvalidateVisual();
    }
}