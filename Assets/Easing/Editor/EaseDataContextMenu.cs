using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace GD3D.Easing.Editor
{
    [InitializeOnLoad]
    public static class EaseDataContextMenu
    {
        // This is called when the editor is compiled
        static EaseDataContextMenu()
        {
            // Subscribe to the contextual property menu thingy so we can do a custom context menu
            EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
        }

        private static readonly Destructor destructor = new Destructor();

        // This class is purely here because static classes can't have destructors
        private sealed class Destructor
        {
            // This is called when this object is destroyed
            ~Destructor()
            {
                // UNSubscribe to the contextual property menu thingy when the destructor is destroyed
                EditorApplication.contextualPropertyMenu -= OnPropertyContextMenu;
            }
        }


        private static void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
        {
            try
            {
                #region Get SerializedProperty Type
                // I copied this from https://answers.unity.com/questions/929293/get-field-type-of-serializedproperty.html cuz I'm lazy
                
                // Gets parent type info
                string[] slices = property.propertyPath.Split('.');
                Type type = property.serializedObject.targetObject.GetType();

                for (int i = 0; i < slices.Length; i++)
                {
                    if (slices[i] == "Array")
                    {
                        i++; // Skips "data[x]"
                        type = type.GetElementType(); // Gets info on array elements
                    }
                    // Gets info on field and its type
                    else
                    {
                        BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance;

                        type = type.GetField(slices[i], flags).FieldType;
                    }
                }
                #endregion

                // Only work on menus with the EaseData type
                if (type != typeof(EaseData))
                {
                    return;
                }
            }
            catch (Exception)
            {
                // Do not do anything to the context menu since our type casting thing failed and gave an error
                return;
            }

            // Add a reset button for reseting all of the values on the property
            menu.AddItem(new GUIContent("Reset"), false, () => EaseDataPropertyDrawer.Reset(property));

            // Add a seperator
            menu.AddSeparator("");

            // Add a open graph settings button which opens a window when pressed
            bool windowOpen = EditorWindow.HasOpenInstances<EaseDataGraphSettingsWindow>();

            // Have different label depending on if the window is open or not
            string openGraphSettingsLabel = windowOpen ? "Focus Graph Settings" : "Open Graph Settings";

            menu.AddItem(new GUIContent(openGraphSettingsLabel), windowOpen, () => EaseDataPropertyDrawer.OpenGraphSettings(windowOpen));

            // Add a reset graph settings button
            menu.AddItem(new GUIContent("Reset Graph Settings"), false, () => EaseDataPropertyDrawer.ResetGraphSettings(windowOpen));

            // Add a seperator because unitys default context menus always have that for some reason
            menu.AddSeparator("");
        }
    }
}
