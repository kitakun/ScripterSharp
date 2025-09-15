using System;

namespace Scripter.Infrastructure.Console
{
    /// <summary>
    /// Интерфейс для вывода в консоль, позволяющий кастомизировать вывод
    /// </summary>
    public interface IScripterConsole
    {
        /// <summary>
        /// Выводит строку в консоль
        /// </summary>
        /// <param name="value">Строка для вывода</param>
        void Write(string value);

        /// <summary>
        /// Выводит строку в консоль с переводом строки
        /// </summary>
        /// <param name="value">Строка для вывода</param>
        void WriteLine(string value = "");

        /// <summary>
        /// Выводит строку в консоль с переводом строки
        /// </summary>
        /// <param name="value">Объект для вывода</param>
        void WriteLine(object value);

        /// <summary>
        /// Читает строку из консоли
        /// </summary>
        /// <returns>Введенная строка или null если достигнут конец потока</returns>
        string ReadLine();

        /// <summary>
        /// Читает символ из консоли
        /// </summary>
        /// <returns>Прочитанный символ</returns>
        int Read();

        /// <summary>
        /// Проверяет, достигнут ли конец потока ввода
        /// </summary>
        /// <returns>true если достигнут конец потока</returns>
        bool IsInputRedirected { get; }
    }
}
