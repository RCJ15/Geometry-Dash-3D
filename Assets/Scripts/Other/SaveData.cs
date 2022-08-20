using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using GD3D.Level;
using GD3D.Player;

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

        [SerializeField] private AudioMixer audioMixer;

        //-- Static variables
        private static bool s_subscribedToEvents;

        //-- File
        public static SaveFile SaveFile = new SaveFile();
        public static SaveFile.LevelSaveData CurrentLevelData = null;

        /// <summary>
        /// Returns whether we are allowed to save or not. Currently saving is only disabled when playing in WebGL.
        /// </summary>
        public static bool CanSave => Application.platform != RuntimePlatform.WebGLPlayer;

        private void Awake()
        {
            if (Instance == null)
            {
                // Set the instance
                Instance = this;

                transform.SetParent(null);

                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            if (PlayerIcons.MeshDataDictionary == null)
            {
                // Find all PlayerIcons scripts (including disabled ones since FindObjectOfType will ignore those (?????))
                PlayerIcons[] allPlayerIcons = Resources.FindObjectsOfTypeAll<PlayerIcons>();

                // Try create dictionary 
                if (allPlayerIcons != null && allPlayerIcons.Length > 0 && allPlayerIcons[0] != null)
                {
                    allPlayerIcons[0].TryCreateDictionary();
                }
            }

            // Load at awake
            Load();

            LoadLevel();

            // Check if we have not subscribed to events
            if (!s_subscribedToEvents)
            {
                // Subscribe to Scene manager events
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.sceneUnloaded += OnSceneUnloaded;

                // This bool makes sure we only subscribe once
                s_subscribedToEvents = true;
            }
        }

        private void Start()
        {
            // Must call this in start because otherwise it won't work idk
            SetMixerVolume(SaveFile.MusicVolume, "Music Volume");
            SetMixerVolume(SaveFile.SFXVolume, "SFX Volume");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            // Load when the scene is loaded
            Load();

            LoadLevel();
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

        private static void LoadLevel()
        {
            // Get the level data instance
            LevelData levelData = LevelData.Instance;

            if (levelData != null)
            {
                // Get the level data for this level using the level name and cache it in a static variable so any script can reach it
                CurrentLevelData = SaveFile.GetLevelData(levelData.LevelName);
            }
        }

        /// <summary>
        /// Saves this <see cref="SaveData"/> current data to the save file. <para/>
        /// You don't really have to call this at all because the game will save when the application closes down.
        /// </summary>
        public static void Save()
        {
            // Return if we are in WebGL since saving files is not allowed there
            if (!CanSave)
            {
                return;
            }

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
            // Return if we are in WebGL since loading files is not allowed there
            if (!CanSave)
            {
                return;
            }

            // Return if the file doesn't exist
            if (!File.Exists(_saveFilePath))
            {
                return;
            }

            // Read the text from the file
            string jsonText = File.ReadAllText(_saveFilePath);

            // Try to convert the JSON text into a SaveFile object
            try
            {
                SaveFile = JsonUtility.FromJson<SaveFile>(jsonText);
            }
            // If it fails, throw error only in editor, otherwise return
            catch (Exception)
            {
#if UNITY_EDITOR
                throw;
#else
                return;
#endif
            }

            // Fix some saved values that are illegal
            if (string.IsNullOrEmpty(SaveFile.PlayerName)) SaveFile.PlayerName = SaveFile.DEFAULT_PLAYER_NAME;
            if (SaveFile.PlayerName.Length > SaveFile.PLAYER_NAME_MAX_LENGTH) SaveFile.PlayerName = SaveFile.PlayerName.Substring(0, SaveFile.PLAYER_NAME_MAX_LENGTH);

            if (SaveFile.StarsCollected < 0) SaveFile.StarsCollected = 0;
            if (SaveFile.CoinsCollected < 0) SaveFile.CoinsCollected = 0;

            SaveFile.SFXVolume = Mathf.Clamp01(SaveFile.SFXVolume);
            SaveFile.MusicVolume = Mathf.Clamp01(SaveFile.MusicVolume);

            // Clamp all icon data indexes between 0 and their index max length from the PlayerIcons script
            if (PlayerIcons.MeshDataDictionary != null)
            {
                foreach (var icon in SaveFile.IconData)
                {
                    icon.Index = Mathf.Clamp(icon.Index, 0, PlayerIcons.GetIndexMaxLength(icon.Gamemode));
                }
            }

            // Fix some saved values in each level that could be illegal
            foreach (var level in SaveFile.LevelData)
            {
                level.NormalPercent = Mathf.Clamp01(level.NormalPercent);
                level.PracticePercent = Mathf.Clamp01(level.PracticePercent);

                if (level.TotalAttempts < 0) level.TotalAttempts = 0;
                if (level.TotalJumps < 0) level.TotalJumps = 0;

                if (level.GottenCoins.Length != 3) level.GottenCoins = new bool[] { false, false, false };
            }
        }

        /// <summary>
        /// Sets the audio parameter called <paramref name="name"/> to the given <paramref name="volume"/> converted to the logarithmic scale.
        /// </summary>
        private void SetMixerVolume(float volume, string name)
        {
            // Scale the volume to use the logarithmic scale because decibel is stupid
            float scaledVolume = Mathf.Log10(volume) * 20;

            if (volume <= 0.0f)
            {
                scaledVolume = -80.0f;
            }

            audioMixer.SetFloat(name, scaledVolume);
        }
    }

    /// <summary>
    /// Class that contains data for a players save file. <para/>
    /// Use <see cref="SaveData.SaveFile"/> to access the global save file.
    /// </summary>
    [Serializable]
    public class SaveFile
    {
        public const string DEFAULT_PLAYER_NAME = "Name";
        public const int PLAYER_NAME_MAX_LENGTH = 30;

        public string PlayerName = DEFAULT_PLAYER_NAME;
        public int StarsCollected = 0;
        public int CoinsCollected = 0;

        public float MusicVolume = 1;
        public float SFXVolume = 1;

        public bool AutoRetryEnabled = true;
        public bool AutoCheckpointsEnabled = true;
        public bool ProgressBarEnabled = true;
        public bool ShowPercentEnabled = true;

        public ColorNoAlpha PlayerColor1 = PlayerColors.DefaultColor1;
        public ColorNoAlpha PlayerColor2 = PlayerColors.DefaultColor2;

        #region Saving Icons
        private Dictionary<Gamemode, int> _equippedIconIndex = null;

        /// <summary>
        /// Will return the index of the currently equipped icon of the given <paramref name="gamemode"/>.
        /// </summary>
        public int GetEquippedIconIndex(Gamemode gamemode)
        {
            TryCreateDictionary(gamemode);

            // Return the index in the dictionary
            return _equippedIconIndex[gamemode];
        }

        public void SetEquippedIcon(Gamemode gamemode, int index)
        {
            IconData[(int)gamemode].Index = index;
            _equippedIconIndex[gamemode] = index;
        }

        private void TryCreateDictionary(Gamemode gamemode)
        {
            // Check if our dictionary is null
            if (_equippedIconIndex == null)
            {
                // If so, then we will create a new dictionary for getting the equipped icon
                _equippedIconIndex = new Dictionary<Gamemode, int>();

                // We will now populate the dictionary by looping through our icon data list
                foreach (IconSaveData icon in IconData)
                {
                    _equippedIconIndex.Add(icon.Gamemode, icon.Index);
                }
            }

            // Check if the gamemode doesn't exist in the dictionary
            if (!_equippedIconIndex.ContainsKey(gamemode))
            {
                // If it doesn't exist, then we will create a new entry in the dictionary with the default value
                _equippedIconIndex.Add(gamemode, 0);
                IconData.Add(new IconSaveData(gamemode, 0));
            }
        }

        public List<IconSaveData> IconData = new List<IconSaveData>();

        /// <summary>
        /// Class that contains data for a single icon.
        /// </summary>
        [Serializable]
        public class IconSaveData
        {
            public Gamemode Gamemode = Gamemode.cube;
            public int Index = 0;

            public IconSaveData(Gamemode gamemode, int index)
            {
                Gamemode = gamemode;
                Index = index;
            }
        }
#endregion

#region Saving Level Progress
        private Dictionary<string, LevelSaveData> _levelDataDictionary = null;

        /// <summary>
        /// Will return the LevelData that matches with the given <paramref name="name"/>. If none is found, then a new empty LevelData is created and returned.
        /// </summary>
        public LevelSaveData GetLevelData(string name)
        {
            // Check if our dictionary is null
            if (_levelDataDictionary == null)
            {
                // If so, then we will create a new dictionary for getting the level data
                _levelDataDictionary = new Dictionary<string, LevelSaveData>();

                // We will now populate the dictionary by looping through our icon data list
                foreach (LevelSaveData levelData in LevelData)
                {
                    _levelDataDictionary.Add(levelData.Name, levelData);
                }
            }

            // Check if the level doesn't exist in the dictionary
            if (!_levelDataDictionary.ContainsKey(name))
            {
                // If it doesn't exist, then we will create a new entry in the dictionary with the given name
                LevelSaveData newData = new LevelSaveData(name);

                _levelDataDictionary.Add(name, newData);
                LevelData.Add(newData);
            }

            // Return the LevelData in the dictionary
            return _levelDataDictionary[name];
        }

        public List<LevelSaveData> LevelData = new List<LevelSaveData>();

        /// <summary>
        /// Class that contains data for a single level.
        /// </summary>
        [Serializable]
        public class LevelSaveData
        {
            public string Name = "null";

            public bool CompletedLevel => NormalPercent >= 1;
            public bool CompletedLevelPractice => PracticePercent >= 1;

            public float NormalPercent = 0;
            public float PracticePercent = 0;

            public bool[] GottenCoins = new bool[] { false, false, false };

            public int TotalAttempts = 0;
            public int TotalJumps = 0;

            public LevelSaveData(string name)
            {
                Name = name;
            }
        }
#endregion

        /// <summary>
        /// Simply a color struct without any alpha
        /// </summary>
        [Serializable]
        public struct ColorNoAlpha
        {
            public float r, g, b;

            public static implicit operator Color(ColorNoAlpha colorNoAlpha)
            {
                return new Color(colorNoAlpha.r, colorNoAlpha.g, colorNoAlpha.b, 1);
            }

            public static implicit operator ColorNoAlpha(Color col)
            {
                return new ColorNoAlpha(col.r, col.g, col.b);
            }

            public ColorNoAlpha(float r, float g, float b)
            {
                this.r = r;
                this.g = g;
                this.b = b;
            }
        }
    }
}
