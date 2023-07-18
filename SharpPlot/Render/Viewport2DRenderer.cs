using System;
using System.Drawing;
using System.Globalization;
using SharpGL;
using SharpGL.Enumerations;
using SharpPlot.Objects;
using SharpPlot.Objects.Axes;
using SharpPlot.Text;

namespace SharpPlot.Render;

public class Viewport2DRenderer : IRenderer
{
    private readonly double[] _multipliers = { 1, 2, 5, 10 };
    public bool DrawingGrid { get; set; } = true;
    public Axis HorizontalAxis { get; }
    public Axis VerticalAxis { get; }
    public SharpPlotFont Font { get; set; }
    public IBaseGraphic BaseGraphic { get; set; }

    public Viewport2DRenderer(IBaseGraphic graphic)
    {
        HorizontalAxis = new Axis("X");
        VerticalAxis = new Axis("Y");
        Font = new SharpPlotFont()
        {
            Color = Color.Black
        };
        
        BaseGraphic = graphic;
    }

    public void AppendRenderable(IRenderable obj)
    {
    }

    public void DrawObjects()
    {
        DrawHorizontalAxis();
        DrawVerticalAxis();
        BaseGraphic.GL.Viewport((int)BaseGraphic.Indent.Horizontal, (int)BaseGraphic.Indent.Vertical, 
            (int)BaseGraphic.ScreenSize.Width, (int)BaseGraphic.ScreenSize.Height);
        
        if (DrawingGrid) DrawGrid();
        
        DrawBorders();
        BaseGraphic.UpdateViewMatrix();
    }

