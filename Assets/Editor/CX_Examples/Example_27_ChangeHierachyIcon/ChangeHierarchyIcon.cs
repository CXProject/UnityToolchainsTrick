using UnityEditor;
using UnityEngine;

public static class ChangeHierarchyIcon
{
    private static bool _isShow;
    
    [MenuItem("CX_Tools/ShowCustomHierarchyInfo/Show",false ,priority = 6)]
    private static void ShowCustomHierarchyInfo()
    {
        _isShow = true;
        EditorApplication.hierarchyWindowItemOnGUI += AddIconToHierarchyItem;
    }
    [MenuItem("CX_Tools/ShowCustomHierarchyInfo/Show",true ,priority = 6)]
    private static bool CheckShowCustomHierarchyInfo()
    {
        return !_isShow;
    }
    [MenuItem("CX_Tools/ShowCustomHierarchyInfo/Hide",false ,priority = 6)]
    private static void HideCustomHierarchyInfo()
    {
        _isShow = false;
        EditorApplication.hierarchyWindowItemOnGUI -= AddIconToHierarchyItem;
    }
    [MenuItem("CX_Tools/ShowCustomHierarchyInfo/Hide",true ,priority = 6)]
    private static bool CheckHideCustomHierarchyInfo()
    {
        return _isShow;
    }

    private static void AddIconToHierarchyItem(int instanceID, Rect selectionRect)
    {
        GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if(gameObject == null) return;
        
        Rect iconRect = new Rect(selectionRect.x, selectionRect.y, 16f, 16f);
        GUI.DrawTexture(iconRect, EditorGUIUtility.IconContent("Prefab Icon").image);
        Rect labelRect = new Rect(selectionRect.x + 200f, selectionRect.y - 1, selectionRect.width, selectionRect.height);
        EditorGUI.LabelField(labelRect, "Hello!!! Custom Text");
    }
}
