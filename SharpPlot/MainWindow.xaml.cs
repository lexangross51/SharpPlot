using System.Windows;
using System.Windows.Input;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.WPF;
using SharpPlot.Viewport;
using SharpPlot.Text;
using SharpPlot.Render;
using System.Drawing;

namespace SharpPlot;

public partial class MainWindow
{
    private readonly AxesViewer _axesViewer;
    private readonly IBaseGraphic _graphic;

    public MainWindow()
    {
        InitializeComponent();
        
        _axesViewer = new AxesViewer();
        var textMes = TextPrinter.TextMeasure("0", new SharpPlotFont());
        _graphic = new BaseGraphic(
            GlControl.OpenGL,
            new ScreenSize(Width, Height), 
            new OrthographicProjection(new double[] { -1, 1, -1, 1, -1, 1 }, Height / Width, true),
            new Indent(textMes.Height, textMes.Height));
    }

    private void RenderBorders()
    {
        _graphic.GL.MatrixMode(MatrixMode.Modelview);
        _graphic.GL.PushMatrix();
        _graphic.GL.MatrixMode(MatrixMode.Projection);
        _graphic.GL.PushMatrix();
        _graphic.GL.MatrixMode(MatrixMode.Projection);
        _graphic.GL.LoadIdentity();

        var indent = _graphic.Indent;
        _graphic.GL.Viewport(
            (int)indent.Horizontal, (int)indent.Vertical, 
            (int)(_graphic.ScreenSize.Width - indent.Horizontal),
            (int)(_graphic.ScreenSize.Height - indent.Vertical));
        _graphic.GL.Ortho(-1, 1, -1, 1, -1, 1);

        _graphic.GL.MatrixMode(MatrixMode.Modelview);
        _graphic.GL.LoadIdentity();

        _graphic.GL.Color(0f, 0f, 0f);
        _graphic.GL.Begin(OpenGL.GL_LINE_LOOP);
        _graphic.GL.Vertex(-1, -1);
        _graphic.GL.Vertex(1, -1);
        _graphic.GL.Vertex(1, 1);
        _graphic.GL.Vertex(-1, 1);
        _graphic.GL.End();
        _graphic.GL.PopMatrix();
        _graphic.GL.PopMatrix();
    }

    private void OnRender(object sender, OpenGLRoutedEventArgs args)
    {
        _graphic.GL.ClearColor(0.7f, 1f, 1f, 1f);
        _graphic.GL.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        _graphic.GL.MatrixMode(MatrixMode.Modelview);
        _graphic.GL.LoadIdentity();

        RenderBorders();

        //_axesViewer.Draw(_graphic);

        _graphic.GL.Finish();
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var point = _graphic.Projection.FromProjectionToWorld(e.GetPosition(this), _graphic.ScreenSize, _graphic.Indent);
        _graphic.Projection.Scale(point, e.Delta);
        _graphic.UpdateViewMatrix();
        GlControl.DoRender();
    }

    private void GlControl_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var newVp = _graphic.GetNewViewPort(new ScreenSize(Width, Height));
        _graphic.GL.Viewport((int)newVp[0], (int)newVp[1], (int)newVp[2], (int)newVp[3]);
        _graphic.UpdateViewMatrix();
        GlControl.DoRender();
    }

    private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Width = e.NewSize.Width;
        Height = e.NewSize.Height;
        GlControl_OnSizeChanged(sender, e);
    }
}