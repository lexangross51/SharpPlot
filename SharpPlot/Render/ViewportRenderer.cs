using System;
using System.Drawing;
using System.Globalization;
using SharpGL;
using SharpGL.Enumerations;
using SharpPlot.Objects.Axes;
using SharpPlot.Text;

namespace SharpPlot.Render;

public class ViewportRenderer : IViewable
{
    private readonly double[] _multipliers = { 1, 2, 5, 10 };
    private readonly Axis _horizontalAxis;
    private readonly Axis _verticalAxis;
    
    public ViewportRenderer()
    {
        _horizontalAxis = new Axis("X");
        _verticalAxis = new Axis("Y");
    }

    public void Draw(IBaseGraphic graphic)
    {
        DrawHorizontalAxis(graphic);
        DrawVerticalAxis(graphic);
        graphic.GL.Viewport((int)graphic.Indent.Horizontal, (int)graphic.Indent.Vertical, 
            (int)graphic.ScreenSize.Width, (int)graphic.ScreenSize.Height);
        DrawGrid(graphic);
        DrawBorders(graphic);
    }

    private double CalculateStepHorizontal(IBaseGraphic graphic)
    {
        graphic.Projection.GetProjection(out var projection);

        double dH = projection[1] - projection[0];
        double hh = graphic.ScreenSize.Width;

        double fontSize = TextPrinter.TextMeasure(Axis.TemplateCaption.Text, _horizontalAxis.AxisName.Font).Width * dH / hh;
        double dTiles = Math.Floor(dH / fontSize);

        double dStep = dH / dTiles;
        double dMul = Math.Pow(10, Math.Floor(Math.Log10(dStep)));

        int i;
        for (i = 1; i < _multipliers.Length - 1; ++i)
        {
            if (dMul * _multipliers[i] > dStep) break;
        }

        dStep = _multipliers[i] * dMul;
        return dStep;
    }

    private double CalculateStepVertical(IBaseGraphic graphic)
    {
        graphic.Projection.GetProjection(out var projection);

        double dH = projection[3] - projection[2];
        double hh = graphic.ScreenSize.Height;

        double fontSize = TextPrinter.TextMeasure(Axis.TemplateCaption.Text, _horizontalAxis.AxisName.Font).Width * dH / hh;
        double dTiles = Math.Floor(dH / fontSize);

        double dStep = dH / dTiles;
        double dMul = Math.Pow(10, Math.Floor(Math.Log10(dStep)));

        int i;
        for (i = 1; i < _multipliers.Length - 1; ++i)
        {
            if (dMul * _multipliers[i] > dStep) break;
        }

        dStep = _multipliers[i] * dMul;
        return dStep;
    }

