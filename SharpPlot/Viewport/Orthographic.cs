using System;
using System.Windows;

namespace SharpPlot.Viewport;

public class OrthographicProjection : IProjection
{
    private readonly bool _isEqualScale;
    private double _oldHorizontalCenter, _oldVerticalCenter;
    private double _oldWidth, _oldHeight;
    private double DHorizontal => Width / 2.0;
    private double DVertical => _isEqualScale ? DHorizontal * RationHeightToWidth : Height / 2.0;
    public double RationHeightToWidth { get; set; }
    public double HorizontalCenter { get; set; }
    public double VerticalCenter { get; set; }
    public double Scaling { get; set; }
    public double Width { get; private set; }
    public double Height { get; private set; }
    public double ZBuffer { get; set; }

    public OrthographicProjection(double[] orthographic, double ratio, bool isEqualScale)
    {
        SetProjection(orthographic);

        RationHeightToWidth = ratio;
        _isEqualScale = isEqualScale;
        Scaling = 1;
    }

    public void SetProjection(double[] projection)
    {
        HorizontalCenter = (projection[0] + projection[1]) / 2.0;
        VerticalCenter = (projection[2] + projection[3]) / 2.0;
        Width = projection[1] - projection[0];
        Height = projection[3] - projection[2];

        _oldHorizontalCenter = HorizontalCenter;
        _oldVerticalCenter = VerticalCenter;
        _oldWidth = Width;
        _oldHeight = Height;
    }

    public void GetProjection(out double[] projection)
    {
        projection = new[]
        {
            HorizontalCenter - DHorizontal,
            HorizontalCenter + DHorizontal,
            VerticalCenter - DVertical,
            VerticalCenter + DVertical,
            0.0, 0.0
        };
    }

    public Point FromProjectionToWorld(Point point, ScreenSize screenSize, Indent indent)
    {
        Point converted = new();

        if (point.X < indent.Horizontal)
        {
            converted.X = HorizontalCenter - DHorizontal;
        }
        else if (point.X > screenSize.Width + indent.Horizontal)
        {
            converted.X = HorizontalCenter + DHorizontal;
        }
        else
        {
            double coefficient = (point.X - indent.Horizontal) / screenSize.Width;
            converted.X = HorizontalCenter + (2 * coefficient - 1) * DHorizontal;
        }

        if (point.Y < 0.0)
        {
            converted.Y = VerticalCenter + DVertical;
        }
        else if (point.Y > screenSize.Height)
        {
            converted.Y = VerticalCenter - DVertical;
        }
        else
        {
            double coefficient = (screenSize.Height - point.Y) / screenSize.Height;
            converted.Y = VerticalCenter + (2 * coefficient - 1) * DVertical;
        }

        return converted;
    }

    public Point FromWorldToProjection(Point point, ScreenSize screenSize, Indent indent)
    {
        Point converted = new();

        double dx = point.X - (HorizontalCenter - DHorizontal);
        double dy = point.Y - (VerticalCenter - DVertical);

        double coefficient = dx / Width;
        converted.X = (coefficient * screenSize.Width + indent.Horizontal);

        coefficient = dy / Height;
        converted.Y = screenSize.Height - coefficient * screenSize.Height;

        return converted;
    }

    public void Scale(Point pivot, double delta)
    {
        double scale = delta < 1.05 ? 1.05 : 1.0 / 1.05;
        double left = pivot.X + scale * (HorizontalCenter - DHorizontal - pivot.X);
        double right = pivot.X + scale * (HorizontalCenter - DHorizontal + 2.0 * DHorizontal - pivot.X);
        double bottom = pivot.Y + scale * (VerticalCenter - DVertical - pivot.Y);
        double top = pivot.Y + scale * (VerticalCenter - DVertical + 2.0 * DVertical - pivot.Y);
        double newCenterX = (left + right) / 2.0;
        double newCenterY = (bottom + top) / 2.0;
        double newDHorizontal = newCenterX - left;
        double newDVertical = newCenterY - bottom;

        if (Math.Abs(2 * newDHorizontal) >
            Math.Max(Math.Abs(newCenterX - DHorizontal), Math.Abs(newCenterX + DHorizontal)) * 10E-5 &&
            Math.Abs(2 * newDVertical) >
            Math.Max(Math.Abs(newCenterY - DVertical), Math.Abs(newCenterY + DVertical)) * 10E-5)
        {
            HorizontalCenter = newCenterX;
            VerticalCenter = newCenterY;
            Width = 2.0 * newDHorizontal;
            Height = 2.0 * newDVertical;

            _oldHorizontalCenter = HorizontalCenter;
            _oldVerticalCenter = VerticalCenter;
            _oldWidth = Width;
            _oldHeight = Height;
        }
    }

    public void Translate(double h, double v)
    {
        HorizontalCenter += h;
        VerticalCenter += v;
    }

    public void Reset()
    {
        HorizontalCenter = _oldHorizontalCenter;
        VerticalCenter = _oldVerticalCenter;
        Width = _oldWidth;
        Height = _oldHeight;
    }
}