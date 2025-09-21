using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
            else if (Keyboard.IsKeyDown(Key.K)) //TODO: couldn't get this to work with Key.Up
            {
                (DataContext as SearchViewModel).SelectPreviousProgram();
            }
            else if (Keyboard.IsKeyDown(Key.J)) //TODO: couldn't get this to work with Key.Down
            {
                (DataContext as SearchViewModel).SelectNextProgram();
            }
        }
    }
}
