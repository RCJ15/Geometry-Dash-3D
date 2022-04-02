using GD3D.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using PathCreation;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Audio;
#endif

namespace GD3D.Audio
{
    /// <summary>
    /// Will play the level music
    /// </summary>
    [ExecuteAlways]
    public class MusicPlayer : MonoBehaviour
    {
        //-- Static Variables
        public static MusicPlayer Instance;

        [Header("Important Stuff")]
        [SerializeField] private AudioClip song;

        [Header("BPM")]
        [SerializeField] private float bpm = 160;
        [SerializeField] private float bpmStartOffset = 0;

#if UNITY_EDITOR
        [SerializeField] private bool showBpmLines;
#endif

        public static readonly float[] MAIN_LEVEL_BPMS = new float[]
        {
            160, // Stereo Madness
            142, // Back on Track
            163, // Polargeist
            145, // Dry Out
            141, // Base After Base
            170, // Cant Let Go
            143, // Time Machine
            175, // Jumper
            130, // xStep
            140, // Cycles
            130, // Theory of Everything
            170, // Electroman Adventures
            128, // Clubstep
            127, // Electrodynamix
            140, // Clutterfunk
            81,  // Hexagon Force
            132, // Theory of Everything 2
            135, // Blast Processing
            135, // Deadlocked
            148, // Geometrical Dominator
            112, // Fingerdash
        };

        //-- Other References
        private AudioSource _source;
        private PlayerMain _player;
        private PlayerMovement _playerMovement;

        private PathCreator _pathCreator;
        private VertexPath Path => _pathCreator.path;

        [HideInInspector] public float EndDistance;

        private void Awake()
        {
            // Set instance
            Instance = this;
        }

        private void Start()
        {
            // Get references
            _source = GetComponent<AudioSource>();
            _player = PlayerMain.Instance;

            if (_player != null && _player.movement != null)
            {
                _playerMovement = _player.movement;
            }

            _pathCreator = FindObjectOfType<PathCreator>();

            // Setup the audio source
            _source.clip = song;

#if UNITY_EDITOR
            // Editor stuff
            OnEditorChange();

            if (!Application.isPlaying)
            {
                return;
            }
#endif

            // Subscribe to events
            _player.OnDeath += OnDeath;
            _player.OnRespawn += OnRespawn;
            _source.Play();
        }

        public void PlayAtDistance(float distance)
        {
            float playerDist = Path.GetClosestDistanceAlongPath(_player.transform.position);
            float time = 0;

            if (distance < playerDist)
            {
                distance = playerDist;
            }

            int length = Mathf.CeilToInt(distance * 10);
            float currentDistance = playerDist;

            for (int i = 0; i < length; i++)
            {
                float speed = _playerMovement.GetSpeedAtDistance(currentDistance);

                float addedTime = (float)((distance - playerDist) / speed) / (float)length;
                time += addedTime;

                currentDistance += speed * addedTime;
            }

            time = Mathf.Clamp(time, 0, _source.clip.length);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                float frequencyMultiplier = (float)44100 / (float)_source.clip.frequency;

                float samplesPerSecond = (float)_source.clip.samples / (float)_source.clip.length;

                int sampleStart = Mathf.CeilToInt(samplesPerSecond * time * frequencyMultiplier);

                AudioUtility.StopAllPreviewClips();
                AudioUtility.PlayPreviewClip(_source.clip, sampleStart);
                return;
            }
#endif

            _source.time = time;
            _source.Play();
        }

        public void Stop()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                AudioUtility.StopAllPreviewClips();
            }
