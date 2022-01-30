using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// Handles the players death. Including detecting when the player should die. Disable this script to basically enable noclip.
    /// </summary>
    public class PlayerDeath : PlayerScript
    {
        [Header("Main")]
        [SerializeField] private LayerMask deathLayer;
        private bool touchingDeath;

        [Header("Effects")]
        [SerializeField] private GameObject deathEffect;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        public override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        public override void Update()
        {
            base.Update();

            // Detect if the player is touching deadly stuff
            touchingDeath = Physics.OverlapBox(transform.position, transform.localScale / 2 + (Vector3.one / 15), transform.rotation, deathLayer).Length >= 1;

            // Die if we are touching death
            if (touchingDeath)
            {
                Die();
            }
        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        /// <summary>
        /// Makes the player explode, plays the death sound effect, disables the mesh and respawns the player afterwards.
        /// </summary>
        public void Die()
        {
            // Don't die again if we have already died
            if (p.dead)
            {
                return;
            }

            // Spawn death particles
            Instantiate(deathEffect, transform.position, Quaternion.identity);

            // Play death sound
            SoundManager.PlaySound("Player Explode", 1);

            // Invoke on death event
            p.InvokeDeathEvent();
        }
    }
}
