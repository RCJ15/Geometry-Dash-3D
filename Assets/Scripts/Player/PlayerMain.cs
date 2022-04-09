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

        private int? _layerInt = null;
        public int GetLayer
        {
            get
            {
                // Cache the layer
                if (_layerInt == null)
                {
                    _layerInt = gameObject.layer;
                }

                return (int)_layerInt;
            }
        }

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
        private Key _clickKey;

        private bool _keyDown;
        private bool _keyHold;
        private bool _keyUp;

        [SerializeField] private float inputBufferTime = 0.1f;
        private float _currentInputBufferTime;

        public float InputBuffer
        {
            get => _currentInputBufferTime;
            set => _currentInputBufferTime = value;
        }

        public Key ClickKey => _clickKey;
        public bool KeyDown => _keyDown;
        public bool KeyHold => _keyHold;
        public bool KeyUp => _keyUp;

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
            _clickKey = PlayerInput.GetKey("Click");

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
                        _keyHold = keyPressed;
                        break;

                    case PressMode.down:
                        _keyDown = keyPressed;
                        break;

                    case PressMode.up:
                        _keyUp = keyPressed;
                        break;
                }

                // Check if the key is pressed with this press mode
                if (keyPressed)
                {
                    // Call the OnClick event with this press mode
                    OnClick?.Invoke(mode);
                }
            }

            // Input buffer time
            if (KeyDown)
            {
                _currentInputBufferTime = inputBufferTime;
            }
            else if (_currentInputBufferTime > 0)
            {
                _currentInputBufferTime -= Time.deltaTime;
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
