# Scripter - C# Script Interpreter

A full-featured script interpreter written in C# that can convert .sharp files into executable scripts with support for functions, classes, and static classes.

## Features

- ✅ **Variables and Data Types**: numbers, strings, boolean values
- ✅ **Arithmetic Operations**: +, -, *, /, %
- ✅ **Logical Operations**: &&, ||, !
- ✅ **Comparison Operators**: ==, !=, <, >, <=, >=
- ✅ **Conditional Statements**: if/else
- ✅ **Loops**: while, for
- ✅ **Functions**: declaration, call, parameters, return values
- ✅ **Classes**: creation, properties, methods, instances
- ✅ **Static Classes**: static methods and properties
- ✅ **Built-in Functions**: print(), input(), length()
- ✅ **Closures**: functions can return other functions
- ✅ **Recursion**: support for recursive calls
- ✅ **Optimizations**: readonly struct, ref struct, aggressive inlining
- ✅ **Modularity**: IScripterConsole interface for custom output
- ✅ **SOLID Principles**: separation of concerns, dependency inversion
- ✅ **Clean Architecture**: Core, Infrastructure, Extensions layers

## Installation and Running

1. Make sure you have .NET 6.0 or higher installed
2. Clone or download the project
3. Compile the project:
   ```bash
   dotnet build
   ```
4. Run the interpreter:
   ```bash
   dotnet run
   ```

### VS Code Extension
For convenient work with `.sharp` files, install the VS Code extension:

1. **Go to the folder** `VsCodeExtension`
2. **Follow the instructions** in `INSTALL.md`
3. **Enjoy** syntax highlighting and autocompletion!

## Project Structure

```
Scripter/
├── Scripter/                    # Main interpreter code
│   ├── Core/                    # Domain logic
│   │   ├── Domain/              # Domain models
│   │   │   ├── Token.cs         # Tokens (readonly struct)
│   │   │   ├── AST.cs           # Abstract Syntax Tree
│   │   │   ├── Runtime.cs       # Runtime environment
│   │   │   └── Position.cs      # Ref struct for position
│   │   ├── Lexer.cs             # Lexical analyzer
│   │   ├── Parser.cs            # Syntax analyzer
│   │   ├── Interpreter.cs       # Interpreter
│   │   └── ScriptService.cs     # Script execution service
│   ├── Infrastructure/          # External dependencies
│   │   ├── FileSystem/          # File system operations
│   │   │   ├── IFileService.cs  # File service interface
│   │   │   └── FileService.cs   # File service implementation
│   │   └── Console/             # Console output
│   │       ├── IScripterConsole.cs  # Output interface
│   │       └── DefaultConsole.cs    # Standard implementation
│   └── Extensions/              # Extensions
│       ├── StringHelper.cs      # Optimized string methods
│       └── CollectionExtensions.cs # Collection extensions
├── tests/                       # Test scripts (.sharp)
├── VsCodeExtension/             # VS Code extension for .sharp
│   ├── package.json             # Extension configuration
│   ├── syntaxes/                # Grammar for syntax highlighting
│   ├── language-configuration/  # Language configuration
│   ├── snippets/                # Code snippets
│   └── README.md                # Extension documentation
├── UnityPackage/                # Unity package
│   ├── Runtime/                 # Runtime scripts
│   ├── Editor/                  # Editor scripts
│   ├── Samples~/                # Example scripts
│   └── package.json             # Unity package manifest
├── Scripter.csproj              # Project file
└── README.md                    # Documentation
```

## Architecture

The project is built according to **SOLID** and **Clean Architecture** principles:

### 🏗️ **Architecture Layers:**

1. **Core** - Domain logic
   - **Domain** - Domain models (Token, AST, Runtime)
   - **Lexer, Parser, Interpreter** - Main interpreter components
   - **ScriptService** - Service for script execution

2. **Infrastructure** - External dependencies
   - **FileSystem** - File system operations

