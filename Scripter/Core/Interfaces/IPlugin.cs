using System;
using System.Collections.Generic;
using Scripter.Core.Domain;

namespace Scripter.Core.Interfaces
{
    /// <summary>
    /// Интерфейс для плагинов Scripter
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Имя плагина
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Версия плагина
        /// </summary>
        string Version { get; }
        
        /// <summary>
        /// Описание плагина
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Инициализация плагина
        /// </summary>
        /// <param name="interpreter">Интерпретатор для регистрации функций</param>
        void Initialize(IInterpreter interpreter);
        
        /// <summary>
        /// Получить список функций, предоставляемых плагином
        /// </summary>
        /// <returns>Список функций</returns>
        IEnumerable<PluginFunction> GetFunctions();
    }
    
    /// <summary>
    /// Представляет функцию плагина
    /// </summary>
    public class PluginFunction
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ParameterInfo> Parameters { get; set; } = new List<ParameterInfo>();
        public string ReturnType { get; set; }
        public Func<List<RuntimeValue>, RuntimeValue> Implementation { get; set; }
    }
    
    /// <summary>
    /// Информация о параметре функции
    /// </summary>
    public class ParameterInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsOptional { get; set; } = false;
        public RuntimeValue DefaultValue { get; set; }
    }
}
