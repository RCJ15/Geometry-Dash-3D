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

        [Space]

        private GameObject _currentMeshObject;

        public GameObject CurrentMeshObject => _currentMeshObject;

        private void Awake()
        {
            // Create the new mesh data dictionary and disable all the mesh objects
            foreach (GamemodeMeshObject meshData in gamemodeMeshData)
            {
                _meshDataDictionary.Add(meshData.gamemode, meshData.meshObject);

                meshData.meshObject.SetActive( false);
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

            // Get the meshObject with the given gamemode key and enable it
            GameObject meshObject = _meshDataDictionary[newGamemode];
            meshObject.SetActive(true);

            // Set new currentMeshObject
            _currentMeshObject = meshObject;
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
        }
    }
}
