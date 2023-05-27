using System.Drawing;

namespace SharpPlot.Text;

public struct SharpPlotFont
{
    public string FontFamily { get; set; }
    public int Size { get; set; }
    public Color Color { get; set; }
    public FontStyle Style { get; set; }

    public SharpPlotFont()
    {
        FontFamily = "Times New Roman";
        Size = 12;
        Color = Color.Black;
        Style = FontStyle.Bold;
    }

    public Font MakeFont() => new(System.Drawing.FontFamily.GenericMonospace, Size, Style);
}