using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowManager.ViewModels;

namespace WindowManager.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void StartProgramButton(object sender, RoutedEventArgs e)
    {
        (DataContext as MainViewModel)?.OnStartProgramButtonClick(0);
    }

    private void ListALLRunnigProgramsButton(object sender, RoutedEventArgs e)
    {
        var programs = (DataContext as MainViewModel)?.OnListALLRunnigProgramsButtonClick()
                ?? new Process[0];

        ProgramsList.Items.Clear();
        foreach (var process in programs)
        {
            ProgramsList.Items.Add(process.ProcessName);
        }
    }
    private void WindowKeyDown(object sender, KeyEventArgs e)
    {
        if (Keyboard.IsKeyDown(Key.LeftShift) && Keyboard.IsKeyDown(Key.RightShift))
        {
            var programs = (DataContext as MainViewModel)?.OnListALLRunnigProgramsButtonClick()
                ?? new Process[0];


            if (programs.Length > 0)
            {
                var searchWindow = new SearchWindow(programs);

                searchWindow.Show();
            }
            // TODO : Open a window enter what program to open
            
        }
    }
}