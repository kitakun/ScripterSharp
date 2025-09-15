using System;
using System.Collections.Generic;
using System.Linq;
using Scripter.Core.Domain;
using Scripter.Core.Interfaces;
using Scripter.Core.Services;
using Scripter.Infrastructure.Console;

namespace Scripter.Core
{
    public class Interpreter : IInterpreter
    {
        private RuntimeEnvironment _globalEnvironment;
        private Dictionary<string, Class> _classes;
        private readonly IScripterConsole _console;
        private PluginManager _pluginManager;
        private FunctionRegistry _functionRegistry;

        public Interpreter(IScripterConsole console = null)
        {
            _console = console ?? new DefaultConsole();
            _globalEnvironment = new RuntimeEnvironment();
            _classes = new Dictionary<string, Class>();
            _pluginManager = new PluginManager(_console, this);
            _functionRegistry = new FunctionRegistry(_console);
            InitializeBuiltins();
        }

        // IInterpreter implementation
        public RuntimeEnvironment GlobalEnvironment => _globalEnvironment;

        public void RegisterFunction(string name, Function function)
        {
            _globalEnvironment.Define(name, new RuntimeValue(function));
        }

        public void ExecuteCode(string code)
        {
            // Простая реализация - можно расширить
            _console.WriteLine($"Выполнение кода: {code}");
        }

        /// <summary>
        /// Получить реестр функций для регистрации из C#
        /// </summary>
        public FunctionRegistry GetFunctionRegistry()
        {
            return _functionRegistry;
        }

        private void InitializeBuiltins()
        {
            // Built-in functions
            _globalEnvironment.Define("print", new RuntimeValue(new Function("print", new List<string> { "value" }, null, true, true)));
            _globalEnvironment.Define("input", new RuntimeValue(new Function("input", new List<string> { "prompt" }, null, true, true)));
            _globalEnvironment.Define("length", new RuntimeValue(new Function("length", new List<string> { "value" }, null, true, true)));
            
            // Plugin management functions
            _globalEnvironment.Define("loadPlugin", new RuntimeValue(new Function("loadPlugin", new List<string> { "pluginName" }, null, true, true)));
            _globalEnvironment.Define("unloadPlugin", new RuntimeValue(new Function("unloadPlugin", new List<string> { "pluginName" }, null, true, true)));
            _globalEnvironment.Define("listPlugins", new RuntimeValue(new Function("listPlugins", new List<string>(), null, true, true)));
        }

        public void Interpret(Program program)
        {
            try
            {
                foreach (var statement in program.Statements)
                {
                    Execute(statement, _globalEnvironment);
                }
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Runtime error: {ex.Message}");
            }
        }

        private RuntimeValue Execute(Statement statement, RuntimeEnvironment environment)
        {
            if (statement == null) return new RuntimeValue(null);
            
            switch (statement)
            {
                case ExpressionStatement exprStmt:
                    return Evaluate(exprStmt.Expression, environment);

                case VariableDeclaration varDecl:
                    var value = varDecl.Initializer != null ? Evaluate(varDecl.Initializer, environment) : new RuntimeValue(null);
                    environment.Define(varDecl.Name, value);
                    return value;

                case BlockStatement block:
                    return ExecuteBlock(block, new RuntimeEnvironment(environment));

                case IfStatement ifStmt:
                    return ExecuteIfStatement(ifStmt, environment);

                case WhileStatement whileStmt:
                    return ExecuteWhileStatement(whileStmt, environment);

                case ForStatement forStmt:
                    return ExecuteForStatement(forStmt, environment);

                case FunctionDeclaration funcDecl:
                    var function = new Function(funcDecl.Name, funcDecl.Parameters, funcDecl.Body, funcDecl.IsStatic, funcDecl.IsPublic, environment);
                    environment.Define(funcDecl.Name, new RuntimeValue(function));
                    return new RuntimeValue(function);

                case ClassDeclaration classDecl:
                    return ExecuteClassDeclaration(classDecl, environment);

                case PropertyDeclaration propDecl:
                    var propValue = propDecl.Initializer != null ? Evaluate(propDecl.Initializer, environment) : new RuntimeValue(null);
                    environment.Define(propDecl.Name, propValue);
                    return propValue;

                case ReturnStatement returnStmt:
                    var returnValue = returnStmt.Value != null ? Evaluate(returnStmt.Value, environment) : new RuntimeValue(null);
                    throw new ReturnException(returnValue);

                default:
                    throw new Exception($"Unknown statement type: {statement.GetType().Name}");
            }
        }

