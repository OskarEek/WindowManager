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
        private Process? _selectedProgram;
        public ObservableCollection<Process> Programs { get; }
        public Process? SelectedProgram
        {
            get => _selectedProgram;
            set
            {
                if (_selectedProgram != value)
                {
                    _selectedProgram = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedProgram)));
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public SearchViewModel(ProgramService programService)
        {
            _programService = programService;
            Programs = new ObservableCollection<Process>();
            LoadPrograms();
            if (Programs.Count() > 0)
                SelectedProgram = Programs.First();
        }

        public bool OpenSelectedProgram()
        {
            if (_selectedProgram == null)
                return false;
            _programService.OpenProgram(_selectedProgram.Id);
            return true;
        }
        public void SelectPreviousProgram()
        {
            int index = Programs.IndexOf(_selectedProgram);

            if (index <= 0)
                return;

            SelectedProgram = Programs[index - 1];
        }
        public void SelectNextProgram()
        {
            int index = Programs.IndexOf(_selectedProgram);

            if (index >= Programs.Count() - 1)
                return;

            SelectedProgram = Programs[index + 1];
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
