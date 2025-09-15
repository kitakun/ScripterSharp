using System;

namespace Scripter.Infrastructure.FileSystem
{
    /// <summary>
    /// Интерфейс для работы с файловой системой
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Проверяет существование файла
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>true если файл существует</returns>
        bool Exists(string path);

        /// <summary>
        /// Читает содержимое файла
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>Содержимое файла</returns>
        string ReadAllText(string path);

        /// <summary>
        /// Получает имя файла из пути
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>Имя файла</returns>
        string GetFileName(string path);

        /// <summary>
        /// Проверяет, имеет ли файл расширение
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>true если файл имеет расширение</returns>
        bool HasExtension(string path);

        /// <summary>
        /// Объединяет пути
        /// </summary>
        /// <param name="path1">Первый путь</param>
        /// <param name="path2">Второй путь</param>
        /// <returns>Объединенный путь</returns>
        string Combine(string path1, string path2);
    }
}
