using System;
using System.Drawing;
using SharpGL;
using SharpGL.Enumerations;
using SharpPlot.Objects.Axes;
using SharpPlot.Text;

namespace SharpPlot.Render;

public class AxesViewer : IViewable
{
    private readonly Axis _horizontalAxis;
    //private readonly Axis _verticalAxis;

    public AxesViewer()
    {
        _horizontalAxis = new HorizontalAxis();
    }

    public void Draw(IBaseGraphic graphic)
    {
        DrawHorizontalAxis(graphic);

        var newVp = graphic.GetNewViewPort(graphic.ScreenSize);
        graphic.GL.Viewport((int)newVp[0], (int)newVp[1], (int)newVp[2], (int)newVp[3]);
    }

    private double CalculateStep(IBaseGraphic graphic)
    {
        double[] multipliers = { 1, 2, 5, 10 };
        graphic.Projection.GetProjection(out var projection);

        double dH = projection[1] - projection[0];
        double hh = graphic.ScreenSize.Width - graphic.Indent.Horizontal;

        double fontSize = TextPrinter.TextMeasure(Axis.TemplateCaption.Text, _horizontalAxis.AxisName.Font).Width * dH / hh;
        double dTiles = Math.Floor(dH / fontSize);

        double dStep = dH / dTiles;
        double dMul = Math.Pow(10, Math.Floor(Math.Log10(dStep)));

        int i;
        for (i = 1; i < multipliers.Length - 1; ++i)
        {
            if (dMul * multipliers[i] > dStep) break;
        }

        dStep = multipliers[i] * dMul;
        return dStep;
    }

    private void DrawHorizontalAxis(IBaseGraphic graphic)
    {
        graphic.Projection.GetProjection(out var projection);
        double step = CalculateStep(graphic);
        _horizontalAxis.GeneratePoints(projection[0], projection[1], step);

        var textWidth = _horizontalAxis.AxisName.Size.Width;
        int heightText = _horizontalAxis.AxisName.Text == ""
            ? TextPrinter.TextMeasure("0", _horizontalAxis.AxisName.Font).Height
            : TextPrinter.TextMeasure(_horizontalAxis.AxisName.Text, _horizontalAxis.AxisName.Font).Height;

        double hRatio = (projection[1] - projection[0]) / (graphic.ScreenSize.Width - graphic.Indent.Horizontal);

        graphic.GL.MatrixMode(MatrixMode.Projection);
        graphic.GL.PushMatrix();
        graphic.GL.MatrixMode(MatrixMode.Modelview);
        graphic.GL.PushMatrix();
        graphic.GL.MatrixMode(MatrixMode.Projection);
        graphic.GL.LoadIdentity();
        graphic.GL.Viewport((int)graphic.Indent.Horizontal, 0, (int)(graphic.ScreenSize.Width - graphic.Indent.Horizontal), (int)graphic.ScreenSize.Height);
        graphic.GL.Ortho(projection[0], projection[1], 0, graphic.ScreenSize.Height, -1, 1);
        graphic.GL.MatrixMode(MatrixMode.Modelview);
        graphic.GL.LoadIdentity();

        double minDrawLetter = projection[0] + graphic.Indent.Horizontal * hRatio;
        double maxDrawLetter = projection[1] - textWidth * hRatio;
        int lastIndex = -1;

        int i = 0;
        foreach (var it in _horizontalAxis.Points)
        {
            if (it < projection[0])
            {
                i++;
                continue;
            }

            double fVal = it * graphic.Projection.Scaling;

            if (lastIndex != -1 && fVal - _horizontalAxis.Points[lastIndex] < 0)
            {
                i++;
                continue;
            }

            lastIndex = i;

            var msVal = $"{fVal:G10}";
            var stringSize = TextPrinter.TextMeasure(msVal, _horizontalAxis.AxisName.Font).Width;
            var stringPositionL = fVal - stringSize * 0.5 * hRatio;
            var stringPositionR = fVal + stringSize * 0.5 * hRatio;

            if (stringPositionL >= minDrawLetter && stringPositionR <= maxDrawLetter)
            {
                var caption = new Caption() { Text = msVal };
                var tmpFont = caption.Font;
                tmpFont.Color = Math.Abs(fVal) < 1E-15 ? Color.Red : Color.Black;
                caption.Font = tmpFont;

                TextPrinter.DrawText(graphic, caption, stringPositionL, 0);
            }
            i++;
        }

        TextPrinter.DrawText(graphic, new Caption() { Text = "123" }, 0.0, 100);
        graphic.GL.Color(0f, 0f, 0f);
        graphic.GL.Begin(OpenGL.GL_LINES);

        double dy = 4.0;
        foreach (var it in _horizontalAxis.Points)
        {
            //GL.Vertex2(it, graphic.Indent.Vertical + heightText - 1);
            //GL.Vertex2(it, graphic.Indent.Vertical + heightText + dy);
            graphic.GL.Vertex(it, heightText - 2);
            graphic.GL.Vertex(it, heightText + dy);
        }

        graphic.GL.End();

        graphic.GL.Color(0f, 0f, 0f);
        graphic.GL.Begin(OpenGL.GL_LINES);
        graphic.GL.Vertex(projection[0], heightText + 1);
        graphic.GL.Vertex(projection[1], heightText + 1);
        graphic.GL.End();
        graphic.GL.MatrixMode(MatrixMode.Projection);
        graphic.GL.LoadIdentity();

        graphic.GL.Ortho(0, graphic.ScreenSize.Width, 0, graphic.ScreenSize.Height, -1, 1);
        graphic.GL.MatrixMode(MatrixMode.Modelview);
        graphic.GL.LoadIdentity();
        graphic.GL.Color(0f, 0f, 1f);
        //font.PrintText(ClientWidth - font.GetWidthCaption, indentV, 0, orth.HorAxisName);
        
        graphic.GL.LoadIdentity();
        graphic.GL.PopMatrix();
        graphic.GL.MatrixMode(MatrixMode.Projection);
        graphic.GL.PopMatrix();
        graphic.GL.MatrixMode(MatrixMode.Modelview);
    }
}