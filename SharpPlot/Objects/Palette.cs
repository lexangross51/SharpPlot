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
    public int ColorsCount => _colorStorage.Length;
    private Palette(params Color[] colors)
    {
        _colorStorage = colors;
    }

    public Color this[int index] => _colorStorage[index];

    public static Palette Create(PaletteType paletteType)
        => paletteType switch
        {
            PaletteType.Rainbow => Rainbow,
            PaletteType.Autumn => Autumn,
            PaletteType.AutumnReverse => AutumnReverse,
            PaletteType.RainbowReverse => RainbowReverse,
            _ => throw new ArgumentOutOfRangeException(nameof(paletteType), paletteType, null)
        };

    private static Palette Rainbow
        => new(Color.FromArgb(255, 255, 69, 0),
            Color.FromArgb(255, 255, 165, 60),
            Color.FromArgb(230, 255, 255, 0),
            Color.FromArgb(255, 152, 251, 152),
            Color.FromArgb(255, 135, 206, 250),
            Color.FromArgb(255, 30, 144, 255),
            Color.FromArgb(255, 138, 43, 226));
    
    private static Palette RainbowReverse
        => new(Color.FromArgb(255, 138, 43, 226),
            Color.FromArgb(255, 30, 144, 255),
            Color.FromArgb(255, 135, 206, 250),
            Color.FromArgb(255, 152, 251, 152),
            Color.FromArgb(230, 255, 255, 0),
            Color.FromArgb(255, 255, 165, 60),
            Color.FromArgb(255, 255, 69, 0));

    private static Palette Autumn
        => new(Color.FromArgb(255, 255, 0, 0),
            Color.FromArgb(255, 255, 69, 0),
            Color.FromArgb(255, 255, 140, 0),
            Color.FromArgb(255, 255, 215, 0),
            Color.FromArgb(255, 255, 255, 0),
            Color.FromArgb(255, 255, 255, 150),
            Color.FromArgb(255, 255, 255, 255));

    private static Palette AutumnReverse
        => new(Color.FromArgb(255, 255, 255, 255),
            Color.FromArgb(255, 255, 255, 150),
            Color.FromArgb(255, 255, 255, 0),
            Color.FromArgb(255, 255, 215, 0),
            Color.FromArgb(255, 255, 140, 0),
            Color.FromArgb(255, 255, 69, 0),
            Color.FromArgb(255, 255, 0, 0));
}