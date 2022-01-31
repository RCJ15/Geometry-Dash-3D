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

#if UNITY_EDITOR
            // DEBUG!!! Die when R is pressed
            if (Input.GetKeyDown(KeyCode.R))
            {
                Die();
            }
#endif
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

            // Spawn death effect
            GameObject obj = Instantiate(deathEffect, transform.position, Quaternion.identity);

            // Change the death particles color to match the player colors
            ParticleSystemRenderer particles = obj.GetComponentInChildren<ParticleSystemRenderer>();

            // Create a clone material and set the new material
            Material newMaterial = CloneMaterial(particles.materials[0], 1, true, true);

            particles.materials = new Material[] { newMaterial };

            // Change the death sphere color to match the player colors
            MaterialColorer colorer = obj.GetComponentInChildren<MaterialColorer>();

            // Make sure to have the same alpha value
            Color playerColor = PlayerColor1;
            playerColor.a = colorer.GetColor.a;

            colorer.GetColor = playerColor;

            // Play death sound
            SoundManager.PlaySound("Player Explode", 1);

            // Invoke on death event
            p.InvokeDeathEvent();
        }
    }
}
