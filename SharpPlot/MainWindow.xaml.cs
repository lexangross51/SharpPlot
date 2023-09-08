using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SharpPlot.Scenes;
using SharpPlot.Viewport;

namespace SharpPlot;

public sealed partial class MainWindow
{
    private readonly int _rowsCount;
    private readonly int _columnsCount;
    private readonly double _allMargin = 22.0;
    private readonly List<Scene2D> _scenes2D;
    private readonly Scene3D _scene3D;
    
    public MainWindow()
    {
        InitializeComponent();

        _rowsCount = 1;
        _columnsCount = 1;

        _scenes2D = new List<Scene2D>();
        _scene3D = new Scene3D(Width - 20, Height - 30);
        
        PrepareMainForm();
    }
    
    private void PrepareMainForm()
    {
        MainGrid2D.RowDefinitions.Clear();
        MainGrid2D.ColumnDefinitions.Clear();
        MainGrid2D.Children.Clear();

        for (int i = 0; i < _rowsCount; i++)
        {
            MainGrid2D.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Star)});
        }
        
        for (int i = 0; i < _columnsCount; i++)
        {
            MainGrid2D.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)});
        }

        _scenes2D.Clear();

        var controlWidth = (Width - 4.0 * _allMargin) / _columnsCount;
        var controlHeight = (Height - 4.0 * _allMargin) / _rowsCount + 3;

        if (_rowsCount > 1) controlHeight -= 20.0;
        if (_columnsCount > 1) controlWidth -= 20.0;
        
        for (int row = 0; row < _rowsCount; row++)
        {
            for (int column = 0; column < _columnsCount; column++)
            {
                _scenes2D.Add(new Scene2D(controlWidth, controlHeight));
            }
        }
        
        for (int row = 0, index = 0; row < _rowsCount; row++)
        {
            for (int column = 0; column < _columnsCount; column++)
            {
                Grid.SetRow(_scenes2D[index], row);
                Grid.SetColumn(_scenes2D[index], column);

                MainGrid2D.Children.Add(_scenes2D[index]);
                index++;
            }
        }


        MainGrid3D.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        MainGrid3D.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        Grid.SetRow(_scene3D, 0);
        Grid.SetColumn(_scene3D, 0);
        MainGrid3D.Children.Add(_scene3D);
    }

    private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var controlWidth = (e.NewSize.Width - 4.0 * _allMargin) / _columnsCount;
        var controlHeight = (e.NewSize.Height - 4.0 * _allMargin) / _rowsCount + 3;

        if (_rowsCount > 1) controlHeight -= 20.0;
        if (_columnsCount > 1) controlWidth -= 20.0;
        
        foreach (var scene in _scenes2D)
        {
            scene.OnChangeSize(new ScreenSize { Width = controlWidth, Height = controlHeight });
        }

        _scene3D.OnChangeSize(new ScreenSize { Width = e.NewSize.Width - 40, Height = e.NewSize.Height - 85 });
    }
}