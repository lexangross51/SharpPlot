using System;
using OpenTK.Mathematics;

namespace SharpPlot.Core;

public enum ColorInterpolationType
{
    Nearest,
    Linear
}

public static class ColorInterpolator
{
    public static Color4 InterpolateColor(double[] range, double value, Palette.Palette palette,
        ColorInterpolationType interpolation = ColorInterpolationType.Linear)
    {
        return interpolation switch
        {
            ColorInterpolationType.Nearest => NearestInterpolation(range, value, palette),
            ColorInterpolationType.Linear => LinearInterpolation(range, value, palette),
            _ => throw new ArgumentOutOfRangeException(nameof(interpolation), interpolation, null)
        };
    }
    
    private static Color4 NearestInterpolation(double[] range, double value, Palette.Palette palette)
    {
        int rangeNumber = 0;

        for (int i = 0; i < range.Length - 1; i++)
        {
            if (value < range[i] || value > range[i + 1]) continue;
            rangeNumber = i;
            break;
        }
        
        return palette[rangeNumber];
    }
    
    private static Color4 LinearInterpolation(double[] range, double value, Palette.Palette palette)
    {
        int rangeNumber = 0; 

        for (int i = 0; i < range.Length - 1; i++)
        {
            if (value < range[i] || value > range[i + 1]) continue;
            rangeNumber = i;
            break;
        }

        var start = palette[rangeNumber];
        var end = rangeNumber == palette.ColorsCount - 1 ? palette[rangeNumber] : palette[rangeNumber + 1];

        var t = (value - range[rangeNumber]) / (range[rangeNumber + 1] - range[rangeNumber]);
        var r = start.R + t * (end.R - start.R);
        var g = start.G + t * (end.G - start.G);
        var b = start.B + t * (end.B - start.B);
        return new Color4((float)r, (float)g, (float)b, 1.0f);
    }
}