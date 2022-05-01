using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.ObjectPooling;
using GD3D.Easing;

namespace GD3D.Player
{
    /// <summary>
    /// Spawns all the objects for the players secondary trail, which is a bunch of copies of the player that slowly fade out.
    /// </summary>
    public class PlayerSecondaryTrailManager : PlayerScript
    {
        public static PlayerSecondaryTrailManager Instance;

        private bool _haveTrail = false;
        public static bool HaveTrail
        {
            get => Instance._haveTrail;
            set
            {
                PlayerSecondaryTrailManager I = Instance;

                // Only do this if the values are different, otherwise don't bother
                if (I._haveTrail != value)
                {
                    // Have the trail, so start spawning trails
                    if (value)
                    {
                        I._currentTimeBtwTrails = 0;
                    }

                    I._haveTrail = value;
                }
            }
        }

        [Header("Settings")]
        [SerializeField] private PlayerSecondaryTrail trailOriginal;
        [SerializeField] private float timeBtwTrails = 1 / PlayerMovement.NORMAL_SPEED / 2; // Do this since the player spawn about 1 trail per every half block traveled on normal speed
        private float _currentTimeBtwTrails;
        [SerializeField] private float trailLifetime = 1;

        [Header("Pool")]
        [SerializeField] private int poolSize = 20;
        private ObjectPool<PlayerSecondaryTrail> _pool;

        private MeshFilter _meshFilter;

        public override void Awake()
        {
            base.Awake();

            // Set the instance
            Instance = this;
        }

        public override void Start()
        {
            base.Start();

            // Setup the deathEffect obj by creating a copy and setting the copy
            GameObject obj = Instantiate(trailOriginal.gameObject, transform.position, Quaternion.identity);
            obj.name = trailOriginal.gameObject.name;

            // Change the MaterialColorer to match the player colors
            MaterialColorer colorer = obj.GetComponentInChildren<MaterialColorer>();

            // Make sure to have the same alpha value
            Color playerColor = PlayerColor1;
            playerColor.a = colorer.GetColor.a;

            colorer.GetColor = playerColor;

            // Create pool
            _pool = new ObjectPool<PlayerSecondaryTrail>(obj, poolSize);

            // Destroy the newly created object because we have no use out of it anymore
            Destroy(obj);

            // Subscribe to events
            player.GamemodeHandler.OnChangeGamemode += OnChangeGamemode;
            player.OnDeath += OnDeath;
            player.OnRespawn += OnRespawn;

            // Update gamemode manually
            OnChangeGamemode(player.GamemodeHandler.CurrentGamemode);
        }

        public override void Update()
        {
            base.Update();

            if (_pool.IsEmpty() || !HaveTrail || player.IsDead)
            {
                return;
            }

            if (_currentTimeBtwTrails > 0)
            {
                _currentTimeBtwTrails -= Time.deltaTime;
                return;
            }

            // Spawn trail
            PlayerSecondaryTrail trail = _pool.SpawnFromPool(_meshFilter.transform.position, _meshFilter.transform.rotation);
            trail.RemoveAfterTime(trailLifetime);

            // Ease the scale
            trail.transform.localScale = _meshFilter.transform.lossyScale - (Vector3.one * 0.01f);
            trail.transform.EaseScale(trail.transform.localScale / 2, trailLifetime);

            _currentTimeBtwTrails = timeBtwTrails;
        }

        private void OnDeath()
        {
            // Disable the trail
            HaveTrail = false;
        }

        private void OnRespawn(bool inPracticeMode, Checkpoint checkpoint)
        {
            // Set if trail is enabled or not based on the checkpoint data
            if (inPracticeMode)
            {
                HaveTrail = checkpoint.SecondaryTrailEnabled;
            }
            else
            {
                // Disabled if we are not in practice mode
                HaveTrail = false;
            }
        }

        private void OnChangeGamemode(Gamemode mode)
        {
            // Get mesh filter
            _meshFilter = player.Mesh.CurrentTrailMesh;

            foreach (PlayerSecondaryTrail trail in _pool.Queue)
            {
                // Update mesh
                trail.UpdateMesh(_meshFilter.sharedMesh, player.Mesh.CurrentTrailMaterialIndex);
            }
        }
    }
}
