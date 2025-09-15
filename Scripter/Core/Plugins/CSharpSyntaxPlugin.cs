using System;
using System.Collections.Generic;
using System.Linq;
using Scripter.Core.Interfaces;
using Scripter.Core.Domain;
using Scripter.Infrastructure.Console;

namespace Scripter.Core.Plugins
{
    /// <summary>
    /// Плагин для поддержки C# синтаксиса в Scripter
    /// </summary>
    public class CSharpSyntaxPlugin : IPlugin
    {
        public string Name => "CSharp Syntax Plugin";
        public string Version => "1.0.0";
        public string Description => "Поддержка C# синтаксиса для создания функций";

        private readonly IScripterConsole _console;
        private readonly Dictionary<string, PluginFunction> _functions;

        public CSharpSyntaxPlugin(IScripterConsole console)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _functions = new Dictionary<string, PluginFunction>();
        }

        public void Initialize(IInterpreter interpreter)
        {
            // Регистрируем функцию CreateFunction
            RegisterCreateFunction();
            
            // Регистрируем другие C# функции
            RegisterUtilityFunctions();
        }

        private void RegisterCreateFunction()
        {
            var createFunction = new PluginFunction
            {
                Name = "CreateFunction",
                Description = "Создает новую функцию с C# синтаксисом",
                Parameters = new List<ParameterInfo>
                {
                    new ParameterInfo { Name = "functionName", Type = "string" },
                    new ParameterInfo { Name = "functionBody", Type = "string" }
                },
                ReturnType = "bool",
                Implementation = (args) =>
                {
                    if (args.Count < 2)
                    {
                        _console.WriteLine("CreateFunction требует 2 аргумента: имя функции и тело функции");
                        return new RuntimeValue(false);
                    }

                    var functionName = args[0].ToString();
                    var functionBody = args[1].ToString();

                    try
                    {
                        // Парсим C# синтаксис и создаем Sharp функцию
                        var sharpFunction = ParseCSharpFunction(functionName, functionBody);
                        
                        // Регистрируем функцию в интерпретаторе
                        RegisterSharpFunction(functionName, sharpFunction);
                        
                        _console.WriteLine($"Функция {functionName} успешно создана");
                        return new RuntimeValue(true);
                    }
                    catch (Exception ex)
                    {
                        _console.WriteLine($"Ошибка создания функции {functionName}: {ex.Message}");
                        return new RuntimeValue(false);
                    }
                }
            };

            _functions["CreateFunction"] = createFunction;
        }

        private void RegisterUtilityFunctions()
        {
            // Функция для работы с типами
            var typeOfFunction = new PluginFunction
            {
                Name = "TypeOf",
                Description = "Возвращает тип значения",
                Parameters = new List<ParameterInfo>
                {
                    new ParameterInfo { Name = "value", Type = "any" }
                },
                ReturnType = "string",
                Implementation = (args) =>
                {
                    if (args.Count == 0)
                        return new RuntimeValue("null");

                    var value = args[0];
                    var typeName = GetTypeName(value);
                    return new RuntimeValue(typeName);
                }
            };

            // Функция для приведения типов
            var castFunction = new PluginFunction
            {
                Name = "Cast",
                Description = "Приводит значение к указанному типу",
                Parameters = new List<ParameterInfo>
                {
                    new ParameterInfo { Name = "value", Type = "any" },
                    new ParameterInfo { Name = "targetType", Type = "string" }
                },
                ReturnType = "any",
                Implementation = (args) =>
                {
                    if (args.Count < 2)
                        return new RuntimeValue(null);

                    var value = args[0];
                    var targetType = args[1].ToString().ToLower();

                    var castedValue = CastValue(value, targetType);
                    return castedValue;
                }
            };

            _functions["TypeOf"] = typeOfFunction;
            _functions["Cast"] = castFunction;
        }

        private string ParseCSharpFunction(string functionName, string functionBody)
        {
            // Простой парсер C# лямбда-выражений
            // Пример: Func<int, string, string> (argumentInt: int, argumentString: string) => { return "ok it works"; }
            
            var sharpFunction = $"function {functionName}(";
            
            // Извлекаем параметры из C# синтаксиса
            var paramStart = functionBody.IndexOf('(');
            var paramEnd = functionBody.IndexOf(')');
            
            if (paramStart >= 0 && paramEnd > paramStart)
            {
                var paramString = functionBody.Substring(paramStart + 1, paramEnd - paramStart - 1);
                var parameters = ParseParameters(paramString);
                sharpFunction += string.Join(", ", parameters);
            }
            
            sharpFunction += ") {\n";
            
            // Извлекаем тело функции
            var bodyStart = functionBody.IndexOf("=>");
            if (bodyStart >= 0)
            {
                var body = functionBody.Substring(bodyStart + 2).Trim();
                
                // Убираем фигурные скобки если есть
                if (body.StartsWith("{") && body.EndsWith("}"))
                {
                    body = body.Substring(1, body.Length - 2).Trim();
                }
                
                // Конвертируем return statement
                if (body.StartsWith("return"))
                {
                    var returnValue = body.Substring(6).Trim().TrimEnd(';');
                    sharpFunction += $"    return {returnValue};\n";
                }
                else
                {
                    sharpFunction += $"    {body};\n";
                }
            }
            
            sharpFunction += "}";
            
            return sharpFunction;
        }

        private List<string> ParseParameters(string paramString)
        {
            var parameters = new List<string>();
            
            if (string.IsNullOrWhiteSpace(paramString))
                return parameters;

            // Разделяем параметры по запятым
            var paramParts = paramString.Split(',');
            
            foreach (var part in paramParts)
            {
                var trimmed = part.Trim();
                
                // Извлекаем имя параметра (после двоеточия)
                var colonIndex = trimmed.IndexOf(':');
                if (colonIndex > 0)
                {
                    var paramName = trimmed.Substring(0, colonIndex).Trim();
                    parameters.Add(paramName);
                }
                else
                {
                    // Если нет двоеточия, используем весь параметр как имя
                    parameters.Add(trimmed);
                }
            }
            
            return parameters;
        }

        private void RegisterSharpFunction(string functionName, string sharpFunction)
        {
            // Здесь должна быть логика регистрации функции в интерпретаторе
            // Пока что просто выводим созданную функцию
            _console.WriteLine($"Создана Sharp функция:\n{sharpFunction}");
        }

        private string GetTypeName(RuntimeValue value)
        {
            if (value.Value == null)
                return "null";
            
            var type = value.Value.GetType();
            
            if (type == typeof(int) || type == typeof(double))
                return "number";
            else if (type == typeof(string))
                return "string";
            else if (type == typeof(bool))
                return "boolean";
            else
                return type.Name.ToLower();
        }

        private RuntimeValue CastValue(RuntimeValue value, string targetType)
        {
            try
            {
                switch (targetType)
                {
                    case "int":
                    case "number":
                        if (value.Value is string str && double.TryParse(str, out var num))
                            return new RuntimeValue((int)num);
                        return new RuntimeValue(Convert.ToInt32(value.Value));
                    
                    case "string":
                        return new RuntimeValue(value.ToString());
                    
                    case "bool":
                    case "boolean":
                        if (value.Value is string boolStr)
                            return new RuntimeValue(boolStr.ToLower() == "true");
                        return new RuntimeValue(Convert.ToBoolean(value.Value));
                    
                    default:
                        return value;
                }
            }
            catch
            {
                return value; // Возвращаем исходное значение при ошибке приведения
            }
        }

        public IEnumerable<PluginFunction> GetFunctions()
        {
            return _functions.Values;
        }
    }
}
