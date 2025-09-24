using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowManager.Models;

namespace WindowManager.Services
{
    public class ConfigService
    {
        private static readonly string s_configFilePath = "config.json";
        private readonly FileService _fileService;
        private Config _config;

        public ConfigService(FileService fileService)
        {
            _fileService = fileService;
            _fileService.CreateFileIfNotExist(s_configFilePath);
            _config = _fileService.GetJsonFileData<Config>(s_configFilePath) ?? new();

            ValidateProgramPaths();
        }

        public Config GetConfig()
        {
            return _config;
        }

        public void AddProgram(string exePath)
        {
            if (!File.Exists(exePath))
                throw new Exception("Tried adding non existing program to config");

            if (!exePath.EndsWith(".exe"))
                throw new Exception("Tried adding a non .exe path to config");


            var name = Path.GetFileNameWithoutExtension(exePath);

            if (_config.Programs.Any(x => x.Path == exePath))
                return;

            var program = new ProcessModel()
            {
                Name = name,
                Path = exePath,
                IsValidProgramPath = true
            };

            _config.Programs.Add(program);
            ValidateProgramPaths();
            _fileService.WriteToJsonFile<Config>(s_configFilePath, _config);
        }

        public void DeleteProgram(ProcessModel processModel)
        {
            if (!_config.Programs.Contains(processModel))
                throw new Exception("Tried deleting non-existing program from config");

            _config.Programs.Remove(processModel);
            ValidateProgramPaths();
            _fileService.WriteToJsonFile(s_configFilePath, _config);
        }

        public void ValidateProgramPaths()
        {
            foreach (ProcessModel program in _config.Programs)
            {
                program.IsValidProgramPath = program.Path == null ? false : Path.Exists(program.Path);
            }
        }
    }
}
