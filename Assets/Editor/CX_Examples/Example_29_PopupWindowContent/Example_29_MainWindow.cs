using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Example_29_MainWindow : EditorWindow
{
    public static GUIContent myPopupContent = new GUIContent("MyPopup");
    public static GUIContent overridesContent = new GUIContent("Overrides");

    public Transform _target;

    [MenuItem("CX_Tools/ShowPopupWindowContent")]
    private static void ShowWindow()
    {
        var window = GetWindow<Example_29_MainWindow>();
        window.Show();
    }

    private void OnGUI()
    {
        var rect = GUILayoutUtility.GetRect(myPopupContent, "button");
        if (EditorGUI.DropdownButton(rect, myPopupContent, FocusType.Passive))
        {
            PopupWindow.Show(rect, new TestPopupWindow(rect));
        }

        _target = (Transform)EditorGUILayout.ObjectField("Object", _target, typeof(Transform), true);
        
        if(_target == null) return;
        rect = GUILayoutUtility.GetRect(overridesContent, "button");
        if (EditorGUI.DropdownButton(rect, overridesContent, FocusType.Passive))
        {
            PopupWindow.Show(rect, new ComparisonViewPopup(_target, _target));
        }
    }
}