    private double CalculateStepHorizontal()
    {
        BaseGraphic.Projection.GetProjection(out var projection);

        double dH = projection[1] - projection[0];
        double hh = BaseGraphic.ScreenSize.Width;

        double fontSize = TextPrinter.TextMeasure(Axis.TemplateCaption, Font).Width * dH / hh;
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

    private double CalculateStepVertical()
    {
        BaseGraphic.Projection.GetProjection(out var projection);

        double dH = projection[3] - projection[2];
        double hh = BaseGraphic.ScreenSize.Height;

        double fontSize = TextPrinter.TextMeasure(Axis.TemplateCaption, Font).Width * dH / hh;
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

    private void DrawHorizontalAxis()
    {
        BaseGraphic.Projection.GetProjection(out var projection);
        double step = CalculateStepHorizontal();
        HorizontalAxis.GeneratePoints(projection[0], projection[1], step);

        var textWidth = TextPrinter.TextMeasure(HorizontalAxis.AxisName, Font).Width;
        int heightText = HorizontalAxis.AxisName == ""
            ? TextPrinter.TextMeasure("0", Font).Height
            : TextPrinter.TextMeasure(HorizontalAxis.AxisName, Font).Height;

        double hRatio = (projection[1] - projection[0]) / BaseGraphic.ScreenSize.Width;
        double vRatio = (projection[3] - projection[2]) / BaseGraphic.ScreenSize.Height;

        BaseGraphic.GL.MatrixMode(MatrixMode.Projection);
        BaseGraphic.GL.PushMatrix();
        BaseGraphic.GL.LoadIdentity();
        BaseGraphic.GL.Viewport((int)BaseGraphic.Indent.Horizontal, 0, 
            (int)BaseGraphic.ScreenSize.Width, (int)(BaseGraphic.ScreenSize.Height + BaseGraphic.Indent.Vertical));
        BaseGraphic.GL.Ortho(projection[0], projection[1], projection[2], projection[3], -1, 1);        
        BaseGraphic.GL.MatrixMode(MatrixMode.Modelview);
        BaseGraphic.GL.PushMatrix();
        BaseGraphic.GL.LoadIdentity();

        double minDrawLetter = projection[0];
        double maxDrawLetter = projection[1] - textWidth * hRatio;

        foreach (var it in HorizontalAxis.Points)
        {
            double fVal = it * BaseGraphic.Projection.Scaling;
            var msVal = fVal.ToString("G10", CultureInfo.InvariantCulture);
            var stringSize = TextPrinter.TextMeasure(msVal, Font).Width;
            var stringPositionL = fVal - stringSize * 0.5 * hRatio;
            var stringPositionR = fVal + stringSize * 0.5 * hRatio;

            if (stringPositionL >= minDrawLetter && stringPositionR <= maxDrawLetter)
            {
                var color = Math.Abs(fVal) < 1E-15 ? Color.Red : Color.Black;
                var sharpPlotFont = Font;
                sharpPlotFont.Color = color;
                
                TextPrinter.DrawText(BaseGraphic, msVal, stringPositionL, projection[2], sharpPlotFont);
            }
        }
        
        BaseGraphic.GL.Color(0f, 0f, 0f);
        BaseGraphic.GL.Begin(OpenGL.GL_LINES);

        double dy = 6.0;
        foreach (var it in HorizontalAxis.Points)
        {
            BaseGraphic.GL.Vertex(it, projection[2] + (heightText + dy) * vRatio);
            BaseGraphic.GL.Vertex(it, projection[2] + (heightText - 2) * vRatio);
        }

        BaseGraphic.GL.End();

        if (textWidth != 0)
        {
            BaseGraphic.GL.Color(Font.Color.R, Font.Color.G, Font.Color.B);
            TextPrinter.DrawText(BaseGraphic, HorizontalAxis.AxisName, projection[1] - textWidth * hRatio, projection[2],
                Font);
        }
        
        BaseGraphic.GL.PopMatrix();
        BaseGraphic.GL.MatrixMode(MatrixMode.Projection);
        BaseGraphic.GL.PopMatrix();
        BaseGraphic.GL.MatrixMode(MatrixMode.Modelview);
    }

    private void DrawVerticalAxis()
    {
        BaseGraphic.Projection.GetProjection(out var projection);
        double step = CalculateStepVertical();
        VerticalAxis.GeneratePoints(projection[2], projection[3], step);

        var textWidth = TextPrinter.TextMeasure(VerticalAxis.AxisName, Font).Width;
        int heightText = VerticalAxis.AxisName == ""
            ? TextPrinter.TextMeasure("0", Font).Height
            : TextPrinter.TextMeasure(VerticalAxis.AxisName, Font).Height;

        double hRatio = (projection[1] - projection[0]) / BaseGraphic.ScreenSize.Width;
        double vRatio = (projection[3] - projection[2]) / BaseGraphic.ScreenSize.Height;

        BaseGraphic.GL.MatrixMode(MatrixMode.Projection);
        BaseGraphic.GL.PushMatrix();
        BaseGraphic.GL.LoadIdentity();

        BaseGraphic.GL.Viewport(0, (int)BaseGraphic.Indent.Vertical,
            (int)(BaseGraphic.ScreenSize.Width + BaseGraphic.Indent.Horizontal), (int)BaseGraphic.ScreenSize.Height);
        BaseGraphic.GL.Ortho(projection[0], projection[1], projection[2], projection[3], -1, 1);

        BaseGraphic.GL.MatrixMode(MatrixMode.Modelview);
        BaseGraphic.GL.PushMatrix();
        BaseGraphic.GL.LoadIdentity();
        
        double minDrawLetter = projection[2];
        double maxDrawLetter = projection[3] - textWidth * vRatio;

        foreach (var it in VerticalAxis.Points)
        {
            double fVal = it * BaseGraphic.Projection.Scaling;
            var msVal = fVal.ToString("G10", CultureInfo.InvariantCulture);
            var stringSize = TextPrinter.TextMeasure(msVal, Font).Width;
            var stringPositionL = fVal - stringSize * 0.5 * vRatio;
            var stringPositionR = fVal + stringSize * 0.5 * vRatio;

            if (stringPositionL >= minDrawLetter && stringPositionR <= maxDrawLetter)
            {
                var sharpPlotFont = Font;
                sharpPlotFont.Color = Math.Abs(fVal) < 1E-15 ? Color.Red : Color.Black;

                TextPrinter.DrawText(BaseGraphic, msVal, projection[0], stringPositionL, sharpPlotFont,
                    TextOrientation.Vertical);
            }
        }

        double dx = 7.0;
        BaseGraphic.GL.Color(0f, 0f, 0f);
        BaseGraphic.GL.Begin(OpenGL.GL_LINES);
        
        foreach (var it in VerticalAxis.Points)
        {
            BaseGraphic.GL.Vertex(projection[0] + heightText * hRatio, it);
            BaseGraphic.GL.Vertex(projection[0] + (heightText + dx) * hRatio, it);
        }

        BaseGraphic.GL.End();

        if (textWidth != 0)
        {
            BaseGraphic.GL.Color(Font.Color.R, Font.Color.G, Font.Color.B);
            TextPrinter.DrawText(BaseGraphic, VerticalAxis.AxisName, projection[0], projection[3] - textWidth * vRatio,
                Font, TextOrientation.Vertical);
        }

        BaseGraphic.GL.PopMatrix();
        BaseGraphic.GL.MatrixMode(MatrixMode.Projection);
        BaseGraphic.GL.PopMatrix();
        BaseGraphic.GL.MatrixMode(MatrixMode.Modelview);
    }

    private void DrawBorders()
    {
        BaseGraphic.GL.MatrixMode(MatrixMode.Projection);
        BaseGraphic.GL.PushMatrix();
        BaseGraphic.GL.LoadIdentity();
        BaseGraphic.GL.Viewport((int)BaseGraphic.Indent.Horizontal - 2, (int)BaseGraphic.Indent.Vertical - 2, 
            (int)BaseGraphic.ScreenSize.Width + 4, (int)BaseGraphic.ScreenSize.Height + 4);
        BaseGraphic.GL.Ortho(BaseGraphic.Indent.Horizontal - 1, BaseGraphic.ScreenSize.Width + 1, 
            BaseGraphic.Indent.Vertical - 1, BaseGraphic.ScreenSize.Height + 1, -1, 1);
        BaseGraphic.GL.MatrixMode(MatrixMode.Modelview);
        BaseGraphic.GL.PushMatrix();
        BaseGraphic.GL.LoadIdentity();
        
        BaseGraphic.GL.LineWidth(2);
        BaseGraphic.GL.Color(0f, 0f, 0f);
        BaseGraphic.GL.Begin(OpenGL.GL_LINE_LOOP);
        BaseGraphic.GL.Vertex(BaseGraphic.Indent.Horizontal, BaseGraphic.Indent.Vertical);
        BaseGraphic.GL.Vertex(BaseGraphic.ScreenSize.Width, BaseGraphic.Indent.Vertical);
        BaseGraphic.GL.Vertex(BaseGraphic.ScreenSize.Width, BaseGraphic.ScreenSize.Height);
        BaseGraphic.GL.Vertex(BaseGraphic.Indent.Horizontal, BaseGraphic.ScreenSize.Height);
        BaseGraphic.GL.End();
        BaseGraphic.GL.LineWidth(1);
        
        BaseGraphic.GL.PopMatrix();
        BaseGraphic.GL.MatrixMode(MatrixMode.Projection);
        BaseGraphic.GL.PopMatrix();
        BaseGraphic.GL.MatrixMode(MatrixMode.Modelview);
    }
    
    private void DrawGrid()
    {
        BaseGraphic.Projection.GetProjection(out var projection);
        BaseGraphic.GL.Color(0.7f, 0.7f, 0.7f);
        BaseGraphic.GL.LineWidth(1);
        BaseGraphic.GL.Begin(OpenGL.GL_LINES);

        foreach (var it in HorizontalAxis.Points)
        {
            BaseGraphic.GL.Vertex(it, projection[2]);
            BaseGraphic.GL.Vertex(it, projection[3]);
        }

        foreach (var it in VerticalAxis.Points)
        {
            BaseGraphic.GL.Vertex(projection[0], it);
            BaseGraphic.GL.Vertex(projection[1], it);
        }

        BaseGraphic.GL.End();
    }
}