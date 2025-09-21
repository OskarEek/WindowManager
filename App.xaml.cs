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
    private static readonly string s_programPath = "C:\\Program Files\\Mozilla Firefox\\firefox.exe";
    private ProgramService? _programService;
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _programService = new ProgramService();
        var window = new MainWindow()
        {
            DataContext = new MainViewModel(s_programPath, _programService)
        };
        window.Show();
    }
}

