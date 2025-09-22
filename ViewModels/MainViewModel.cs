using System.Collections.ObjectModel;
using System.ComponentModel;
using WindowManager.Models;
using WindowManager.Services;

namespace WindowManager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ConfigService _configService;
        private string? _programPath;
        public ObservableCollection<ProcessModel> StoredPrograms { get; }
        public string? ProgramPath
        {
            get => _programPath;
            set
            {
                if (_programPath != value)
                {
                    _programPath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProgramPath)));
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

        public void AddProgramButton()
        {
            if (string.IsNullOrEmpty(_programPath))
                return;

            _configService.AddProgram(_programPath);

            RefreshStoredPrograms();
            ProgramPath = "";
        }

        public void RefreshStoredPrograms()
        {
            Config config = _configService.GetConfig();
            StoredPrograms.Clear();
            foreach (ProcessModel program in config.Programs)
            {
                StoredPrograms.Add(program);
            }
        }
    }
}
