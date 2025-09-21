using System.Windows;
using System.Windows.Input;
using WindowManager.ViewModels;

namespace WindowManager.Views
{
    /// <summary>
    /// Interaction logic for SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        public SearchWindow(SearchViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;


            Loaded += (s, e) => SearchBox.Focus();

        }

        private void KeyDownEvent(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                bool selected = (DataContext as SearchViewModel).OpenSelectedProgram();
                if (selected)
                    Close();
            }
            else if (Keyboard.IsKeyDown(Key.Escape))
            {
                Close();
            }
            else if (Keyboard.IsKeyDown(Key.Up))
            {
                (DataContext as SearchViewModel).SelectPreviousProgram();
                e.Handled = true; //Prevent listbox from handling event
            }
            else if (Keyboard.IsKeyDown(Key.Down))
            {
                (DataContext as SearchViewModel).SelectNextProgram();
                e.Handled = true; //Prevent listbox from handling event
            }
        }
    }
        
}
