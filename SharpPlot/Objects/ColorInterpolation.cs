using System;
using System.Windows.Media;

namespace SharpPlot.Objects;

public enum ColorInterpolation
{
    Constant,
    Linear
}

public static class ColorInterpolator
{
    public static Color InterpolateColor(double[] range, double value, Palette palette,
        ColorInterpolation interpolation = ColorInterpolation.Constant)
    {
        return interpolation switch
        {
            ColorInterpolation.Constant => ConstantInterpolation(range, value, palette),
            ColorInterpolation.Linear => LinearInterpolation(range, value, palette),
            _ => throw new ArgumentOutOfRangeException(nameof(interpolation), interpolation, null)
        };
    }

    private static Color ConstantInterpolation(double[] range, double value, Palette palette)
    {
        int rangeNumber = 0; 

        for (int i = 0; i < range.Length - 1; i++)
        {
            if (!(value <= range[i]) || !(value >= range[i + 1])) continue;
            rangeNumber = i;
            break;
        }
        
        return palette[rangeNumber];
    }
    
    private static Color LinearInterpolation(double[] range, double value, Palette palette)
    {
        int rangeNumber = 0; 

        for (int i = 0; i < range.Length - 1; i++)
        {
            if (!(value <= range[i]) || !(value >= range[i + 1])) continue;
            rangeNumber = i;
            break;
        }

        var start = palette[rangeNumber];
        var end = rangeNumber == palette.ColorsCount - 1 ? palette[rangeNumber] : palette[rangeNumber + 1];

        var t = (value - range[rangeNumber + 1]) / (range[rangeNumber] - range[rangeNumber + 1]);
        var r = end.R + t * (start.R - end.R);
        var g = end.G + t * (start.G - end.G);
        var b = end.B + t * (start.B - end.B);
        return Color.FromRgb((byte)r, (byte)g, (byte)b);
    }
}