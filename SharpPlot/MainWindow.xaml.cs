using System.Windows.Input;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.WPF;
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
        
        _axesViewer = new AxesViewer();
        var textMes = TextPrinter.TextMeasure("0", new SharpPlotFont());
        _graphic = new BaseGraphic(
            GlControl.OpenGL,
            new ScreenSize(Width, Height), 
            new OrthographicProjection(new double[] { -1, 1, -1, 1, -1, 1 }, Height / Width, true),
            new Indent(textMes.Height, textMes.Height));
    }

    private void Render()
    {
        _graphic.GL.ClearColor(1f, 1f, 1f, 1f);
        _graphic.GL.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        _graphic.GL.MatrixMode(MatrixMode.Modelview);
        _graphic.GL.LoadIdentity();
        // _graphic.GL.MatrixMode(MatrixMode.Projection);
        // _graphic.GL.LoadIdentity();
        // _graphic.GL.Viewport(0, 0, (int)Width, (int)Height);
        // _graphic.GL.Ortho(-1, 1, -1, 1, -1, 1);
        // _graphic.GL.Color(0f, 0f, 0f);
        // _graphic.GL.Begin(OpenGL.GL_LINES);
        // _graphic.GL.Vertex(-1, -0.8);
        // _graphic.GL.Vertex(1, -0.8);
        // _graphic.GL.End();
        _axesViewer.Draw(_graphic);
    }
    
    private void OnRender(object sender, OpenGLRoutedEventArgs args)
    {
        Render();
    }
    
    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var point = _graphic.Projection.FromProjectionToWorld(e.GetPosition(this), _graphic.ScreenSize, _graphic.Indent);
        _graphic.Projection.Scale(point, e.Delta);
        _graphic.UpdateViewMatrix();
    }

    // private void OpenTkControl_OnSizeChanged(object sender, SizeChangedEventArgs e)
    // {
    //     var newVp = _graphic.GetNewViewPort(new ScreenSize(e.NewSize.Width, e.NewSize.Height));
    //     _graphic.GL.Viewport((int)newVp[0], (int)newVp[1], (int)newVp[2], (int)newVp[3]);
    //     _graphic.UpdateViewMatrix();
    //     GlControl.InvalidateVisual();
    // }
}