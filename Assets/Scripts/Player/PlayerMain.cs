using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Input;

namespace Game.Player
{
    /// <summary>
    /// This script contains a reference to all the other player scripts and acts as a communicator between them. <para/>
    /// Also has crucial player methods like Win() and Die()
    /// </summary>
    public class PlayerMain : PlayerScript
    {
        //-- Instance
        public static PlayerMain instance;

        //-- Other player scripts
        internal PlayerMovement movement;
        internal PlayerInput input;
        internal PlayerMesh mesh;
        internal PlayerGamemodeHandler gamemode;

        /// <summary>
        /// Awake is called when the script instance is being loaded
        /// </summary>
        private void Awake()
        {
            // Set instance
            instance = this;
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
        /// Win
        /// </summary>
        public void Win()
        {

        }

        /// <summary>
        /// Die
        /// </summary>
        public void Die()
        {

        }
    }
}
