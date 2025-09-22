using Microsoft.Extensions.DependencyInjection;
using System.Windows;
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
                existing.Focus();
                return;
            }
            var window = _sp.GetRequiredService<SearchWindow>();
            window.Show();
            window.Activate();
            window.Focus();
        }
    }
}
