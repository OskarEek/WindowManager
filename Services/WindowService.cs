using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Input;
using WindowManager.Views;

namespace WindowManager.Services
{
    public class WindowService
    {
        private readonly IServiceProvider _sp;

        public WindowService(IServiceProvider sp)
        {
            _sp = sp;
        }

        public void ShowSearchWindow()
        {
            var existing = Application.Current.Windows.OfType<SearchWindow>().FirstOrDefault();
            if (existing != null)
            {
                if (existing.WindowState == WindowState.Minimized)
                {
                    existing.WindowState = WindowState.Normal;
                }
                existing.Topmost = true;
                existing.Activate();
                Keyboard.Focus(existing.SearchBox);
                return;
            }

            var window = _sp.GetRequiredService<SearchWindow>();
            window.Show();
            window.Dispatcher.BeginInvoke(new Action(() =>
            {
                window.Topmost = true;
                window.Activate();
                Keyboard.Focus(window.SearchBox);
            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        }
    }
}
