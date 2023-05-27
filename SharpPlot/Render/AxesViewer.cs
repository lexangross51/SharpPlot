using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using SharpPlot.Objects.Axes;
using SharpPlot.Text;
using SharpPlot.Viewport;

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

        //var newVp = graphic.GetNewViewPort(graphic.ScreenSize);
        //GL.Viewport((int)newVp[0], (int)newVp[1], (int)newVp[2], (int)newVp[3]);
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

        GL.MatrixMode(MatrixMode.Projection);
        GL.PushMatrix();
        GL.MatrixMode(MatrixMode.Modelview);
        GL.PushMatrix();
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GL.Viewport((int)graphic.Indent.Horizontal, 0, (int)(graphic.ScreenSize.Width - graphic.Indent.Horizontal), (int)graphic.ScreenSize.Height);
        GL.Ortho(projection[0], projection[1], 0, graphic.ScreenSize.Height, -1, 1);

        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadIdentity();

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
        GL.Color3(0f, 0f, 0f);
        GL.Begin(PrimitiveType.Lines);

        double dy = 4.0;
        foreach (var it in _horizontalAxis.Points)
        {
            //GL.Vertex2(it, graphic.Indent.Vertical + heightText - 1);
            //GL.Vertex2(it, graphic.Indent.Vertical + heightText + dy);
            GL.Vertex2(it, heightText - 2);
            GL.Vertex2(it, heightText + dy);
        }

        GL.End();

        GL.Color3(0, 0, 0);
        GL.Begin(PrimitiveType.Lines);
        GL.Vertex2(projection[0], heightText + 1);
        GL.Vertex2(projection[1], heightText + 1);
        GL.End();
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();

        GL.Ortho(0, graphic.ScreenSize.Width, 0, graphic.ScreenSize.Height, -1, 1);
        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadIdentity();
        GL.Color3(0, 0, 1);
        //font.PrintText(ClientWidth - font.GetWidthCaption, indentV, 0, orth.HorAxisName);

        GL.LoadIdentity();
        GL.PopMatrix();
        GL.MatrixMode(MatrixMode.Projection);
        GL.PopMatrix();
        GL.MatrixMode(MatrixMode.Modelview);
    }
}