using System.Drawing;

namespace SharpPlot.Objects;

public enum PaletteType
{
    Rainbow,
    Autumn
}

public class Palette
{
    private readonly Color[] _colorStorage;

    public Palette(params Color[] colors)
    {
        _colorStorage = colors;
    }

    public Color this[int index] => _colorStorage[index];

    public static Palette Rainbow
        => new(Color.Red, Color.Orange, Color.Yellow, Color.LawnGreen, Color.Aqua, Color.Blue,
            Color.Purple);

    public static Palette Autumn
        => new(Color.Red, Color.Orange, Color.Yellow, Color.Khaki, Color.White);
}