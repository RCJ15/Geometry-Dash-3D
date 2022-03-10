using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Objects;

namespace GD3D.Player
{
    /// <summary>
    /// This script contains a reference to all the other player scripts and acts as a communicator between them.
    /// </summary>
    public class PlayerMain : PlayerScript
    {
        //-- Instance
        public static PlayerMain Instance;

        //-- Player scripts
        internal PlayerMovement movement;
        internal PlayerInput input;
        internal PlayerColors colors;
        internal PlayerMesh mesh;
        internal PlayerWin win;
        internal PlayerDeath death;
        internal PlayerSpawn spawn;
        internal PlayerCamera cam;
        internal PlayerGamemodeHandler gamemode;

        //-- Other Stuff
        internal bool dead;

        //-- Events
        public Action OnDeath;
        public Action OnRespawn;
        public Action<Portal> OnEnterPortal;

        // Start values
        internal Vector3 startPos;
        internal Vector3 startScale;
        internal Quaternion startRotation;

        private void Awake()
        {
            // Set instance
            Instance = this;

            // Set start values
            startPos = transform.position;
            startScale = transform.localScale;
            startRotation = transform.rotation;

            GetPlayerScripts();
        }

        /// <summary>
        /// Gets all player scripts and stores them in their respective variables
        /// </summary>
        private void GetPlayerScripts()
        {
            movement = GetChildComponent<PlayerMovement>();
            input = GetChildComponent<PlayerInput>();
            colors = GetChildComponent<PlayerColors>();
            mesh = GetChildComponent<PlayerMesh>();
            win = GetChildComponent<PlayerWin>();
            death = GetChildComponent<PlayerDeath>();
            spawn = GetChildComponent<PlayerSpawn>();
            cam = GetChildComponent<PlayerCamera>();
            gamemode = GetChildComponent<PlayerGamemodeHandler>();
        }

        /// <summary>
        /// Invokes the OnDeath event cuz p.OnDeath?.Invoke() won't work outside of this script
        /// </summary>
        public void InvokeDeathEvent()
        {
            player.dead = true;

            OnDeath?.Invoke();
        }

        /// <summary>
        /// Invokes the OnRespawn event cuz p.OnRespawn?.Invoke() won't work outside of this script
        /// </summary>
        public void InvokeRespawnEvent()
        {
            player.dead = false;

            // Reset transform
            transform.position = startPos;
            transform.localScale = startScale;
            transform.rotation = startRotation;

            OnRespawn?.Invoke();
        }
    }
}
