using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using WindowManager.Models;
using WindowManager.Views;

namespace WindowManager.Services
{
    public class ShortcutService : IDisposable
    {
        private readonly ConfigService _configService;
        private readonly ProgramService _programService;

        private const int WM_HOTKEY = 0x0312;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;

        private int _hotKeyListenerID = 1;
        private readonly Dictionary<int, ProcessModel> _shortcutActions = new();
        private readonly Dictionary<ProcessModel, int> _reverseShortcutActions = new();


        private HwndSource? _src;
        private IntPtr _hwnd;

        public event Action? HotkeyPressed;

        private Func<bool> _isCapturingNewShortcut;

        public ShortcutService(ConfigService configService, ProgramService programService)
        {
            _configService = configService;
            _programService = programService;
        }
        public void Initialize(MainWindow window, Func<bool> isCapturingNewShortcutProvider)
        {

            _isCapturingNewShortcut = isCapturingNewShortcutProvider;
            _hwnd = new WindowInteropHelper(window).Handle;
            _src = HwndSource.FromHwnd(_hwnd);
            _src.AddHook(WndProc);

        }

        public bool RegisterTestHotkey()
        {

            // TODO: read from config to set actual shortcut
            uint mods = MOD_CONTROL | MOD_SHIFT;
            uint vk = (uint)KeyInterop.VirtualKeyFromKey(Key.Q);
            return RegisterHotKey(_hwnd, _hotKeyListenerID, mods, vk);
        }

       public void RegisterAllHotkeysFromConfig()
        {
            var config = _configService.GetConfig();
            foreach (var program in config.Programs)
            {
                RegisterHotkeyFromProgram(program);
            }
        }

        private void RegisterHotkeyFromProgram(ProcessModel program)
        {
            if (program.Shortcut != "" && program.Shortcut != null)
            {
                var shortcutList = SplitShortcut(program.Shortcut);
                uint mods = GetModifiers(shortcutList);
                uint shortcutKey = GetShortcutKey(shortcutList.Last());

                RegisterHotKey(_hwnd, _hotKeyListenerID, mods, shortcutKey);
                _shortcutActions.Add(_hotKeyListenerID, program);
                _reverseShortcutActions.Add(program, _hotKeyListenerID);
                _hotKeyListenerID++;
            }
        }

        private List<string> SplitShortcut(string shortcutText)
        {
            var shortcutList = shortcutText.Split("+").ToList();
            return shortcutList;
        }

        private uint GetModifiers(List<string> shortcutList)
        {
            uint mods = 0;

            foreach (var part in shortcutList)
            {
                switch (part.ToLowerInvariant())
                {
                    case "ctrl": mods |= MOD_CONTROL; break;
                    case "shift": mods |= MOD_SHIFT; break;
                }
            }
            return mods;
        }

        private uint GetShortcutKey(string shortcutKeyString)
        {
            uint vk;

            if (shortcutKeyString.StartsWith("D") && shortcutKeyString.Length == 2 && char.IsDigit(shortcutKeyString[1]))
            {
                vk = (uint)shortcutKeyString[1];
            } else if (int.TryParse(shortcutKeyString, out int d))
            {
                vk = (uint)(0x30 + d);
            } else if (Enum.TryParse<Key>(shortcutKeyString, true, out var key))
            {
                vk = (uint)KeyInterop.VirtualKeyFromKey(key);
            }else
            {
                vk = 0;
            }

                return vk;
        }

        public void RemoveProgramListener(ProcessModel program)
        {
            Debug.WriteLine("Trying to remove listener");
            Debug.WriteLine(_reverseShortcutActions);
            foreach (var key in _reverseShortcutActions.Keys)
            {
                Debug.WriteLine(key.Name);
            }
            if (_reverseShortcutActions.TryGetValue(program, out var ListenerID))
            {
                UnregisterHotKey(_hwnd, ListenerID);
                _reverseShortcutActions.Remove(program);
            }
        }

        public void UpdateExistingShortcutListener(ProcessModel program)
        {
            RemoveProgramListener(program);

            if (program.Shortcut != "")
            {
                RegisterHotkeyFromProgram(program);
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                if (_isCapturingNewShortcut())
                {
                    return IntPtr.Zero;
                }

                var id = wParam.ToInt32();
                
                if (_shortcutActions.ContainsKey(id))
                {
                    ProcessModel programToOpen = _shortcutActions[id];
                    _programService.StartOrOpenProgram(programToOpen);
                    handled = true;
                }

            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            if (_hwnd != IntPtr.Zero)
                foreach (var listener in _shortcutActions.Keys)
                {

                    UnregisterHotKey(_hwnd, listener);
                }
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
