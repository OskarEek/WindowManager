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

namespace WindowManager.Views
{
    /// <summary>
    /// Interaction logic for SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        private readonly Process[] _programs;
        public Process? SelectedProcess { get; private set; }

        public SearchWindow(Process[] programs)
        {
            InitializeComponent();
            _programs = programs;
            ResultsList.ItemsSource = _programs;

            Loaded += (s, e) => SearchBox.Focus();

        }
    }
}
