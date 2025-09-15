using UnityEngine;
using Scripter.Core;
using Scripter.Core.Domain;
using System;
using System.Collections.Generic;

namespace Scripter.Unity
{
    public static class SharpExtensions
    {
        private static readonly Dictionary<string, Func<List<RuntimeValue>, RuntimeValue>> _customFunctions = 
            new Dictionary<string, Func<List<RuntimeValue>, RuntimeValue>>();
        
        public static void RegisterFunction(string functionName, Func<List<RuntimeValue>, RuntimeValue> function)
        {
            if (string.IsNullOrEmpty(functionName))
            {
                Debug.LogError("[SharpExtensions] Имя функции не может быть пустым");
                return;
            }
            
            if (function == null)
            {
                Debug.LogError("[SharpExtensions] Функция не может быть null");
                return;
            }
            
            _customFunctions[functionName] = function;
            Debug.Log($"[SharpExtensions] Зарегистрирована функция: {functionName}");
        }
        
        public static void RegisterFunction(string functionName, Func<RuntimeValue> function)
        {
            RegisterFunction(functionName, args => function());
        }
        
        public static void RegisterFunction<T>(string functionName, Func<T, RuntimeValue> function)
        {
            RegisterFunction(functionName, args =>
            {
                if (args.Count < 1) return new RuntimeValue(null);
                var value = ConvertValue<T>(args[0]);
                return function(value);
            });
        }
        
        public static void RegisterFunction<T1, T2>(string functionName, Func<T1, T2, RuntimeValue> function)
        {
            RegisterFunction(functionName, args =>
            {
                if (args.Count < 2) return new RuntimeValue(null);
                var value1 = ConvertValue<T1>(args[0]);
                var value2 = ConvertValue<T2>(args[1]);
                return function(value1, value2);
            });
        }
        
        public static void RegisterFunction<T1, T2, T3>(string functionName, Func<T1, T2, T3, RuntimeValue> function)
        {
            RegisterFunction(functionName, args =>
            {
                if (args.Count < 3) return new RuntimeValue(null);
                var value1 = ConvertValue<T1>(args[0]);
                var value2 = ConvertValue<T2>(args[1]);
                var value3 = ConvertValue<T3>(args[2]);
                return function(value1, value2, value3);
            });
        }
        
        public static bool IsFunctionRegistered(string functionName)
        {
            return _customFunctions.ContainsKey(functionName);
        }
        
        public static Func<List<RuntimeValue>, RuntimeValue> GetFunction(string functionName)
        {
            _customFunctions.TryGetValue(functionName, out var function);
            return function;
        }
        
        public static Dictionary<string, Func<List<RuntimeValue>, RuntimeValue>> GetAllFunctions()
        {
            return new Dictionary<string, Func<List<RuntimeValue>, RuntimeValue>>(_customFunctions);
        }
        
        public static bool UnregisterFunction(string functionName)
        {
            var removed = _customFunctions.Remove(functionName);
            if (removed)
            {
                Debug.Log($"[SharpExtensions] Удалена функция: {functionName}");
            }
            return removed;
        }
        
        public static void ClearAllFunctions()
        {
            _customFunctions.Clear();
            Debug.Log("[SharpExtensions] Все функции очищены");
        }
        
        public static RuntimeValue CallFunction(string functionName, List<RuntimeValue> arguments)
        {
            if (_customFunctions.TryGetValue(functionName, out var function))
            {
                try
                {
                    return function(arguments);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[SharpExtensions] Ошибка вызова функции {functionName}: {ex.Message}");
                    return new RuntimeValue(null);
                }
            }
            
            Debug.LogError($"[SharpExtensions] Функция {functionName} не найдена");
            return new RuntimeValue(null);
        }
        
        private static T ConvertValue<T>(RuntimeValue value)
        {
            if (value?.Value == null) return default(T);
            
            try
            {
                if (typeof(T) == typeof(string))
                {
                    return (T)(object)(value.Value.ToString());
                }
                
                if (typeof(T) == typeof(int))
                {
                    return (T)(object)Convert.ToInt32(value.Value);
                }
                
                if (typeof(T) == typeof(float))
                {
                    return (T)(object)Convert.ToSingle(value.Value);
                }
                
                if (typeof(T) == typeof(bool))
                {
                    return (T)(object)Convert.ToBoolean(value.Value);
                }
                
                if (typeof(T) == typeof(GameObject))
                {
                    return (T)(object)(value.Value as GameObject);
                }
                
                return (T)value.Value;
            }
            catch
            {
                return default(T);
            }
        }
    }
}
