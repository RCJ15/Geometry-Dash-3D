using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GD3D.Easing.Editor
{
    /// <summary>
    /// The custom property drawer for <see cref="EaseData"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(EaseData))]
    public class EaseDataPropertyDrawer : PropertyDrawer
    {
        private EaseData _thisObj;

        private int _startIndent;

        private Dictionary<string, float> _extraHeight = new Dictionary<string, float>();

        // Graph Settings
        public static int GraphResolution = 50;

        public static Color GraphColor = Color.red;

        public static Vector2 GraphSize = new Vector2(200, 100);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Get properties
            var type = GetProperty(property, "Type");
            var easeRate = GetProperty(property, "EaseRate");

            // Create this object
            _thisObj = new EaseData((EasingType)type.enumValueIndex, easeRate.floatValue);

            // Create and draw foldout
            Rect foldoutRect = new Rect(position.x, position.y, position.width, 20);

            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);

            // Cache indent level so we can reset it later
            _startIndent = EditorGUI.indentLevel;

            // Add extra height key if this one is missing from the dictionary
            if (!_extraHeight.ContainsKey(property.propertyPath))
            {
                _extraHeight.Add(property.propertyPath, 0);
            }

            // Reset extra height
            _extraHeight[property.propertyPath] = 0;

            // Do not draw anything else if the foldout is closed
            if (!property.isExpanded)
            {
                DoLast();
                return;
            }

            // Draw a dark field thingy on the rect
            EditorGUI.DrawRect(new Rect(position.x, position.y + 18, position.width, position.height - 16), new Color(0, 0, 0, 0.2f));

            // Calculate rects
            Rect typeRect = new Rect(position.x, position.y + 20, position.width, 20);

            EditorGUI.indentLevel++;

            Serialize(typeRect, type, property);

            // Serialize easeRate and customCurve based in what easing type this object has
            EasingType easingType = (EasingType)type.enumValueIndex;

            switch (easingType)
            {
                case EasingType.easeInOut:
                case EasingType.easeIn:
                case EasingType.easeOut:
                case EasingType.elasticInOut:
                case EasingType.elasticIn:
                case EasingType.elasticOut:
                    Rect extraRect = new Rect(position.x, position.y + _extraHeight[property.propertyPath] + 20, position.width, 20);
                    Serialize(extraRect, easeRate, property);
                    break;
            }

            DrawGraph(position, property, easingType);

            DoLast();
        }

        private void DrawGraph(Rect position, SerializedProperty property, EasingType easingType)
        {
            // Draw label
            Rect graphLabelRect = new Rect(position.x, position.y + _extraHeight[property.propertyPath] + 30, position.width, 20);
            _extraHeight[property.propertyPath] += 10 + graphLabelRect.height;

            GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
            boldStyle.fontStyle = FontStyle.Bold;

            EditorGUI.LabelField(graphLabelRect, "Preview Graph", boldStyle);

            // Create graph rect
            Rect graphRect = new Rect(position.x, position.y + _extraHeight[property.propertyPath] + 20, GraphSize.x, GraphSize.y);
            _extraHeight[property.propertyPath] += graphRect.height;

            // This will be the curve we will draw at the end
            // This is set in the next if else statement
            AnimationCurve drawCurve;

            // Create a biggest and smallest value which we will use later for setting ranges for the graphs
            float biggestValue = 1;
            float smallestValue = 0;

            // Create list of keyframes
            List<Keyframe> keyframes = new List<Keyframe>();

            // Loop for the amount in graph resolution
            for (int i = 0; i < GraphResolution + 1; i++)
            {
                float t = (float)i / (float)GraphResolution;

                float val = _thisObj.Evaluate(t);

                // Determine if val is bigger or smaller than the biggest or smallest values
                if (val > biggestValue)
                {
                    biggestValue = val;
                }
                else if (val < smallestValue)
                {
                    smallestValue = val;
                }

                // Add new keyframe
                Keyframe newKeyframe = new Keyframe(t, val);
                newKeyframe.weightedMode = WeightedMode.Both;

                keyframes.Add(newKeyframe);
            }

            drawCurve = new AnimationCurve(keyframes.ToArray());

            // Make the ranges of the graph contain the biggest and smallest values
            Rect graphRanges = new Rect(0, smallestValue, 1, biggestValue - smallestValue);

            // Draw preview curve
            EditorGUI.CurveField(graphRect, drawCurve, GraphColor, graphRanges);
        }

        private void DoLast()
        {
            // Set indent level back to what it was
            EditorGUI.indentLevel = _startIndent;

            EditorGUI.EndProperty();
        }

        private void Serialize(Rect rect, SerializedProperty property, SerializedProperty thisProperty)
        {
            EditorGUI.PropertyField(rect, property);
            _extraHeight[thisProperty.propertyPath] += rect.height;
        }

        private static SerializedProperty GetProperty(SerializedProperty prop, string name)
        {
            return prop.FindPropertyRelative(name);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _extraHeight.TryGetValue(property.propertyPath, out float height);

            return base.GetPropertyHeight(property, label) + height;
        }

        #region Context Menu
        public static void Reset(SerializedProperty property)
        {
            // Get properties
            var type = GetProperty(property, "Type");
            var easeRate = GetProperty(property, "EaseRate");

            // Get the default value of EaseData
            EaseData defaultValue = new EaseData();

            // Set properties to default value
            type.enumValueIndex = (int)defaultValue.Type;
            easeRate.floatValue = defaultValue.EaseRate;

            // Update properties
            property.serializedObject.ApplyModifiedProperties();
        }

        public static void OpenGraphSettings(bool windowOpen)
        {
            // If the window is already open, focus on that opened window
            if (windowOpen)
            {
                EditorWindow.FocusWindowIfItsOpen<EaseDataGraphSettingsWindow>();
                return;
            }

            // Create window
            var newWindow = EditorWindow.CreateWindow<EaseDataGraphSettingsWindow>();

            newWindow.titleContent = new GUIContent("Ease Data Graph Settings");
        }

        public static void ResetGraphSettings(bool windowOpen = false)
        {
            GraphResolution = 50;
            GraphColor = Color.red;
            GraphSize = new Vector2(200, 100);

            // Update the windows if it's open
            if (windowOpen)
            {
                // Find all EaseDataGraphSettingsWindow windows
                EaseDataGraphSettingsWindow[] windows = Resources.FindObjectsOfTypeAll<EaseDataGraphSettingsWindow>();

                // Repaint them all
                foreach (EaseDataGraphSettingsWindow window in windows)
                {
                    window.Repaint();
                }
            }
        }
        #endregion
    }
}
