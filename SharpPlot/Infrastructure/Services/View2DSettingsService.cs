using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SharpPlot.Infrastructure.Services;

public record View2DSettingsService
{
    public IEnumerable<string> FontFamilies { get; } = ["Arial", "Calibri", "Consolas", "Times New Roman"];
    
    public IEnumerable<FontStyle> FontStyles { get; } = Enum.GetValues(typeof(FontStyle)).Cast<FontStyle>();

    public string HorizontalAxisName { get; set; } = "X";

    public string VerticalAxisName { get; set; } = "Y";

    public string SelectedFontFamily { get; set; } = "Arial";

    public FontStyle SelectedFontStyle { get; set; } = FontStyle.Regular;

    public int FontSize { get; set; }

    public bool DrawShortTicks { get; set; }
    
    public bool DrawLongTicks { get; set; }
}