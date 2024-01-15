using System.Windows.Input;
using SharpPlot.Application;
using SharpPlot.Infrastructure.Interfaces;
using SharpPlot.MVVM;
using SharpPlot.MVVM.Commands;

namespace SharpPlot.ViewModels;

public class MainViewModel(
    IUserDialogService userDialogService) : NotifyObject
{
    public string Title => $"{ApplicationSettings.ApplicationTitle} v{ApplicationSettings.Version}";

    public ICommand OpenSettings { get; } =
        RelayCommand.Create(_ => userDialogService.OpenSettingsWindow());
}