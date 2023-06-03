using System.Drawing;
using System.Linq;
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

        var args = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };
        var values = args.Select(arg => arg % 2 == 0 ? arg / 2.0 : arg + 1).ToArray();
        
        Plotter.Plot(args, values, Color.Black);
        Plotter.Scatter(args, values, Color.Red);
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
        PlotterSettings plotterSettings = new();
        plotterSettings.ShowDialog();
        
        if ((bool)plotterSettings.DialogResult!)
        {
            if (Plotter.NeedToRebuild)
            {
                Plotter.Make2DScenes();
                PrepareMainForm();
            }
        }
    }
}