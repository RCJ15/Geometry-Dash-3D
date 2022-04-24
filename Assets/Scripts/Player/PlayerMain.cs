using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Objects;
using GD3D.CustomInput;
using GD3D.UI;

namespace GD3D.Player
{
    /// <summary>
    /// This script contains a reference to all the other player scripts and acts as a communicator between them.
    /// </summary>
    public class PlayerMain : PlayerScript
    {
        //-- Instance
        public static PlayerMain Instance;

        //-- Jump & time stat
        public static int TimesJumped;
        public static float TimeSpentPlaying;

        //-- Player scripts
        [HideInInspector] public PlayerMovement Movement;
        [HideInInspector] public PlayerInput Input;
        [HideInInspector] public PlayerColors Colors;
        [HideInInspector] public PlayerMesh Mesh;
        [HideInInspector] public PlayerWin Win;
        [HideInInspector] public PlayerDeath Death;
        [HideInInspector] public PlayerSpawn Spawn;
        [HideInInspector] public PlayerCamera Camera;
        [HideInInspector] public PlayerGamemodeHandler GamemodeHandler;
        [HideInInspector] public PlayerPracticeMode PracticeMode;

        //-- Other Stuff
        [HideInInspector] public bool IsDead;

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
        public Action<bool, Checkpoint> OnRespawn;
        public Action<PressMode> OnClick;
        public Action<Portal> OnEnterPortal;

        //-- Start values
        [HideInInspector] public Vector3 StartPos;
        [HideInInspector] public Vector3 StartScale;
        [HideInInspector] public Quaternion StartRotation;

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

        public Rigidbody Rigidbody => rb;

        public override void Awake()
        {
            base.Awake();

            // Set instance
            Instance = this;

            // Set start values
            StartPos = transform.position;
            StartScale = transform.localScale;
            StartRotation = transform.rotation;

            // Get input key
            _clickKey = PlayerInput.GetKey("Click");

            // Reset jumps and time because they are static
            TimesJumped = 0;
            TimeSpentPlaying = 0;

            GetPlayerScripts();
        }

        /// <summary>
        /// Gets all player scripts and stores them in their respective variables
        /// </summary>
        private void GetPlayerScripts()
        {
            Movement = GetChildComponent<PlayerMovement>();
            Input = GetChildComponent<PlayerInput>();
            Colors = GetChildComponent<PlayerColors>();
            Mesh = GetChildComponent<PlayerMesh>();
            Win = GetChildComponent<PlayerWin>();
            Death = GetChildComponent<PlayerDeath>();
            Spawn = GetChildComponent<PlayerSpawn>();
            Camera = GetChildComponent<PlayerCamera>();
            GamemodeHandler = GetChildComponent<PlayerGamemodeHandler>();
            PracticeMode = GetChildComponent<PlayerPracticeMode>();
        }

        public override void Update()
        {
            base.Update();

            // If the game is paused, then all input will automatically be ignored
            if (PauseMenu.IsPaused)
            {
                IgnoreInput();

                return;
            }

            // Increase time if the player is not dead
            if (!IsDead)
            {
                TimeSpentPlaying += Time.deltaTime;
            }

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
        /// Sets <see cref="KeyDown"/>, <see cref="KeyHold"/> and <see cref="KeyUp"/> to false. <para/>
        /// Also sets <see cref="InputBuffer"/> to 0.
        /// </summary>
        public void IgnoreInput()
        {
            _keyDown = false;
            _keyHold = false;
            _keyUp = false;

            _currentInputBufferTime = 0;
        }

        /// <summary>
        /// Invokes the OnDeath event cuz p.OnDeath?.Invoke() won't work outside of this script
        /// </summary>
        public void InvokeDeathEvent()
        {
            player.IsDead = true;

            OnDeath?.Invoke();
        }

        /// <summary>
        /// Invokes the OnRespawn event cuz p.OnRespawn?.Invoke() won't work outside of this script :(
        /// </summary>
        public void InvokeRespawnEvent(bool inPracticeMode, Checkpoint checkpoint)
        {
            player.IsDead = false;

            // Check if we are not in practice mode
            if (!inPracticeMode)
            {
                // Reset transform
                transform.position = StartPos;
                transform.localScale = StartScale;
                transform.rotation = StartRotation;
            }
            else
            {
                // Set to practice 
                transform.position = checkpoint.PlayerPosition;
                transform.localScale = checkpoint.PlayerScale;
                transform.rotation = checkpoint.PlayerRotation;
            }

            OnRespawn?.Invoke(inPracticeMode, checkpoint);
        }
    }
}
