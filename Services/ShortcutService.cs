using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Input;

namespace WindowManager.Services
{
    public class ShortcutService : IDisposable
    {
        private const int WM_HOTKEY = 0x0312;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;

        private const int HOTKEY_LISTENER_ID = 67; 

        private HwndSource? _src;
        private IntPtr _hwnd;

        public event Action? HotkeyPressed;

        public void Initialize(Window window)
        {
            _hwnd = new WindowInteropHelper(window).Handle;
            _src = HwndSource.FromHwnd(_hwnd);
            _src.AddHook(WndProc);
        }

        public bool RegisterTestHotkey()
        {

            // TODO: read from config to set actual shortcut
            uint mods = MOD_CONTROL | MOD_SHIFT;
            uint vk = (uint)KeyInterop.VirtualKeyFromKey(Key.Q);
            return RegisterHotKey(_hwnd, HOTKEY_LISTENER_ID, mods, vk);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_LISTENER_ID)
            {
                HotkeyPressed?.Invoke();
                handled = true;
            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            if (_hwnd != IntPtr.Zero)
                UnregisterHotKey(_hwnd, HOTKEY_LISTENER_ID);
            if (_src != null)
            {
                _src.RemoveHook(WndProc);
                _src = null;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
