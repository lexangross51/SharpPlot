using SharpGL;
using SharpGL.Enumerations;
using SharpPlot.Viewport;

namespace SharpPlot.Render;

public class BaseGraphic : IBaseGraphic
{
    public OpenGL GL { get; set; }
    public ScreenSize ScreenSize { get; set; }
    public Indent Indent { get; set; }
    public IProjection Projection { get; set; }

    public BaseGraphic(OpenGL glContext, ScreenSize screenSize, IProjection projection, Indent indent)
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
            Width = newScreenSize.Width - Indent.Horizontal, 
            Height = newScreenSize.Height - Indent.Vertical
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
        Projection.RationHeightToWidth = ScreenSize.Height / ScreenSize.Width;
        Projection.GetProjection(out var projection);

        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GL.Viewport((int)Indent.Horizontal, (int)Indent.Vertical, (int)ScreenSize.Width, (int)ScreenSize.Height);
        GL.Ortho(projection[0], projection[1], projection[2], projection[3], -1, 1);
        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadIdentity();
    }
}