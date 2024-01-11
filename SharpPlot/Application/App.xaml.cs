using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SharpPlot.Infrastructure.Implementations;
using SharpPlot.Infrastructure.Interfaces;
using SharpPlot.ViewModels;
using SharpPlot.Views;

namespace SharpPlot.Application;

public partial class App
{
    private IServiceProvider? _services;
    
    public IServiceProvider Services 
        => _services ??= InitializeServices().BuildServiceProvider();
    
    public App()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
    }
    
    private static IServiceCollection InitializeServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IUserDialogService, UserDialogService>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<SettingsViewModel>();

        services.AddSingleton(s =>
        {
            var viewModel = s.GetRequiredService<MainViewModel>();
            var window = new MainWindow { DataContext = viewModel };

            return window;
        });
        
        services.AddTransient(s =>
        {
            var viewModel = s.GetRequiredService<SettingsViewModel>();
            var window = new SettingsWindow { DataContext = viewModel };

            return window;
        });

        return services;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Services.GetRequiredService<IUserDialogService>().OpenMainWindow();
    }
}