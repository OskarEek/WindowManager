using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WindowManager.Services
{
    public class FileService
    {
        public void CreateFileIfNotExist(string filePath)
        {
            if (!File.Exists(filePath))
                File.WriteAllText(filePath, string.Empty);
        }

        public T? GetJsonFileData<T>(string filePath)
        {
            try
            {
                using (var r = new StreamReader(filePath))
                {
                    string json = r.ReadToEnd();
                    return JsonSerializer.Deserialize<T>(json);
                }
            }
            catch (JsonException)
            {
                return default;
            }
            catch (FileNotFoundException)
            {
                return default;
            }
        }

        public void WriteToJsonFile<T>(string filePath, T data)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            var serializedData = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filePath, serializedData, Encoding.UTF8); //TODO: Is there some better way to do this? Is the current file deleted and created again every time this runs?
        }
    }
}
