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
            Color playerColor = PlayerColor1;
            playerColor.a = colorer.GetColor.a;

            colorer.GetColor = playerColor;

            // Create deathEffect pool
            _pool = new ObjectPool<PoolObject>(obj, poolSize);

            // Destroy the newly created object because we have no use out of it anymore
            Destroy(obj);
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
            if (player.dead)
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
