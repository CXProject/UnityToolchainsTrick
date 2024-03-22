using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CollectAllResourcesTools : EditorWindow
{
    [MenuItem("CX_Tools/CollectAllResources")]
    public static void ShowWindow()
    {
        var win = GetWindow<CollectAllResourcesTools>();
        win.Show();
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Collect"))
        {
            CollectResources();
        }
    }

    private void CollectResources()
    {
        var assetP = Path.GetFullPath(Application.dataPath);
        Debug.Log(assetP);
        Debug.Log(Path.GetFullPath("Packages"));
    }
    
}
