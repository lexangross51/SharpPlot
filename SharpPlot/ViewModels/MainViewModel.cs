using System.Windows.Input;
using SharpPlot.Application;
using SharpPlot.Infrastructure.Interfaces;
using SharpPlot.Infrastructure.Services;
using SharpPlot.MVVM;
using SharpPlot.MVVM.Commands;

namespace SharpPlot.ViewModels;

public class MainViewModel(
    IUserDialogService userDialogService,
    View2DSettingsService settings) : NotifyObject
{
    public string Title => $"{ApplicationSettings.ApplicationTitle} v{ApplicationSettings.Version}";

    public View2DSettingsService Settings => settings;
    
    public ICommand OpenSettings { get; } =
        RelayCommand.Create(_ => userDialogService.OpenSettingsWindow());
}