using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
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

    private static readonly string s_firefoxProgramPath = "C:\\Program Files\\Mozilla Firefox\\firefox.exe";

    private static readonly string s_forkProgramPath = "C:\\Users\\farss\\AppData\\Local\\Fork\\current\\Fork.exe";

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        // Register services
        services.AddSingleton<User32Service>();
        services.AddSingleton<ProgramService>();

        // Register viewmodels
        services.AddSingleton<MainViewModel>(sp => new MainViewModel(s_forkProgramPath, sp.GetRequiredService<ProgramService>()));

        // Register views
        services.AddSingleton<MainWindow>();

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

