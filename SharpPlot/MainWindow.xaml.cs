using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using OpenTK.Graphics.OpenGL;
using OpenTK.Wpf;
using SharpPlot.Viewport;
using SharpPlot.Text;
using SharpPlot.Render;

namespace SharpPlot;

public partial class MainWindow
{
    private readonly AxesViewer _axesViewer;
    private readonly IBaseGraphic _graphic;

    public MainWindow()
    {
        InitializeComponent();

        OpenTkControl.Start(new GLWpfControlSettings()
        {
            MajorVersion = 2,
            MinorVersion = 1,
            RenderContinuously = false,
        });

        _axesViewer = new AxesViewer();
        var textMes = TextPrinter.TextMeasure("0", new SharpPlotFont());
        _graphic = new BaseGraphic(
            new ScreenSize(Width, Height), 
            new OrthographicProjection(new double[] { -1, 1, -1, 1, -1, 1 }, Height / Width, true),
            new Indent(textMes.Height, textMes.Height));
    }

    private void Render()
    {
        GL.ClearColor(Color.White);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadIdentity();
        GL.Viewport(0, 0, (int)Width, (int)Height);
        GL.Ortho(-1, 1, -1, 1, -1, 1);

        _axesViewer.Draw(_graphic);
    }

    private void OpenTkControl_OnRender(TimeSpan obj)
    {
        Render();
    }

    private void OpenTkControl_OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var point = _graphic.Projection.FromProjectionToWorld(e.GetPosition(this), _graphic.ScreenSize, _graphic.Indent);
        _graphic.Projection.Scale(point, e.Delta);
        _graphic.UpdateViewMatrix();
        OpenTkControl.InvalidateVisual();
    }

    private void OpenTkControl_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var newVp = _graphic.GetNewViewPort(new ScreenSize(Width, Height));
        GL.Viewport((int)newVp[0], (int)newVp[1], (int)newVp[2], (int)newVp[3]);
        _graphic.UpdateViewMatrix();
        OpenTkControl.InvalidateVisual();
    }

    private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Width = e.NewSize.Width;
        Height = e.NewSize.Height;
        OpenTkControl_OnSizeChanged(sender, e);
    }
}