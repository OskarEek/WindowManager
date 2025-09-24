using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using WindowManager.Models;
using WindowManager.Services;

namespace WindowManager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ConfigService _configService;
        private string? _programPath;
        private string? _errorMessage;
        private ProcessModel? _selectedProgram;
        public ObservableCollection<ProcessModel> StoredPrograms { get; }
        public string? ProgramPath
        {
            get => _programPath;
            set
            {
                if (_programPath != value)
                {
                    _programPath = CleanProgramPathInput(value);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProgramPath)));
                }
            }
        }
        public string? ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
                }
            }
        }

        public ProcessModel? SelectedProgram
        {
            get => _selectedProgram;
            set
            {
                if (_selectedProgram != value)
                {
                    _selectedProgram = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProcessModel)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public MainViewModel(ConfigService configSerivce)
        {
            _configService = configSerivce;
            StoredPrograms = new ObservableCollection<ProcessModel>();
            RefreshStoredPrograms();
        }

        public void AddProgram()
        {
            if (string.IsNullOrEmpty(_programPath))
                return;

            //TODO: these checks is both in _configService.AddProgram, solve this in some better way
            if (!Path.Exists(_programPath))
            {
                SetErrorMessage("This path does not exist");
                return;
            }

            if (!_programPath.EndsWith(".exe"))
            {
                SetErrorMessage("You need to enter path to a .exe file");
                return;
            }

            SetErrorMessage(string.Empty);

            _configService.AddProgram(_programPath);

            RefreshStoredPrograms();
            ProgramPath = "";
        }

        public void DeleteSelectedProgram()
        {
            if (_selectedProgram == null)
                return;

            _configService.DeleteProgram(_selectedProgram);
            _selectedProgram = null;
            RefreshStoredPrograms();
        }

        private void RefreshStoredPrograms()
        {
            Config config = _configService.GetConfig();
            StoredPrograms.Clear();
            foreach (ProcessModel program in config.Programs)
            {
                StoredPrograms.Add(program);
            }
        }

        private void SetErrorMessage(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        private string? CleanProgramPathInput(string? pathValue)
        {
            if (!string.IsNullOrEmpty(pathValue) && pathValue.StartsWith("\"") && pathValue.EndsWith("\""))
                return pathValue.Trim('"');

            return pathValue;
        }
    }
}
