using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowManager.Models
{
    public class ProcessModel
    {
        public string Name { get; set; } = "";
        public int? ProcessId { get; set; }
        public string? Path { get; set; }
        public bool IsValidProgramPath { get; set; }
        public string DisplayName => Name.ToLower();
    }
}
