using SharpPlot.Infrastructure.Interfaces;
using SharpPlot.MVVM;

namespace SharpPlot.ViewModels;

public class SettingsViewModel(IUserDialogService userDialogService) : NotifyObject
{
    public string Title => "Draw settings";
}