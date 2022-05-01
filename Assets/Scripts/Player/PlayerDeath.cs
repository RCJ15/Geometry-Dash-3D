using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Audio;
using GD3D.ObjectPooling;

namespace GD3D.Player
{
    /// <summary>
    /// Handles the players death. Including detecting when the player should die. Disable this script to basically enable noclip.
    /// </summary>
    public class PlayerDeath : PlayerScript
    {
        [Header("Main")]
        [SerializeField] private LayerMask deathLayer;
        private bool _touchingDeath;

        [Header("Death Effect Pooling")]
        [SerializeField] private int poolSize = 2;
        private ObjectPool<PoolObject> _pool;

        [Space]
        [SerializeField] private PoolObject deathEffect;

        //-- HashSets
        // This is mainly here so we can later update the death effect colors when the player dies in the main menu
        private HashSet<ParticleSystemRenderer> _particleRenderers = new HashSet<ParticleSystemRenderer>();
        private HashSet<MaterialColorer> _materialColorers = new HashSet<MaterialColorer>();

        public override void Start()
        {
            base.Start();

            // Setup the deathEffect obj by creating a copy and setting the copy
            GameObject obj = Instantiate(deathEffect.gameObject, transform.position, Quaternion.identity);
            obj.name = deathEffect.gameObject.name;

            // Change the particles color to match the first player color
            ParticleSystemRenderer particles = obj.GetComponentInChildren<ParticleSystemRenderer>();

            MaterialColorer.UpdateRendererMaterials(particles, PlayerColor1, true, true);

            // Change the death sphere color to match the player colors
            MaterialColorer colorer = obj.GetComponentInChildren<MaterialColorer>();

            // Make sure to have the same alpha value
            Color playerColor = GetPlayerColor(colorer.GetColor.a);

            colorer.GetColor = playerColor;

            // Create deathEffect pool
            _pool = new ObjectPool<PoolObject>(obj, poolSize, (obj) =>
            {
                // Add components to HashSets
                _particleRenderers.Add(obj.GetComponentInChildren<ParticleSystemRenderer>());
                _materialColorers.Add(obj.GetComponentInChildren<MaterialColorer>());
            });

            // Destroy the newly created object because we have no use out of it anymore
            Destroy(obj);

            // Subscribe to main menu teleport event if we are in the main menu
            if (player.InMainMenu)
            {
                player.Movement.OnMainMenuTeleport += () =>
                {
                    // Update all the colors for the particles renderers
                    foreach (var renderer in _particleRenderers)
                    {
                        MaterialColorer.UpdateRendererMaterials(renderer, PlayerColor1, true, true);
                    }

                    // Update all the colors for the material colorers
                    foreach (var colorer in _materialColorers)
                    {
                        Color playerColor = GetPlayerColor(colorer.GetColor.a);

                        colorer.GetColor = playerColor;
                    }
                };
            }
        }

        private Color GetPlayerColor(float a)
        {
            Color newCol = PlayerColor1;
            newCol.a = a;

            return newCol;
        }

        public override void Update()
        {
            base.Update();

            // Detect if the player is touching deadly stuff
            _touchingDeath = Physics.OverlapBox(transform.position, transform.localScale / 2 + (Vector3.one / 15), transform.rotation, deathLayer).Length >= 1;

            // Die if we are touching death stuff
            if (_touchingDeath)
            {
                Die();
            }

#if UNITY_EDITOR
            // DEBUG!!! Die when R is pressed
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("Debug reset button moment");
                Die();
            }
#endif
        }

        /// <summary>
        /// Makes the player explode, plays the death sound effect, disables the mesh and respawns the player afterwards.
        /// </summary>
        public void Die()
        {
            // Don't die again if we have already died
            if (player.IsDead)
            {
                return;
            }

            // Spawn death effect
            PoolObject obj = _pool.SpawnFromPool(transform.position);

            // Remove after a second
            obj.RemoveAfterTime(1);

            // Play death sound
            SoundManager.PlaySound("Player Explode", 1);

            // Invoke on death event
            player.InvokeDeathEvent();
        }
    }
}
