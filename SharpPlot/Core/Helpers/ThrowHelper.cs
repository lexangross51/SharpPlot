using System;

namespace SharpPlot.Core.Helpers;

public static class ThrowHelper
{
    public static ArgumentNullException ThrowIfNull(object? obj, string? paramName)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(paramName);
        }

        return new ArgumentNullException(paramName);
    }
    
    public static Exception ThrowException(string message)
    {
        throw new Exception(message);
    }
}