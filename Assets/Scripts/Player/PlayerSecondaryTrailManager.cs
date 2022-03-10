using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.ObjectPooling;

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
        private ObjectPool<PlayerSecondaryTrail> pool;

        private MeshFilter meshFilter;

        private void Awake()
        {
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
            pool = new ObjectPool<PlayerSecondaryTrail>(obj, poolSize);

            // Destroy the newly created object because we have no use out of it anymore
            Destroy(obj);

            // Subscribe to events
            player.gamemode.OnChangeGamemode += OnChangeGamemode;
            player.OnDeath += OnDeath;

            // Update gamemode manually
            OnChangeGamemode(player.gamemode.CurrentGamemode);
        }

        public override void Update()
        {
            base.Update();

            if (pool.IsEmpty() || !HaveTrail || player.dead)
            {
                return;
            }

            if (_currentTimeBtwTrails > 0)
            {
                _currentTimeBtwTrails -= Time.deltaTime;
                return;
            }

            // Spawn trail
            PlayerSecondaryTrail trail = pool.SpawnFromPool(meshFilter.transform.position, meshFilter.transform.rotation);
            trail.RemoveAfterTime(trailLifetime);

            // Tween scale
            trail.transform.localScale = meshFilter.transform.lossyScale - (Vector3.one * 0.01f);
            trail.transform.LeanScale(trail.transform.localScale / 2, trailLifetime).setEase(LeanTweenType.linear);

            _currentTimeBtwTrails = timeBtwTrails;
        }

        private void OnDeath()
        {
            HaveTrail = false;
        }

        private void OnChangeGamemode(Gamemode mode)
        {
            // Get mesh filter
            meshFilter = player.mesh.CurrentTrailMesh;

            foreach (PlayerSecondaryTrail trail in pool.Queue)
            {
                // Update mesh
                trail.UpdateMesh(meshFilter.sharedMesh, player.mesh.CurrentTrailMaterialIndex);
            }
        }
    }
}
