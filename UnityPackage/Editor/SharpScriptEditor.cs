using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Scripter.Unity.Editor
{
    [CustomEditor(typeof(SharpScript))]
    public class SharpScriptEditor : UnityEditor.Editor
    {
        private SharpScript _script;
        private Vector2 _scrollPosition;
        private bool _showStatistics = false;
        private bool _showExecutionSettings = true;
        private bool _showMetadata = false;
        
        private readonly Color _keywordColor = new Color(0.8f, 0.2f, 0.8f);
        private readonly Color _stringColor = new Color(0.8f, 0.4f, 0.2f);
        private readonly Color _commentColor = new Color(0.2f, 0.8f, 0.2f);
        private readonly Color _numberColor = new Color(0.2f, 0.6f, 0.8f);
        private readonly Color _functionColor = new Color(0.8f, 0.6f, 0.2f);
        
        private readonly string[] _keywords = {
            "var", "function", "class", "static", "public", "private", "return", "if", "else", 
            "while", "for", "true", "false", "null", "new", "this", "base"
        };
        
        private void OnEnable()
        {
            _script = (SharpScript)target;
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawHeader();
            DrawScriptContent();
            DrawExecutionSettings();
            DrawMetadata();
            DrawStatistics();
            DrawButtons();
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawHeader()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Sharp Script Editor", EditorStyles.boldLabel);
            EditorGUILayout.Space();
        }
        
        private void DrawScriptContent()
        {
            EditorGUILayout.LabelField("Script Content", EditorStyles.boldLabel);
            
            var scriptContentProperty = serializedObject.FindProperty("scriptContent");
            
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(300));
            
            DrawSyntaxHighlightedTextArea(scriptContentProperty);
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.Space();
        }
        
        private void DrawSyntaxHighlightedTextArea(SerializedProperty property)
        {
            var content = property.stringValue ?? "";
            var lines = content.Split('\n');
            
            var style = new GUIStyle(EditorStyles.textArea)
            {
                richText = true,
                wordWrap = true,
                fontSize = 12
            };
            
            var highlightedContent = ApplySyntaxHighlighting(content);
            
            var newContent = EditorGUILayout.TextArea(highlightedContent, style, GUILayout.ExpandHeight(true));
            
            if (newContent != content)
            {
                property.stringValue = newContent;
            }
        }
        
        private string ApplySyntaxHighlighting(string content)
        {
            if (string.IsNullOrEmpty(content))
                return content;
            
            var highlighted = content;
            
            highlighted = Regex.Replace(highlighted, @"(//.*$)", 
                match => $"<color=#{ColorUtility.ToHtmlStringRGB(_commentColor)}>{match.Value}</color>", 
                RegexOptions.Multiline);
            
            highlighted = Regex.Replace(highlighted, @"""([^""]*)""", 
                match => $"<color=#{ColorUtility.ToHtmlStringRGB(_stringColor)}>{match.Value}</color>");
            
            highlighted = Regex.Replace(highlighted, @"\b\d+\.?\d*\b", 
                match => $"<color=#{ColorUtility.ToHtmlStringRGB(_numberColor)}>{match.Value}</color>");
            
            foreach (var keyword in _keywords)
            {
                var pattern = $@"\b{keyword}\b";
                highlighted = Regex.Replace(highlighted, pattern, 
                    match => $"<color=#{ColorUtility.ToHtmlStringRGB(_keywordColor)}>{match.Value}</color>");
            }
            
            highlighted = Regex.Replace(highlighted, @"\b(\w+)\s*\(", 
                match => $"<color=#{ColorUtility.ToHtmlStringRGB(_functionColor)}>{match.Groups[1].Value}</color>(");
            
            return highlighted;
        }
        
        private void DrawExecutionSettings()
        {
            _showExecutionSettings = EditorGUILayout.Foldout(_showExecutionSettings, "Execution Settings");
            if (_showExecutionSettings)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.PropertyField(serializedObject.FindProperty("autoExecute"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("showDebugInfo"));
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();
        }
        
        private void DrawMetadata()
        {
            _showMetadata = EditorGUILayout.Foldout(_showMetadata, "Metadata");
            if (_showMetadata)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.PropertyField(serializedObject.FindProperty("description"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("version"));
                
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("Created Date", _script.createdDate);
                EditorGUILayout.TextField("Last Modified", _script.lastModified);
                EditorGUI.EndDisabledGroup();
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();
        }
        
        private void DrawStatistics()
        {
            _showStatistics = EditorGUILayout.Foldout(_showStatistics, "Script Statistics");
            if (_showStatistics)
            {
                EditorGUI.indentLevel++;
                
                var stats = _script.GetStatistics();
                
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.IntField("Total Lines", stats.totalLines);
                EditorGUILayout.IntField("Non-Empty Lines", stats.nonEmptyLines);
                EditorGUILayout.IntField("Comment Lines", stats.commentLines);
                EditorGUILayout.IntField("Total Characters", stats.totalCharacters);
                EditorGUILayout.FloatField("Comment Percentage", stats.commentPercentage);
                EditorGUI.EndDisabledGroup();
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();
        }
        
        private void DrawButtons()
        {
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Execute Script"))
            {
                ExecuteScript();
            }
            
            if (GUILayout.Button("Validate Script"))
            {
                ValidateScript();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Clear Script"))
            {
                if (EditorUtility.DisplayDialog("Clear Script", "Are you sure you want to clear the script content?", "Yes", "No"))
                {
                    _script.SetScriptContent("");
                }
            }
            
            if (GUILayout.Button("Reset to Default"))
            {
                if (EditorUtility.DisplayDialog("Reset Script", "Are you sure you want to reset the script to default content?", "Yes", "No"))
                {
                    _script.SetScriptContent("// Введите ваш Sharp код здесь\nvar x = 10;\nprint(\"Hello from Sharp!\");");
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
                private void ExecuteScript()
                {
                    if (!_script.IsValid())
                    {
                        EditorUtility.DisplayDialog("Invalid Script", "The script content is empty or invalid.", "OK");
                        return;
                    }
                    
                    _script.ExecuteScript();
                    Debug.Log($"[SharpScriptEditor] Script '{_script.name}' executed successfully");
                }
        
        private void ValidateScript()
        {
            if (!_script.IsValid())
            {
                EditorUtility.DisplayDialog("Validation Failed", "The script content is empty or invalid.", "OK");
                return;
            }
            
            var stats = _script.GetStatistics();
            var message = $"Script validation successful!\n\n" +
                         $"Total Lines: {stats.totalLines}\n" +
                         $"Non-Empty Lines: {stats.nonEmptyLines}\n" +
                         $"Comment Lines: {stats.commentLines}\n" +
                         $"Total Characters: {stats.totalCharacters}\n" +
                         $"Comment Percentage: {stats.commentPercentage:F1}%";
            
            EditorUtility.DisplayDialog("Validation Successful", message, "OK");
        }
    }
}
