using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace GD3D.LevelEditor
{
    /// <summary>
    /// Reads the Level Editor Placement file and saves all the data into a dictionary.
    /// </summary>
    public static class LevelEditorPlacementFile
    {
        public const string CATEGORY_MARKER = "CATEGORY = ";
        public static Dictionary<string, List<string>> Data = null;

        public static void ReadFileIfNull()
        {
            // File is not null, so return
            if (Data != null)
            {
                return;
            }

            // Else, we will read the file and convert it into a dictionary
            Data = new Dictionary<string, List<string>>();

            string rawText = Resources.Load<TextAsset>(Path.Combine("Objects", "Object Level Editor Placement")).text;

            string[] lines = rawText.Split('\n', System.StringSplitOptions.RemoveEmptyEntries);

            string currentCategory = null;

            // Loop through all lines
            foreach (string line in lines)
            {
                // Ignore empty lines or comments
                if (string.IsNullOrEmpty(line) || line.StartsWith("//"))
                {
                    continue;
                }

                // Create new category
                if (line.StartsWith(CATEGORY_MARKER))
                {
                    currentCategory = line.Substring(CATEGORY_MARKER.Length).Trim();
                    continue;
                }

                // Ignore this line if the category name is empty
                if (string.IsNullOrEmpty(currentCategory))
                {
                    continue;
                }

                // Add category to dictionary if it doesn't exist
                if (!Data.ContainsKey(currentCategory))
                {
                    Data.Add(currentCategory, new List<string>());
                }

                // Add this item to the category (but trimmed)
                Data[currentCategory].Add(line.Trim());
            }
        }
    }
}
