using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using SharpPlot.Objects;

namespace SharpPlot;

public sealed partial class MainWindow
{
    public Plotter Plotter { get; }

    public MainWindow()
    {
        DataContext = this;
        InitializeComponent();

        Plotter = Plotter.Figure(Width - 25.0, Height - 25.0);
        PrepareMainForm();

        // var args = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };
        // var values = args.Select(arg => arg % 2 == 0 ? arg / 2.0 : arg + 1).ToArray();

        var points = new List<Point>();
        var values = new List<double>();
        var indices = new List<int>();
        ReadMeshValues("ViscosityField.txt", points, values);
        //ReadIndices("indices.txt", indices);
        
        // Plotter.Plot(args, values, Color.Black);
        // Plotter.Scatter(args, values, Color.Red);
        //Plotter.ColorMesh(points, values, PaletteType.Rainbow, ColorInterpolation.Linear);
        Plotter.ContourF(points, values, PaletteType.AutumnReverse, 20);
        //Plotter.Contour(points, values, 10);
        Plotter.Colorbar(new Colorbar(values, Palette.Create(PaletteType.AutumnReverse)));
    }

    private void ReadMeshValues(string filename, ICollection<Point> points, ICollection<double> values)
    {
        using var sr = new StreamReader(filename);

        while (sr.ReadLine() is { } line)
        {
            var nums = line.Split("\t");
            var x = double.Parse(nums[0], CultureInfo.InvariantCulture);
            var y = double.Parse(nums[1], CultureInfo.InvariantCulture);
            var v = double.Parse(nums[2], CultureInfo.InvariantCulture);
            
            points.Add(new Point(x, y));
            values.Add(v);
        }
    }

    private void ReadIndices(string filename, ICollection<int> indices)
    {
        using var sr = new StreamReader(filename);

        while (sr.ReadLine() is { } line)
        {
            var nums = line.Split(" ");

            foreach (var num in nums)
            {
                indices.Add(int.Parse(num));
            }
        }
    }
    
    private void PrepareMainForm()
    {
        MainGrid.RowDefinitions.Clear();
        MainGrid.ColumnDefinitions.Clear();
        MainGrid.Children.Clear();

        for (int i = 0; i < Plotter.RowsCount; i++)
        {
            MainGrid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Star)});
        }
        
        for (int i = 0; i < Plotter.ColumnsCount; i++)
        {
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)});
        }
        
        for (int row = 0, index = 0; row < Plotter.RowsCount; row++)
        {
            for (int column = 0; column < Plotter.ColumnsCount; column++)
            {
                Grid.SetRow(Plotter.Scenes2D[index], row);
                Grid.SetColumn(Plotter.Scenes2D[index], column);

                MainGrid.Children.Add(Plotter.Scenes2D[index]);
                index++;
            }
        }
    }
    
    private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Plotter.ResizeAll(e.NewSize.Width - 25.0, e.NewSize.Height - 25.0);
    }

    private void PlotterOnClick(object sender, RoutedEventArgs e)
    {
        PlotterSettings plotterSettings = new()
        {
            DataContext = this
        };
        plotterSettings.ShowDialog();
        
        if ((bool)plotterSettings.DialogResult!)
        {
            if (Plotter.NeedToRebuild)
            {
                Plotter.Make2DScenes();
                PrepareMainForm();
                InvalidateVisual();
            }
        }
    }
}