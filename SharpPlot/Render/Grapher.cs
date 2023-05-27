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
        ScreenSize = newScreenSize;

        return new[]
        {
            Indent.Horizontal,
            Indent.Vertical,
            newScreenSize.Width - Indent.Horizontal,
            newScreenSize.Height - Indent.Vertical
        };
    }

    public void UpdateViewMatrix()
    {
        Projection.RationHeightToWidth = ScreenSize.Height / ScreenSize.Width;
        Projection.GetProjection(out var projection);
        var newVp = GetNewViewPort(ScreenSize);
        
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GL.Viewport((int)newVp[0], (int)newVp[1], (int)newVp[2], (int)newVp[3]);
        GL.Ortho(projection[0], projection[1], projection[2], projection[3], -1, 1);
        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadIdentity();
    }
}