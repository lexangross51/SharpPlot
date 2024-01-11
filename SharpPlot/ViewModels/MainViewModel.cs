using System.Reactive;
using ReactiveUI;
using SharpPlot.Application;
using SharpPlot.Infrastructure.Interfaces;

namespace SharpPlot.ViewModels;

public class MainViewModel(IUserDialogService userDialogService) : ReactiveObject
{
    public string Title => $"{ApplicationSettings.ApplicationTitle} v{ApplicationSettings.Version}";

    public ReactiveCommand<Unit, Unit> OpenSettings { get; } = 
        ReactiveCommand.Create(userDialogService.OpenSettingsWindow);
}