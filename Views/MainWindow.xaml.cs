using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WindowManager.Models;
using WindowManager.Services;
using WindowManager.ViewModels;


namespace WindowManager.Views
{
    public partial class MainWindow : Window, IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int VK_LSHIFT = 0xA0;
        private const int VK_RSHIFT = 0xA1;

        private readonly WindowService _windowService;
        private readonly ConfigService _configService;
        private readonly ShortcutService _shortcutService; 
        private readonly ProgramService _programService;

        private bool _leftShiftDown = false;
        private bool _rightShiftDown = false;

        private bool _capturingShortcut = false;

        private TextBox? _shortcutTextBox;
        private ProcessModel? _shortcutProgram;
        private string _previousShortcut;


        private static bool IsModifierKey(Key k) =>
            k == Key.LeftCtrl || k == Key.RightCtrl ||
            k == Key.LeftShift || k == Key.RightShift ||
            k == Key.LeftAlt || k == Key.RightAlt;

        private IntPtr _hookID = IntPtr.Zero;
        private LowLevelKeyboardProc? _proc;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public MainWindow(MainViewModel viewModel, WindowService windowService, ConfigService configService, ProgramService programService)
        {
            InitializeComponent();
            _windowService = windowService;
            DataContext = viewModel;

            _shortcutService = new ShortcutService(configService, programService);

            Loaded += (s, e) =>
            {
                _proc = HookCallback;
                _hookID = SetHook(_proc);

                _shortcutService.Initialize(this, () => _capturingShortcut);
                _shortcutService.RegisterAllHotkeysFromConfig();

            };

            Closing += (s, e) =>
            {
                Application.Current.Shutdown();
            };
            _configService = configService;
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var currentProcess = Process.GetCurrentProcess())
            using (var currentModule = currentProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(currentModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (_capturingShortcut) 
                return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);  
            
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (wParam == WM_KEYDOWN)
                {
                    if (vkCode == VK_LSHIFT) _leftShiftDown = true;
                    if (vkCode == VK_RSHIFT) _rightShiftDown = true;

                    if (_leftShiftDown && _rightShiftDown)
                    {
                        OpenSearchWindow();
                    }
                }
                else if (wParam == WM_KEYUP)
                {
                    if (vkCode == VK_LSHIFT) _leftShiftDown = false;
                    if (vkCode == VK_RSHIFT) _rightShiftDown = false;
                }
            }

            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private void OpenSearchWindow()
        {

            _windowService.ShowSearchWindow();
        }

        private void AddProgramButton(object sender, RoutedEventArgs e)
        {
            // Trigger ViewModel method
            (DataContext as MainViewModel)?.AddProgram();
        }

        private void ExitProgramButton(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeProgramButton(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;    
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                try { DragMove(); } catch { }
            }
        }

        private void UpdateKeyboardShortcut(object sender, MouseButtonEventArgs e)
        {
            _capturingShortcut = true;
            _shortcutTextBox = (TextBox)sender;
            _shortcutProgram = _shortcutTextBox.DataContext as ProcessModel;
            _previousShortcut = _shortcutProgram.Shortcut;

        _shortcutTextBox.Text = "Press a Shortcut";
            this.Focus();
            PreviewKeyDown += OnNewShortcutKeyDown;

            _rightShiftDown = _leftShiftDown = false;
            e.Handled = true;
        }

        private void OnNewShortcutKeyDown(object sender, KeyEventArgs e)
        {
            var key = (e.Key == Key.System) ? e.SystemKey : e.Key;
            
            if (!_capturingShortcut)
                return;

            if (key == Key.Back)
            {
                _shortcutProgram.Shortcut = "";
                (DataContext as MainViewModel)?.SaveShortcut(_shortcutProgram);
                _shortcutTextBox?.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();

                PreviewKeyDown -= OnNewShortcutKeyDown;
                _capturingShortcut = false;
                _shortcutService.UpdateExistingShortcutListener(_shortcutProgram);

                e.Handled = true;
                return;
            }

            if (key == Key.Escape)
            {
                _shortcutTextBox.Text = _previousShortcut;
                _shortcutTextBox?.GetBindingExpression(TextBox.TextProperty).UpdateTarget();

                PreviewKeyDown -= OnNewShortcutKeyDown;
                _capturingShortcut = false;

                e.Handled = true;
                return;
            }

            if (IsModifierKey(key)) 
            {
                e.Handled = true;
                return;
            }

            var mods = Keyboard.Modifiers;
            var combo = new List<String>();
            if ((mods & ModifierKeys.Shift) != 0) combo.Add("Shift");
            if ((mods & ModifierKeys.Control) != 0) combo.Add("Ctrl");
            if ((mods & ModifierKeys.Alt) != 0) combo.Add("Alt");

            combo.Add(key.ToString());
            string shortcutLabel = string.Join("+", combo);

            _capturingShortcut = false;
            PreviewKeyDown -= OnNewShortcutKeyDown;

            if (_shortcutProgram != null)
                _shortcutProgram.Shortcut = shortcutLabel;
                _shortcutService.UpdateExistingShortcutListener(_shortcutProgram);

            (DataContext as MainViewModel)?.SaveShortcut(_shortcutProgram);

            _shortcutTextBox?.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();

        }

        public void Dispose()
        {
            _shortcutService.Dispose();
            if (_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
            }
            _proc = null;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

    }
}