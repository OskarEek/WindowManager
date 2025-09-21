using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using WindowManager.Services;

namespace WindowManager.ViewModels
{
    public class SearchViewModel : INotifyPropertyChanged
    {
        private readonly ProgramService _programService;
        private Process? _selectedProgram;
        private string? _searchText;
        private List<Process> _allPrograms;
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
        public string? SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    FilterPrograms();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchText)));
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public SearchViewModel(ProgramService programService)
        {
            _programService = programService;
            _allPrograms = new List<Process>();
            Programs = new ObservableCollection<Process>();
            LoadPrograms();
            SelectFirstProgram();
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

        private void FilterPrograms()
        {
            if (string.IsNullOrEmpty(_searchText))
            {
                RefreshPrograms(_allPrograms);
                return;
            }
            List<Process> programs = Programs.Where(x => x.ProcessName.ToLower().StartsWith(_searchText)).ToList();
            RefreshPrograms(programs);
            SelectFirstProgram();
        }

        private void LoadPrograms()
        {
            Programs.Clear();
            List<Process>? result = _programService.ListRunningPrograms();
            _allPrograms = result ?? new();
            RefreshPrograms(_allPrograms);
        }

        private void RefreshPrograms(List<Process> processes)
        {
            Programs.Clear();
            foreach (var company in processes)
            {
                Programs.Add(company);
            }
        }

        private void SelectFirstProgram()
        {
            if (Programs.Count() > 0)
                SelectedProgram = Programs.First();
        }

    }
}