        private RuntimeValue ExecuteBlock(BlockStatement block, RuntimeEnvironment environment)
        {
            RuntimeValue lastValue = new RuntimeValue(null);

            foreach (var statement in block.Statements)
            {
                lastValue = Execute(statement, environment);
            }

            return lastValue;
        }

        private RuntimeValue ExecuteIfStatement(IfStatement ifStmt, RuntimeEnvironment environment)
        {
            if (Evaluate(ifStmt.Condition, environment).IsTruthy())
            {
                return Execute(ifStmt.ThenStatement, environment);
            }
            else if (ifStmt.ElseStatement != null)
            {
                return Execute(ifStmt.ElseStatement, environment);
            }

            return new RuntimeValue(null);
        }

        private RuntimeValue ExecuteWhileStatement(WhileStatement whileStmt, RuntimeEnvironment environment)
        {
            RuntimeValue lastValue = new RuntimeValue(null);

            while (Evaluate(whileStmt.Condition, environment).IsTruthy())
            {
                lastValue = Execute(whileStmt.Body, environment);
            }

            return lastValue;
        }

        private RuntimeValue ExecuteForStatement(ForStatement forStmt, RuntimeEnvironment environment)
        {
            var forEnvironment = new RuntimeEnvironment(environment);
            RuntimeValue lastValue = new RuntimeValue(null);

            if (forStmt.Initializer != null)
            {
                Execute(forStmt.Initializer, forEnvironment);
            }

            while (forStmt.Condition == null || Evaluate(forStmt.Condition, forEnvironment).IsTruthy())
            {
                lastValue = Execute(forStmt.Body, forEnvironment);

                if (forStmt.Increment != null)
                {
                    Execute(forStmt.Increment, forEnvironment);
                }
            }

            return lastValue;
        }

        private RuntimeValue ExecuteClassDeclaration(ClassDeclaration classDecl, RuntimeEnvironment environment)
        {
            var class_ = new Class(classDecl.Name, classDecl.IsStatic);
            _classes[classDecl.Name] = class_;

            var classEnvironment = new RuntimeEnvironment(environment);

            foreach (var member in classDecl.Members)
            {
                if (member is FunctionDeclaration funcDecl)
                {
                    var method = new Function(funcDecl.Name, funcDecl.Parameters, funcDecl.Body, funcDecl.IsStatic, funcDecl.IsPublic, classEnvironment);
                    class_.Methods[funcDecl.Name] = method;
                }
                else if (member is PropertyDeclaration propDecl)
                {
                    var value = propDecl.Initializer != null ? Evaluate(propDecl.Initializer, classEnvironment) : new RuntimeValue(null);
                    class_.Properties[propDecl.Name] = value;
                }
            }

            environment.Define(classDecl.Name, new RuntimeValue(class_));
            return new RuntimeValue(class_);
        }

        private RuntimeValue Evaluate(Expression expression, RuntimeEnvironment environment)
        {
            if (expression == null) return new RuntimeValue(null);
            
            
            switch (expression)
            {
                case NumberLiteral num:
                    return new RuntimeValue(num.Value);

                case StringLiteral str:
                    return new RuntimeValue(str.Value);

                case BooleanLiteral bool_:
                    return new RuntimeValue(bool_.Value);

                case Identifier identifier:
                    var value = environment.Get(identifier.Name);
                    // Check if this is a class and we need to create an instance
                    if (value.Value is Class class_)
                    {
                        return new RuntimeValue(class_.CreateInstance());
                    }
                    return value;

                case BinaryExpression binary:
                    return EvaluateBinaryExpression(binary, environment);

                case UnaryExpression unary:
                    return EvaluateUnaryExpression(unary, environment);

                case FunctionCall call:
                    return EvaluateFunctionCall(call, environment);

                case MemberAccess member:
                    return EvaluateMemberAccess(member, environment);

                case AssignmentExpression assignment:
                    return EvaluateAssignment(assignment, environment);

                default:
                    throw new Exception($"Unknown expression type: {expression.GetType().Name}");
            }
        }

