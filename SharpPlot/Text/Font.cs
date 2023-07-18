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
        FontFamily = "Calibri";
        Size = 12;
        Color = Color.Black;
        Style = FontStyle.Regular;
    }

    public Font MakeFont() => new(FontFamily, Size, Style);
}