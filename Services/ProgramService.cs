using System.Diagnostics;
using System.IO;

namespace WindowManager.Services
{
    public class ProgramService
    {
        private readonly User32Service _user32Service;
        public ProgramService(User32Service user32Service)
        {
            _user32Service = user32Service;
        }

        public void OpenProgram(int processId)
        {
            List<IntPtr> windows = _user32Service.GetProcessWindows(processId);

            if (windows.Count == 0)
                throw new Exception($"No windows found for program with id: {processId}");

            //TODO: rebuild this
            var index = windows.Count() > 1 ? 1 : 0; 
            IntPtr windowPtr = windows[index];

            _user32Service.RestoreWindowFromMinimized(windowPtr); 
            _user32Service.FocusWindow(windowPtr);
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

            List<IntPtr> windows = _user32Service.GetProcessWindows(process.Id);

            //TODO: rebuild this
            var index = windows.Count() > 1 ? 1 : 0; 
            IntPtr windowPtr = windows[index];

            _user32Service.RestoreWindowFromMinimized(windowPtr); 
            _user32Service.FocusWindow(windowPtr);
        }

        private void StartProgram(string path)
        {
            Process.Start(path);
        }
    }
}
