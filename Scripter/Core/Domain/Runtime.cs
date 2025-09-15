using System;
using System.Collections.Generic;
using System.Reflection;

namespace Scripter.Core.Domain
{
    public class RuntimeValue
    {
        public object Value { get; set; }
        public Type Type { get; set; }

        public RuntimeValue(object value, Type type = null)
        {
            Value = value;
            Type = type ?? (value?.GetType() ?? typeof(object));
        }

        public T GetValue<T>()
        {
            if (Value is T directValue)
                return directValue;
            
            if (Value != null && typeof(T).IsAssignableFrom(Value.GetType()))
                return (T)Value;
            
            return default(T);
        }

        public bool IsTruthy()
        {
            if (Value == null) return false;
            if (Value is bool boolValue) return boolValue;
            if (Value is double doubleValue) return doubleValue != 0;
            if (Value is string stringValue) return !string.IsNullOrEmpty(stringValue);
            return true;
        }

        public override string ToString()
        {
            return Value?.ToString() ?? "null";
        }
    }

    public class Function
    {
        public string Name { get; set; }
        public List<string> Parameters { get; set; }
        public BlockStatement Body { get; set; }
        public bool IsStatic { get; set; }
        public bool IsPublic { get; set; }
        public RuntimeEnvironment Closure { get; set; }

        public Function(string name, List<string> parameters, BlockStatement body, bool isStatic = false, bool isPublic = true, RuntimeEnvironment closure = null)
        {
            Name = name;
            Parameters = parameters ?? new List<string>();
            Body = body;
            IsStatic = isStatic;
            IsPublic = isPublic;
            Closure = closure;
        }
    }

    public class Class
    {
        public string Name { get; set; }
        public Dictionary<string, Function> Methods { get; set; }
        public Dictionary<string, RuntimeValue> Properties { get; set; }
        public bool IsStatic { get; set; }

        public Class(string name, bool isStatic = false)
        {
            Name = name;
            Methods = new Dictionary<string, Function>();
            Properties = new Dictionary<string, RuntimeValue>();
            IsStatic = isStatic;
        }

        public ClassInstance CreateInstance()
        {
            if (IsStatic)
                throw new Exception($"Cannot create instance of static class '{Name}'");
            
            return new ClassInstance(this);
        }
    }

    public class ClassInstance
    {
        public Class Class { get; set; }
        public Dictionary<string, RuntimeValue> Fields { get; set; }

        public ClassInstance(Class class_)
        {
            Class = class_;
            Fields = new Dictionary<string, RuntimeValue>();
            
            // Initialize fields from class properties
            foreach (var prop in class_.Properties)
            {
                Fields[prop.Key] = prop.Value;
            }
        }

        public RuntimeValue GetProperty(string name)
        {
            if (Fields.ContainsKey(name))
                return Fields[name];
            
            if (Class.Methods.ContainsKey(name))
                return new RuntimeValue(Class.Methods[name]);
            
            throw new Exception($"Property '{name}' not found in class '{Class.Name}'");
        }

        public void SetProperty(string name, RuntimeValue value)
        {
            Fields[name] = value;
        }
    }

    public class RuntimeEnvironment
    {
        private readonly Dictionary<string, RuntimeValue> _variables;
        private readonly RuntimeEnvironment _enclosing;

        public RuntimeEnvironment(RuntimeEnvironment enclosing = null)
        {
            _variables = new Dictionary<string, RuntimeValue>();
            _enclosing = enclosing;
        }

        public void Define(string name, RuntimeValue value)
        {
            _variables[name] = value;
        }

        public RuntimeValue Get(string name)
        {
            if (_variables.ContainsKey(name))
                return _variables[name];

            if (_enclosing != null)
                return _enclosing.Get(name);

            throw new Exception($"Undefined variable '{name}'");
        }

        public void Assign(string name, RuntimeValue value)
        {
            if (_variables.ContainsKey(name))
            {
                _variables[name] = value;
                return;
            }

            if (_enclosing != null)
            {
                _enclosing.Assign(name, value);
                return;
            }

            throw new Exception($"Undefined variable '{name}'");
        }

        public bool Contains(string name)
        {
            if (_variables.ContainsKey(name))
                return true;

            if (_enclosing != null)
                return _enclosing.Contains(name);

            return false;
        }
    }
}
