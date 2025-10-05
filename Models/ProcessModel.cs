using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WindowManager.Models
{
    public class ProcessModel : INotifyPropertyChanged
    {
        public string Name { get; set; } = "";
        public int? ProcessId { get; set; }
        public string? Path { get; set; }

        private string? _shortcut;
        public string? Shortcut { 
            get => _shortcut;
            set 
            {
                if (_shortcut != value)
                {
                    _shortcut = value;
                    OnPropertyChanged();
                }
            } 
        }
        public bool IsValidProgramPath { get; set; }
        public string DisplayName => Name.ToLower();

        public event PropertyChangedEventHandler? PropertyChanged;
        
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