#endif

            _source.Stop();
        }

        private void OnDeath()
        {
            _source.Stop();
        }

        private void OnRespawn()
        {
            _source.Play();
        }

        /// <summary>
        /// Updates the distance at the path located at the end of the song
        /// </summary>
        public void UpdateEndDistance()
        {
            float playerDist = Path.GetClosestDistanceAlongPath(_player.transform.position);
            float distance = playerDist;

            float time = _source.clip.length;

            int length = Mathf.CeilToInt((Path.length - playerDist) * 10);

            for (int i = 0; i < length; i++)
            {
                float speed = _playerMovement.GetSpeedAtDistance(distance);

                distance += (float)(speed * time) / (float)length;
            }

            EndDistance = distance;
        }

        /// <summary>
        /// Returns the correct distance for the song at the given <paramref name="musicTime"/>
        /// </summary>
        public float GetDistanceFromMusicTime(float musicTime)
        {
            float playerDist = Path.GetClosestDistanceAlongPath(_player.transform.position);

            float distance = Helpers.Map(0, _source.clip.length, playerDist, EndDistance, musicTime);

            return distance;
        }

        #region Gizmos
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_player == null || _playerMovement == null || _pathCreator == null)
            {
                return;
            }

            // Set player color
            Color playerColor = Color.black;
            playerColor.a = 0.5f;

            Gizmos.color = playerColor;

            // Draw player line
            Vector3 playerPos = Path.GetClosestPointOnPath(_player.transform.position);

            PlaceBottomTopLine(playerPos);

            // Set the camera color
            Color cameraColor = Color.white;
            cameraColor.a = 0.5f;

            Gizmos.color = cameraColor;

            // Draw camera line
            float camDist = Path.GetClosestDistanceAlongPath(SceneView.lastActiveSceneView.camera.transform.position);
            Vector3 cameraPos = Path.GetPointAtDistance(camDist, EndOfPathInstruction.Stop);

            PlaceBottomTopLine(cameraPos);

            // Draw bpm lines if showBpmLines is true
            if (showBpmLines)
            {
                // Set bpm color
                Color bpmColor = new Color(0.9607843137254902f, 0.6549019607843137f, 0.2588235294117647f, 0.5f);

                Gizmos.color = bpmColor;

                // Beats per second
                float beatsPerSecond = 60f / bpm;

                // Loop for the amount of beats in the song length
                int length = Mathf.CeilToInt(_source.clip.length / beatsPerSecond);

                for (int i = 0; i < length; i++)
                {
                    float bpmDist = GetDistanceFromMusicTime(bpmStartOffset + (float)beatsPerSecond * (float)i);

                    // Do not draw bpm line if they are outside the range of the Path length
                    if (bpmDist >= Path.length)
                    {
                        break;
                    }

                    Vector3 bpmPos = Path.GetPointAtDistance(bpmDist, EndOfPathInstruction.Stop);

                    bpmColor.a = i % 4 == 0 ? 0.5f : 0.2f;

                    Gizmos.color = bpmColor;

                    PlaceBottomTopLine(bpmPos);
                }
            }
        }

        private void OnDrawGizmos()
        {
            // Do not draw music line if no music is playing
            if (!AudioUtility.IsPreviewClipPlaying())
            {
                return;
            }

            // Set the music color
            Color musicColor = Color.green;
            musicColor.a = 0.5f;

            Gizmos.color = musicColor;

            // Draw music line
            float musicTime = AudioUtility.GetPreviewClipPosition();

            float distance = GetDistanceFromMusicTime(musicTime);

            Vector3 musicPos = Path.GetPointAtDistance(distance, EndOfPathInstruction.Stop);

            PlaceBottomTopLine(musicPos);
        }

        private void PlaceBottomTopLine(Vector3 pos)
        {
            Vector3 bottomPos = pos;
            bottomPos.y = -100;

            Vector3 topPos = pos;
            topPos.y = 100;

            Gizmos.DrawLine(bottomPos, topPos);
        }

        internal void OnEditorChange()
        {
            // Correct missing components in case we are missing them in the editor
            if (_player == null)
            {
                _player = FindObjectOfType<PlayerMain>();
            }

            if (_playerMovement == null)
            {
                _playerMovement = FindObjectOfType<PlayerMovement>();
            }

            // Setup the audio source
            _source.clip = song;

            // Update end distance if the song is not null
            if (song != null)
            {
                UpdateEndDistance();
            }
        }
#endif
        #endregion
    }

    #region Editor
