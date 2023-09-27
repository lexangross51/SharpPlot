using System;
using System.Drawing;
using System.Windows.Input;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Wpf;
using SharpPlot.Camera;
using SharpPlot.Core.Algorithms;
using SharpPlot.Core.Primitives;
using SharpPlot.Render;
using SharpPlot.Text;
using SharpPlot.Viewport;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace SharpPlot.Scenes;

public partial class Scene3D
{
    private readonly Viewport3DRenderer _viewPortRenderer;
    private readonly IRenderContext _baseGraphic;
    private bool _isMouseDown;

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

        // var camera = new Camera3D(new PerspectiveProjection(width / height), Vector3.Zero, Vector3.Zero);
        var camera = new Camera3D(new OrthographicProjection(new double[] { -3, 3, -3, 3, -3, 3 }, height / width), Vector3.Zero, Vector3.Zero);

        _viewPortRenderer = new Viewport3DRenderer(renderSettings, camera) { Font = font };
        _baseGraphic = new BaseGraphic3D(renderSettings, camera);

        GL.ClearColor(Color.White);
        
        // Debugger.ReadData("spline", out var points, out var values);
        // for (int i = 0; i < points.Count; i++)
        // {
        //     var p = points[i];
        //     p.Z = values[i];
        //     points[i] = p;
        // }
        // Debugger.ReadData("HTop.dat", out var points, out var values);
        // for (var i = 0; i < points.Count; i++)
        // {
        //     var point = points[i];
        //     point.X -= 2139000;
        //     point.Y -= 6540000;
        //     point.Z = values[i];
        //     points[i] = point;
        // }
        // var delaunay = new DelaunayTriangulation();
        // var mesh = delaunay.Triangulate(points);
        //
        // _baseGraphic.AddObject(new ColorMap(mesh, values, Palette.Rainbow));
        // _baseGraphic.AddObject(mesh);
        // _baseGraphic.AddObject(new Cube());
    }

    private void OnRender(TimeSpan obj)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);
        _baseGraphic.DrawObjects();
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {        
        e.Handled = true;
        _viewPortRenderer.GetCamera().Zoom(0, 0, e.Delta);
        _viewPortRenderer.UpdateView();
        GlControl.InvalidateVisual();
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        _isMouseDown = true;
    }
    
    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        _isMouseDown = false;
        _viewPortRenderer.GetCamera().FirstMouse = true;
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (!_isMouseDown) return;

        var pos = e.GetPosition(this);
        _viewPortRenderer.GetCamera().Move((float)pos.X, (float)pos.Y);
        _viewPortRenderer.UpdateView();
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
}