# Scripter Unity Package

Unity package for executing Sharp scripts with syntax highlighting and custom editor.

## Features

- 🎯 **ScriptableObject Integration** - Store Sharp scripts as Unity assets
- 🎨 **Syntax Highlighting** - Custom editor with syntax highlighting
- 🚀 **Performance Optimized** - Minimal GC allocation
- 🔧 **Custom Editor** - Rich editor with statistics and validation
- 📊 **Script Statistics** - Line count, comment percentage, and more
- 🎮 **Unity Integration** - Uses Debug.Log instead of Console.WriteLine
- 🎯 **Unity Plugin** - Access to GameObject functions like Find, transform.position, GetComponent, etc.

## Installation

### From GitHub (Recommended)

1. Open Unity Package Manager
2. Click the "+" button
3. Select "Add package from git URL"
4. Enter: `https://github.com/kitakun/ScripterSharp.git?path=UnityPackage`

### Manual Installation

1. Download the UnityPackage folder
2. Copy it to your project's Packages folder
3. Refresh Unity

## Usage

### Creating a Sharp Script

1. Right-click in Project window
2. Select `Create > Scripter > Sharp Script`
3. Open the script in Inspector
4. Write your Sharp code in the text area
5. Click "Execute Script" to run

### Basic Example

```sharp
// Simple variable and arithmetic
var x = 10;
var y = 5;
var sum = x + y;
print("Sum: " + sum);

// Functions
function add(a, b) {
    return a + b;
}

print("Result: " + add(3, 4));

// Classes
class Person {
    public name;
    public age;
    
    function greet() {
        return "Hello, I'm " + this.name;
    }
}

var person = new Person();
person.name = "John";
person.age = 25;
print(person.greet());
```

### Unity GameObject Functions

The Unity Plugin provides access to GameObject functions using static syntax:

```sharp
// Find GameObject by name
var player = Unity.FindGameObject("Player");

// Get and set position
var pos = Unity.GetTransformPosition(player);
Unity.SetTransformPosition(player, 10, 5, 0);

// Get and set rotation
var rot = Unity.GetTransformRotation(player);
Unity.SetTransformRotation(player, 0, 90, 0);

// Get and set scale
var scale = Unity.GetTransformScale(player);
Unity.SetTransformScale(player, 2, 2, 2);

// Get component
var transform = Unity.GetComponent(player, "Transform");

// Set active state
Unity.SetGameObjectActive(player, true);

// Get child count and children
var childCount = Unity.GetChildCount(player);
var firstChild = Unity.GetChild(player, 0);

// Calculate distance between objects
var distance = Unity.GetDistance(player, enemy);

// Make object look at another
Unity.LookAt(player, enemy);

// Create and destroy objects
var newObject = Unity.InstantiateGameObject(player);
Unity.DestroyGameObject(newObject);
```

#### Available Unity Functions

**GameObject Operations:**
- `Unity.FindGameObject(name)` - Find GameObject by name
- `Unity.FindGameObjectWithTag(tag)` - Find GameObject by tag
- `Unity.GetGameObjectName(go)` - Get GameObject name
- `Unity.SetGameObjectName(go, name)` - Set GameObject name
- `Unity.IsGameObjectActive(go)` - Check if GameObject is active
- `Unity.SetGameObjectActive(go, active)` - Set GameObject active state
- `Unity.GetGameObjectTag(go)` - Get GameObject tag
- `Unity.SetGameObjectTag(go, tag)` - Set GameObject tag
- `Unity.GetGameObjectLayer(go)` - Get GameObject layer
- `Unity.SetGameObjectLayer(go, layer)` - Set GameObject layer

**Transform Operations:**
- `Unity.GetTransform(go)` - Get Transform component
- `Unity.GetTransformPosition(go)` - Get world position
- `Unity.SetTransformPosition(go, x, y, z)` - Set world position
- `Unity.GetTransformRotation(go)` - Get world rotation
- `Unity.SetTransformRotation(go, x, y, z)` - Set world rotation
- `Unity.GetTransformScale(go)` - Get local scale
- `Unity.SetTransformScale(go, x, y, z)` - Set local scale

**Hierarchy Operations:**
- `Unity.GetParent(go)` - Get parent GameObject
- `Unity.SetParent(go, parent)` - Set parent GameObject
- `Unity.GetChild(go, index)` - Get child by index
- `Unity.GetChildCount(go)` - Get number of children

