using Microsoft.Extensions.DependencyInjection;
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
            var window = _sp.GetRequiredService<SearchWindow>();
            window.Show();
            window.Activate();
            window.Focus();
        }
    }
}
