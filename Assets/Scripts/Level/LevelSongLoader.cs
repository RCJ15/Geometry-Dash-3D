using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

namespace GD3D.Level
{
    /// <summary>
    /// The class that loads all audio files that can be used as songs. Call <see cref="ReloadSongs"/> in order to reload the <see cref="Songs"/> dictionary.
    /// </summary>
    public class LevelSongLoader : MonoBehaviour
    {
        public static readonly string SongsPath = Path.Combine(Application.streamingAssetsPath, "Songs");

        public static Dictionary<string, AudioClip> Songs = new Dictionary<string, AudioClip>();

        public static LevelSongLoader Instance;

        //-- Variables used to determine if the songs have been loaded or not
        private static int _totalSongs;
        private static int _songsLoaded;

        public static bool AllSongsLoaded => _songsLoaded >= _totalSongs;

        private void Awake()
        {
            // Set singleton instance
            Instance = this;

            DontDestroyOnLoad(gameObject);

            LoadAllSongs();
        }

        private void LoadAllSongs()
        {
            LoadDirectory(SongsPath);

            // Loop through all the directories in the "Custom Songs" folder
            foreach (string directory in Directory.GetDirectories(SongsPath))
            {
                LoadDirectory(directory);
            }
        }

        public static void ReloadSongs()
        {
            // Clear the old dictionary
            Songs = new Dictionary<string, AudioClip>();

            _totalSongs = 0;
            _songsLoaded = 0;

            Instance.LoadAllSongs();
        }

        private void LoadDirectory(string directory)
        {
            // Loop through all files in the directory
            foreach (string filePath in Directory.GetFiles(directory))
            {
                // Get the file extension from the file path
                string fileExtension = Path.GetExtension(filePath).ToLower();

                // Ignore files that are not MP3, WAV or OGG files
                if (fileExtension != ".mp3" && fileExtension != ".wav" && fileExtension != ".ogg")
                {
                    continue;
                }

                // Default to MP3 format
                AudioType audioType;

                switch (fileExtension)
                {
                    case ".wav":
                        audioType = AudioType.WAV;
                        break;

                    case ".ogg":
                        audioType = AudioType.OGGVORBIS;
                        break;

                    // Default is MP3
                    default:
                        audioType = AudioType.MPEG;
                        break;
                }

                _totalSongs++;

                // Load the song into the dictionary!
                StartCoroutine(LoadSong(filePath, audioType, () =>
                {
                    _songsLoaded++;
                }));
            }
        }

        private IEnumerator LoadSong(string path, AudioType audioType = AudioType.MPEG, Action onFinishLoad = null)
        {
            using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, audioType);

            // Send web request (will still work offline since this is searching for a file on the computer, not the online web)
            yield return www.SendWebRequest();

            // Check if the result was successful
            if (www.result == UnityWebRequest.Result.Success)
            {
                // If so, then we download the content as an audio clip
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);

                // Determine a unique name for the song
                string fileName = path.Substring(SongsPath.Length + 1);

                int lastDotIndex = fileName.LastIndexOf('.');

                fileName = fileName.Substring(0, lastDotIndex);

                string songName = fileName;
                int number = 1;

                // Loop until the song name is unique by adding a number to the end
                while (Songs.ContainsKey(songName))
                {
                    songName = $"{fileName} {number}";
                    number++;
                }

                // Cache the song in a dictionary
                Songs.Add(songName, clip);
            }
            // The load was not successful
            else
            {
#if UNITY_EDITOR
                // Throw error
                throw new FileLoadException("Error trying to load song at path: \"" + path + "\". Result message from UnityWebRequest: \"" + www.result.ToString() + "\"");
#endif
            }

            // Invoke on finish action
            onFinishLoad?.Invoke();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            // Create an instance when the game starts
            new GameObject("Song Loader", typeof(LevelSongLoader));
        }
    }
}
