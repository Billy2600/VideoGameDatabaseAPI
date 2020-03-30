using System.IO;
using System.Globalization;
using CsvHelper;

namespace VideoGameAPI.Helpers
{
    // Exists to wrap file handling stuff (StreamReader, CsvHelper, etc.) for unit testing 
    public interface IFileManager
    {
        public StreamReader StreamReader(string path);
        public CsvReader CsvReader(StreamReader streamReader, CultureInfo cultureInfo);
        public bool FileExists(string filePath);
    }
}
