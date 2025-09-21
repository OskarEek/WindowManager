using System.Diagnostics;
using System.IO;

namespace WindowManager.Services
{
    class ProgramService
    {
        private readonly WindowsService _windowsService;
        public ProgramService()
        {
            _windowsService = new WindowsService(); //TODO: Rebuild this with dependecy injection
        }

        public void OpenProgram(string path, bool newWindow = false)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            var process= Process.GetProcessesByName(name).FirstOrDefault();

            if (process == null || newWindow)
            {
                StartProgram(path);
                return; 
            }

            List<IntPtr> windows = _windowsService.GetProcessWindows(process.Id);

            //TODO: rebuild this
            var index = windows.Count() > 1 ? 1 : 0; 
            IntPtr windowPtr = windows[index];

            _windowsService.RestoreWindowFromMinimized(windowPtr); 
            _windowsService.FocusWindow(windowPtr);
        }

        private void StartProgram(string path)
        {
            Process.Start(path);
        }
    }
}