    private void DrawHorizontalAxis(IBaseGraphic graphic)
    {
        graphic.Projection.GetProjection(out var projection);
        double step = CalculateStepHorizontal(graphic);
        _horizontalAxis.GeneratePoints(projection[0], projection[1], step);

        var textWidth = _horizontalAxis.AxisName.Size.Width;
        int heightText = _horizontalAxis.AxisName.Text == ""
            ? TextPrinter.TextMeasure("0", _horizontalAxis.AxisName.Font).Height
            : TextPrinter.TextMeasure(_horizontalAxis.AxisName.Text, _horizontalAxis.AxisName.Font).Height;

        double hRatio = (projection[1] - projection[0]) / graphic.ScreenSize.Width;
        double vRatio = (projection[3] - projection[2]) / graphic.ScreenSize.Height;

        graphic.GL.MatrixMode(MatrixMode.Projection);
        graphic.GL.PushMatrix();
        graphic.GL.LoadIdentity();
        graphic.GL.Viewport((int)graphic.Indent.Horizontal, 0, 
            (int)graphic.ScreenSize.Width, (int)(graphic.ScreenSize.Height + graphic.Indent.Vertical));
        graphic.GL.Ortho(projection[0], projection[1], projection[2], projection[3], -1, 1);        
        graphic.GL.MatrixMode(MatrixMode.Modelview);
        graphic.GL.PushMatrix();
        graphic.GL.LoadIdentity();

        double minDrawLetter = projection[0];
        double maxDrawLetter = projection[1] - textWidth * hRatio;

        foreach (var it in _horizontalAxis.Points)
        {
            double fVal = it * graphic.Projection.Scaling;
            var msVal = fVal.ToString("G10", CultureInfo.InvariantCulture);
            var stringSize = TextPrinter.TextMeasure(msVal, _horizontalAxis.AxisName.Font).Width;
            var stringPositionL = fVal - stringSize * 0.5 * hRatio;
            var stringPositionR = fVal + stringSize * 0.5 * hRatio;

            if (stringPositionL >= minDrawLetter && stringPositionR <= maxDrawLetter)
            {
                var caption = new Caption() { Text = msVal };
                var tmpFont = caption.Font;
                tmpFont.Color = Math.Abs(fVal) < 1E-15 ? Color.Red : Color.Black;
                caption.Font = tmpFont;
                
                TextPrinter.DrawText(graphic, caption, stringPositionL, projection[2]);
            }
        }
        
        graphic.GL.Color(0f, 0f, 0f);
        graphic.GL.Begin(OpenGL.GL_LINES);

        double dy = 8.0;
        foreach (var it in _horizontalAxis.Points)
        {
            graphic.GL.Vertex(it, projection[2] + (heightText + dy) * vRatio);
            graphic.GL.Vertex(it, projection[2] + heightText * vRatio);
        }

        graphic.GL.End();

        if (textWidth != 0)
        {
            var color = _horizontalAxis.AxisName.Font.Color;
            graphic.GL.Color(color.R, color.G, color.B);
            TextPrinter.DrawText(graphic, _horizontalAxis.AxisName, projection[1] - textWidth * hRatio, projection[2]);
        }
        
        graphic.GL.PopMatrix();
        graphic.GL.MatrixMode(MatrixMode.Projection);
        graphic.GL.PopMatrix();
        graphic.GL.MatrixMode(MatrixMode.Modelview);
    }