        private RuntimeValue EvaluateBinaryExpression(BinaryExpression binary, RuntimeEnvironment environment)
        {
            var left = Evaluate(binary.Left, environment);
            var right = Evaluate(binary.Right, environment);

            switch (binary.Operator)
            {
                case TokenType.Plus:
                    if (left.Value is double && right.Value is double)
                        return new RuntimeValue(left.GetValue<double>() + right.GetValue<double>());
                    if (left.Value is string || right.Value is string)
                        return new RuntimeValue(left.ToString() + right.ToString());
                    break;

                case TokenType.Minus:
                    return new RuntimeValue(left.GetValue<double>() - right.GetValue<double>());

                case TokenType.Multiply:
                    return new RuntimeValue(left.GetValue<double>() * right.GetValue<double>());

                case TokenType.Divide:
                    var divisor = right.GetValue<double>();
                    if (divisor == 0) throw new Exception("Division by zero");
                    return new RuntimeValue(left.GetValue<double>() / divisor);

                case TokenType.Modulo:
                    return new RuntimeValue(left.GetValue<double>() % right.GetValue<double>());

                case TokenType.Equal:
                    return new RuntimeValue(IsEqual(left, right));

                case TokenType.NotEqual:
                    return new RuntimeValue(!IsEqual(left, right));

                case TokenType.GreaterThan:
                    return new RuntimeValue(left.GetValue<double>() > right.GetValue<double>());

                case TokenType.GreaterThanOrEqual:
                    return new RuntimeValue(left.GetValue<double>() >= right.GetValue<double>());

                case TokenType.LessThan:
                    return new RuntimeValue(left.GetValue<double>() < right.GetValue<double>());

                case TokenType.LessThanOrEqual:
                    return new RuntimeValue(left.GetValue<double>() <= right.GetValue<double>());

                case TokenType.And:
                    return new RuntimeValue(left.IsTruthy() && right.IsTruthy());

                case TokenType.Or:
                    return new RuntimeValue(left.IsTruthy() || right.IsTruthy());
            }

            throw new Exception($"Unknown binary operator: {binary.Operator}");
        }

        private RuntimeValue EvaluateUnaryExpression(UnaryExpression unary, RuntimeEnvironment environment)
        {
            var right = Evaluate(unary.Operand, environment);

            switch (unary.Operator)
            {
                case TokenType.Minus:
                    return new RuntimeValue(-right.GetValue<double>());

                case TokenType.Not:
                    return new RuntimeValue(!right.IsTruthy());
            }

            throw new Exception($"Unknown unary operator: {unary.Operator}");
        }

        private RuntimeValue EvaluateFunctionCall(FunctionCall call, RuntimeEnvironment environment)
        {
            var callee = Evaluate(call.Function, environment);
            
            // Built-in functions - check by name directly first
            if (call.Function is Identifier identifier && IsBuiltinFunction(identifier.Name))
            {
                return CallBuiltinFunction(identifier.Name, call.Arguments, environment);
            }

            // Plugin functions - check if it's a plugin function
            if (call.Function is Identifier pluginIdentifier && _pluginManager.IsFunctionAvailable(pluginIdentifier.Name))
            {
                var pluginArgs = call.Arguments.Select(arg => Evaluate(arg, environment)).ToList();
                return _pluginManager.CallFunction(pluginIdentifier.Name, pluginArgs);
            }

            // Registered functions - check if it's a registered function
            if (call.Function is Identifier regIdentifier && _functionRegistry.IsFunctionRegistered(regIdentifier.Name))
            {
                var regArgs = call.Arguments.Select(arg => Evaluate(arg, environment)).ToList();
                return _functionRegistry.CallFunction(regIdentifier.Name, regArgs);
            }

            if (callee.Value is Function function)
            {
                return CallFunction(function, call.Arguments, environment);
            }

            throw new Exception("Can only call functions");
        }

        private RuntimeValue CallFunction(Function function, List<Expression> arguments, RuntimeEnvironment environment)
        {
            if (arguments.Count != function.Parameters.Count)
            {
                throw new Exception($"Expected {function.Parameters.Count} arguments but got {arguments.Count}");
            }

            var functionEnvironment = new RuntimeEnvironment(function.Closure);

            for (int i = 0; i < function.Parameters.Count; i++)
            {
                var value = Evaluate(arguments[i], environment);
                functionEnvironment.Define(function.Parameters[i], value);
            }

            try
            {
                return Execute(function.Body, functionEnvironment);
            }
            catch (ReturnException returnEx)
            {
                return returnEx.Value;
            }
        }

