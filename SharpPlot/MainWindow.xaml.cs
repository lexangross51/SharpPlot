using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SharpPlot.GraphicControl;
using SharpPlot.Viewport;

namespace SharpPlot;

public partial class MainWindow
{
    private readonly List<GlControl> _controls;

    public static double FigureWidth { get; set; } = 1000;
    public static double FigureHeight { get; set; } = 600;
    public static int RowsCount { get; set; } = 1;
    public static int ColumnsCount { get; set; } = 1;
    
    public MainWindow()
    {
        InitializeComponent();

        Width = FigureWidth;
        Height = FigureHeight;

        _controls = new List<GlControl>();

        var controlWidth = Width / ColumnsCount - 4 * 20;
        var controlHeight = Height / RowsCount - 4 * 20;

        for (int i = 0; i < RowsCount; i++)
        {
            MainGrid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Star)});
        }
        
        for (int i = 0; i < ColumnsCount; i++)
        {
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)});
        }
        
        for (int row = 0; row < RowsCount; row++)
        {
            for (int column = 0; column < ColumnsCount; column++)
            {
                var control = new GlControl(controlWidth, controlHeight);

                Grid.SetRow(control, row);
                Grid.SetColumn(control, column);

                MainGrid.Children.Add(control);
                _controls.Add(control);
            }
        }
    }
    
    private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var controlWidth = e.NewSize.Width / ColumnsCount - 4 * 20;
        var controlHeight = e.NewSize.Height / RowsCount - 4 * 20;
        
        foreach (var control in _controls)
        {
            control.OnChangeSize(new ScreenSize(controlWidth, controlHeight));
        }
    }
}