using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Example_13
{
    public class Material_EditorWindow : EditorWindow
    {

        [MenuItem("CX_Tools/Editor_Material", priority = 6)]
        private static void ShowWindow()
        {
            var window = GetWindow<Material_EditorWindow>();
            window.titleContent = new GUIContent("Editor_Material");
            window.Show();
        }

        private Material _selectMaterial;
        private Vector2 _scrollPos;
        private string _addKeyWords;
        private string _removeKeyWords;

        private void OnGUI()
        {
            _selectMaterial = (Material)EditorGUILayout.ObjectField(_selectMaterial, typeof(Material), false);
            if (_selectMaterial == null) return;
            SerializedObject o = new SerializedObject(_selectMaterial);
            SerializedProperty disabledShaderPasses = o.FindProperty("disabledShaderPasses");
            SerializedProperty tagsMap = o.FindProperty("stringTagMap");
            using (var scroll = new GUILayout.ScrollViewScope(_scrollPos))
            {
                _scrollPos = scroll.scrollPosition;
                GUILayout.Label("Shader Property:", (GUIStyle)"BoldLabel");
                var shader = _selectMaterial.shader;
                var propertyCount = shader.GetPropertyCount();
                for (var i = 0; i < propertyCount; i++)
                {
                    var propertyType = shader.GetPropertyType(i);
                    var propName = shader.GetPropertyName(i);
                    GUILayout.Label($"{propertyType} : {propName}");
                }
                GUILayout.Space(10f);
                GUILayout.Label("keyWord:", (GUIStyle)"BoldLabel");
                var sb = new StringBuilder();
                var keyWords = _selectMaterial.shaderKeywords;
                for (int i = 0; i < keyWords.Length; i++)
                {
                    sb.Append(keyWords[i]);
                    if (i < keyWords.Length - 1)
                        sb.Append("/");
                }
                GUILayout.Label(sb.ToString());
                
                //tags map
                GUILayout.Label("Tags Map:", (GUIStyle)"BoldLabel");
                for (int i = 0; i < tagsMap.arraySize; i++)
                {
                    var pair = tagsMap.GetArrayElementAtIndex(i);
                    var first = pair.FindPropertyRelative("first").stringValue;
                    var second = pair.FindPropertyRelative("second").stringValue;
                    GUILayout.Label($"{first}:{second}");
                }
                
                //pass
                //var passCount = _selectMaterial.passCount;
                //GUILayout.Label($"shader passes: {passCount}", (GUIStyle)"BoldLabel");
                // for (int i = 0; i < passCount; i++)
                // {
                //     var passName = _selectMaterial.GetPassName(i);
                //     var enabled = _selectMaterial.GetShaderPassEnabled(passName);
                //     GUILayout.Label($"{passName}:{enabled}");
                // }
                //pass
                GUILayout.Label("Disabled Shader Pass:", (GUIStyle)"BoldLabel");
                for (int i = 0; i < disabledShaderPasses.arraySize; i++)
                {
                    var property = disabledShaderPasses.GetArrayElementAtIndex(i);
                    GUILayout.Label($"{property.stringValue}");
                }
            }

            using (new GUILayout.VerticalScope("Box", GUILayout.Height(100)))
            {
                GUILayout.Label("Set keyWord:",(GUIStyle)"BoldLabel");
                _addKeyWords = EditorGUILayout.TextField("Add keyWord: ", _addKeyWords);
                _removeKeyWords = EditorGUILayout.TextField("Remove keyWord: ", _removeKeyWords);
                using (new GUILayout.VerticalScope("Box"))
                {
                    if (GUILayout.Button("Set to this"))
                    {
                        SetMaterial(_selectMaterial);
                        AssetDatabase.SaveAssets();
                        Repaint();
                    }
                    GUILayout.Space(10f);
                }

            }
        }

        private void SetMaterial(Material material)
        {
            if (!string.IsNullOrEmpty(_addKeyWords))
            {
                var addKeyWords = _addKeyWords.ToUpper().Split(new char[] { ' ', ',', ';', '|', '/', '\\' },
                    StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in addKeyWords)
                {
                    if (string.IsNullOrEmpty(word)) continue;
                    material.EnableKeyword(word);
                }
            }

            if (!string.IsNullOrEmpty(_removeKeyWords))
            {
                var removKeyWords = _removeKeyWords.ToUpper().Split(new char[] { ' ', ',', ';', '|', '/', '\\' },
                    StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in removKeyWords)
                {
                    if (string.IsNullOrEmpty(word)) continue;
                    material.DisableKeyword(word);
                }
            }
            EditorUtility.SetDirty(material);
        }
    }
}