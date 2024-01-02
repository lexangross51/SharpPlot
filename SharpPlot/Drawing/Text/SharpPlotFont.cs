using System.Drawing;

namespace SharpPlot.Drawing.Text;

public class SharpPlotFont
{
    private string _fontFamily = "Times New Roman";
    private FontStyle _fontStyle = FontStyle.Regular;
    private float _size = 14;
    private Font _systemFont;

    public float Size
    {
        get => _size;
        set
        {
            _size = value;
            _systemFont = new Font(_fontFamily, _size, _fontStyle);
        }
    }

    public string FontFamily
    {
        get => _fontFamily;
        set
        {
            _fontFamily = value; 
            _systemFont = new Font(_fontFamily, _size, _fontStyle);
        }
    }

    public FontStyle FontStyle
    {
        get => _fontStyle;
        set
        {
            _fontStyle = value;
            _systemFont = new Font(_fontFamily, _size, _fontStyle);
        }
    }
    
    public Font SystemFont => _systemFont;

    public SharpPlotFont() => _systemFont = new Font(_fontFamily, _size, _fontStyle);

    public void Print(double x, double y, double z, string text, Color color, 
        TextRenderOrientation orientation = TextRenderOrientation.Horizontal) 
        => TextRenderer.GetInstance().Print(x, y, z, text, this, color, orientation);
}