using System.Windows;

namespace SharpPlot.Objects;

public partial class PlotterSettings
{
    public PlotterSettings()
    {
        InitializeComponent();
    }

    private void OkClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}