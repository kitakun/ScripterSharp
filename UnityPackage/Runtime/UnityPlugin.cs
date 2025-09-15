using UnityEngine;
using Scripter.Core;
using Scripter.Core.Domain;
using Scripter.Core.Plugins;
using Scripter.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Scripter.Unity
{
    public class UnityPlugin : IPlugin
    {
        public string Name => "Unity Plugin";
        public string Version => "1.0.0";
        public string Description => "Provides access to Unity GameObject functions";
        
        private readonly Dictionary<string, Func<List<RuntimeValue>, RuntimeValue>> _functions;
        
        public UnityPlugin()
        {
            _functions = new Dictionary<string, Func<List<RuntimeValue>, RuntimeValue>>
            {
                ["Unity.FindGameObject"] = FindGameObject,
                ["Unity.FindGameObjectWithTag"] = FindGameObjectWithTag,
                ["Unity.GetTransform"] = GetTransform,
                ["Unity.GetTransformPosition"] = GetPosition,
                ["Unity.SetTransformPosition"] = SetPosition,
                ["Unity.GetTransformRotation"] = GetRotation,
                ["Unity.SetTransformRotation"] = SetRotation,
                ["Unity.GetTransformScale"] = GetScale,
                ["Unity.SetTransformScale"] = SetScale,
                ["Unity.GetComponent"] = GetComponent,
                ["Unity.AddComponent"] = AddComponent,
                ["Unity.DestroyGameObject"] = DestroyGameObject,
                ["Unity.InstantiateGameObject"] = InstantiateGameObject,
                ["Unity.IsGameObjectActive"] = GetActive,
                ["Unity.SetGameObjectActive"] = SetActive,
                ["Unity.GetGameObjectName"] = GetName,
                ["Unity.SetGameObjectName"] = SetName,
                ["Unity.GetGameObjectTag"] = GetTag,
                ["Unity.SetGameObjectTag"] = SetTag,
                ["Unity.GetGameObjectLayer"] = GetLayer,
                ["Unity.SetGameObjectLayer"] = SetLayer,
                ["Unity.GetParent"] = GetParent,
                ["Unity.SetParent"] = SetParent,
                ["Unity.GetChild"] = GetChild,
                ["Unity.GetChildCount"] = GetChildCount,
                ["Unity.GetDistance"] = GetDistance,
                ["Unity.LookAt"] = LookAt,
                ["Unity.MoveTowards"] = MoveTowards,
                ["Unity.RotateTowards"] = RotateTowards
            };
        }
        
        public void Initialize(IScripterConsole console)
        {
            console.WriteLine($"Unity Plugin {Version} initialized");
        }
        
        public void Initialize(Interpreter interpreter)
        {
            interpreter.WriteLine($"Unity Plugin {Version} initialized");
        }
        
        public bool IsFunctionAvailable(string functionName)
        {
            return _functions.ContainsKey(functionName);
        }
        
        public RuntimeValue CallFunction(string functionName, List<RuntimeValue> arguments)
        {
            if (_functions.TryGetValue(functionName, out var function))
            {
                try
                {
                    return function(arguments);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[UnityPlugin] Error calling {functionName}: {ex.Message}");
                    return new RuntimeValue(null);
                }
            }
            
            Debug.LogError($"[UnityPlugin] Function {functionName} not found");
            return new RuntimeValue(null);
        }
        
        private RuntimeValue FindGameObject(List<RuntimeValue> args)
        {
            if (args.Count != 1) return new RuntimeValue(null);
            
            var name = args[0].Value?.ToString();
            if (string.IsNullOrEmpty(name)) return new RuntimeValue(null);
            
            var go = GameObject.Find(name);
            return new RuntimeValue(go);
        }
        
        private RuntimeValue FindGameObjectWithTag(List<RuntimeValue> args)
        {
            if (args.Count != 1) return new RuntimeValue(null);
            
            var tag = args[0].Value?.ToString();
            if (string.IsNullOrEmpty(tag)) return new RuntimeValue(null);
            
            var go = GameObject.FindGameObjectWithTag(tag);
            return new RuntimeValue(go);
        }
        
        private RuntimeValue GetTransform(List<RuntimeValue> args)
        {
            if (args.Count != 1) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            return new RuntimeValue(go.transform);
        }
        
        private RuntimeValue GetPosition(List<RuntimeValue> args)
        {
            if (args.Count != 1) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            return new RuntimeValue(go.transform.position);
        }
        
        private RuntimeValue SetPosition(List<RuntimeValue> args)
        {
            if (args.Count != 4) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            var x = Convert.ToSingle(args[1].Value);
            var y = Convert.ToSingle(args[2].Value);
            var z = Convert.ToSingle(args[3].Value);
            
            go.transform.position = new Vector3(x, y, z);
            return new RuntimeValue(true);
        }
        
        private RuntimeValue GetRotation(List<RuntimeValue> args)
        {
            if (args.Count != 1) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            return new RuntimeValue(go.transform.rotation);
        }
        
        private RuntimeValue SetRotation(List<RuntimeValue> args)
        {
            if (args.Count != 4) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            var x = Convert.ToSingle(args[1].Value);
            var y = Convert.ToSingle(args[2].Value);
            var z = Convert.ToSingle(args[3].Value);
            
            go.transform.rotation = Quaternion.Euler(x, y, z);
            return new RuntimeValue(true);
        }
        
        private RuntimeValue GetScale(List<RuntimeValue> args)
        {
            if (args.Count != 1) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            return new RuntimeValue(go.transform.localScale);
        }
        
        private RuntimeValue SetScale(List<RuntimeValue> args)
        {
            if (args.Count != 4) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            var x = Convert.ToSingle(args[1].Value);
            var y = Convert.ToSingle(args[2].Value);
            var z = Convert.ToSingle(args[3].Value);
            
            go.transform.localScale = new Vector3(x, y, z);
            return new RuntimeValue(true);
        }
        
        private RuntimeValue GetComponent(List<RuntimeValue> args)
        {
            if (args.Count != 2) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            var componentType = args[1].Value?.ToString();
            if (string.IsNullOrEmpty(componentType)) return new RuntimeValue(null);
            
            var component = go.GetComponent(componentType);
            return new RuntimeValue(component);
        }
        
        private RuntimeValue AddComponent(List<RuntimeValue> args)
        {
            if (args.Count != 2) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            var componentType = args[1].Value?.ToString();
            if (string.IsNullOrEmpty(componentType)) return new RuntimeValue(null);
            
            var component = go.AddComponent(System.Type.GetType(componentType));
            return new RuntimeValue(component);
        }
        
        private RuntimeValue DestroyGameObject(List<RuntimeValue> args)
        {
            if (args.Count != 1) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            GameObject.Destroy(go);
            return new RuntimeValue(true);
        }
        
        private RuntimeValue InstantiateGameObject(List<RuntimeValue> args)
        {
            if (args.Count != 1) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            var instance = GameObject.Instantiate(go);
            return new RuntimeValue(instance);
        }
        
        private RuntimeValue GetActive(List<RuntimeValue> args)
        {
            if (args.Count != 1) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            return new RuntimeValue(go.activeInHierarchy);
        }
        
        private RuntimeValue SetActive(List<RuntimeValue> args)
        {
            if (args.Count != 2) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            var active = Convert.ToBoolean(args[1].Value);
            go.SetActive(active);
            return new RuntimeValue(true);
        }
        
        private RuntimeValue GetName(List<RuntimeValue> args)
        {
            if (args.Count != 1) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            return new RuntimeValue(go.name);
        }
        
        private RuntimeValue SetName(List<RuntimeValue> args)
        {
            if (args.Count != 2) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            var name = args[1].Value?.ToString();
            if (string.IsNullOrEmpty(name)) return new RuntimeValue(null);
            
            go.name = name;
            return new RuntimeValue(true);
        }
        
        private RuntimeValue GetTag(List<RuntimeValue> args)
        {
            if (args.Count != 1) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            return new RuntimeValue(go.tag);
        }
        
        private RuntimeValue SetTag(List<RuntimeValue> args)
        {
            if (args.Count != 2) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            var tag = args[1].Value?.ToString();
            if (string.IsNullOrEmpty(tag)) return new RuntimeValue(null);
            
            go.tag = tag;
            return new RuntimeValue(true);
        }
        
        private RuntimeValue GetLayer(List<RuntimeValue> args)
        {
            if (args.Count != 1) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            return new RuntimeValue(go.layer);
        }
        
        private RuntimeValue SetLayer(List<RuntimeValue> args)
        {
            if (args.Count != 2) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            var layer = Convert.ToInt32(args[1].Value);
            go.layer = layer;
            return new RuntimeValue(true);
        }
        
        private RuntimeValue GetParent(List<RuntimeValue> args)
        {
            if (args.Count != 1) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            return new RuntimeValue(go.transform.parent?.gameObject);
        }
        
        private RuntimeValue SetParent(List<RuntimeValue> args)
        {
            if (args.Count != 2) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            var parent = args[1].Value as GameObject;
            go.transform.SetParent(parent?.transform);
            return new RuntimeValue(true);
        }
        
        private RuntimeValue GetChild(List<RuntimeValue> args)
        {
            if (args.Count != 2) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            var index = Convert.ToInt32(args[1].Value);
            if (index < 0 || index >= go.transform.childCount) return new RuntimeValue(null);
            
            return new RuntimeValue(go.transform.GetChild(index).gameObject);
        }
        
        private RuntimeValue GetChildCount(List<RuntimeValue> args)
        {
            if (args.Count != 1) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            return new RuntimeValue(go.transform.childCount);
        }
        
        private RuntimeValue GetDistance(List<RuntimeValue> args)
        {
            if (args.Count != 2) return new RuntimeValue(null);
            
            var go1 = args[0].Value as GameObject;
            var go2 = args[1].Value as GameObject;
            if (go1 == null || go2 == null) return new RuntimeValue(null);
            
            var distance = Vector3.Distance(go1.transform.position, go2.transform.position);
            return new RuntimeValue(distance);
        }
        
        private RuntimeValue LookAt(List<RuntimeValue> args)
        {
            if (args.Count != 2) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            var target = args[1].Value as GameObject;
            if (target == null) return new RuntimeValue(null);
            
            go.transform.LookAt(target.transform);
            return new RuntimeValue(true);
        }
        
        private RuntimeValue MoveTowards(List<RuntimeValue> args)
        {
            if (args.Count != 4) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            var targetX = Convert.ToSingle(args[1].Value);
            var targetY = Convert.ToSingle(args[2].Value);
            var targetZ = Convert.ToSingle(args[3].Value);
            
            var target = new Vector3(targetX, targetY, targetZ);
            go.transform.position = Vector3.MoveTowards(go.transform.position, target, Time.deltaTime);
            return new RuntimeValue(true);
        }
        
        private RuntimeValue RotateTowards(List<RuntimeValue> args)
        {
            if (args.Count != 4) return new RuntimeValue(null);
            
            var go = args[0].Value as GameObject;
            if (go == null) return new RuntimeValue(null);
            
            var targetX = Convert.ToSingle(args[1].Value);
            var targetY = Convert.ToSingle(args[2].Value);
            var targetZ = Convert.ToSingle(args[3].Value);
            
            var target = Quaternion.Euler(targetX, targetY, targetZ);
            go.transform.rotation = Quaternion.RotateTowards(go.transform.rotation, target, Time.deltaTime * 90f);
            return new RuntimeValue(true);
        }
    }
}