        private RuntimeValue CallBuiltinFunction(string name, List<Expression> arguments, RuntimeEnvironment environment)
        {
            switch (name)
            {
                case "print":
                    if (arguments.Count > 0)
                    {
                        var value = Evaluate(arguments[0], environment);
                        _console.WriteLine(value.ToString());
                    }
                    else
                    {
                        _console.WriteLine();
                    }
                    return new RuntimeValue(null);

                case "input":
                    if (arguments.Count > 0)
                    {
                        var prompt = Evaluate(arguments[0], environment).ToString();
                        _console.Write(prompt);
                    }
                    return new RuntimeValue(_console.ReadLine());

                case "length":
                    if (arguments.Count > 0)
                    {
                        var value = Evaluate(arguments[0], environment);
                        if (value.Value is string str)
                            return new RuntimeValue(str.Length);
                        if (value.Value is System.Collections.ICollection collection)
                            return new RuntimeValue(collection.Count);
                    }
                    return new RuntimeValue(0);

                case "loadPlugin":
                    if (arguments.Count > 0)
                    {
                        var pluginName = Evaluate(arguments[0], environment).ToString();
                        return LoadPlugin(pluginName);
                    }
                    return new RuntimeValue(false);

                case "unloadPlugin":
                    if (arguments.Count > 0)
                    {
                        var pluginName = Evaluate(arguments[0], environment).ToString();
                        _pluginManager.UnloadPlugin(pluginName);
                        return new RuntimeValue(true);
                    }
                    return new RuntimeValue(false);

                case "listPlugins":
                    var plugins = _pluginManager.GetLoadedPlugins();
                    var pluginList = string.Join(", ", plugins.Select(p => p.Name));
                    _console.WriteLine($"Загруженные плагины: {pluginList}");
                    return new RuntimeValue(pluginList);


                default:
                    throw new Exception($"Unknown built-in function: {name}");
            }
        }

        private RuntimeValue EvaluateMemberAccess(MemberAccess member, RuntimeEnvironment environment)
        {
            var obj = Evaluate(member.Object, environment);

            if (obj.Value is ClassInstance instance)
            {
                return instance.GetProperty(member.Member);
            }

            if (obj.Value is Class class_)
            {
                if (class_.Methods.ContainsKey(member.Member))
                {
                    return new RuntimeValue(class_.Methods[member.Member]);
                }
                if (class_.Properties.ContainsKey(member.Member))
                {
                    return class_.Properties[member.Member];
                }
            }

            throw new Exception($"Property '{member.Member}' not found");
        }

        private RuntimeValue EvaluateAssignment(AssignmentExpression assignment, RuntimeEnvironment environment)
        {
            var value = Evaluate(assignment.Right, environment);

            if (assignment.Left is Identifier identifier)
            {
                switch (assignment.Operator)
                {
                    case TokenType.Assignment:
                        environment.Assign(identifier.Name, value);
                        break;
                    case TokenType.PlusAssignment:
                        var current = environment.Get(identifier.Name);
                        var newValue = new RuntimeValue(current.GetValue<double>() + value.GetValue<double>());
                        environment.Assign(identifier.Name, newValue);
                        break;
                    case TokenType.MinusAssignment:
                        current = environment.Get(identifier.Name);
                        newValue = new RuntimeValue(current.GetValue<double>() - value.GetValue<double>());
                        environment.Assign(identifier.Name, newValue);
                        break;
                }
                return value;
            }

            if (assignment.Left is MemberAccess memberAccess)
            {
                var obj = Evaluate(memberAccess.Object, environment);
                if (obj.Value is ClassInstance instance)
                {
                    instance.SetProperty(memberAccess.Member, value);
                    return value;
                }
                throw new Exception($"Cannot assign to property '{memberAccess.Member}' of non-object");
            }

            throw new Exception("Invalid assignment target");
        }

        private bool IsEqual(RuntimeValue left, RuntimeValue right)
        {
            if (left.Value == null && right.Value == null) return true;
            if (left.Value == null) return false;

            return left.Value.Equals(right.Value);
        }

        private bool IsBuiltinFunction(string name)
        {
            return name == "print" || name == "input" || name == "length" || 
                   name == "loadPlugin" || name == "unloadPlugin" || name == "listPlugins";
        }

        private RuntimeValue LoadPlugin(string pluginName)
        {
            try
            {
                switch (pluginName.ToLower())
                {
                    case "csharp":
                    case "csharpsyntax":
                        var csharpPlugin = new Plugins.CSharpSyntaxPlugin(_console);
                        _pluginManager.LoadPlugin(csharpPlugin);
                        return new RuntimeValue(true);
                    
                    default:
                        _console.WriteLine($"Плагин '{pluginName}' не найден");
                        return new RuntimeValue(false);
                }
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Ошибка загрузки плагина '{pluginName}': {ex.Message}");
                return new RuntimeValue(false);
            }
        }

    }

    public class ReturnException : Exception
    {
        public RuntimeValue Value { get; }

        public ReturnException(RuntimeValue value) : base()
        {
            Value = value;
        }
    }
}