3. **Console** - Console output
   - Abstraction for console output

4. **Extensions** - Extensions
   - Helper methods and optimizations

### 🔧 **SOLID Principles:**

- **S** - Single Responsibility: each class has one responsibility
- **O** - Open/Closed: open for extension, closed for modification
- **L** - Liskov Substitution: interfaces can be replaced with implementations
- **I** - Interface Segregation: interfaces are separated by functionality
- **D** - Dependency Inversion: dependencies through interfaces

## Usage

### Interactive Mode
```bash
dotnet run
```
Then enter commands or load a file:
```
scripter> load example_script
```

### Run with File
```bash
dotnet run tests/example_script.sharp
```

### Available Test Scripts

In the `tests/` folder, you can find the following test scripts:

- **basic_test.sharp** - Basic operations (variables, arithmetic, conditions)
- **working_test.sharp** - Working test with conditions
- **minimal_test.sharp** - Minimal test
- **debug_test.sharp** - Debug test
- **simple_test.sharp** - Simple test with functions
- **simple_example.sharp** - Simple example
- **simple_function.sharp** - Function test
- **function_test.sharp** - Function test
- **test_function_call.sharp** - Function call test
- **example_script.sharp** - Extended example
- **advanced_example.sharp** - Advanced example

Example run:
```bash
dotnet run tests/basic_test.sharp
dotnet run tests/working_test.sharp
```

## Syntax

### Variables
```javascript
var x = 10;
var name = "Hello";
var isActive = true;
```

### Functions
```javascript
function add(a, b) {
    return a + b;
}

function greet(name) {
    return "Hello, " + name;
}
```

### Classes
```javascript
class Person {
    public name;
    public age;
    
    function greet() {
        return "Hello, my name is " + this.name;
    }
}

var person = new Person();
person.name = "Alex";
print(person.greet());
```

### Static Classes
```javascript
static class MathUtils {
    static function square(x) {
        return x * x;
    }
    
    static function factorial(n) {
        if (n <= 1) {
            return 1;
        } else {
            return n * factorial(n - 1);
        }
    }
}

print(MathUtils.square(5));
```

### Conditions and Loops
```javascript
if (x > 0) {
    print("Positive number");
} else {
    print("Negative or zero");
}

var i = 0;
while (i < 10) {
    print("Iteration: " + i);
    i = i + 1;
}
```

### Built-in Functions
```javascript
print("Hello, world!");
var input = input("Enter your name: ");
var len = length("String");
```

## Examples

The project includes test scripts in the `tests/` folder:
- `basic_test.sharp` - basic operations (variables, arithmetic, conditions)
- `working_test.sharp` - working test with conditions
- `minimal_test.sharp` - minimal test
- `simple_test.sharp` - simple test with functions
- `function_test.sharp` - function tests
- `plugin_example.sharp` - plugin usage examples

## Unity Integration

The project includes a Unity package for easy integration:

### Features
- **ScriptableObject Integration** - Store Sharp scripts as Unity assets
- **Syntax Highlighting** - Custom editor with syntax highlighting
- **Unity Plugin** - Access to GameObject functions
- **Custom Functions** - Register your own functions via SharpExtensions
- **Auto Execution** - Scripts can execute automatically when dragged to scene

### Installation
Add from GitHub: `https://github.com/kitakun/ScripterSharp.git?path=UnityPackage`

### Usage
1. Create SharpScript asset via `Create > Scripter > Sharp Script`
2. Write your Sharp code with Unity functions
3. Drag to scene for automatic execution (if enabled)

## Limitations

- No array support (planned for future versions)
- Limited string operations
- No exception support
- Simple type system

## Development

To add new features:

1. Add new tokens in `Token.cs`
2. Update the lexer in `Lexer.cs`
3. Add new AST nodes in `AST.cs`
4. Update the parser in `Parser.cs`
5. Implement execution in `Interpreter.cs`

## License

This project is created for educational purposes and can be freely used and modified.