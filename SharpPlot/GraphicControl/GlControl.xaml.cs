using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.WPF;
using SharpPlot.Render;
using SharpPlot.Text;
using SharpPlot.Viewport;

namespace SharpPlot.GraphicControl;

public partial class GlControl
{
    private readonly AxesViewer _axesViewer;
    private readonly IBaseGraphic _graphic;
    private readonly OpenGL _glContext;
    private bool _isMouseDown;
    private double _mouseXPrevious, _mouseYPrevious;

    public GlControl(double width, double height)
    {
        InitializeComponent();

        Width = width;
        Height = height;

        _glContext = GraphicControl.OpenGL;
        _axesViewer = new AxesViewer();
        var textMes = TextPrinter.TextMeasure("0", new SharpPlotFont());
        var indent = new Indent(textMes.Height + 8, textMes.Height + 8);
        var clientWidth = Width - indent.Horizontal;
        var clientHeight = Height - indent.Vertical;
        _graphic = new BaseGraphic(
            GraphicControl.OpenGL,
            new ScreenSize(clientWidth, clientHeight), 
            new OrthographicProjection(new double[] { -1, 1, -1, 1, -1, 1 }, clientHeight / clientWidth, true),
            indent);
    }

    private void RenderBorders()
    {
        _graphic.GL.MatrixMode(MatrixMode.Projection);
        _graphic.GL.PushMatrix();
        _graphic.GL.MatrixMode(MatrixMode.Modelview);
        _graphic.GL.PushMatrix();
        _graphic.GL.MatrixMode(MatrixMode.Projection);
        _graphic.GL.LoadIdentity();
        _graphic.GL.Viewport((int)_graphic.Indent.Horizontal - 1, (int)_graphic.Indent.Vertical - 1, 
            (int)_graphic.ScreenSize.Width + 1, (int)_graphic.ScreenSize.Height + 2);
        _graphic.GL.Ortho(-1, _graphic.ScreenSize.Width + 1, -1, _graphic.ScreenSize.Height + 1, -1, 1);
        _graphic.GL.MatrixMode(MatrixMode.Modelview);
        _graphic.GL.LoadIdentity();
        
        _graphic.GL.Color(0f, 0f, 0f);
        _graphic.GL.Begin(OpenGL.GL_LINE_LOOP);
        _graphic.GL.Vertex(0, 0);
        _graphic.GL.Vertex(_graphic.ScreenSize.Width, 0);
        _graphic.GL.Vertex(_graphic.ScreenSize.Width, _graphic.ScreenSize.Height);
        _graphic.GL.Vertex(0, _graphic.ScreenSize.Height);
        _graphic.GL.End();
        
        // _graphic.GL.MatrixMode(MatrixMode.Projection);
        // _graphic.GL.LoadIdentity();
        // _graphic.GL.MatrixMode(MatrixMode.Modelview);
        // _graphic.GL.LoadIdentity();
        // _graphic.GL.Color(0f, 0f, 1f);
        // _graphic.GL.LoadIdentity();
        _graphic.GL.PopMatrix();
        _graphic.GL.MatrixMode(MatrixMode.Projection);
        _graphic.GL.PopMatrix();
        _graphic.GL.MatrixMode(MatrixMode.Modelview);
        _graphic.GL.Viewport((int)_graphic.Indent.Horizontal, (int)_graphic.Indent.Vertical, 
            (int)_graphic.ScreenSize.Width, (int)_graphic.ScreenSize.Height);
    }
    
    private void OnRender(object sender, OpenGLRoutedEventArgs args)
    {
        //_glContext.MakeCurrent();
        _glContext.ClearColor(1f, 1f, 1f, 1f);
        _glContext.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        _glContext.MatrixMode(MatrixMode.Modelview);
        _glContext.LoadIdentity();
        
        _axesViewer.Draw(_graphic);
        RenderBorders();

        _glContext.Color(1f, 0f, 0f);
        _glContext.Begin(OpenGL.GL_TRIANGLES);
        _glContext.Vertex(0, 0);
        _glContext.Vertex(2, 1);
        _glContext.Vertex(1, 3);
        _glContext.End();
        _glContext.Finish();
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var pos = e.GetPosition(this);
        _graphic.Projection.FromWorldToProjection(pos.X, pos.Y, _graphic.ScreenSize, _graphic.Indent, out var x, out var y);
        _graphic.Projection.Scale(x, y, e.Delta);
        _graphic.UpdateViewMatrix();
        GraphicControl.DoRender();
    }
    
    private void OnRenderSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var newVp = _graphic.GetNewViewPort(new ScreenSize(Width, Height));
        _graphic.GL.Viewport((int)newVp[0], (int)newVp[1], (int)newVp[2], (int)newVp[3]);
        _graphic.UpdateViewMatrix();
        GraphicControl.DoRender();
    }

    private void OnMainSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Width = e.NewSize.Width;
        Height = e.NewSize.Height;
        OnRenderSizeChanged(sender, e);
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_isMouseDown)
        {
            var mousePosition = e.GetPosition(this);
            _graphic.Projection.FromWorldToProjection(_mouseXPrevious, _mouseYPrevious, _graphic.ScreenSize,
                _graphic.Indent, out var xPrevious, out var yPrevious);
            
            _graphic.Projection.FromWorldToProjection(mousePosition.X, mousePosition.Y, _graphic.ScreenSize,
                _graphic.Indent, out var xCurrent, out var yCurrent);
            
            _graphic.Projection.Translate(-xCurrent + xPrevious, -yCurrent + yPrevious);

            _mouseXPrevious = mousePosition.X;
            _mouseYPrevious = mousePosition.Y;
            
            _graphic.UpdateViewMatrix();
            GraphicControl.DoRender();
        }
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        var mousePosition = e.GetPosition(this);
        _isMouseDown = true;
        _mouseXPrevious = mousePosition.X;
        _mouseYPrevious = mousePosition.Y;
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        _isMouseDown = false;
    }
}