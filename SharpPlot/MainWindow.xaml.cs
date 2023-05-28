using System.Collections.Generic;
using System.Windows;
using SharpPlot.GraphicControl;

namespace SharpPlot;

public partial class MainWindow
{
    // private readonly AxesViewer _axesViewer;
    // private readonly IBaseGraphic _graphic;

    private readonly List<GlControl> _controls;
    
    public MainWindow()
    {
        InitializeComponent();

        _controls = new List<GlControl>();
         var control = new GlControl(1000, 600)
         {
             VerticalAlignment = VerticalAlignment.Center,
             HorizontalAlignment = HorizontalAlignment.Center,
         };
         MainGrid.Children.Add(control);
         _controls.Add(control);

        // _axesViewer = new AxesViewer();
        // var textMes = TextPrinter.TextMeasure("0", new SharpPlotFont());
        // var indent = new Indent(textMes.Height + 8, textMes.Height + 8);
        // var clientWidth = Width - indent.Horizontal;
        // var clientHeight = Height - indent.Vertical;
        // _graphic = new BaseGraphic(
        //     GlControl.OpenGL,
        //     new ScreenSize(clientWidth, clientHeight), 
        //     new OrthographicProjection(new double[] { -1, 1, -1, 1, -1, 1 }, clientHeight / clientWidth, true),
        //     indent);
    }

    // private void OnRender(object sender, OpenGLRoutedEventArgs args)
    // {
    //     //_graphic.GL.MakeCurrent();
    //     _graphic.GL.ClearColor(1f, 1f, 1f, 1f);
    //     _graphic.GL.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
    //     _graphic.GL.MatrixMode(MatrixMode.Modelview);
    //     _graphic.GL.LoadIdentity();
    //     
    //     _axesViewer.Draw(_graphic);
    //     var newVp = _graphic.GetNewViewPort(new ScreenSize(Width, Height));
    //     _graphic.GL.Viewport((int)newVp[0], (int)newVp[1], (int)newVp[2], (int)newVp[3]);
    //     
    //     _graphic.GL.Color(1f, 0f, 0f);
    //     _graphic.GL.Begin(OpenGL.GL_TRIANGLES);
    //     _graphic.GL.Vertex(0, 0);
    //     _graphic.GL.Vertex(2, 1);
    //     _graphic.GL.Vertex(1, 3);
    //     _graphic.GL.End();
    //     _graphic.GL.Finish();
    // }
    //
    // private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    // {
    //     var point = _graphic.Projection.FromProjectionToWorld(e.GetPosition(this), _graphic.ScreenSize, _graphic.Indent);
    //     _graphic.Projection.Scale(point, e.Delta);
    //     _graphic.UpdateViewMatrix();
    //     GlControl.DoRender();
    // }
    //
    // private void GlControl_OnSizeChanged(object sender, SizeChangedEventArgs e)
    // {
    //     var newVp = _graphic.GetNewViewPort(new ScreenSize(Width, Height));
    //     _graphic.GL.Viewport((int)newVp[0], (int)newVp[1], (int)newVp[2], (int)newVp[3]);
    //     _graphic.UpdateViewMatrix();
    //     GlControl.DoRender();
    // }

    private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Width = e.NewSize.Width;
        Height = e.NewSize.Height;

        //GlControl_OnSizeChanged(sender, e);
        foreach (var control in _controls)
        {
            control.InvalidateMeasure();
        }
    }
}