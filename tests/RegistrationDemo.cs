using System;
using Scripter.Core;
using Scripter.Core.Domain;
using Scripter.Extensions;
using Scripter.Infrastructure.Console;

namespace Scripter.Examples
{
    /// <summary>
    /// Demonstration of function registration from C# code
    /// </summary>
    public class RegistrationDemo
    {
        public static void RunDemo()
        {
            Console.WriteLine("=== Function Registration from C# Demonstration ===");

            // Create interpreter
            var console = new DefaultConsole();
            var interpreter = new Interpreter(console);
            var registry = interpreter.GetFunctionRegistry();

            // Register functions with simplified syntax
            Console.WriteLine("\n1. Registering functions...");

            // Function with one parameter
            registry.RegisterFunction<int>("DoSomething", addTo => new RuntimeValue(addTo + 1), "Adds 1 to a number");

            // Function for string operations
            registry.RegisterFunction<string>("Greet", name => new RuntimeValue($"Hello, {name}!"), "Greets the user");

            // Function with two parameters
            registry.RegisterFunction<int, int>("Add", (a, b) => new RuntimeValue(a + b), "Adds two numbers");

            Console.WriteLine("All functions registered!");

            // Now you can execute Sharp script that will use these functions
            Console.WriteLine("\n2. Functions are ready to use in Sharp scripts:");
            Console.WriteLine("   - DoSomething(5) will return 6");
            Console.WriteLine("   - Greet(\"World\") will return \"Hello, World!\"");
            Console.WriteLine("   - Add(3, 4) will return 7");

            Console.WriteLine("\n=== Demonstration completed! ===");
        }
    }
}
