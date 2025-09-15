using UnityEngine;
using Scripter.Core;
using Scripter.Core.Services;
using Scripter.Core.Domain;
using Scripter.Infrastructure.FileSystem;
using System;
using System.Linq;

namespace Scripter.Unity
{
    public class UnityScriptService : MonoBehaviour
    {
        private static UnityScriptService _instance;
        public static UnityScriptService Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("UnityScriptService");
                    _instance = go.AddComponent<UnityScriptService>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        
        public bool showDebugInfo = true;
        
        private Interpreter _interpreter;
        private UnityScripterConsole _console;
        private FileService _fileService;
        private UnityPlugin _unityPlugin;
        private bool _isInitialized = false;
        
        public event Action<string, bool> OnScriptExecuted;
        public event Action<string, Exception> OnScriptError;
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        private void Initialize()
        {
            if (_isInitialized) return;
            
            try
            {
                _console = new UnityScripterConsole(showDebugInfo);
                _fileService = new FileService();
                _interpreter = new Interpreter(_console);
                
                _unityPlugin = new UnityPlugin();
                _unityPlugin.Initialize(_interpreter);
                
                RegisterUnityFunctions();
                RegisterSharpExtensions();
                
                _isInitialized = true;
                
                if (showDebugInfo)
                {
                    Debug.Log("[UnityScriptService] Service initialized successfully with Unity Plugin");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UnityScriptService] Ошибка инициализации: {ex.Message}");
                OnScriptError?.Invoke("Ошибка инициализации", ex);
            }
        }
        
        public static void ExecuteScript(SharpScript sharpScript)
        {
            if (sharpScript == null)
            {
                Debug.LogError("[UnityScriptService] SharpScript не может быть null");
                return;
            }
            
            if (!sharpScript.IsValid())
            {
                Debug.LogError("[UnityScriptService] SharpScript не содержит валидного кода");
                return;
            }
            
            ExecuteScript(sharpScript.GetScriptContent());
        }
        
        public static void ExecuteScript(string scriptContent)
        {
            if (string.IsNullOrEmpty(scriptContent))
            {
                Debug.LogError("[UnityScriptService] Содержимое скрипта не может быть пустым");
                return;
            }
            
            var instance = Instance;
            
            try
            {
                if (instance.showDebugInfo)
                {
                    Debug.Log($"[UnityScriptService] Executing script ({scriptContent.Length} characters)");
                }
                
                instance._interpreter.Interpret(scriptContent);
                
                instance.OnScriptExecuted?.Invoke("Script executed successfully", true);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UnityScriptService] Ошибка выполнения скрипта: {ex.Message}");
                instance.OnScriptError?.Invoke(scriptContent, ex);
                instance.OnScriptExecuted?.Invoke(ex.Message, false);
            }
        }
        
        public static bool IsInitialized()
        {
            return Instance._isInitialized;
        }
        
        public static ExecutionStatistics GetStatistics()
        {
            return new ExecutionStatistics
            {
                lastExecutionTime = DateTime.Now,
                isInitialized = IsInitialized()
            };
        }
        
        private void RegisterUnityFunctions()
        {
            var unityFunctions = new[]
            {
                "Unity.FindGameObject", "Unity.FindGameObjectWithTag", "Unity.GetTransform",
                "Unity.GetTransformPosition", "Unity.SetTransformPosition", "Unity.GetTransformRotation",
                "Unity.SetTransformRotation", "Unity.GetTransformScale", "Unity.SetTransformScale",
                "Unity.GetComponent", "Unity.AddComponent", "Unity.DestroyGameObject",
                "Unity.InstantiateGameObject", "Unity.IsGameObjectActive", "Unity.SetGameObjectActive",
                "Unity.GetGameObjectName", "Unity.SetGameObjectName", "Unity.GetGameObjectTag",
                "Unity.SetGameObjectTag", "Unity.GetGameObjectLayer", "Unity.SetGameObjectLayer",
                "Unity.GetParent", "Unity.SetParent", "Unity.GetChild", "Unity.GetChildCount",
                "Unity.GetDistance", "Unity.LookAt", "Unity.MoveTowards", "Unity.RotateTowards"
            };
            
            foreach (var funcName in unityFunctions)
            {
                var sharpFunction = new Function(funcName, new List<string>(), null, true, true);
                _interpreter.RegisterFunction(funcName, sharpFunction);
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"[UnityScriptService] Registered {unityFunctions.Length} Unity functions");
            }
        }
        
        private void RegisterSharpExtensions()
        {
            var customFunctions = SharpExtensions.GetAllFunctions();
            foreach (var func in customFunctions)
            {
                var sharpFunction = new Function(func.Key, new List<string>(), null, true, true);
                _interpreter.RegisterFunction(func.Key, sharpFunction);
            }
            
            if (showDebugInfo && customFunctions.Count > 0)
            {
                Debug.Log($"[UnityScriptService] Registered {customFunctions.Count} custom functions");
            }
        }
    }
    
    [System.Serializable]
    public class ExecutionStatistics
    {
        public DateTime lastExecutionTime = DateTime.MinValue;
        public bool isInitialized = false;
    }
}