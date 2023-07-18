using System;
using System.Windows.Media;

namespace SharpPlot.Objects;

public enum PaletteType
{
    Rainbow,
    RainbowReverse,
    Autumn,
    AutumnReverse
}

public class Palette
{
    private readonly Color[] _colorStorage;
    private int _colorsCounter;
    public int ColorsCount => _colorStorage.Length;

    public Palette(int colorsCount)
    {
        _colorStorage = new Color[colorsCount];
    }
    
    private Palette(params Color[] colors)
    {
        _colorStorage = colors;
    }

    public Color this[int index] => _colorStorage[index];

    public void AddColor(Color color)
    {
        _colorStorage[_colorsCounter++] = color;
    }
    
    public static Palette Create(PaletteType paletteType)
        => paletteType switch
        {
            PaletteType.Rainbow => Rainbow,
            PaletteType.Autumn => Autumn,
            PaletteType.AutumnReverse => AutumnReverse,
            PaletteType.RainbowReverse => RainbowReverse,
            _ => throw new ArgumentOutOfRangeException(nameof(paletteType), paletteType, null)
        };

    public static Palette Rainbow
        => new(Color.FromArgb(255, 255, 69, 0),
            Color.FromArgb(255, 255, 165, 60),
            Color.FromArgb(230, 255, 255, 0),
            Color.FromArgb(255, 152, 251, 152),
            Color.FromArgb(255, 135, 206, 250),
            Color.FromArgb(255, 30, 144, 255),
            Color.FromArgb(255, 138, 43, 226));
    
    public static Palette RainbowReverse
        => new(Color.FromArgb(255, 138, 43, 226),
            Color.FromArgb(255, 30, 144, 255),
            Color.FromArgb(255, 135, 206, 250),
            Color.FromArgb(255, 152, 251, 152),
            Color.FromArgb(230, 255, 255, 0),
            Color.FromArgb(255, 255, 165, 60),
            Color.FromArgb(255, 255, 69, 0));

    public static Palette Autumn
        => new(Color.FromArgb(255, 255, 0, 0),
            Color.FromArgb(255, 255, 69, 0),
            Color.FromArgb(255, 255, 140, 0),
            Color.FromArgb(255, 255, 215, 0),
            Color.FromArgb(255, 255, 255, 0),
            Color.FromArgb(255, 255, 255, 150),
            Color.FromArgb(255, 255, 255, 255));

    public static Palette AutumnReverse
        => new(Color.FromArgb(255, 255, 255, 255),
            Color.FromArgb(255, 255, 255, 150),
            Color.FromArgb(255, 255, 255, 0),
            Color.FromArgb(255, 255, 215, 0),
            Color.FromArgb(255, 255, 140, 0),
            Color.FromArgb(255, 255, 69, 0),
            Color.FromArgb(255, 255, 0, 0));
}