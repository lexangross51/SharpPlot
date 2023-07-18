using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using SharpPlot.Objects.Plots;
using SharpPlot.Scenes;
using SharpPlot.Viewport;
using System.Windows.Media;
using Point = System.Windows.Point;

namespace SharpPlot.Objects;

public enum GraphType
{
    G2D,
    G3D
}

public sealed class Plotter : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private int _rowsCount = 1, _columnsCount = 1;
    private double _allMargin = 20.0;
    public readonly List<Scene2D> Scenes2D;
    public double Width { get; init; }
    public double Height { get; init; }

    public int RowsCount
    {
        get => _rowsCount;
        set
        {
            _rowsCount = value;
            NeedToRebuild = true;
            OnPropertyChanged();
        }
    }
    public int ColumnsCount
    {
        get => _columnsCount;
        set
        {
            _columnsCount = value;
            NeedToRebuild = true;
            OnPropertyChanged();
        }
    }
    public double AllMargin
    {
        get => _allMargin;
        set
        {
            _allMargin = value;
            NeedToRebuild = true;
            OnPropertyChanged();
        }
    }
    public bool NeedToRebuild { get; private set; }
    public GraphType GraphType { get; set; } = GraphType.G2D;

    private Plotter()
    {
        Scenes2D = new List<Scene2D>();
    }

    public static Plotter Figure(double width = 1000.0, double height = 600.0)
    {
        var plotter = new Plotter
        {
            Width = width,
            Height = height
        };
        
        plotter.Make2DScenes();

        return plotter;
    }

    public void ResizeAll(double newWidth, double newHeight)
    {
        var controlWidth = (newWidth - 4.0 * AllMargin) / ColumnsCount;
        var controlHeight = (newHeight - 4.0 * AllMargin) / RowsCount;

        if (RowsCount > 1) controlHeight -= 20.0;
        if (ColumnsCount > 1) controlWidth -= 20.0;

        foreach (var scene in Scenes2D)
        {
            scene.OnChangeSize(new ScreenSize(controlWidth, controlHeight));
        }
    }
    
    public void Make2DScenes()
    {
        NeedToRebuild = false;
        Scenes2D.Clear();

        var controlWidth = (Width - 4.0 * AllMargin) / ColumnsCount;
        var controlHeight = (Height - 4.0 * AllMargin) / RowsCount;

        if (RowsCount > 1) controlHeight -= 20.0;
        if (ColumnsCount > 1) controlWidth -= 20.0;
        
        for (int row = 0; row < RowsCount; row++)
        {
            for (int column = 0; column < ColumnsCount; column++)
            {
                Scenes2D.Add(new Scene2D(controlWidth, controlHeight));
            }
        }
    }

    public void Subplots(int rowsCount, int columnsCount)
    {
        RowsCount = rowsCount;
        ColumnsCount = columnsCount;
        
        Make2DScenes();
    }
    
    public void Plot(IEnumerable<double> args, IEnumerable<double> values, Color color)
    {
        Scenes2D[0].ObjectsRenderer.AppendRenderable(new Plot(args, values, color));
    }

    public void Scatter(IEnumerable<double> args, IEnumerable<double> values, Color color, int size = 20)
    {
        Scenes2D[0].ObjectsRenderer.AppendRenderable(new Scatter(args, values, color, size));
    }

    public void ColorMesh(IEnumerable<Point> points, IEnumerable<double> values, PaletteType palette, 
        ColorInterpolation interpolation = ColorInterpolation.Constant)
    {
        Scenes2D[0].ObjectsRenderer.AppendRenderable(new ColorMesh(points, values, palette, interpolation));
    }

    public void ColorMesh(IEnumerable<Point> points, IEnumerable<double> values, IEnumerable<int> indices,
        PaletteType palette, ColorInterpolation interpolation = ColorInterpolation.Constant)
    {
        Scenes2D[0].ObjectsRenderer.AppendRenderable(new ColorMesh(points, indices, values, palette, interpolation));
    }
    
    public void Mesh(IEnumerable<Point> points)
    {
        Scenes2D[0].ObjectsRenderer.AppendRenderable(new Mesh(points));
    }
    
    public void Mesh(IEnumerable<Point> points, IEnumerable<int> indices)
    {
        Scenes2D[0].ObjectsRenderer.AppendRenderable(new Mesh(points, indices));
    }
    
    public Colorbar ContourF(IEnumerable<Point> points, IEnumerable<double> values, Palette palette, int levels)
    {
        var valuesArray = values as double[] ?? values.ToArray();
        Scenes2D[0].ObjectsRenderer.AppendRenderable(new ContourF(points, valuesArray, palette, levels));
        
        var maxValue = valuesArray.Max();
        var minValue = valuesArray.Min();
        var levelsStep = (maxValue - minValue) / (levels + 1);
        var valueStep = (maxValue - minValue) / palette.ColorsCount;
        var valuesByPalette = new double[palette.ColorsCount + 1];
        var valuesByIsolines = new double[levels + 2];
        
        for (int i = 0; i < levels + 2; i++)
        {
            valuesByIsolines[i] = minValue + i * levelsStep;
        }

        for (int i = 0; i < palette.ColorsCount + 1; i++)
        {
            valuesByPalette[i] = minValue + i * valueStep;
        }

        var newPalette = new Palette(levels + 1);
        for (int i = 0; i < levels + 1; i++)
        {
            var lower = minValue + i * levelsStep;
            var upper = minValue + (i + 1) * levelsStep;
            var interpolated = ColorInterpolator.InterpolateColor(valuesByPalette, (lower + upper) / 2.0, palette,
                ColorInterpolation.Linear);
            newPalette.AddColor(interpolated);
        }

        return new Colorbar(valuesArray, newPalette, ColorInterpolation.Constant);
    }
    
    public void Contour(IEnumerable<Point> points, IEnumerable<double> values, int levels)
    {
        Scenes2D[0].ObjectsRenderer.AppendRenderable(new Contour(points, values, levels));
    }

    public void Colorbar(Colorbar cb)
    {
        Scenes2D[0].ObjectsRenderer.AppendRenderable(cb);
    }
}