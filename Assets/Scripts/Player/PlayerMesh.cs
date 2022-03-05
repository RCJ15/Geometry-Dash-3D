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
    /// 
    /// </summary>
    public class PlayerMesh : PlayerScript
    {
        [SerializeField] private GamemodeMeshObject[] gamemodeMeshData;

        private Dictionary<Gamemode, GameObject> _meshDataDictionary = new Dictionary<Gamemode, GameObject>();
        private Dictionary<Gamemode, Collider> _meshHitboxDictionary = new Dictionary<Gamemode, Collider>();

        [Space]

        private GameObject _currentMeshObject;
        private Collider _currentMeshHitbox;

        public GameObject CurrentMeshObject => _currentMeshObject;
        public Collider CurrentMeshHitbox => _currentMeshHitbox;

        private void Awake()
        {
            foreach (GamemodeMeshObject meshData in gamemodeMeshData)
            {
                // Create the new mesh dictionaries
                _meshDataDictionary.Add(meshData.gamemode, meshData.meshObject);
                _meshHitboxDictionary.Add(meshData.gamemode, meshData.meshHitbox);

                // Disable all meshes by default
                meshData.meshObject.SetActive( false);
                meshData.meshHitbox.enabled = false;
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
            if (!_meshDataDictionary.ContainsKey(newGamemode))
            {
                return;
            }

            // Disable old gamemode and enable new gamemode
            ToggleCurrentMesh(false);
            ToggleCurrentHitbox(false);

            // Get the meshObject with the given gamemode key and enable it
            GameObject meshObject = _meshDataDictionary[newGamemode];
            meshObject.SetActive(true);

            // Set new currentMeshObject
            _currentMeshObject = meshObject;

            // Do the same thing but for the hitbox
            Collider meshHitbox = _meshHitboxDictionary[newGamemode];
            meshHitbox.enabled = true;

            _currentMeshHitbox = meshHitbox;
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
            public Gamemode gamemode;
            public GameObject meshObject;
            public Collider meshHitbox;
        }
    }
}
