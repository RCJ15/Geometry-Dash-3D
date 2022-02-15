using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.CustomInput;

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
        internal PlayerMesh mesh;
        internal PlayerWin win;
        internal PlayerDeath death;
        internal PlayerSpawn spawn;
        internal PlayerCamera cam;
        internal PlayerGamemodeHandler gamemode;

        //-- Other Stuff
        internal bool _dead;
        public MaterialColorer Colorer;

        //-- Events
        public Action OnDeath;
        public Action OnRespawn;

        // Start values
        internal Vector3 _startPos;
        internal Vector3 _startScale;
        internal Quaternion _startRotation;

        /// <summary>
        /// Awake is called when the script instance is being loaded
        /// </summary>
        private void Awake()
        {
            // Set instance
            Instance = this;

            // Set start values
            _startPos = transform.position;
            _startScale = transform.localScale;
            _startRotation = transform.rotation;
        }

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        public override void Start()
        {
            base.Start();

            GetPlayerScripts();
        }

        /// <summary>
        /// Gets all player scripts and stores them in their respective variables
        /// </summary>
        private void GetPlayerScripts()
        {
            movement = GetChildComponent<PlayerMovement>();
            input = GetChildComponent<PlayerInput>();
            mesh = GetChildComponent<PlayerMesh>();
            win = GetChildComponent<PlayerWin>();
            death = GetChildComponent<PlayerDeath>();
            spawn = GetChildComponent<PlayerSpawn>();
            cam = GetChildComponent<PlayerCamera>();
            gamemode = GetChildComponent<PlayerGamemodeHandler>();
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        public override void Update()
        {
            base.Update();


        }

        /// <summary>
        /// Invokes the OnDeath event cuz p.OnDeath?.Invoke() won't work outside of this script
        /// </summary>
        public void InvokeDeathEvent()
        {
            _player._dead = true;

            OnDeath?.Invoke();
        }

        /// <summary>
        /// Invokes the OnRespawn event cuz p.OnRespawn?.Invoke() won't work outside of this script
        /// </summary>
        public void InvokeRespawnEvent()
        {
            _player._dead = false;

            // Reset transform
            transform.position = _startPos;
            transform.localScale = _startScale;
            transform.rotation = _startRotation;

            OnRespawn?.Invoke();
        }
    }
}
