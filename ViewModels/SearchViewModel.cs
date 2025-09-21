using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowManager.Services;

namespace WindowManager.ViewModels
{
    public class SearchViewModel : INotifyPropertyChanged
    {
        private readonly ProgramService _programService;

        public ObservableCollection<Process> Programs { get; }
        public Process? SelectedProcess { get; private set; }
        public event PropertyChangedEventHandler? PropertyChanged;

        public SearchViewModel(ProgramService programService)
        {
            _programService = programService;
            Programs = new ObservableCollection<Process>();
            LoadPrograms();
        }

        private void LoadPrograms()
        {
            Programs.Clear();
            List<Process>? result = _programService.ListRunningPrograms();
            foreach (var company in result)
            {
                Programs.Add(company);
            }
        }

    }
}
