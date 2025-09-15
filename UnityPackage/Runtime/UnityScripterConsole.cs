using UnityEngine;

namespace Scripter.Unity
{
    public class UnityScripterConsole : Scripter.Core.Interfaces.IScripterConsole
    {
        private readonly bool _showDebugInfo;
        
        public UnityScripterConsole(bool showDebugInfo = true)
        {
            _showDebugInfo = showDebugInfo;
        }
        
        public void Write(string value)
        {
            if (_showDebugInfo)
            {
                Debug.Log($"[Scripter] {value}");
            }
        }
        
        public void WriteLine(string message)
        {
            if (_showDebugInfo)
            {
                Debug.Log($"[Scripter] {message}");
            }
        }
        
        public void WriteLine()
        {
            if (_showDebugInfo)
            {
                Debug.Log("[Scripter]");
            }
        }
        
        public void WriteLine(object value)
        {
            var message = value?.ToString() ?? "null";
            WriteLine(message);
        }
        
        public string ReadLine()
        {
            return string.Empty;
        }
        
        public int Read()
        {
            return 0;
        }
        
        public bool IsInputRedirected => true;
    }
}
