using System;
using System.IO;
using Scripter.Infrastructure.Console;
using Scripter.Core;
using Scripter.Infrastructure.FileSystem;

namespace Scripter
{
    class ScripterProgram
    {
        private static IScripterConsole _console = new DefaultConsole();

        public static void Run(string[] args)
        {
            _console.WriteLine("=== Scripter - Интерпретатор скриптов на C# ===");
            _console.WriteLine();

            if (args.Length > 0)
            {
                // Запуск с файлом
                string filePath = args[0];
                if (File.Exists(filePath))
                {
                    RunScriptFromFile(filePath);
                }
                else
                {
                    _console.WriteLine($"Файл не найден: {filePath}");
                }
            }
            else
            {
                // Интерактивный режим
                RunInteractiveMode();
            }
        }

        static void RunScriptFromFile(string filePath)
        {
            try
            {
                // Если файл не найден, попробуем найти его в папке tests
                if (!File.Exists(filePath))
                {
                    string fileName = Path.GetFileName(filePath);
                    string testsPath;
                    
                    // Если расширение не указано, попробуем .sharp
                    if (!Path.HasExtension(fileName))
                    {
                        testsPath = Path.Combine("tests", fileName + ".sharp");
                    }
                    else
                    {
                        testsPath = Path.Combine("tests", fileName);
                    }
                    
                    if (File.Exists(testsPath))
                    {
                        filePath = testsPath;
                    }
                }
                
                string script = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
                _console.WriteLine($"Выполнение скрипта: {filePath}");
                _console.WriteLine("=" + new string('=', 50));
                
                RunScript(script);
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            }
        }

        static void RunInteractiveMode()
        {
            _console.WriteLine("Интерактивный режим. Введите 'exit' для выхода.");
            _console.WriteLine("Примеры команд:");
            _console.WriteLine("  load script.sharp  - загрузить и выполнить скрипт");
            _console.WriteLine("  load script        - загрузить скрипт из папки tests");
            _console.WriteLine("  help               - показать справку");
            _console.WriteLine();

            while (true)
            {
                _console.Write("scripter> ");
                string input = _console.ReadLine();

                if (input == null) // EOF from pipe
                    break;

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                if (input.ToLower() == "exit")
                    break;

                if (input.ToLower().StartsWith("load "))
                {
                    string fileName = input.Substring(5).Trim();
                    RunScriptFromFile(fileName);
                }
                else if (input.ToLower() == "help")
                {
                    ShowHelp();
                }
                else
                {
                    // Выполнить как скрипт
                    RunScript(input);
                }
            }
        }

        static void RunScript(string script)
        {
            try
            {
                var lexer = new Lexer(script);
                var tokens = lexer.Tokenize();

                var parser = new Parser(tokens);
                var program = parser.Parse();
                var interpreter = new Interpreter(_console);
                interpreter.Interpret(program);
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Ошибка выполнения: {ex.Message}");
                _console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        static void ShowHelp()
        {
            _console.WriteLine();
            _console.WriteLine("=== Справка по Scripter ===");
            _console.WriteLine();
            _console.WriteLine("Поддерживаемые конструкции:");
            _console.WriteLine("• Переменные: var x = 10;");
            _console.WriteLine("• Функции: function add(a, b) { return a + b; }");
            _console.WriteLine("• Классы: class Person { public name; }");
            _console.WriteLine("• Статические классы: static class Math { static function square(x) { return x * x; } }");
            _console.WriteLine("• Условия: if (x > 0) { print('positive'); }");
            _console.WriteLine("• Циклы: while (i < 10) { i = i + 1; }");
            _console.WriteLine("• Встроенные функции: print(), input(), length()");
            _console.WriteLine();
            _console.WriteLine("Примеры:");
            _console.WriteLine("  var x = 5;");
            _console.WriteLine("  print(x);");
            _console.WriteLine("  function greet(name) { return 'Hello ' + name; }");
            _console.WriteLine("  print(greet('World'));");
            _console.WriteLine();
        }
    }
}
