# Sharp Language Support for VS Code

This extension provides Sharp language support for Visual Studio Code, including syntax highlighting, autocompletion, and snippets.

## Features

- 🎨 **Enhanced Syntax Highlighting** - Full Sharp language syntax support with keyword categorization
- 🌈 **Color Scheme** - Built-in "Sharp Dark" theme with optimized colors
- 📝 **Snippets** - Ready-to-use templates for quick code writing
- 🔧 **Auto-closing Brackets** - Automatic closing of brackets, quotes, and other symbols
- 📏 **Indentation Rules** - Automatic indentation formatting
- 💬 **Comments** - Support for single-line and multi-line comments

## Supported Constructs

### Keywords (with color categorization)

#### 🔵 Declaration Keywords (blue, bold)
- `var` - variable declaration
- `function` - function declaration  
- `class` - class declaration
- `static` - static classes

#### 🟡 Access Modifiers (yellow, italic)
- `public` - public access
- `private` - private access

#### 🟣 Flow Control Keywords (purple, bold)
- `if`, `else` - conditional statements
- `while`, `for` - loops
- `return` - return value

#### 🔵 Literals (blue)
- `true`, `false` - boolean values
- `null` - null value

#### 🟢 Special Keywords (green)
- `new` - object creation
- `this` - reference to current object
- `base` - reference to base class

### Operators
- Arithmetic: `+`, `-`, `*`, `/`, `%`
- Comparison: `==`, `!=`, `<`, `>`, `<=`, `>=`
- Logical: `&&`, `||`, `!`
- Assignment: `=`, `+=`, `-=`

### Data Types
- Numbers: `123`, `45.67`
- Strings: `"hello"`, `'world'`
- Boolean values: `true`, `false`
- Null: `null`

## Snippets

Available snippets (type prefix and press Tab):

- `func` - function declaration
- `class` - class declaration
- `static` - static class
- `var` - variable declaration
- `if` - conditional statement
- `ifelse` - conditional statement with else
- `while` - while loop
- `for` - for loop
- `print` - console output
- `input` - console input
- `return` - return value
- `//` - single-line comment
- `/*` - multi-line comment

## Installation

1. Copy the extension folder to VS Code extensions directory
2. Restart VS Code
3. Extension automatically activates for files with `.sharp` extension

## Usage

After installing the extension:

1. Open a file with `.sharp` extension
2. Enjoy enhanced syntax highlighting
3. Use snippets for quick code writing
4. Autocompletion and formatting work automatically

### "Sharp Dark" Color Scheme

The extension includes a built-in "Sharp Dark" theme optimized for Sharp language:

- **Declaration Keywords** - blue, bold font
- **Access Modifiers** - yellow, italic
- **Flow Control Keywords** - purple, bold
- **Literals** - blue
- **Special Keywords** - green
- **Strings** - orange
- **Numbers** - light green
- **Functions** - yellow
- **Classes** - green, bold
- **Variables** - light blue
- **Comments** - green, italic

To activate the theme:
1. Press `Ctrl+Shift+P` (or `Cmd+Shift+P` on Mac)
2. Type "Preferences: Color Theme"
3. Select "Sharp Dark"

## Examples

### Simple Function
```sharp
function greet(name) {
    print("Hello, " + name + "!");
}

greet("World");
```

### Class with Methods
```sharp
class Person {
    var name;
    var age;
    
    function Person(n, a) {
        name = n;
        age = a;
    }
    
    function sayHello() {
        print("Hi, I'm " + name);
    }
}

var person = new Person("Alice", 25);
person.sayHello();
```

### Conditional Statements and Loops
```sharp
var numbers = [1, 2, 3, 4, 5];
var sum = 0;

for (var i = 0; i < 5; i = i + 1) {
    if (numbers[i] > 2) {
        sum = sum + numbers[i];
    }
}

print("Sum: " + sum);
```

## Development

For extension development:

1. Install Node.js and npm
2. Install dependencies: `npm install`
3. Run in development mode: `F5` in VS Code

## License

MIT License

## Support

If you have questions or suggestions, create an issue in the project repository.