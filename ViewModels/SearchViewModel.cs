using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using WindowManager.Models;
using WindowManager.Services;

namespace WindowManager.ViewModels
{
    public class SearchViewModel : INotifyPropertyChanged
    {
        private readonly ProgramService _programService;
        private ProcessModel? _selectedProgram;
        private string? _searchText;
        private List<ProcessModel> _allPrograms;
        public ObservableCollection<ProcessModel> Programs { get; }
        public ProcessModel? SelectedProgram
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
            _allPrograms = new List<ProcessModel>();
            Programs = new ObservableCollection<ProcessModel>();
            LoadPrograms();
            SelectFirstProgram();
        }

        public bool OpenSelectedProgram()
        {
            if (_selectedProgram == null)
                return false;
            _programService.StartOrOpenProgram(_selectedProgram);
            return true;
        }

        public void SelectPreviousProgram()
        {
            if (_selectedProgram == null)
                return;

            int index = Programs.IndexOf(_selectedProgram);

            if (index <= 0)
                return;

            SelectedProgram = Programs[index - 1];
        }

        public void SelectNextProgram()
        {
            if (_selectedProgram == null)
                return;

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

            //Filter
            //TODO: if _searchText contains more characters than previous _searchText, only filter on Programs.Where instad of _allPrograms.Where to increase seach speed
            List<ProcessModel> programs = _allPrograms.Where(x => x.DisplayName.StartsWith(_searchText)).ToList();

            RefreshPrograms(programs);
            SelectFirstProgram();
        }

        private void LoadPrograms()
        {
            Programs.Clear();

            //TODO: Get all programs from config
            //TODO: Filter programs from config based on name
            var path = "C:\\Program Files\\Mozilla Firefox\\firefox.exe";
            var name = Path.GetFileNameWithoutExtension(path);
            var test = new ProcessModel { Name = name, Path = path };
            var configPrograms = new List<ProcessModel>() { test };

            //Running programs
            List<Process>? result = _programService.ListRunningPrograms();
            List<ProcessModel> runningPrograms = result.Select(x => 
                new ProcessModel() {
                    Name = x.ProcessName,
                    ProcessId = x.Id
                }
            ).ToList() ??
            new();

            _allPrograms = configPrograms.Where(p => !runningPrograms.Any(rP => rP.Name == p.Name)).ToList(); 
            _allPrograms.AddRange(runningPrograms);

            RefreshPrograms(_allPrograms);
        }

        private void RefreshPrograms(List<ProcessModel> processes)
        {
            Programs.Clear();
            foreach (var process in processes)
            {
                Programs.Add(process);
            }
        }

        private void SelectFirstProgram()
        {
            if (Programs.Count() > 0)
                SelectedProgram = Programs.First();
        }

    }
}
