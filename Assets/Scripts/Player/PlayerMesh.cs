using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuickOutline;
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

        private Dictionary<Gamemode, MeshFilter[]> _meshFiltersDictionary = new Dictionary<Gamemode, MeshFilter[]>();
        private Dictionary<Gamemode, GameObject> _meshObjectDictionary = new Dictionary<Gamemode, GameObject>();
        private Dictionary<Gamemode, Collider> _meshHitboxDictionary = new Dictionary<Gamemode, Collider>();

        private Dictionary<Gamemode, MeshFilter> _meshTrailDictionary = new Dictionary<Gamemode, MeshFilter>();
        private Dictionary<Gamemode, int> _trailMaterialIndexDictionary = new Dictionary<Gamemode, int>();

        private Dictionary<Gamemode, Transform> _trailPositionDictionary = new Dictionary<Gamemode, Transform>();

        #region Properties
        public MeshFilter[] CurrentMeshFilters { get; private set; }
        public GameObject CurrentMeshObject { get; private set; }
        public Collider CurrentMeshHitbox { get; private set; }

        public MeshFilter CurrentTrailMesh { get; private set; }
        public int CurrentTrailMaterialIndex { get; private set; }

        public Transform CurrentTrailPosition { get; private set; }
        #endregion

        public override void Awake()
        {
            base.Awake();

            foreach (GamemodeMeshObject meshData in gamemodeMeshData)
            {
                // Create the new mesh dictionaries
                _meshFiltersDictionary.Add(meshData.Gamemode, meshData.MeshFilters);
                _meshObjectDictionary.Add(meshData.Gamemode, meshData.MeshObject);
                _meshHitboxDictionary.Add(meshData.Gamemode, meshData.MeshHitbox);

                _meshTrailDictionary.Add(meshData.Gamemode, meshData.MeshTrail);
                _trailMaterialIndexDictionary.Add(meshData.Gamemode, meshData.MeshTrailMaterialIndex);

                _trailPositionDictionary.Add(meshData.Gamemode, meshData.TrailPosition);

                // Disable all meshes by default
                meshData.MeshObject.SetActive(false);
                meshData.MeshHitbox.enabled = false;
            }

            // Update all the MeshFilters icon to match the currently equipped icon
            foreach (GamemodeMeshObject meshData in gamemodeMeshData)
            {
                ChangeIcon(meshData.Gamemode, PlayerIcons.GetCurrentMesh(meshData.Gamemode));
            }
        }

        public override void Start()
        {
            base.Start();

            // Subscribe to events
            player.GamemodeHandler.OnChangeGamemode += OnChangeGamemode;

            // Enable the correct mesh
            OnChangeGamemode(player.GamemodeHandler.CurrentGamemode);

            // Subscribe to main menu teleport event if we are in the main menu
            if (player.InMainMenu)
            {
                player.Movement.OnMainMenuTeleport += () =>
                {
                    // Update all the MeshFilters icon to match the currently equipped icon
                    foreach (GamemodeMeshObject meshData in gamemodeMeshData)
                    {
                        int index = Random.Range(0, PlayerIcons.GetIndexMaxLength(meshData.Gamemode));

                        ChangeIcon(meshData.Gamemode, PlayerIcons.MeshDataDictionary[meshData.Gamemode][index].Mesh, true);
                    }
                };
            }
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

            // Set the CurrentMeshFilters
            CurrentMeshFilters = _meshFiltersDictionary[newGamemode];

            // Get the meshObject with the given gamemode key and enable it
            GameObject meshObject = _meshObjectDictionary[newGamemode];
            meshObject.SetActive(true);

            // Set new CurrentMeshObject
            CurrentMeshObject = meshObject;

            // Do the same thing but for the hitbox
            Collider meshHitbox = _meshHitboxDictionary[newGamemode];
            meshHitbox.enabled = true;

            CurrentMeshHitbox = meshHitbox;

            // Also do the same thing but for the trails stuff
            CurrentTrailMesh = _meshTrailDictionary[newGamemode];
            CurrentTrailMaterialIndex = _trailMaterialIndexDictionary[newGamemode];
            CurrentTrailPosition = _trailPositionDictionary[newGamemode];
        }

        /// <summary>
        /// Toggles the currentMeshHitbox on/off based on <paramref name="enable"/>
        /// </summary>
        public void ToggleCurrentHitbox(bool enable)
        {
            if (CurrentMeshHitbox == null)
            {
                return;
            }

            CurrentMeshHitbox.enabled = enable;
        }

        /// <summary>
        /// Toggles the currentMeshObject on/off based on <paramref name="enable"/>
        /// </summary>
        public void ToggleCurrentMesh(bool enable)
        {
            if (CurrentMeshObject == null)
            {
                return;
            }

            CurrentMeshObject.SetActive(enable);
        }

        /// <summary>
        /// Changes the given <paramref name="gamemode"/> icon Mesh to the given <paramref name="newMesh"/>.
        /// </summary>
        public void ChangeIcon(Gamemode gamemode, Mesh newMesh, bool updateOutline = false)
        {
            // Loop through all MeshFilters of the given gamemode
            foreach (MeshFilter meshFilter in _meshFiltersDictionary[gamemode])
            {
                meshFilter.mesh = newMesh;

                // Remove and add back the outline component since it's stupid and only refreshes if you do this stupid stupid stupid
                if (updateOutline && meshFilter.TryGetComponent(out Outline outline))
                {
                    Destroy(outline);

                    Helpers.TimerEndOfFrame(this, () =>
                    {
                        outline = meshFilter.gameObject.AddComponent<Outline>();

                        outline.OutlineColor = Color.black;
                        outline.OutlineWidth = 10;
                    });
                }
            }
        }

        /// <summary>
        /// Class for storing a gamemode and mesh data
        /// </summary>
        [System.Serializable]
        public class GamemodeMeshObject
        {
            [Tooltip("The gamemode this array element is reffering to.")]
            public Gamemode Gamemode;

            [Space]
            [Tooltip("An array of all MeshFilters that should change their mesh when this gamemode icon is updated in the icon kit. \nThis is used for example, when the cube icon is changed, then the ships cube MeshFilter also needs to be updated along with the regular cube MeshFilter")]
            public MeshFilter[] MeshFilters;
            [Tooltip("The parent object of the entire mesh. This will be toggled on/off when the gamemode is active/inactive. Make sure that disabling this will hide the whole mesh.")]
            public GameObject MeshObject;
            [Tooltip("The hitbox of the entire mesh. This will be toggled on/off when the gamemode is active/inactive.")]
            public Collider MeshHitbox;

            [Space]
            [Tooltip("The MeshFilter that will be used to create trails. This is used for creating a cube trail for the ship for example.")]
            public MeshFilter MeshTrail;
            [Tooltip("The material that will be changed on the mesh to make it more see-through. This is here because some blender models could have their material indexes swapped around so this is just here to fix that issue.")]
            public int MeshTrailMaterialIndex;

            [Space]
            [Tooltip("Where the trail will be spawned. This is used for the ship gamemode to spawn the trail at the cube mesh. This can be left null if the trial should spawn on the MeshObject instead.")]
            public Transform TrailPosition;
        }
    }
}