#if UNITY_EDITOR
    [CustomEditor(typeof(MusicPlayer))]
    public class MusicPlayerEditor : Editor
    {
        private const string PLAY_BUTTON = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAJYSURBVFhHxZe7rhJRFIYXdwMmSAIkBAouCYFwhxBew0ew8AF8BC0sLEjO0Hg6m/MAJnRUlnZa2GlLNDRoJ+GE8V+LvVBGDteZ8CUrZ886Q9Y3+zJ7D9m2/ZSuCQSYT4jriKzrb/iKeIYImn97j5QF8/nctAQWeY54ZG7zDikHRqORPZlM7NlsZjLCd8QLhGciPq7CDQhIgikWizQYDCiVSpkM/UBYiFufz/dTMi6xU0DJ5/PU7XYpl8vJNW79BYEbNG/cEtkroGSzWekRh8g7NN/gL/fO2RwloKTTaer3+1QqleQaP11A4C2aZ4ucJKAkk0npkUKhQH6/n1P3iDvEa4h848SxnCWgxONx6ZFKpeIUGULkCycOcZGAwiI8WavVKgWDm3fYe8QriHxeX+7GFQElGo1Sr9ejRqPhFLEg8mF9uY2rAgqLNJtNarVaFIlETJZYgHtkS8QTASUcDlOn06F2u/2vyEcErxruGW8FlFAoRLVaTYYnFouZLPHcGMrU9ZrVakWLxYKWy6XJCI8RQU97IBAIyIR0PDkvzyHiDsNw74kAj329Xpfxd3Q5T0IZe8VVgQcmHc96XoZbhRVXBHjZcWE8tY3CPpPeueycXCSw58XDy4yX20HOEtA9oFwu2yisT8yFD756nZwkkEgk5Il3bD4n74LKUQK6/eo5APxGcGHu6rMKK3sFMpmM7HKOwreIi09Cyk4B5xEM8PmPC/NycqXwBhZgLMuyx+OxPZ1OTUbgj4WXiCfmdveRMuCB7wHvCitS7i+ef4j8h5S9xjehcrXCAtEfg++ulBk+lqkAAAAASUVORK5CYII=";
        private const string STOP_BUTTON = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABLSURBVFhH7dexDQAgCAVRdGCGYWEl5JeWGizuGrB7odOou6FZrUzr00am1aZmW8cLRES9b+fuNb+6AAAAAAAAAAAAAAAA/AuoObMNCgkPMmti+GMAAAAASUVORK5CYII=";

        private MusicPlayer musicPlayer;
        private PathCreator pathCreator;

        private static bool PreviewClipPlaying;

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            GUILayout.Space(10);

            // Load texture from base 64 string
            byte[] bytes = Convert.FromBase64String(PreviewClipPlaying ? STOP_BUTTON : PLAY_BUTTON);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);

            // Create this style to make sure that the width of the button doesn't take up the entire inspector
            GUIStyle newStyle = new GUIStyle() { stretchWidth = false };

            // Also cache some stuff for a bit more optimization
            Transform sceneCam = SceneView.lastActiveSceneView.camera.transform;
            VertexPath path = pathCreator.path;

            // Create button
            if (GUILayout.Button(texture, newStyle))
            {
                // Stop music if it's already playing
                if (PreviewClipPlaying)
                {
                    musicPlayer.Stop();
                }
                // Start music if it't not playing
                else if (sceneCam != null)
                {
                    float dist = path.GetClosestDistanceAlongPath(sceneCam.position);
                    musicPlayer.PlayAtDistance(dist);
                }
            }

            // Make a "Follow Song Line" button appear when the clip is playing
            if (PreviewClipPlaying)
            {
                if (GUILayout.Button("Follow Song Line"))
                {
                    // Calculate distance using music time
                    float musicTime = AudioUtility.GetPreviewClipPosition();
                    float dist = musicPlayer.GetDistanceFromMusicTime(musicTime);

                    // Calculate position using distance
                    Vector3 pos = path.GetPointAtDistance(dist, EndOfPathInstruction.Stop);

                    // Make the camera look at the position on the path
                    SceneView.lastActiveSceneView.LookAt(pos);
                }
            }

            // Call OnEditorChange when the editor changes (duh)
            if (EditorGUI.EndChangeCheck())
            {
                musicPlayer.OnEditorChange();
            }
        }

        private void OnEnable()
        {
            // Setup
            musicPlayer = (MusicPlayer)target;
            pathCreator = FindObjectOfType<PathCreator>();

            // Subscribe :)
            EditorApplication.update += Update;
        }

        private void OnDisable()
        {
            // Unsubscribe :(
            EditorApplication.update -= Update;
        }

        private static void Update()
        {
            PreviewClipPlaying = AudioUtility.IsPreviewClipPlaying();

            // Constantly repaint the scene if the song is playing so the line won't have janky movement
            if (PreviewClipPlaying)
            {
                SceneView.RepaintAll();
            }
        }
    }
#endif
    #endregion
}
