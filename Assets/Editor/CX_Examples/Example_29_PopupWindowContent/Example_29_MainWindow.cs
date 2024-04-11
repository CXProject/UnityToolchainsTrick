using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Example_29_MainWindow : EditorWindow
{
    public static GUIContent overridesContent = new GUIContent("Overrides");
    
    [MenuItem("CX_Tools/ShowPopupWindowContent")]
    private static void ShowWindow()
    {
        var window = GetWindow<Example_29_MainWindow>();
        window.Show();
    }

    private void OnGUI()
    {
        var rect = GUILayoutUtility.GetRect(overridesContent, "button");
        if (EditorGUI.DropdownButton(rect, overridesContent, FocusType.Passive))
        {
            PopupWindow.Show(rect, new TestPopupWindow(rect));
        }
    }
}
