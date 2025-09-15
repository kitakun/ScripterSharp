using UnityEngine;
using System;

namespace Scripter.Unity
{
    [CreateAssetMenu(fileName = "New Sharp Script", menuName = "Scripter/Sharp Script")]
    public class SharpScript : ScriptableObject
    {
        public string description = "Sharp Script";
        public string scriptContent = "var x = 10;\nprint(\"Hello from Sharp!\");";
        public bool autoExecute = false;
        public bool showDebugInfo = true;
        public string version = "1.0.0";
        
        [SerializeField, HideInInspector]
        private string _createdDate = "";
        
        [SerializeField, HideInInspector]
        private string _lastModified = "";
        
        public string createdDate => _createdDate;
        public string lastModified => _lastModified;
        
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_createdDate))
            {
                _createdDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            _lastModified = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            if (autoExecute && !string.IsNullOrEmpty(scriptContent))
            {
                ExecuteScript();
            }
        }
        
        public void ExecuteScript()
        {
            if (!IsValid())
            {
                Debug.LogError($"[SharpScript] Скрипт '{name}' не содержит валидного кода");
                return;
            }
            
            UnityScriptService.ExecuteScript(this);
        }
        
        public string GetScriptContent()
        {
            return scriptContent ?? "";
        }
        
        public void SetScriptContent(string content)
        {
            scriptContent = content;
            _lastModified = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(scriptContent) && scriptContent.Trim().Length > 0;
        }
        
        public ScriptStatistics GetStatistics()
        {
            if (string.IsNullOrEmpty(scriptContent))
            {
                return new ScriptStatistics();
            }
            
            var totalCharacters = scriptContent.Length;
            var totalLines = 1;
            var nonEmptyLines = 0;
            var commentLines = 0;
            
            for (int i = 0; i < scriptContent.Length; i++)
            {
                if (scriptContent[i] == '\n')
                {
                    totalLines++;
                }
            }
            
            var lines = scriptContent.Split('\n');
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (!string.IsNullOrEmpty(trimmedLine))
                {
                    nonEmptyLines++;
                    if (trimmedLine.StartsWith("//"))
                    {
                        commentLines++;
                    }
                }
            }
            
            return new ScriptStatistics
            {
                totalLines = totalLines,
                nonEmptyLines = nonEmptyLines,
                commentLines = commentLines,
                totalCharacters = totalCharacters
            };
        }
    }
    
    [System.Serializable]
    public class ScriptStatistics
    {
        public int totalLines = 0;
        public int nonEmptyLines = 0;
        public int commentLines = 0;
        public int totalCharacters = 0;
        
        public float commentPercentage => nonEmptyLines > 0 ? (float)commentLines / nonEmptyLines * 100f : 0f;
    }
}
