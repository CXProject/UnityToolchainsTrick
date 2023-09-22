using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class CombineSkinMeshTool : EditorWindow
{
    [MenuItem("CX_Tools/CombineSkinMeshTool")]
    public static void ShowWindow()
    {
        var win = GetWindow<CombineSkinMeshTool>();
        win.Show();
    }

    private GameObject _prefab;
    
    private void OnGUI()
    {
        _prefab = (GameObject)EditorGUILayout.ObjectField("Prefab Data", _prefab, typeof(GameObject), false);

        if (GUILayout.Button("Combine mesh") && _prefab)
        {
            var instanceObj = (GameObject) PrefabUtility.InstantiatePrefab(_prefab);
            PrefabUtility.UnpackPrefabInstance(instanceObj, PrefabUnpackMode.Completely,
                InteractionMode.AutomatedAction);
            List<Material> materials = new List<Material>(); //the list of materials
            List<CombineInstance> combineInstances = new List<CombineInstance>(); //the list of meshes
            List<Transform> bones = new List<Transform>(); //the list of bones

            var smrs = instanceObj.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var smr in smrs)
            {
                if(smr.enabled == false || smr.gameObject.activeSelf == false) continue;
                materials.AddRange(smr.sharedMaterials);
                for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
                {
                    CombineInstance ci = new CombineInstance();
                    ci.mesh = smr.sharedMesh;
                    ci.subMeshIndex = sub;
                    combineInstances.Add(ci);
                    bones.AddRange(smr.bones);
                }
            }
            
            //Create a new SkinnedMeshRenderer and combine
            var r = instanceObj.GetComponent<SkinnedMeshRenderer>();
            if (r == null)
                r = instanceObj.AddComponent<SkinnedMeshRenderer>();
            r.sharedMesh = new Mesh();
            r.sharedMesh.CombineMeshes(combineInstances.ToArray(), false, false); // Combine meshes
            r.bones = bones.ToArray(); // Use new bones
            r.materials = materials.ToArray();
            //Destroy all old part for performance
            for (int i = smrs.Length - 1; i >=0; i--)
            {
                Object.DestroyImmediate(smrs[i].gameObject);
            }
            
            //save
            AssetDatabase.CreateAsset(r.sharedMesh, $"Assets/{r.sharedMesh.GetInstanceID()}.asset");
            PrefabUtility.SaveAsPrefabAsset(instanceObj, $"Assets/{instanceObj.name}_CombineMesh.prefab");
            AssetDatabase.Refresh();
        }
    }
}
