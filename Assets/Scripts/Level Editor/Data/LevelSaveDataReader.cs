using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace GD3D.LevelEditor
{
    /// <summary>
    /// Can save and load level data in the level editor. Will create itself when the game is started so no need to put this onto a GameObject.
    /// </summary>
    public class LevelSaveDataReader : MonoBehaviour
    {
        public const string LevelFileExtension = ".json";

        public static readonly string MainLevelsFolder = Path.Combine(Application.streamingAssetsPath, "Main Levels");
        public static readonly string UserLevelsFolder = Path.Combine(Application.streamingAssetsPath, "User Levels");

        public static Dictionary<string, string> MainLevels = new Dictionary<string, string>();
        public static Dictionary<string, string> UserLevels = new Dictionary<string, string>();

        private void Start()
        {
        
        }

        private void Update()
        {

        }

        public static void ReloadMainLevels()
        {
            LoadLevelsIntoDictionary(ref MainLevels, MainLevelsFolder);
        }

        public static void ReloadUserLevels()
        {
            LoadLevelsIntoDictionary(ref UserLevels, UserLevelsFolder);
        }

        private static void LoadLevelsIntoDictionary(ref Dictionary<string, string> dictionary, string folder)
        {
            // Clear the dictionary
            dictionary.Clear();

            // Loop through all file paths in the given folder
            foreach (string levelPath in Directory.GetFiles(folder))
            {
                // Go to the next file if this file is not a level file
                if (Path.GetExtension(levelPath).ToLower() != LevelFileExtension)
                {
                    continue;
                }

                // Add the level into the dictionary
                dictionary.Add(Path.GetFileNameWithoutExtension(levelPath), levelPath);
            }
        }

        /// <summary>
        /// Saves level data into a json file in either the Main Levels folder or User Levels folder depending on if <paramref name="isMainLevel"/> is true or not.
        /// </summary>
        /// <returns>If the save was successful or not. Will only be false if the json parse gives an error.</returns>
        public static bool Save(LevelSaveData data, bool isMainLevel = false)
        {
            string json;

            try
            {
                // Try to parse the level data into json
                json = JsonUtility.ToJson(data);
            }
            catch (System.Exception)
            {
#if UNITY_EDITOR
                // Throw error in editor
                throw;
#else
                // Return false as we did not save the level successfully
                return false;
#endif
            }

            // Determine file path
            string path = Path.Combine(isMainLevel ? MainLevelsFolder : UserLevelsFolder, data.Name + LevelFileExtension);

            // Write json data into file
            File.WriteAllText(path, json);

            // Return true as we successfully saved the level
            return true;
        }

        public static LevelSaveData Load(string levelName, bool isMainLevel = false)
        {
            string path = isMainLevel ? MainLevelsFolder : UserLevelsFolder;

            path = Path.Combine(path, levelName + LevelFileExtension);

            // Return null if the file doesn't exist
            if (!File.Exists(path))
            {
                return null;
            }

            try
            {
                string json = File.ReadAllText(path);

                // Try to parse the json into level data
                return JsonUtility.FromJson<LevelSaveData>(json);
            }
            catch (System.Exception)
            {
#if UNITY_EDITOR
                // Throw error in editor
                throw;
#else
                // Return null as we did not load the level successfully
                return null;
#endif
            }
        }
    }
}
