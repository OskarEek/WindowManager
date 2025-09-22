using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WindowManager.Services;
using WindowManager.ViewModels;
using WindowManager.Views;

namespace WindowManager;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        // Register services
        services.AddSingleton<FileService>();
        services.AddSingleton<ConfigService>(_ => new ConfigService(_serviceProvider.GetRequiredService<FileService>()));
        services.AddSingleton<User32Service>();
        services.AddSingleton<ProgramService>();
        services.AddSingleton<WindowService>();

        // Register viewmodels
        services.AddSingleton<MainViewModel>();
        services.AddTransient<SearchViewModel>();

        // Register views
        services.AddSingleton<MainWindow>();
        services.AddTransient<SearchWindow>();

        _serviceProvider = services.BuildServiceProvider();

        var window = _serviceProvider.GetRequiredService<MainWindow>();
        window.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}

