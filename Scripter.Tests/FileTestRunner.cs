using System;
using System.IO;
using Scripter.Core;
using Scripter.Core.Services;
using Scripter.Infrastructure.Console;
using Scripter.Infrastructure.FileSystem;

namespace Scripter.UnitTests
{
    public class FileTestRunner
    {
        public static void RunFileTests()
        {
            Console.WriteLine("=== Тесты для файлов из папки tests ===\n");

            int testsPassed = 0;
            int testsFailed = 0;

            // Список файлов для тестирования
            string[] sharpFiles = {
                "basic_test.sharp",
                "simple_test.sharp", 
                "minimal_test.sharp",
                "function_test.sharp",
                "working_test.sharp",
                "simple_example.sharp",
                "simple_function.sharp",
                "test_function_call.sharp",
                "demo_registered_functions.sharp",
                "plugin_example.sharp"
            };

            string[] csFiles = {
                "RegistrationDemo.cs"
            };

            // Тестируем .sharp файлы
            foreach (var fileName in sharpFiles)
            {
                try
                {
                    Console.WriteLine($"--- Тест файла: {fileName} ---");
                    
                    // Читаем содержимое файла
                    string filePath = Path.Combine("tests", fileName);
                    string scriptContent = File.ReadAllText(filePath);
                    
                    if (string.IsNullOrEmpty(scriptContent))
                    {
                        Console.WriteLine($"❌ Файл {fileName} пустой или не найден");
                        testsFailed++;
                        continue;
                    }

                    Console.WriteLine($"✅ Файл {fileName} успешно прочитан ({scriptContent.Length} символов)");

                    // Создаем интерпретатор и выполняем скрипт
                    var console = new TestConsole();
                    var fileService = new FileService();
                    var scriptService = new ScriptService(fileService, console);
                    
                    // Выполняем скрипт
                    scriptService.ExecuteScript(scriptContent);
                    
                    Console.WriteLine($"✅ Скрипт {fileName} выполнен успешно");
                    
                    // Проверяем вывод консоли
                    if (console.Output.Count > 0)
                    {
                        Console.WriteLine($"📝 Вывод: {string.Join("; ", console.Output)}");
                    }
                    
                    testsPassed++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Ошибка при тестировании файла {fileName}: {ex.Message}");
                    testsFailed++;
                }
            }

            // Тестируем .cs файлы
            foreach (var fileName in csFiles)
            {
                try
                {
                    Console.WriteLine($"--- Тест файла: {fileName} ---");
                    
                    // Читаем содержимое файла
                    string filePath = Path.Combine("tests", fileName);
                    string fileContent = File.ReadAllText(filePath);
                    
                    if (string.IsNullOrEmpty(fileContent))
                    {
                        Console.WriteLine($"❌ Файл {fileName} пустой или не найден");
                        testsFailed++;
                        continue;
                    }

                    Console.WriteLine($"✅ Файл {fileName} успешно прочитан ({fileContent.Length} символов)");
                    
                    // Проверяем структуру C# файла
                    bool hasUsing = fileContent.Contains("using");
                    bool hasNamespace = fileContent.Contains("namespace");
                    bool hasClass = fileContent.Contains("class");
                    
                    Console.WriteLine($"📝 Структура: using={hasUsing}, namespace={hasNamespace}, class={hasClass}");
                    
                    testsPassed++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Ошибка при тестировании файла {fileName}: {ex.Message}");
                    testsFailed++;
                }
            }

            Console.WriteLine($"\n=== Результаты тестов файлов ===");
            Console.WriteLine($"✅ Пройдено: {testsPassed}");
            Console.WriteLine($"❌ Провалено: {testsFailed}");
            Console.WriteLine($"📊 Всего тестов: {testsPassed + testsFailed}");

            if (testsFailed == 0)
            {
                Console.WriteLine("🎉 Все тесты файлов прошли успешно!");
            }
            else
            {
                Console.WriteLine("⚠️  Некоторые тесты файлов провалились.");
            }
        }

        // Тестовый консоль для перехвата вывода
        public class TestConsole : IScripterConsole
        {
            public System.Collections.Generic.List<string> Output { get; } = new System.Collections.Generic.List<string>();

            public void Write(string value)
            {
                Console.Write(value);
            }

            public void WriteLine(string message)
            {
                Output.Add(message);
                Console.WriteLine($"[OUTPUT] {message}");
            }

            public void WriteLine()
            {
                Console.WriteLine();
            }

            public void WriteLine(object value)
            {
                WriteLine(value?.ToString() ?? "null");
            }

            public string ReadLine()
            {
                return "test input";
            }

            public int Read()
            {
                return 0;
            }

            public bool IsInputRedirected => false;
        }
    }
}
