using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;
using SharpPlot.Infrastructure.Services;
using SharpPlot.MVVM;
using SharpPlot.MVVM.Commands;

namespace SharpPlot.ViewModels;

public class SettingsViewModel : NotifyObject
{
    private string _horAxisName, _vertAxisName;
    private string _selectedFamily;
    private FontStyle _selectedStyle;
    private int _fontSize;
    private bool _drawShortTicks, _drawLongTicks;

    public string Title => "Draw settings";

    public IEnumerable<string> FontFamilies { get; }
    
    public IEnumerable<FontStyle> FontStyles { get; }
    
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

    public bool DrawShortTicks
    {
        get => _drawShortTicks;
        set => RaiseAndSetIfChanged(ref _drawShortTicks, value);
    }
    
    public bool DrawLongTicks
    {
        get => _drawLongTicks;
        set => RaiseAndSetIfChanged(ref _drawLongTicks, value);
    }
    
    public ICommand SaveSettingsCommand { get; }
    
    public SettingsViewModel(View2DSettingsService settings)
    {
        FontFamilies = settings.FontFamilies;
        FontStyles = settings.FontStyles;
        
        _horAxisName = settings.HorizontalAxisName;
        _vertAxisName = settings.VerticalAxisName;
        _selectedFamily = settings.SelectedFontFamily;
        _selectedStyle = settings.SelectedFontStyle;
        _fontSize = settings.FontSize;
        _drawShortTicks = settings.DrawShortTicks;
        _drawLongTicks = settings.DrawLongTicks;

        SaveSettingsCommand = RelayCommand.Create(_ =>
        {
            settings.HorizontalAxisName = _horAxisName;
            settings.VerticalAxisName = _vertAxisName;
            settings.SelectedFontFamily = _selectedFamily;
            settings.SelectedFontStyle = _selectedStyle;
            settings.FontSize = _fontSize;
            settings.DrawShortTicks = _drawShortTicks;
            settings.DrawLongTicks = _drawLongTicks;
        });
    }
}