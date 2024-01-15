using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SharpPlot.MVVM;

namespace SharpPlot.ViewModels;

public class SettingsViewModel : NotifyObject
{
    private string _horAxisName = "X", _vertAxisName = "Y";
    private string _selectedFamily = "Arial";
    private FontStyle _selectedStyle = FontStyle.Regular;
    private int _fontSize = 14;
    private bool _showShortTicks, _showLongTicks;
    
    public string Title => "Draw settings";

    public IEnumerable<FontStyle> FontStyles { get; } = Enum.GetValues(typeof(FontStyle)).Cast<FontStyle>();
    public IEnumerable<string> FontFamilies { get; } = ["Arial", "Calibri", "Consolas", "Times New Roman"];

    public string HorizontalAxisName
    {
        get => _horAxisName;
        set => RaiseAndSetIfChanged(ref _horAxisName, value);
    }
    
    public string VerticalAxisName
    {
        get => _vertAxisName;
        set => RaiseAndSetIfChanged(ref _vertAxisName, value);
    }

    public string SelectedFontFamily
    {
        get => _selectedFamily;
        set => RaiseAndSetIfChanged(ref _selectedFamily, value);
    }

    public FontStyle SelectedFontStyle
    {
        get => _selectedStyle;
        set => RaiseAndSetIfChanged(ref _selectedStyle, value);
    }

    public int FontSize
    {
        get => _fontSize;
        set => RaiseAndSetIfChanged(ref _fontSize, value);
    }

    public bool ShowShortTicks
    {
        get => _showShortTicks;
        set => RaiseAndSetIfChanged(ref _showShortTicks, value);
    }
    
    public bool ShowLongTicks
    {
        get => _showLongTicks;
        set => RaiseAndSetIfChanged(ref _showLongTicks, value);
    }
}