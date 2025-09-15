using System;
using System.IO;

namespace Scripter.Infrastructure.FileSystem
{
    /// <summary>
    /// Реализация сервиса для работы с файловой системой
    /// </summary>
    public class FileService : IFileService
    {
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path, System.Text.Encoding.UTF8);
        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public bool HasExtension(string path)
        {
            return Path.HasExtension(path);
        }

        public string Combine(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }
    }
}
