using System;
using System.Collections.Generic;
using System.Linq;
using Scripter.Core.Domain;
using Scripter.Core.Interfaces;
using Scripter.Infrastructure.Console;

namespace Scripter.Core.Services
{
    /// <summary>
    /// Реестр функций для Scripter
    /// </summary>
    public class FunctionRegistry
    {
        private readonly Dictionary<string, RegisteredFunction> _functions;
        private readonly IScripterConsole _console;

        public FunctionRegistry(IScripterConsole console)
        {
            _functions = new Dictionary<string, RegisteredFunction>();
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        /// <summary>
        /// Зарегистрировать функцию
        /// </summary>
        /// <param name="name">Имя функции</param>
        /// <param name="implementation">Реализация функции</param>
        /// <param name="description">Описание функции</param>
        public void RegisterFunction(string name, Func<List<RuntimeValue>, RuntimeValue> implementation, string description = "")
        {
            var registeredFunction = new RegisteredFunction
            {
                Name = name,
                Implementation = implementation,
                Description = description
            };

            _functions[name] = registeredFunction;
            _console.WriteLine($"Функция '{name}' зарегистрирована в реестре");
        }

        /// <summary>
        /// Зарегистрировать функцию с одним параметром
        /// </summary>
        public void RegisterFunction<T>(string name, Func<T, RuntimeValue> implementation, string description = "")
        {
            RegisterFunction(name, args =>
            {
                if (args.Count == 0)
                    throw new ArgumentException($"Функция '{name}' требует 1 аргумент");
                
                var value = ConvertToType<T>(args[0]);
                return implementation(value);
            }, description);
        }

        /// <summary>
        /// Зарегистрировать функцию с двумя параметрами
        /// </summary>
        public void RegisterFunction<T1, T2>(string name, Func<T1, T2, RuntimeValue> implementation, string description = "")
        {
            RegisterFunction(name, args =>
            {
                if (args.Count < 2)
                    throw new ArgumentException($"Функция '{name}' требует 2 аргумента");
                
                var value1 = ConvertToType<T1>(args[0]);
                var value2 = ConvertToType<T2>(args[1]);
                return implementation(value1, value2);
            }, description);
        }

        /// <summary>
        /// Зарегистрировать функцию с тремя параметрами
        /// </summary>
        public void RegisterFunction<T1, T2, T3>(string name, Func<T1, T2, T3, RuntimeValue> implementation, string description = "")
        {
            RegisterFunction(name, args =>
            {
                if (args.Count < 3)
                    throw new ArgumentException($"Функция '{name}' требует 3 аргумента");
                
                var value1 = ConvertToType<T1>(args[0]);
                var value2 = ConvertToType<T2>(args[1]);
                var value3 = ConvertToType<T3>(args[2]);
                return implementation(value1, value2, value3);
            }, description);
        }

        /// <summary>
        /// Вызвать функцию
        /// </summary>
        public RuntimeValue CallFunction(string name, List<RuntimeValue> arguments)
        {
            if (_functions.TryGetValue(name, out var function))
            {
                try
                {
                    return function.Implementation(arguments);
                }
                catch (Exception ex)
                {
                    _console.WriteLine($"Ошибка выполнения функции '{name}': {ex.Message}");
                    return new RuntimeValue(null);
                }
            }
            else
            {
                _console.WriteLine($"Функция '{name}' не найдена в реестре");
                return new RuntimeValue(null);
            }
        }

        /// <summary>
        /// Проверить, зарегистрирована ли функция
        /// </summary>
        public bool IsFunctionRegistered(string name)
        {
            return _functions.ContainsKey(name);
        }

        /// <summary>
        /// Получить список зарегистрированных функций
        /// </summary>
        public IEnumerable<RegisteredFunction> GetRegisteredFunctions()
        {
            return _functions.Values;
        }

        /// <summary>
        /// Удалить функцию из реестра
        /// </summary>
        public void UnregisterFunction(string name)
        {
            if (_functions.Remove(name))
            {
                _console.WriteLine($"Функция '{name}' удалена из реестра");
            }
        }

        private T ConvertToType<T>(RuntimeValue value)
        {
            if (value.Value == null)
                return default(T);

            var targetType = typeof(T);
            
            // Если типы совпадают
            if (value.Value.GetType() == targetType)
                return (T)value.Value;

            // Конвертация для базовых типов
            if (targetType == typeof(int))
            {
                if (value.Value is double d)
                    return (T)(object)(int)d;
                if (value.Value is string s && int.TryParse(s, out var i))
                    return (T)(object)i;
            }
            else if (targetType == typeof(double))
            {
                if (value.Value is int i)
                    return (T)(object)(double)i;
                if (value.Value is string s && double.TryParse(s, out var d))
                    return (T)(object)d;
            }
            else if (targetType == typeof(string))
            {
                return (T)(object)value.ToString();
            }
            else if (targetType == typeof(bool))
            {
                if (value.Value is bool b)
                    return (T)(object)b;
                if (value.Value is string s)
                    return (T)(object)(s.ToLower() == "true");
            }

            // Попытка прямого приведения
            try
            {
                return (T)Convert.ChangeType(value.Value, targetType);
            }
            catch
            {
                throw new InvalidCastException($"Не удается преобразовать {value.Value} в тип {targetType.Name}");
            }
        }
    }

    /// <summary>
    /// Зарегистрированная функция
    /// </summary>
    public class RegisteredFunction
    {
        public string Name { get; set; }
        public Func<List<RuntimeValue>, RuntimeValue> Implementation { get; set; }
        public string Description { get; set; }
    }
}
