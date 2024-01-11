using System;
using Microsoft.Extensions.DependencyInjection;
using SharpPlot.Infrastructure.Interfaces;
using SharpPlot.Views;

namespace SharpPlot.Infrastructure.Implementations;

public class UserDialogService(IServiceProvider services) : IUserDialogService
{
    private MainWindow? _mainWindow;
    private SettingsWindow? _settingsWindow;

    public void OpenMainWindow()
    {
        if (_mainWindow is {} window)
        {
            window.Show();
            return;
        }
        
        _mainWindow = services.GetRequiredService<MainWindow>();
        _mainWindow.Closed += (_, _) => _mainWindow = null;
        _mainWindow.Show();
    }

    public void OpenSettingsWindow()
    {
        if (_settingsWindow is {} window)
        {
            window.Show();
            return;
        }

        _settingsWindow = services.GetRequiredService<SettingsWindow>();
        _settingsWindow.Closed += (_, _) => _settingsWindow = null;
        _settingsWindow.Show();
    }
}