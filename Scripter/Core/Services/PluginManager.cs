using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Scripter.Core.Interfaces;
using Scripter.Core.Domain;
using Scripter.Infrastructure.Console;

namespace Scripter.Core.Services
{
    /// <summary>
    /// Менеджер плагинов для Scripter
    /// </summary>
    public class PluginManager
    {
        private readonly Dictionary<string, IPlugin> _plugins;
        private readonly Dictionary<string, PluginFunction> _functions;
        private readonly IScripterConsole _console;
        private readonly Interpreter _interpreter;

        public PluginManager(IScripterConsole console, Interpreter interpreter)
        {
            _plugins = new Dictionary<string, IPlugin>();
            _functions = new Dictionary<string, PluginFunction>();
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _interpreter = interpreter ?? throw new ArgumentNullException(nameof(interpreter));
        }

        /// <summary>
        /// Загрузить плагин
        /// </summary>
        /// <param name="plugin">Плагин для загрузки</param>
        public void LoadPlugin(IPlugin plugin)
        {
            if (plugin == null)
                throw new ArgumentNullException(nameof(plugin));

            try
            {
                _console.WriteLine($"Загрузка плагина: {plugin.Name} v{plugin.Version}");
                
                // Инициализируем плагин
                plugin.Initialize(_interpreter);
                
                // Регистрируем плагин
                _plugins[plugin.Name] = plugin;
                
                // Регистрируем функции плагина
                foreach (var function in plugin.GetFunctions())
                {
                    _functions[function.Name] = function;
                    
                    // Создаем Sharp функцию для регистрации в интерпретаторе
                    var sharpFunction = new Function(function.Name, 
                        function.Parameters.Select(p => p.Name).ToList(), 
                        null, true, true);
                    
                    _interpreter.RegisterFunction(function.Name, sharpFunction);
                    _console.WriteLine($"  Зарегистрирована функция: {function.Name}");
                }
                
                _console.WriteLine($"Плагин {plugin.Name} успешно загружен");
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Ошибка загрузки плагина {plugin.Name}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Выгрузить плагин
        /// </summary>
        /// <param name="pluginName">Имя плагина</param>
        public void UnloadPlugin(string pluginName)
        {
            if (_plugins.TryGetValue(pluginName, out var plugin))
            {
                // Удаляем функции плагина
                var functionsToRemove = _functions
                    .Where(kvp => kvp.Value.Implementation != null)
                    .Where(kvp => _plugins.Values.Any(p => p.GetFunctions().Any(f => f.Name == kvp.Key)))
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var functionName in functionsToRemove)
                {
                    _functions.Remove(functionName);
                }

                _plugins.Remove(pluginName);
                _console.WriteLine($"Плагин {pluginName} выгружен");
            }
        }

        /// <summary>
        /// Получить список загруженных плагинов
        /// </summary>
        /// <returns>Список плагинов</returns>
        public IEnumerable<IPlugin> GetLoadedPlugins()
        {
            return _plugins.Values;
        }

        /// <summary>
        /// Получить список доступных функций
        /// </summary>
        /// <returns>Список функций</returns>
        public IEnumerable<PluginFunction> GetAvailableFunctions()
        {
            return _functions.Values;
        }

        /// <summary>
        /// Вызвать функцию плагина
        /// </summary>
        /// <param name="functionName">Имя функции</param>
        /// <param name="arguments">Аргументы</param>
        /// <returns>Результат выполнения</returns>
        public RuntimeValue CallFunction(string functionName, List<RuntimeValue> arguments)
        {
            if (_functions.TryGetValue(functionName, out var function))
            {
                try
                {
                    return function.Implementation(arguments);
                }
                catch (Exception ex)
                {
                    _console.WriteLine($"Ошибка выполнения функции {functionName}: {ex.Message}");
                    return new RuntimeValue(null);
                }
            }
            else
            {
                _console.WriteLine($"Функция {functionName} не найдена");
                return new RuntimeValue(null);
            }
        }

        /// <summary>
        /// Проверить, загружен ли плагин
        /// </summary>
        /// <param name="pluginName">Имя плагина</param>
        /// <returns>True, если плагин загружен</returns>
        public bool IsPluginLoaded(string pluginName)
        {
            return _plugins.ContainsKey(pluginName);
        }

        /// <summary>
        /// Проверить, доступна ли функция
        /// </summary>
        /// <param name="functionName">Имя функции</param>
        /// <returns>True, если функция доступна</returns>
        public bool IsFunctionAvailable(string functionName)
        {
            return _functions.ContainsKey(functionName);
        }
    }
}