    private void DrawVerticalAxis(IBaseGraphic graphic)
    {
        graphic.Projection.GetProjection(out var projection);
        double step = CalculateStepVertical(graphic);
        _verticalAxis.GeneratePoints(projection[2], projection[3], step);

        var textWidth = _verticalAxis.AxisName.Size.Width;
        int heightText = _verticalAxis.AxisName.Text == ""
            ? TextPrinter.TextMeasure("0", _verticalAxis.AxisName.Font).Height
            : TextPrinter.TextMeasure(_verticalAxis.AxisName.Text, _verticalAxis.AxisName.Font).Height;

        double hRatio = (projection[1] - projection[0]) / graphic.ScreenSize.Width;
        double vRatio = (projection[3] - projection[2]) / graphic.ScreenSize.Height;

        graphic.GL.MatrixMode(MatrixMode.Projection);
        graphic.GL.PushMatrix();
        graphic.GL.LoadIdentity();

        graphic.GL.Viewport(0, (int)graphic.Indent.Vertical,
            (int)(graphic.ScreenSize.Width + graphic.Indent.Horizontal), (int)graphic.ScreenSize.Height);
        graphic.GL.Ortho(projection[0], projection[1], projection[2], projection[3], -1, 1);

        graphic.GL.MatrixMode(MatrixMode.Modelview);
        graphic.GL.PushMatrix();
        graphic.GL.LoadIdentity();
        
        double minDrawLetter = projection[2];
        double maxDrawLetter = projection[3] - textWidth * vRatio;

        foreach (var it in _verticalAxis.Points)
        {
            double fVal = it * graphic.Projection.Scaling;
            var msVal = fVal.ToString("G10", CultureInfo.InvariantCulture);
            var stringSize = TextPrinter.TextMeasure(msVal, _horizontalAxis.AxisName.Font).Width;
            var stringPositionL = fVal - stringSize * 0.5 * vRatio;
            var stringPositionR = fVal + stringSize * 0.5 * vRatio;

            if (stringPositionL >= minDrawLetter && stringPositionR <= maxDrawLetter)
            {
                var caption = new Caption() { Text = msVal };
                var tmpFont = caption.Font;
                tmpFont.Color = Math.Abs(fVal) < 1E-15 ? Color.Red : Color.Black;
                caption.Font = tmpFont;
                
                TextPrinter.DrawText(graphic, caption, projection[0], stringPositionL, TextOrientation.Vertical);
            }
        }

        double dx = 8.0;
        graphic.GL.Color(0f, 0f, 0f);
        graphic.GL.Begin(OpenGL.GL_LINES);
        
        foreach (var it in _verticalAxis.Points)
        {
            graphic.GL.Vertex(projection[0] + heightText * hRatio, it);
            graphic.GL.Vertex(projection[0] + (heightText + dx) * hRatio, it);
        }

        graphic.GL.End();

        if (textWidth != 0)
        {
            var color = _verticalAxis.AxisName.Font.Color;
            graphic.GL.Color(color.R, color.G, color.B);
            TextPrinter.DrawText(graphic, _verticalAxis.AxisName, projection[0], projection[3] - textWidth * vRatio, TextOrientation.Vertical);
        }

        graphic.GL.PopMatrix();
        graphic.GL.MatrixMode(MatrixMode.Projection);
        graphic.GL.PopMatrix();
        graphic.GL.MatrixMode(MatrixMode.Modelview);
    }

    private void DrawBorders(IBaseGraphic graphic)
    {
        graphic.GL.MatrixMode(MatrixMode.Projection);
        graphic.GL.PushMatrix();
        graphic.GL.LoadIdentity();
        graphic.GL.Viewport((int)graphic.Indent.Horizontal - 1, (int)graphic.Indent.Vertical - 1, 
            (int)graphic.ScreenSize.Width + 1, (int)graphic.ScreenSize.Height + 1);
        graphic.GL.Ortho(-1, graphic.ScreenSize.Width, -1, graphic.ScreenSize.Height, -1, 1);
        graphic.GL.MatrixMode(MatrixMode.Modelview);
        graphic.GL.PushMatrix();
        graphic.GL.LoadIdentity();
        
        graphic.GL.Color(0f, 0f, 0f);
        graphic.GL.Begin(OpenGL.GL_LINE_LOOP);
        graphic.GL.Vertex(0, 0);
        graphic.GL.Vertex(graphic.ScreenSize.Width, 0);
        graphic.GL.Vertex(graphic.ScreenSize.Width, graphic.ScreenSize.Height);
        graphic.GL.Vertex(0, graphic.ScreenSize.Height);
        graphic.GL.End();
        
        graphic.GL.PopMatrix();
        graphic.GL.MatrixMode(MatrixMode.Projection);
        graphic.GL.PopMatrix();
        graphic.GL.MatrixMode(MatrixMode.Modelview);
    }
    
    private void DrawGrid(IBaseGraphic graphic)
    {
        graphic.Projection.GetProjection(out var projection);
        
        graphic.GL.Color(0.7f, 0.7f, 0.7f);
        graphic.GL.LineWidth(1);
        graphic.GL.Begin(OpenGL.GL_LINES);

        foreach (var it in _horizontalAxis.Points)
        {
            graphic.GL.Vertex(it, projection[2]);
            graphic.GL.Vertex(it, projection[3]);
        }

        foreach (var it in _verticalAxis.Points)
        {
            graphic.GL.Vertex(projection[0], it);
            graphic.GL.Vertex(projection[1], it);
        }

        graphic.GL.End();
    }
}