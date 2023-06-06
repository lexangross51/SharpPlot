using System;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.WPF;
using SharpPlot.Render;
using SharpPlot.Viewport;

namespace SharpPlot.Scenes;

public partial class Scene3D
{
    private readonly IRenderer _viewportRenderer;
    private readonly IBaseGraphic _graphic;
    
    public Scene3D(double width, double height)
    {
        InitializeComponent();

        Width = width;
        Height = height;

        _graphic = new BaseGraphic3D(
            GraphicControl3D.OpenGL,
            new ScreenSize(Width, Height),
            new PerspectiveProjection());
        //_viewportRenderer = new Viewport3DRenderer(_graphic);
    }

    private void OnRender(object sender, OpenGLRoutedEventArgs args)
    {
        var gl = args.OpenGL;
        
        gl.ClearColor(1f, 1f, 1f, 1f);
        gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        gl.Enable(OpenGL.GL_DEPTH_TEST);
        gl.MatrixMode(MatrixMode.Modelview);
        gl.LoadIdentity();
        gl.Rotate(Math.PI / 2.0, 1.0, 0.0, 0.0);
        
        _viewportRenderer.DrawObjects();
        _graphic.UpdateViewMatrix();
        
        gl.Finish();
    }

    private void OnResized(object sender, OpenGLRoutedEventArgs args)
        => GraphicControl3D.DoRender();
    
    public void OnChangeSize(ScreenSize newSize)
    {
        Width = newSize.Width;
        Height = newSize.Height;
        
        _graphic.GL.SetDimensions((int)Width, (int)Height);

        var newVp = _graphic.GetNewViewPort(newSize);
        _graphic.GL.Viewport((int)newVp[0], (int)newVp[1], (int)newVp[2], (int)newVp[3]);
        _graphic.UpdateViewMatrix();
        GraphicControl3D.DoRender();
    }
}