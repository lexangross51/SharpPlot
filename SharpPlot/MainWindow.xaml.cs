using System.Collections.Generic;
using System.Windows;
using SharpPlot.GraphicControl;

namespace SharpPlot;

public partial class MainWindow
{
    private readonly List<GlControl> _controls;
    
    public MainWindow()
    {
        InitializeComponent();

        _controls = new List<GlControl>();
         var control = new GlControl(600, 300)
         {
             VerticalAlignment = VerticalAlignment.Top,
             HorizontalAlignment = HorizontalAlignment.Left,
         };
         MainGrid.Children.Add(control);
         _controls.Add(control);
        
        var control1 = new GlControl(600, 300)
        {
            VerticalAlignment = VerticalAlignment.Bottom,
            HorizontalAlignment = HorizontalAlignment.Right,
        };
        MainGrid.Children.Add(control1);
        _controls.Add(control1);
        
        var control2 = new GlControl(600, 300)
        {
            VerticalAlignment = VerticalAlignment.Bottom,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        MainGrid.Children.Add(control2);
        _controls.Add(control2);
        
        var control3 = new GlControl(600, 300)
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Right,
        };
        MainGrid.Children.Add(control3);
        _controls.Add(control3);
    }
    
    private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Width = e.NewSize.Width;
        Height = e.NewSize.Height;
        
        // foreach (var control in _controls)
        // {
        //     control.InvalidateVisual();
        // }
    }
}