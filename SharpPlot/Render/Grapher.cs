using SharpGL;
using SharpGL.Enumerations;
using SharpPlot.Viewport;

namespace SharpPlot.Render;

public class BaseGraphic2D : IBaseGraphic
{
    public OpenGL GL { get; set; }
    public ScreenSize ScreenSize { get; set; }
    public Indent Indent { get; set; }
    public IProjection Projection { get; set; }

    public BaseGraphic2D(OpenGL glContext, ScreenSize screenSize, IProjection projection, Indent indent = new())
    {
        GL = glContext;
        ScreenSize = screenSize;
        Projection = projection;
        Indent = indent;
    }

    public double[] GetNewViewPort(ScreenSize newScreenSize)
    {
        ScreenSize = new ScreenSize
        {
            Width = newScreenSize.Width - Indent.Horizontal - 2, 
            Height = newScreenSize.Height - Indent.Vertical - 2
        };

        return new[]
        {
            Indent.Horizontal,
            Indent.Vertical,
            ScreenSize.Width,
            ScreenSize.Height
        };
    }

    public void UpdateViewMatrix()
    {
        Projection.Ratio = ScreenSize.Height / ScreenSize.Width;
        Projection.GetProjection(out var projection);

        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GL.Viewport((int)Indent.Horizontal, (int)Indent.Vertical, (int)ScreenSize.Width, (int)ScreenSize.Height);
        GL.Ortho(projection[0], projection[1], projection[2], projection[3], -1, 1);
        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadIdentity();
    }
}

public class BaseGraphic3D : IBaseGraphic
{
    public OpenGL GL { get; set; }
    public ScreenSize ScreenSize { get; set; }
    public Indent Indent { get; set; }
    public IProjection Projection { get; set; }

    public BaseGraphic3D(OpenGL glContext, ScreenSize screenSize, IProjection projection, Indent indent = new())
    {
        GL = glContext;
        ScreenSize = screenSize;
        Projection = projection;
        Indent = indent;
    }
    
    public double[] GetNewViewPort(ScreenSize newScreenSize)
    {
        ScreenSize = newScreenSize;
        return new[] { 0, 0, newScreenSize.Width, newScreenSize.Height };
    }

    public void UpdateViewMatrix()
    {
        Projection.Ratio = ScreenSize.Height / ScreenSize.Width;
        //Projection.GetProjection(out var projection);
        
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GL.Viewport(0, 0, (int)ScreenSize.Width, (int)ScreenSize.Height);
        GL.Perspective(60.0, Projection.Ratio, -1, 100);
        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadIdentity();
    }
}