**Component Operations:**
- `Unity.GetComponent(go, type)` - Get component by type name
- `Unity.AddComponent(go, type)` - Add component by type name

**Object Lifecycle:**
- `Unity.InstantiateGameObject(go)` - Instantiate GameObject
- `Unity.DestroyGameObject(go)` - Destroy GameObject

**Utility Functions:**
- `Unity.GetDistance(go1, go2)` - Calculate distance between objects
- `Unity.LookAt(go, target)` - Make object look at target
- `Unity.MoveTowards(go, x, y, z)` - Move towards position
- `Unity.RotateTowards(go, x, y, z)` - Rotate towards rotation

### Simplified Usage

**Automatic Execution:**
1. Create a SharpScript asset
2. Enable `Auto Execute`
3. Drag to scene - script will execute automatically!

**Manual Execution:**
```csharp
// Through singleton (recommended)
UnityScriptService.ExecuteScript(sharpScript);

// Or through SharpScript itself
sharpScript.ExecuteScript();
```

### Custom Functions with SharpExtensions

Register your own functions for use in Sharp scripts:

```csharp
using Scripter.Unity;

public class MyScript : MonoBehaviour
{
    void Start()
    {
        // Simple function without parameters
        SharpExtensions.RegisterFunction("GetCurrentTime", () => 
        {
            return new RuntimeValue(System.DateTime.Now.ToString());
        });
        
        // Function with parameters
        SharpExtensions.RegisterFunction<int, int>("Add", (a, b) => 
        {
            return new RuntimeValue(a + b);
        });
        
        // Function for working with GameObject
        SharpExtensions.RegisterFunction<GameObject>("GetPlayerInfo", (player) => 
        {
            return new RuntimeValue($"Player: {player.name}");
        });
    }
}
```

**Usage in Sharp script:**
```sharp
// Call custom functions
var time = GetCurrentTime();
var sum = Add(5, 10);
var player = Unity.FindGameObject("Player");
var info = GetPlayerInfo(player);

print("Time: " + time);
print("Sum: " + sum);
print("Info: " + info);
```

### Using UnityScriptService

```csharp
using Scripter.Unity;
using UnityEngine;

public class ScriptRunner : MonoBehaviour
{
    public SharpScript script;
    
    void Start()
    {
        // Service is automatically initialized as singleton
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UnityScriptService.ExecuteScript(script);
        }
    }
}
```

## API Reference

### SharpScript

- `string scriptContent` - The Sharp script code
- `bool autoExecute` - Auto-execute on load
- `bool showDebugInfo` - Show debug information
- `string description` - Script description
- `string version` - Script version
- `string createdDate` - Creation date (read-only)
- `string lastModified` - Last modification date (read-only)

### UnityScriptService (Singleton)

- `static void ExecuteScript(SharpScript script)` - Execute a Sharp script
- `static void ExecuteScript(string content)` - Execute script from string
- `static bool IsInitialized()` - Check if service is initialized
- `static ExecutionStatistics GetStatistics()` - Get execution statistics

### SharpExtensions

- `static void RegisterFunction(string name, Func<RuntimeValue> function)` - Register function without parameters
- `static void RegisterFunction<T>(string name, Func<T, RuntimeValue> function)` - Register function with one parameter
- `static void RegisterFunction<T1, T2>(string name, Func<T1, T2, RuntimeValue> function)` - Register function with two parameters
- `static void RegisterFunction<T1, T2, T3>(string name, Func<T1, T2, T3, RuntimeValue> function)` - Register function with three parameters
- `static bool IsFunctionRegistered(string name)` - Check if function is registered
- `static void UnregisterFunction(string name)` - Remove registered function
- `static void ClearAllFunctions()` - Clear all registered functions

### Events

- `OnScriptExecuted(string output, bool success)` - Fired when script execution completes
- `OnScriptError(string content, Exception error)` - Fired when script execution fails

## Performance Notes

- Uses StringBuilder for efficient string operations
- Minimal GC allocation
- Pre-allocated buffers for common operations
- Optimized statistics calculation
- Singleton pattern for UnityScriptService reduces memory usage

## Requirements

- Unity 2021.3 or later
- .NET Standard 2.1

## License

MIT License - see LICENSE file for details

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## Support

- GitHub Issues: https://github.com/kitakun/ScripterSharp/issues
- Documentation: https://github.com/kitakun/ScripterSharp/wiki