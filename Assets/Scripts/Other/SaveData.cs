using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using GD3D.Level;

namespace GD3D
{
    /// <summary>
    /// Class that saves and loads data from a save file.
    /// </summary>
    public class SaveData : MonoBehaviour
    {
        //-- Constants
        private const string SAVE_FILE_NAME = "SaveFile";
        private const string FORMAT = ".json";
        private static readonly string _saveFilePath = Path.Combine(Application.streamingAssetsPath, SAVE_FILE_NAME + FORMAT);

        //-- Instance
        public static SaveData Instance;

        public static SaveFile SaveFile = new SaveFile();
        public static SaveFile.LevelSaveData CurrentLevelData = null;

        private void Awake()
        {
            // Set the instance
            Instance = this;

            // Subscribe to Scene manager events
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void Start()
        {
            // Get the level data instance
            LevelData levelData = LevelData.Instance;

            // Get the level data for this level using the level name and cache it in a static variable so any script can reach it
            CurrentLevelData = GetLevelData(levelData.LevelName);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            // Load when the scene is loaded
            Load();
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            // Save when the scene is unloaded
            Save();
        }

        private void OnApplicationQuit()
        {
            // Save when the app closes down
            Save();
        }

        /// <summary>
        /// Saves this <see cref="SaveData"/> current data to the save file. <para/>
        /// You don't really have to call this at all because the game will save when the application closes down.
        /// </summary>
        public static void Save()
        {
            // Convert save file to json
            string jsonText = JsonUtility.ToJson(SaveFile, true);

            // Write the json into a file
            File.WriteAllText(_saveFilePath, jsonText);
        }

        /// <summary>
        /// Sets this <see cref="SaveData"/> current file data to the save file.
        /// </summary>
        public static void Load()
        {
            // Return if the file doesn't exist
            if (!File.Exists(_saveFilePath))
            {
                return;
            }

            // Read the json text from the file
            string jsonText = File.ReadAllText(_saveFilePath);

            // Convert the text into a SaveFile object
            SaveFile = JsonUtility.FromJson<SaveFile>(jsonText);
        }

        /// <summary>
        /// Returns the <see cref="SaveFile.LevelSaveData"/> of the level with the name of <paramref name="name"/>. <para/>
        /// If the <paramref name="name"/> doesn't exist, then a new entry is created and returned.
        /// </summary>
        public static SaveFile.LevelSaveData GetLevelData(string name)
        {
            // Try getting the data
            try
            {
                return SaveFile.LevelData.First((item) => item.name == name);
            }
            // this is the exception Linq.First will throw when there are no object that match. Kinda klunky but it works
            catch (InvalidOperationException)
            {
                // Create and return a new entry if the linq method fails
                SaveFile.LevelSaveData data = new SaveFile.LevelSaveData();
                data.name = name;

                SaveFile.LevelData.Add(data);

                return data;
            }
        }
    }

    /// <summary>
    /// Class that contains data for a players save file.
    /// </summary>
    [Serializable]
    public class SaveFile
    {
        public float MusicVolume = 1;
        public float SFXVolume = 1;

        public bool AutoRetryEnabled = true;
        public bool AutoCheckpointsEnabled = true;
        public bool ProgressBarEnabled = false;
        public bool ShowPercentEnabled = false;

        public List<LevelSaveData> LevelData = new List<LevelSaveData>();

        /// <summary>
        /// Class that contains data for a single level.
        /// </summary>
        [Serializable]
        public class LevelSaveData
        {
            public string name = "null";

            public bool completedLevel = false;

            public float normalPercent = 0;
            public float practicePercent = 0;

            public bool[] gottenCoins = new bool[] { false, false, false };

            public int totalAttempts = 0;
            public int totalJumps = 0;
        }
    }
}
