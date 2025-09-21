using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowManager.Services
{
    class WindowsService
    {
        const uint GW_OWNER = 4; //Window owner
        private const int SW_RESTORE = 9; //Restore if minimized

        public bool FocusWindow(IntPtr hWnd) =>
            SetForegroundWindow(hWnd);

        public List<IntPtr> GetWindowsForProcess(int processId)
        {
            var windowHandles = new List<IntPtr>();

            EnumWindows((hWnd, lParam) =>
            {
                GetWindowThreadProcessId(hWnd, out uint windowProcessId);
                if (windowProcessId == processId &&
                    IsWindowVisible(hWnd) &&
                    //If no owner, it means that this is a user windows
                    GetWindow(hWnd, GW_OWNER) == IntPtr.Zero)
                {
                    windowHandles.Add(hWnd);
                }
                return true;
            }, IntPtr.Zero);

            return windowHandles;
        }

        public void RestoreWindowFromMinimized(IntPtr hWnd) {
            if (IsIconic(hWnd))
            {
                ShowWindow(hWnd, SW_RESTORE);
            }
        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
