using System.Windows.Input;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.WPF;
using SharpPlot.Render;
using SharpPlot.Text;
using SharpPlot.Viewport;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace SharpPlot.GraphicControl;

public partial class GlControl
{
    private readonly IViewable _viewportRenderer;
    private readonly IBaseGraphic _graphic;
    private OpenGL _glContext;
    private bool _isMouseDown;
    private double _mouseXPrevious, _mouseYPrevious;
    
    public GlControl(double width, double height)
    {
        InitializeComponent();

        Width = width;
        Height = height;
        
        _glContext = GraphicControl.OpenGL;
        _viewportRenderer = new ViewportRenderer();
        var textMes = TextPrinter.TextMeasure("0", new SharpPlotFont());
        var indent = new Indent(textMes.Height + 8, textMes.Height + 8);
        var clientWidth = Width - indent.Horizontal;
        var clientHeight = Height - indent.Vertical;
        _graphic = new BaseGraphic(
            GraphicControl.OpenGL,
            new ScreenSize(clientWidth, clientHeight), 
            new OrthographicProjection(new double[] { -1, 1, -1, 1, -1, 1 }, clientHeight / clientWidth), 
            indent);
    }

    private void OnRender(object sender, OpenGLRoutedEventArgs args)
    {
        _glContext = _graphic.GL;
        _glContext.ClearColor(1f, 1f, 1f, 1f);
        _glContext.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        _glContext.MatrixMode(MatrixMode.Modelview);
        _glContext.LoadIdentity();

        _viewportRenderer.Draw(_graphic);

        // _glContext.Begin(OpenGL.GL_TRIANGLES);
        // _glContext.Color(1f, 0f, 0f);
        // _glContext.Vertex(0, 0);
        // _glContext.Color(1f, 1f, 0f);
        // _glContext.Vertex(2, 1);
        // _glContext.Color(0f, 1f, 1f);
        // _glContext.Vertex(1, 3);
        // _glContext.End();
        //
        // TextPrinter.DrawText(_graphic,  new Caption {Text = "Suka"}, 0, 0);
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

    private void OnResized(object sender, OpenGLRoutedEventArgs args)
        => GraphicControl.DoRender();

    public void OnChangeSize(ScreenSize newSize)
    {
        Width = newSize.Width;
        Height = newSize.Height;
        
        _glContext.SetDimensions((int)Width, (int)Height);

        var newVp = _graphic.GetNewViewPort(newSize);
        _graphic.GL.Viewport((int)newVp[0], (int)newVp[1], (int)newVp[2], (int)newVp[3]);
        _graphic.UpdateViewMatrix();
        GraphicControl.DoRender();
    }
}