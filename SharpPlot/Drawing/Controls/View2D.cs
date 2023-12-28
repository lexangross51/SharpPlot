﻿using System;
using System.Windows;
using System.Windows.Input;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Wpf;
using SharpPlot.Drawing.Camera;
using SharpPlot.Drawing.Projection.Implementations;
using SharpPlot.Drawing.Render;
namespace SharpPlot.Drawing.Controls;

public class View2D : GLWpfControl
{
    private RenderSettings _settings;
    private readonly BaseCamera _camera;
    private readonly OrthographicProjection _projection;
    private AxesRenderer2D? _axesRenderer;
    
    private double _mouseXPrevious, _mouseYPrevious; 
    private bool _isMouseDown;

    public static readonly DependencyProperty XPositionProperty = DependencyProperty.Register(
        nameof(XPosition), typeof(double), typeof(View2D), new PropertyMetadata(default(double)));

    public static readonly DependencyProperty YPositionProperty = DependencyProperty.Register(
        nameof(YPosition), typeof(double), typeof(View2D), new PropertyMetadata(default(double)));
    
    public double XPosition
    {
        get => (double)GetValue(XPositionProperty);
        set => SetValue(XPositionProperty, value);
    }

    public double YPosition
    {
        get => (double)GetValue(YPositionProperty);
        set => SetValue(YPositionProperty, value);
    }
    
    public View2D()
    {
        Start(new GLWpfControlSettings
        {
            MajorVersion = 2,
            MinorVersion = 1,
            RenderContinuously = false
        });
        
        _projection = new OrthographicProjection(-1, 1, -1, 1, -1, 1);
        _camera = new Camera2D(_projection);
        
        Loaded += OnLoaded;
        Render += RenderScene;
        SizeChanged += OnSizeChanged;
        MouseLeftButtonDown += OnMouseLeftButtonDown;
        MouseLeftButtonUp += OnMouseLeftButtonUp;
        MouseMove += OnMouseMove;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _settings = new RenderSettings
        {
            ScreenWidth = ActualWidth,
            ScreenHeight = ActualHeight,
            Margin = 25
        };
        
        _axesRenderer = new AxesRenderer2D(_projection, _settings);
    }

    private void RenderScene(TimeSpan obj)
    {
        GL.ClearColor(Color4.White);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        _axesRenderer?.Render();
    }
    
    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _settings.ScreenWidth = ActualWidth;
        _settings.ScreenHeight = ActualHeight;
        
        _axesRenderer?.UpdateViewPort(_settings);
        
        InvalidateVisual();
    }
    
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        var mousePosition = e.GetPosition(this);
        var current = _projection.FromWorldToProjection(mousePosition.X, mousePosition.Y, _settings);
        
        XPosition = current.X;
        YPosition = current.Y;
        
        if (!_isMouseDown) return;

        var previous = _projection.FromWorldToProjection(_mouseXPrevious, _mouseYPrevious, _settings);
        
        _camera.Move(-current.X + previous.X, -current.Y + previous.Y);

        _mouseXPrevious = mousePosition.X;
        _mouseYPrevious = mousePosition.Y;
        
        InvalidateVisual();
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isMouseDown = false;
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var mousePosition = e.GetPosition(this);
        
        _isMouseDown = true;
        _mouseXPrevious = mousePosition.X;
        _mouseYPrevious = mousePosition.Y;
    }
}