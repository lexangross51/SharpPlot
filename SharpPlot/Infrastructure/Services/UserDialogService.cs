using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SharpPlot.Infrastructure.Interfaces;
using SharpPlot.Views;

namespace SharpPlot.Infrastructure.Services;

public class UserDialogService(IServiceProvider services) : IUserDialogService
{
    private SettingsWindow? _settingsWindow;

    public void OpenSettingsWindow()
    {
        if (_settingsWindow is {} window)
        {
            window.Show();
            return;
        }

        _settingsWindow = services.GetRequiredService<SettingsWindow>();
        _settingsWindow.Closed += (_, _) => _settingsWindow = null;

        if (_settingsWindow.ShowDialog() == true)
        {
            MessageBox.Show("Settings saved!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}