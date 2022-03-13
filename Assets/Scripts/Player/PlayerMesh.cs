using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace GD3D.Player
{
    /// <summary>
    /// Contains data about the players mesh
    /// </summary>
    public class PlayerMesh : PlayerScript
    {
        [SerializeField] private GamemodeMeshObject[] gamemodeMeshData;

        private Dictionary<Gamemode, GameObject> _meshObjectDictionary = new Dictionary<Gamemode, GameObject>();
        private Dictionary<Gamemode, Collider> _meshHitboxDictionary = new Dictionary<Gamemode, Collider>();

        private Dictionary<Gamemode, MeshFilter> _meshTrailDictionary = new Dictionary<Gamemode, MeshFilter>();
        private Dictionary<Gamemode, int> _trailMaterialIndexDictionary = new Dictionary<Gamemode, int>();
        
        private Dictionary<Gamemode, Transform> _trailPositionDictionary = new Dictionary<Gamemode, Transform>();

        [Space]

        private GameObject _currentMeshObject;
        private Collider _currentMeshHitbox;

        private MeshFilter _currentTrailMesh;
        private int _currentTrailMaterialIndex;

        private Transform _currentTrailPosition;

        #region Properties
        public GameObject CurrentMeshObject => _currentMeshObject;
        public Collider CurrentMeshHitbox => _currentMeshHitbox;

        public MeshFilter CurrentTrailMesh => _currentTrailMesh;
        public int CurrentTrailMaterialIndex => _currentTrailMaterialIndex;

        public Transform CurrentTrailPosition => _currentTrailPosition;
        #endregion

        private void Awake()
        {
            foreach (GamemodeMeshObject meshData in gamemodeMeshData)
            {
                // Create the new mesh dictionaries
                _meshObjectDictionary.Add(meshData.Gamemode, meshData.MeshObject);
                _meshHitboxDictionary.Add(meshData.Gamemode, meshData.MeshHitbox);

                _meshTrailDictionary.Add(meshData.Gamemode, meshData.MeshTrail);
                _trailMaterialIndexDictionary.Add(meshData.Gamemode, meshData.MeshTrailMaterialIndex);

                _trailPositionDictionary.Add(meshData.Gamemode, meshData.TrailPosition);

                // Disable all meshes by default
                meshData.MeshObject.SetActive( false);
                meshData.MeshHitbox.enabled = false;
            }
        }

        public override void Start()
        {
            base.Start();

            // Subscribe to events
            player.gamemode.OnChangeGamemode += OnChangeGamemode;

            // Enable the correct mesh
            OnChangeGamemode(player.gamemode.CurrentGamemode);
        }

        private void OnChangeGamemode(Gamemode newGamemode)
        {
            // Return if the given gamemode does not exist
            if (!_meshObjectDictionary.ContainsKey(newGamemode))
            {
                return;
            }

            // Disable old gamemode and enable new gamemode
            ToggleCurrentMesh(false);
            ToggleCurrentHitbox(false);

            // Get the meshObject with the given gamemode key and enable it
            GameObject meshObject = _meshObjectDictionary[newGamemode];
            meshObject.SetActive(true);

            // Set new currentMeshObject
            _currentMeshObject = meshObject;

            // Do the same thing but for the hitbox
            Collider meshHitbox = _meshHitboxDictionary[newGamemode];
            meshHitbox.enabled = true;

            _currentMeshHitbox = meshHitbox;

            // Also do the same thing but for the trails stuff
            _currentTrailMesh = _meshTrailDictionary[newGamemode];
            _currentTrailMaterialIndex = _trailMaterialIndexDictionary[newGamemode];
            _currentTrailPosition = _trailPositionDictionary[newGamemode];
        }

        /// <summary>
        /// Toggles the currentMeshHitbox on/off based on <paramref name="enable"/>
        /// </summary>
        public void ToggleCurrentHitbox(bool enable)
        {
            if (_currentMeshHitbox == null)
            {
                return;
            }

            _currentMeshHitbox.enabled = enable;
        }

        /// <summary>
        /// Toggles the currentMeshObject on/off based on <paramref name="enable"/>
        /// </summary>
        public void ToggleCurrentMesh(bool enable)
        {
            if (_currentMeshObject == null)
            {
                return;
            }

            _currentMeshObject.SetActive(enable);
        }

        /// <summary>
        /// Class for storing a gamemode and mesh data
        /// </summary>
        [System.Serializable]
        public class GamemodeMeshObject
        {
            public Gamemode Gamemode;

            [Space]
            public GameObject MeshObject;
            public Collider MeshHitbox;

            [Space]
            public MeshFilter MeshTrail;
            public int MeshTrailMaterialIndex;

            [Space]
            public Transform TrailPosition;
        }
    }
}
