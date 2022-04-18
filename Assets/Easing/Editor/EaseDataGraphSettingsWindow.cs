using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GD3D.Easing.Editor
{
    public class EaseDataGraphSettingsWindow : EditorWindow
    {
        public void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontStyle = FontStyle.Bold;

            EditorGUILayout.LabelField("Graph Settings", labelStyle);

            // Add show preview graph field
            EaseDataPropertyDrawer.ShowPreviewGraph = EditorGUILayout.Toggle("Show Preview Graph", EaseDataPropertyDrawer.ShowPreviewGraph);

            // Add graph resolution field
            EaseDataPropertyDrawer.GraphResolution = Mathf.Clamp(EditorGUILayout.IntField("Graph Resolution", EaseDataPropertyDrawer.GraphResolution), 2, 999);

            // Add a space
            EditorGUILayout.Space();

            // Add color fields
            EaseDataPropertyDrawer.PreviewGraphColor = EditorGUILayout.ColorField("Preview Graph Color", EaseDataPropertyDrawer.PreviewGraphColor);
            EaseDataPropertyDrawer.CustomCurveColor = EditorGUILayout.ColorField("Custom Curve Color", EaseDataPropertyDrawer.CustomCurveColor);

            // Add a space
            EditorGUILayout.Space();

            // Add the graph size field
            Vector2 newSize = EditorGUILayout.Vector2Field("Graph Size", EaseDataPropertyDrawer.GraphSize);

            // Limit so the size can't go below 0
            newSize.x = Mathf.Max(newSize.x, 0);
            newSize.y = Mathf.Max(newSize.y, 0);

            EaseDataPropertyDrawer.GraphSize = newSize;

            // Add a space
            EditorGUILayout.Space();

            // Add a reset button that will reset all the values
            if (GUILayout.Button("Reset Graph Settings"))
            {
                EaseDataPropertyDrawer.ResetGraphSettings();
            }

            // Update all of the inspector windows if something changed
            if (EditorGUI.EndChangeCheck())
            {
                // Find all windows
                EditorWindow[] windows = Resources.FindObjectsOfTypeAll<EditorWindow>();

                foreach (EditorWindow window in windows)
                {
                    // We must check if the name is correct because the actual window type is private so we can't compare those
                    if (window.GetType().Name == "InspectorWindow")
                    {
                        window.Repaint();
                    }
                }
            }
        }
    }

}
