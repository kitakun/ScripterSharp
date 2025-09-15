using System;
using Scripter.Infrastructure.FileSystem;
using Scripter.Infrastructure.Console;

namespace Scripter.Core
{
    /// <summary>
    /// Сервис для выполнения скриптов
    /// </summary>
    public class ScriptService
    {
        private readonly IFileService _fileService;
        private readonly IScripterConsole _console;

        public ScriptService(
            IFileService fileService,
            IScripterConsole console)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public void ExecuteScriptFromFile(string filePath)
        {
            try
            {
                // Если файл не найден, попробуем найти его в папке tests
                if (!_fileService.Exists(filePath))
                {
                    string fileName = _fileService.GetFileName(filePath);
                    string testsPath;

                    // Если расширение не указано, попробуем .sharp
                    if (!_fileService.HasExtension(fileName))
                    {
                        testsPath = _fileService.Combine("tests", fileName + ".sharp");
                    }
                    else
                    {
                        testsPath = _fileService.Combine("tests", fileName);
                    }

                    if (_fileService.Exists(testsPath))
                    {
                        filePath = testsPath;
                    }
                }

                string script = _fileService.ReadAllText(filePath);
                _console.WriteLine($"Выполнение скрипта: {filePath}");
                _console.WriteLine("=" + new string('=', 50));

                ExecuteScript(script);
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            }
        }

        public void ExecuteScript(string script)
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
    }
}
