using System.Windows.Input;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.WPF;
using SharpPlot.Render;
using SharpPlot.Text;
using SharpPlot.Viewport;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace SharpPlot.Scenes;

public partial class Scene2D
{
    private readonly IRenderer _viewportRenderer;
    private readonly IBaseGraphic _graphic;
    private bool _isMouseDown;
    private double _mouseXPrevious, _mouseYPrevious;
    public IRenderer ObjectsRenderer { get; }
    
    public Scene2D(double width, double height)
    {
        InitializeComponent();

        Width = width;
        Height = height;
        
        var textMes = TextPrinter.TextMeasure("0", new SharpPlotFont());
        var indent = new Indent(textMes.Height + 10, textMes.Height + 10);
        var clientWidth = Width - indent.Horizontal - 2;
        var clientHeight = Height - indent.Vertical - 2;
        
        _graphic = new BaseGraphic2D(
            GraphicControl2D.OpenGL,
            new ScreenSize(clientWidth, clientHeight), 
            new OrthographicProjection(new double[] { -1, 1, -1, 1, -1, 1 }, clientHeight / clientWidth), 
            indent);
        
        _viewportRenderer = new Viewport2DRenderer(_graphic);
        ObjectsRenderer = new ObjectsRenderer(_graphic);
    }

    private void OnRender(object sender, OpenGLRoutedEventArgs args)
    {
        var gl = args.OpenGL;
        gl.MakeCurrent();
        gl.ClearColor(1f, 1f, 1f, 1f);
        gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        
        _viewportRenderer.DrawObjects();
        ObjectsRenderer.DrawObjects();

        gl.Finish();
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var pos = e.GetPosition(this);
        _graphic.Projection.FromWorldToProjection(pos.X, pos.Y, _graphic.ScreenSize, _graphic.Indent, out var x, out var y);
        _graphic.Projection.Scale(x, y, e.Delta);
        _graphic.UpdateViewMatrix();
        GraphicControl2D.DoRender();
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (!_isMouseDown) return;
        
        var mousePosition = e.GetPosition(this);
        _graphic.Projection.FromWorldToProjection(_mouseXPrevious, _mouseYPrevious, _graphic.ScreenSize,
            _graphic.Indent, out var xPrevious, out var yPrevious);
            
        _graphic.Projection.FromWorldToProjection(mousePosition.X, mousePosition.Y, _graphic.ScreenSize,
            _graphic.Indent, out var xCurrent, out var yCurrent);
            
        _graphic.Projection.Translate(-xCurrent + xPrevious, -yCurrent + yPrevious);

        _mouseXPrevious = mousePosition.X;
        _mouseYPrevious = mousePosition.Y;
            
        _graphic.UpdateViewMatrix();
        GraphicControl2D.DoRender();
    }

    private void OnLeftMouseDown(object sender, MouseButtonEventArgs e)
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
        => GraphicControl2D.DoRender();

    public void OnChangeSize(ScreenSize newSize)
    {
        Width = newSize.Width;
        Height = newSize.Height;
        
        var newVp = _graphic.GetNewViewPort(newSize);
        _graphic.GL.SetDimensions((int)Width, (int)Height);
        _graphic.GL.Viewport((int)newVp[0], (int)newVp[1], (int)newVp[2], (int)newVp[3]);
        _graphic.UpdateViewMatrix();
        GraphicControl2D.DoRender();
    }

    private void OnRightMouseDown(object sender, MouseButtonEventArgs e)
    {
        var renderer = (_viewportRenderer as Viewport2DRenderer)!;
        renderer.DrawingGrid = !renderer.DrawingGrid;
        GraphicControl2D.DoRender();
    }
}