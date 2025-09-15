using System;
using Scripter.Core;
using Scripter.Core.Domain;
using Scripter.Extensions;
using Scripter.Infrastructure.Console;

namespace Scripter.Examples
{
    /// <summary>
    /// Демонстрация регистрации функций из C# кода
    /// </summary>
    public class RegistrationDemo
    {
        public static void RunDemo()
        {
            Console.WriteLine("=== Демонстрация регистрации функций из C# ===");

            // Создаем интерпретатор
            var console = new DefaultConsole();
            var interpreter = new Interpreter(console);
            var registry = interpreter.GetFunctionRegistry();

            // Регистрируем функции с упрощенным синтаксисом
            Console.WriteLine("\n1. Регистрация функций...");

            // Функция с одним параметром
            registry.RegisterFunction<int>("DoSomething", addTo => new RuntimeValue(addTo + 1), "Добавляет 1 к числу");

            // Функция для работы со строками
            registry.RegisterFunction<string>("Greet", name => new RuntimeValue($"Hello, {name}!"), "Приветствует пользователя");

            // Функция с двумя параметрами
            registry.RegisterFunction<int, int>("Add", (a, b) => new RuntimeValue(a + b), "Складывает два числа");

            Console.WriteLine("Все функции зарегистрированы!");

            // Теперь можно выполнить Sharp скрипт, который будет использовать эти функции
            Console.WriteLine("\n2. Функции готовы к использованию в Sharp скриптах:");
            Console.WriteLine("   - DoSomething(5) вернет 6");
            Console.WriteLine("   - Greet(\"World\") вернет \"Hello, World!\"");
            Console.WriteLine("   - Add(3, 4) вернет 7");

            Console.WriteLine("\n=== Демонстрация завершена! ===");
        }
    }
}
