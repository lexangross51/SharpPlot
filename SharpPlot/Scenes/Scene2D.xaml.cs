using System;
using System.Drawing;
using System.Windows.Input;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Wpf;
using SharpPlot.Camera;
using SharpPlot.Render;
using SharpPlot.Viewport;
using Color = System.Drawing.Color;

namespace SharpPlot.Scenes;

public partial class Scene2D
{
    private readonly IRenderContext _context;

    public Scene2D()
    {
        InitializeComponent();
        
        GlControl.Start(new GLWpfControlSettings
        {
            MajorVersion = 4,
            MinorVersion = 6,
            RenderContinuously = false,
        });
        
        _context = new BaseGraphic2D(
            new RenderSettings
            {
                ScreenSize = new ScreenSize
                {
                    StartPoint = new PointF(0, 0),
                    Width = 600,
                    Height = 300,
                },
                Indent = new Indent()
            },
            new Camera2D(new OrthographicProjection(new double[] { -1, 1, -1, 1, -1, 1 }, 0.5)));

        GL.ClearColor(Color.LightCyan);
    }

    private void OnRender(TimeSpan obj)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.Clear(ClearBufferMask.DepthBufferBit);
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
    }
}