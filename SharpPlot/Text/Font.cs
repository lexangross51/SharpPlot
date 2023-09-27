using System.Drawing;

namespace SharpPlot.Text;

public struct SharpPlotFont
{
    private Font? _systemFont;
    
    public string FontFamily { get; set; }
    public int Size { get; set; }
    public Color Color { get; set; }
    public FontStyle Style { get; set; }
    public Font SystemFont => _systemFont ??= MakeSystemFont();

    public SharpPlotFont()
    {
        FontFamily = "Times New Roman";
        Size = 12;
        Color = Color.Black;
        Style = FontStyle.Regular;
    }

    public Font MakeSystemFont() => new(FontFamily, Size, Style);
}