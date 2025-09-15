using System;

namespace Scripter.Infrastructure.Console
{
    /// <summary>
    /// Стандартная реализация IScripterConsole для работы с консолью
    /// </summary>
    public class DefaultConsole : IScripterConsole
    {
        public void Write(string value)
        {
            System.Console.Write(value);
        }

        public void WriteLine(string value = "")
        {
            System.Console.WriteLine(value);
        }

        public void WriteLine(object value)
        {
            System.Console.WriteLine(value);
        }

        public string ReadLine()
        {
            return System.Console.ReadLine();
        }

        public int Read()
        {
            return System.Console.Read();
        }

        public bool IsInputRedirected => System.Console.IsInputRedirected;
    }
}
