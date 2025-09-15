using System.Collections.Generic;
using Scripter.Core.Domain;

namespace Scripter.Core.Interfaces
{
    /// <summary>
    /// Интерфейс интерпретатора для плагинов
    /// </summary>
    public interface IInterpreter
    {
        /// <summary>
        /// Получить глобальную среду выполнения
        /// </summary>
        RuntimeEnvironment GlobalEnvironment { get; }
        
        /// <summary>
        /// Зарегистрировать функцию в глобальной среде
        /// </summary>
        /// <param name="name">Имя функции</param>
        /// <param name="function">Функция</param>
        void RegisterFunction(string name, Function function);
        
        /// <summary>
        /// Выполнить код
        /// </summary>
        /// <param name="code">Код для выполнения</param>
        void ExecuteCode(string code);
    }
}
