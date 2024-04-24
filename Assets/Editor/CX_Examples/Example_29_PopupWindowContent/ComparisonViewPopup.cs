using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ComparisonViewPopup : PopupWindowContent
{
    readonly Object m_Source;
    readonly Object m_Instance;
    readonly UnityEditor.Editor m_SourceEditor;
    readonly UnityEditor.Editor m_InstanceEditor;

    const float k_HeaderHeight = 25f;
    const float k_ScrollbarWidth = 13;
    Vector2 m_PreviewSize = new Vector2(600f, 0);
    Vector2 m_Scroll;
    bool m_RenderOverlayAfterResizeChange;

    static class Styles
    {
        public static GUIStyle borderStyle = new GUIStyle("grey_border");
        public static GUIStyle centeredLabelStyle = new GUIStyle(EditorStyles.label);
        public static GUIStyle headerGroupStyle = new GUIStyle();
        public static GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        public static GUIContent sourceContent = EditorGUIUtility.TrTextContent("Prefab Source");
        public static GUIContent instanceContent = EditorGUIUtility.TrTextContent("Override");
        public static GUIContent removedContent = EditorGUIUtility.TrTextContent("Removed");
        public static GUIContent addedContent = EditorGUIUtility.TrTextContent("Added");
        public static GUIContent noModificationsContent = EditorGUIUtility.TrTextContent("No Overrides");
        public static GUIContent applyContent = EditorGUIUtility.TrTextContent("Apply");
        public static GUIContent revertContent = EditorGUIUtility.TrTextContent("Revert");

        static Styles()
        {
            centeredLabelStyle.alignment = TextAnchor.UpperCenter;
            centeredLabelStyle.padding = new RectOffset(3, 3, 3, 3);

            headerGroupStyle.padding = new RectOffset(0, 0, 3, 3);

            headerStyle.alignment = TextAnchor.MiddleLeft;
            headerStyle.padding.left = 5;
            headerStyle.padding.top = 1;
        }
    }

    public ComparisonViewPopup(Object source, Object instance)
    {
        m_Source = source;
        m_Instance = instance;

        if (m_Source != null)
        {
            m_SourceEditor = UnityEditor.Editor.CreateEditor(m_Source);
        }

        if (m_Instance != null)
        {
            m_InstanceEditor = UnityEditor.Editor.CreateEditor(m_Instance);
        }

        if (m_Source == null || m_Instance == null)
            m_PreviewSize.x /= 2;
    }

    public override void OnClose()
    {
        base.OnClose();
        if (m_SourceEditor != null)
            Object.DestroyImmediate(m_SourceEditor);
        if (m_InstanceEditor != null)
            Object.DestroyImmediate(m_InstanceEditor);
    }

    bool UpdatePreviewHeight(float height)
    {
        if (height > 0 && m_PreviewSize.y != height)
        {
            m_PreviewSize.y = height;
            return true;
        }

        return false;
    }

    public override void OnGUI(Rect rect)
    {
        bool scroll = (m_PreviewSize.y > rect.height - k_HeaderHeight);
        if (scroll)
            rect.width -= k_ScrollbarWidth + 1;
        else
            // We overdraw border by one pixel to the right, so subtract here to account for that.
            rect.width -= 1;

        //EditorGUIUtility.comparisonViewMode = EditorGUIUtility.ComparisonViewMode.Original;
        EditorGUIUtility.wideMode = true;
        EditorGUIUtility.labelWidth = 120;
        int middleCol = Mathf.RoundToInt((rect.width - 1) * 0.5f);

        // if (Event.current.type == EventType.Repaint)
        //     EditorStyles.viewBackground.Draw(rect, GUIContent.none, 0);

        Rect scrollRectPosition =
            new Rect(
                rect.x,
                rect.y + k_HeaderHeight,
                rect.width + (scroll ? k_ScrollbarWidth : 0),
                rect.height - k_HeaderHeight);
        Rect viewPosition = new Rect(0, 0, rect.width, m_PreviewSize.y);

        if (m_Source != null && m_Instance != null)
        {
            Rect sourceHeaderRect = new Rect(rect.x, rect.y, middleCol, k_HeaderHeight);
            Rect instanceHeaderRect = new Rect(rect.x + middleCol, rect.y,
                rect.xMax - middleCol + (scroll ? k_ScrollbarWidth : 0), k_HeaderHeight);
            DrawHeader(sourceHeaderRect, Styles.sourceContent);
            DrawHeader(instanceHeaderRect, Styles.instanceContent);

            DrawRevertApplyButtons(instanceHeaderRect);

            m_Scroll = GUI.BeginScrollView(scrollRectPosition, m_Scroll, viewPosition);
            {
                var leftColumnHeight = DrawEditor(new Rect(0, 0, middleCol, m_PreviewSize.y), m_SourceEditor, true);

                //EditorGUIUtility.comparisonViewMode = EditorGUIUtility.ComparisonViewMode.Modified;
                var rightColumnHeight = DrawEditor(new Rect(middleCol, 0, rect.xMax - middleCol, m_PreviewSize.y),
                    m_InstanceEditor, false);

                if (UpdatePreviewHeight(Math.Max(leftColumnHeight, rightColumnHeight)))
                    m_RenderOverlayAfterResizeChange = true;
            }
            GUI.EndScrollView();
        }
        else
        {
            GUIContent headerContent;
            UnityEditor.Editor editor;
            bool disable;
            if (m_Source != null)
            {
                headerContent = Styles.removedContent;
                editor = m_SourceEditor;
                disable = true;
            }
            else
            {
                headerContent = Styles.addedContent;
                editor = m_InstanceEditor;
                disable = false;
            }

            Rect headerRect = new Rect(rect.x, rect.y, rect.width, k_HeaderHeight);
            DrawHeader(headerRect, headerContent);

            DrawRevertApplyButtons(headerRect);

            m_Scroll = GUI.BeginScrollView(scrollRectPosition, m_Scroll, viewPosition);

            //EditorGUIUtility.comparisonViewMode = EditorGUIUtility.ComparisonViewMode.Modified;
            float columnHeight = DrawEditor(new Rect(0, 0, rect.width, m_PreviewSize.y), editor, disable);
            if (UpdatePreviewHeight(columnHeight))
                m_RenderOverlayAfterResizeChange = true;

            GUI.EndScrollView();
        }

        if (m_RenderOverlayAfterResizeChange && Event.current.type == EventType.Repaint)
        {
            m_RenderOverlayAfterResizeChange = false;
            // The comparison view resizes a frame delayed due to having to wait for the first render to
            // layout the contents. This creates a distorted rendering because the last frame rendered is rendered
            // to the new window size. We therefore 'clear' the comparison view after a resize change by rendering
            // a quad on top with the background color so the distorted rendering is not shown to the user.
            // Fixes case 1069062.
            GUI.Label(rect, GUIContent.none, "TabWindowBackground");
            editorWindow.Repaint();
        }
    }

    void DrawHeader(Rect rect, GUIContent label)
    {
        EditorGUI.LabelField(rect, label, Styles.headerStyle);
        // Overdraw border by one pixel to the right, so adjacent borders overlap.
        // Don't overdraw down, since overlapping scroll view can make controls overlap divider line.
        GUI.Label(new Rect(rect.x, rect.y, rect.width + 1, rect.height), GUIContent.none, Styles.borderStyle);
    }

    void DrawRevertApplyButtons(Rect rect)
    {
        GUILayout.BeginArea(rect);
        GUILayout.BeginHorizontal(Styles.headerGroupStyle);
        GUILayout.FlexibleSpace();

        if (GUILayout.Button(Styles.revertContent, EditorStyles.miniButton, GUILayout.Width(55)))
        {
            UpdateAndClose();
            GUIUtility.ExitGUI();
        }

        using (new EditorGUI.DisabledScope(false))
        {
            Rect applyRect = GUILayoutUtility.GetRect(GUIContent.none, "MiniPulldown", GUILayout.Width(55));
            if (EditorGUI.DropdownButton(applyRect, Styles.applyContent, FocusType.Passive))
            {
                GenericMenu menu = new GenericMenu();
                menu.DropDown(applyRect);
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    void UpdateAndClose()
    {
        if (editorWindow != null)
        {
            editorWindow.Close();
        }
    }

    float DrawEditor(Rect rect, UnityEditor.Editor editor, bool disabled)
    {
        rect.xMin += 1;
        //EditorGUIUtility.leftMarginCoord = rect.x;
        GUILayout.BeginArea(rect);
        Rect editorRect = EditorGUILayout.BeginVertical();
        {
            using (new EditorGUI.DisabledScope(disabled))
            {
                if (editor == null)
                {
                    GUI.enabled = true;
                    GUILayout.Label("None - this should not happen.", Styles.centeredLabelStyle);
                }
                else
                {
                    if (editor.target is GameObject)
                    {
                        editor.DrawHeader();
                    }
                    else
                    {
                        EditorGUIUtility.hierarchyMode = true;
                        EditorGUILayout.InspectorTitlebar(true, editor);
                        EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
                        editor.OnInspectorGUI();
                        EditorGUILayout.Space();
                        EditorGUILayout.EndVertical();
                    }
                }
            }
        }

        EditorGUILayout.EndVertical();
        GUILayout.EndArea();

        // Overdraw border by one pixel in all directions.
        GUI.Label(new Rect(rect.x - 1, -1, rect.width + 2, m_PreviewSize.y + 2), GUIContent.none, Styles.borderStyle);

        return editorRect.height;
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(m_PreviewSize.x, m_PreviewSize.y + k_HeaderHeight + 1f);
    }
}