using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Objects;
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
        public Action<PressMode> OnClick;
        public Action<Portal> OnEnterPortal;

        //-- Start values
        internal Vector3 startPos;
        internal Vector3 startScale;
        internal Quaternion startRotation;

        //-- Input
        public Key ClickKey;

        public bool KeyDown;
        public bool KeyHold;
        public bool KeyUp;

        private readonly static Array s_pressModeValues = Enum.GetValues(typeof(PressMode));

        private void Awake()
        {
            // Set instance
            Instance = this;

            // Set start values
            startPos = transform.position;
            startScale = transform.localScale;
            startRotation = transform.rotation;

            // Get input key
            ClickKey = PlayerInput.GetKey("Click");

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

        public override void Update()
        {
            base.Update();

            // Loop through all press modes (there are only 3)
            foreach (PressMode mode in s_pressModeValues)
            {
                bool keyPressed = ClickKey.Pressed(mode);

                // Set public input bools
                switch (mode)
                {
                    case PressMode.hold:
                        KeyHold = keyPressed;
                        break;

                    case PressMode.down:
                        KeyDown = keyPressed;
                        break;

                    case PressMode.up:
                        KeyUp = keyPressed;
                        break;
                }

                // Check if the key is pressed with this press mode
                if (keyPressed)
                {
                    // Call the OnClick event with this press mode
                    OnClick?.Invoke(mode);
                }
            }
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
