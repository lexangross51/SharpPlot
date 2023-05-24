using System.Drawing;

namespace SharpPlot.Text;

public struct Caption
{
    public string Text { get; set; }
    public SharpPlotFont Font { get; set; }
    public Size Size => TextPrinter.TextMeasure(Text, Font);

    public Caption()
    {
        Text = "";
        Font = new SharpPlotFont();
    }
}