using System.Diagnostics;
using System.IO;
using WindowManager.Models;

namespace WindowManager.Services
{
    public class ProgramService
    {
        private readonly User32Service _user32Service;
        public ProgramService(User32Service user32Service)
        {
            _user32Service = user32Service;
        }

        public void StartOrOpenProgram(ProcessModel processModel)
        {
            if (processModel.ProcessId.HasValue)
            {
                OpenProgram(processModel.ProcessId.Value);
            }
            else if (!string.IsNullOrEmpty(processModel.Path))
            {
                StartProgram(processModel.Path);
            }
        }

        public void OpenProgram(int processId)
        {
            List<IntPtr> windows = _user32Service.GetTopLevelWindows(processId);

            if (windows.Count == 0)
                throw new Exception($"No windows found for program with id: {processId}");

            //TODO: rebuild this
            var index = windows.Count() > 1 ? 1 : 0; 
            IntPtr windowPtr = windows[index];

            _user32Service.RestoreWindowFromMinimized(windowPtr); 
            _user32Service.FocusWindow(windowPtr);
        }

        public void StartOrOpenProgram(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            Process? process= Process.GetProcessesByName(name).FirstOrDefault();

            if (process == null)
            {
                StartProgram(path);
                return; 
            }

            List<IntPtr> windows = _user32Service.GetTopLevelWindows(process.Id);

            //TODO: rebuild this
            var index = windows.Count() > 1 ? 1 : 0; 
            IntPtr windowPtr = windows[index];

            _user32Service.RestoreWindowFromMinimized(windowPtr); 
            _user32Service.FocusWindow(windowPtr);
        }

        public void StartProgram(string path)
        {
            Process.Start(path);
        }

        public List<Process> ListRunningPrograms()
        {
            var processes = Process.GetProcesses().ToList();
            var programs = processes
                .Where(process =>
                {
                    try
                    {
                        return process.MainWindowHandle != IntPtr.Zero && !string.IsNullOrWhiteSpace(process.MainWindowTitle);
                    }
                    catch
                    {
                        return false;
                    }
                }).ToList();
            return programs;
        }
    }
}
