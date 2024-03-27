using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShowGUIDOnInspector
{
    private static bool _isShow;
    
    [MenuItem("CX_Tools/ShowGUIDOnInspector/Show",false ,priority = 6)]
    private static void ShowCustomHierarchyInfo()
    {
        _isShow = true;
        Editor.finishedDefaultHeaderGUI += DisplayGuidIfPersistent;
    }
    [MenuItem("CX_Tools/ShowGUIDOnInspector/Show",true ,priority = 6)]
    private static bool CheckShowCustomHierarchyInfo()
    {
        return !_isShow;
    }
    [MenuItem("CX_Tools/ShowGUIDOnInspector/Hide",false ,priority = 6)]
    private static void HideCustomHierarchyInfo()
    {
        _isShow = false;
        Editor.finishedDefaultHeaderGUI -= DisplayGuidIfPersistent;
    }
    [MenuItem("CX_Tools/ShowGUIDOnInspector/Hide",true ,priority = 6)]
    private static bool CheckHideCustomHierarchyInfo()
    {
        return _isShow;
    }
    
    static void DisplayGuidIfPersistent(Editor editor)
    {
        if (EditorUtility.IsPersistent(editor.target) == false) return;
        var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(editor.target));
        var totalRect = EditorGUILayout.GetControlRect();
        var controlRect = EditorGUI.PrefixLabel(totalRect, EditorGUIUtility.TrTempContent("GUID"));
        if (editor.targets.Length > 1)
            EditorGUI.LabelField(controlRect, EditorGUIUtility.TrTempContent("[Multiple objects selected]"));
        else
            EditorGUI.SelectableLabel(controlRect, guid);
    }
}
