using System;
using System.Collections.Generic;
using Scripter.Core.Domain;
using Scripter.Core.Services;

namespace Scripter.Extensions
{
    /// <summary>
    /// Extension методы для упрощенной регистрации функций
    /// </summary>
    public static class FunctionRegistrationExtensions
    {
        /// <summary>
        /// Зарегистрировать функцию с одним параметром
        /// </summary>
        /// <typeparam name="T">Тип параметра</typeparam>
        /// <param name="registry">Реестр функций</param>
        /// <param name="name">Имя функции</param>
        /// <param name="func">Функция</param>
        /// <param name="description">Описание</param>
        public static void RegisterFunction<T>(this FunctionRegistry registry, string name, Func<T, RuntimeValue> func, string description = "")
        {
            registry.RegisterFunction(name, func, description);
        }

        /// <summary>
        /// Зарегистрировать функцию с двумя параметрами
        /// </summary>
        public static void RegisterFunction<T1, T2>(this FunctionRegistry registry, string name, Func<T1, T2, RuntimeValue> func, string description = "")
        {
            registry.RegisterFunction(name, func, description);
        }

        /// <summary>
        /// Зарегистрировать функцию с тремя параметрами
        /// </summary>
        public static void RegisterFunction<T1, T2, T3>(this FunctionRegistry registry, string name, Func<T1, T2, T3, RuntimeValue> func, string description = "")
        {
            registry.RegisterFunction(name, func, description);
        }

        /// <summary>
        /// Зарегистрировать функцию без параметров
        /// </summary>
        public static void RegisterFunction(this FunctionRegistry registry, string name, Func<RuntimeValue> func, string description = "")
        {
            registry.RegisterFunction(name, args => func(), description);
        }

        /// <summary>
        /// Зарегистрировать функцию с одним параметром, возвращающую T
        /// </summary>
        public static void RegisterFunction<T>(this FunctionRegistry registry, string name, Func<T, T> func, string description = "")
        {
            registry.RegisterFunction(name, (T arg) => new RuntimeValue(func(arg)), description);
        }

        /// <summary>
        /// Зарегистрировать функцию с двумя параметрами, возвращающую T
        /// </summary>
        public static void RegisterFunction<T1, T2, TResult>(this FunctionRegistry registry, string name, Func<T1, T2, TResult> func, string description = "")
        {
            registry.RegisterFunction(name, (T1 arg1, T2 arg2) => new RuntimeValue(func(arg1, arg2)), description);
        }

        /// <summary>
        /// Зарегистрировать функцию с тремя параметрами, возвращающую T
        /// </summary>
        public static void RegisterFunction<T1, T2, T3, TResult>(this FunctionRegistry registry, string name, Func<T1, T2, T3, TResult> func, string description = "")
        {
            registry.RegisterFunction(name, (T1 arg1, T2 arg2, T3 arg3) => new RuntimeValue(func(arg1, arg2, arg3)), description);
        }
    }
}
