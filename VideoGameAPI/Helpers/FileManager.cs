using System.IO;
using System.Globalization;
using CsvHelper;

namespace VideoGameAPI.Helpers
{
    // Exists to wrap file handling stuff (StreamReader, CsvHelper, etc.) for unit testing 
    public class FileManager : IFileManager
    {
        public StreamReader StreamReader(string path)
        {
            return new StreamReader(path);
        }

        public CsvReader CsvReader(StreamReader streamReader, CultureInfo cultureInfo)
        {
            return new CsvReader(streamReader, cultureInfo);
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
