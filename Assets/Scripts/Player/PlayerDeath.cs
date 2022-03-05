using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Audio;

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

        [Header("Effects")]
        [SerializeField] private GameObject deathEffect;

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
            GameObject obj = Instantiate(deathEffect, transform.position, Quaternion.identity);

            // Change the death particles color to match the first player color
            ParticleSystemRenderer particles = obj.GetComponentInChildren<ParticleSystemRenderer>();

            MaterialColorer.UpdateRendererMaterials(particles, PlayerColor1, true, true);

            // Change the death sphere color to match the player colors
            MaterialColorer colorer = obj.GetComponentInChildren<MaterialColorer>();

            // Make sure to have the same alpha value
            Color playerColor = PlayerColor1;
            playerColor.a = colorer.GetColor.a;

            colorer.GetColor = playerColor;

            // Play death sound
            SoundManager.PlaySound("Player Explode", 1);

            // Invoke on death event
            player.InvokeDeathEvent();
        }
    }
}
