using UnityEditor;
using UnityEngine;

public class TestPopupWindow : PopupWindowContent
{
    private Rect _rect;

    public TestPopupWindow(Rect rect)
    {
        _rect = rect;
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(_rect.width, 100);
    }

    public override void OnGUI(Rect rect)
    {
        GUILayout.Label("Custom Popup Content");
            
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace(); //把按钮推到了右边
            if (GUILayout.Button("Revert All", GUILayout.Width(80)))
            {
                Debug.Log("Revert All");
            }
                
            if (GUILayout.Button("Apply All", GUILayout.Width(80)))
            {
                Debug.Log("Apply All");
            }
        }
    }
}