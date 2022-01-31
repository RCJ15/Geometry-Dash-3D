using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.CustomInput;

namespace Game.Player
{
    /// <summary>
    /// This script contains a reference to all the other player scripts and acts as a communicator between them.
    /// </summary>
    public class PlayerMain : PlayerScript
    {
        //-- Instance
        public static PlayerMain instance;

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
        internal bool dead;
        public MaterialColorer colorer;

        //-- Events
        public delegate void OnDeathEvent();
        public event OnDeathEvent OnDeath;

        public delegate void OnRespawnEvent();
        public event OnRespawnEvent OnRespawn;

        // Start values
        internal Vector3 startPos;
        internal Vector3 startScale;
        internal Quaternion startRotation;

        /// <summary>
        /// Awake is called when the script instance is being loaded
        /// </summary>
        private void Awake()
        {
            // Set instance
            instance = this;

            // Set start values
            startPos = transform.position;
            startScale = transform.localScale;
            startRotation = transform.rotation;
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
            p.dead = true;

            OnDeath?.Invoke();
        }

        /// <summary>
        /// Invokes the OnRespawn event cuz p.OnRespawn?.Invoke() won't work outside of this script
        /// </summary>
        public void InvokeRespawnEvent()
        {
            p.dead = false;

            // Reset transform
            transform.position = startPos;
            transform.localScale = startScale;
            transform.rotation = startRotation;

            OnRespawn?.Invoke();
        }
    }
}
