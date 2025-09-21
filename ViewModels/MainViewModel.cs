using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowManager.Services;

namespace WindowManager.ViewModels
{
    public class MainViewModel
    {
        private readonly string _programPath;
        private readonly ProgramService _programService;
        
        public MainViewModel(string programPath, ProgramService programService)
        {
            _programService = programService;
            _programPath = programPath;
        }

        public void OnStartProgramButtonClick()
        {
            _programService.OpenProgram(_programPath);
        }

        public Process[] OnListALLRunnigProgramsButtonClick()
        {
            var processes = _programService.ListRunningPrograms().ToArray();
            return processes;
        }
    }
}
