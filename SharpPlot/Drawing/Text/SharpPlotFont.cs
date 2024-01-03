using System.Drawing;

namespace SharpPlot.Drawing.Text;

public class SharpPlotFont
{
    private string _fontFamily = "Times New Roman";
    private FontStyle _fontStyle = FontStyle.Regular;
    private float _size = 16;

    public float Size
    {
        get => _size;
        set
        {
            _size = value;
            SystemFont = new Font(_fontFamily, _size, _fontStyle);
        }
    }

    public string FontFamily
    {
        get => _fontFamily;
        set
        {
            _fontFamily = value; 
            SystemFont = new Font(_fontFamily, _size, _fontStyle);
        }
    }

    public FontStyle FontStyle
    {
        get => _fontStyle;
        set
        {
            _fontStyle = value;
            SystemFont = new Font(_fontFamily, _size, _fontStyle);
        }
    }
    
    public Font SystemFont { get; private set; }

    public SharpPlotFont() => SystemFont = new Font(_fontFamily, _size, _fontStyle);

    public void Print(double x, double y, double z, string text, Color color, 
        TextRenderOrientation orientation = TextRenderOrientation.Horizontal) 
        => TextRenderer.Instance.Print(x, y, z, text, this, color, orientation);
}