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

        public void OpenProgram(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            var process= Process.GetProcessesByName(name).FirstOrDefault();

            if (process == null)
            {
                StartProgram(path);
                return; 
            }

            //Focus existing window
            List<nint> windows = _windowsService.GetWindowsForProcess(process.Id);
            var index = windows.Count() > 1 ? 1 : 0; //TODO: rebuild this
            _windowsService.FocusWindow(windows[index]);
        }

        private void StartProgram(string path)
        {
            Process.Start(path);
        }
    }
}
