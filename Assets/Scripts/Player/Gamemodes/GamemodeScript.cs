using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.CustomInput;
using GD3D.UI;
using System;

namespace GD3D.Player
{
    /// <summary>
    /// The class all gameplay scripts inherit from.
    /// </summary>
    public class GamemodeScript
    {
        [Header("Gravity")]
        [SerializeField] protected float gravity = 85;

        [Tooltip("X = Min Terminal Velocity \nY = Max Terminal Velocity \nWhen upside down, these are swaped")]
        [SerializeField] protected Vector2 terminalVelocity = new Vector2(28.4f, 28.4f);

        [Header("Ground Detection")]
        [SerializeField] private Vector3 groundOffset;
        [SerializeField] private Vector3 groundDetectSize = new Vector3(0.54f, 0.54f, 0.54f);

        [Header("Other")]
        [SerializeField] private TrailMode trailMode = TrailMode.never;

        [Tooltip("Allows the player to hold in the air to buffer a orb whilst in this gamemode. \nSet this to false for airborne gamemodes, like the Ship.")]
        public bool BufferOrbs = false;

        public bool OnGround;
        private bool _landedOnGround;

        //-- Component references
        [HideInInspector] public PlayerGamemodeHandler GamemodeHandler;
        [HideInInspector] public PlayerMain Player;
        [HideInInspector] public PlayerMovement PlayerMovement;
        [HideInInspector] public Rigidbody Rigidbody;

        internal Transform _transform;
        internal GameObject _gameObject;

        //-- Properties (shortcuts)
        /// <summary>
        /// Shortcut for getting <see cref="PlayerMovement.Current3DOffset"/>.
        /// </summary>
        protected float Current3DOffset => PlayerMovement.Current3DOffset;
        /// <summary>
        /// Shortcut for getting <see cref="PlayerMovement.OffsetVelocity"/>.
        /// </summary>
        protected float OffsetVelocity => PlayerMovement.OffsetVelocity;
        /// <summary>
        /// Is basically just the OffsetVelocity but multiplied by -200.
        /// </summary>
        protected float XRot => OffsetVelocity * -200;

        /// <summary>
        /// Shortcut for getting and setting <see cref="PlayerMain.InputBuffer"/>.
        /// </summary>
        protected float InputBuffer
        {
            get => Player.InputBuffer;
            set => Player.InputBuffer = value;
        }
        /// <summary>
        /// Returns true if the players input buffer is above 0. Use this for more lenient jump timings like in the cube or robot.
        /// </summary>
        protected bool InputBufferAbove0 => Player.InputBuffer > 0;

        /// <summary>
        /// Shortcut for getting <see cref="PlayerGamemodeHandler.UpsideDown"/>.
        /// </summary>
        protected bool UpsideDown => GamemodeHandler.UpsideDown;
        /// <summary>
        /// Will return -1 if the player is upside down, otherwise it'll be 1. <para/>
        /// Multiply stuff with this for upside down behaviour.
        /// </summary>
        protected float UpsideDownMultiplier => GamemodeHandler.UpsideDownMultiplier;

        /// <summary>
        /// Shortcut for getting <see cref="PlayerGamemodeHandler.IsSmall"/>.
        /// </summary>
        protected bool IsSmall => GamemodeHandler.IsSmall;

        /// <summary>
        /// Shortcut for getting and setting the <see cref="PlayerScript.rb"/> Y velocity.
        /// </summary>
        protected float YVelocity
        {
            get => GamemodeHandler.YVelocity;
            set => GamemodeHandler.YVelocity = value;
        }

        /// <summary>
        /// Shortcut for getting <see cref="PlayerMain.IsDead"/>.
        /// </summary>
        protected bool Dead => Player.IsDead;

        /// <summary>
        /// Shortcut for getting <see cref="PauseMenu.IsPaused"/>.
        /// </summary>
        protected bool Paused => PauseMenu.IsPaused;

        /// <summary>
        /// Shortcut for getting and setting <see cref="PlayerTrailManager.HaveTrail"/>.
        /// </summary>
        protected bool HaveTrail
        {
            get => PlayerTrailManager.HaveTrail;
            set => PlayerTrailManager.HaveTrail = value;
        }

        /// <summary>
        /// Shortcut for getting <see cref="PlayerMain.InMainMenu"/>.
        /// </summary>
        protected bool InMainMenu
        {
            get => Player.InMainMenu;
        }

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public virtual void Start()
        {
            _transform = GamemodeHandler.transform;
            _gameObject = GamemodeHandler.gameObject;
        }

        /// <summary>
        /// OnEnable is called when the gamemode is switched to this gamemode.
        /// </summary>
        public virtual void OnEnable()
        {
            // Fix trials being active through portals n stuff
            UpdateTrail();

            // Special case for special object trail mode
            if (trailMode == TrailMode.specialObjects)
            {
                HaveTrail = false;
            }

            // Call OnChangeGravity in case there is a upside down mishap
            OnChangeGravity(UpsideDown);
        }

        /// <summary>
        /// OnDisable is called when the gamemode is switched from this gamemode.
        /// </summary>
        public virtual void OnDisable()
        {

        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        public virtual void Update()
        {
            GroundDetection();
            UpdateTrail();
        }

        /// <summary>
        /// Increases the jump count for both this session and the global total jump count for the current level.
        /// </summary>
        protected void IncreaseJumpCount()
        {
            PlayerMain.TimesJumped++;

            if (SaveData.CurrentLevelData != null)
            {
                SaveData.CurrentLevelData.TotalJumps++;
            }
        }

        /// <summary>
        /// Handles the players trail. Is called in Update().
        /// </summary>
        private void UpdateTrail()
        {
            switch (trailMode)
            {
                // Trail is never on, used for no gamemode
                case TrailMode.never:
                    HaveTrail = false;
                    return;

                // Trail is permanent, used for airborne gamemodes
                case TrailMode.always:
                    HaveTrail = true;
                    return;

                // Trail is only on when a special object is interacted with, used for the cube
                // Interacting with a special object is hitting a jump orb or hitting a jump pad
                // Portals will enable the trails themselves in their own script (like gravity portals for example)
                case TrailMode.specialObjects:
                    // Disable trail if on ground
                    if (OnGround && HaveTrail)
                    {
                        HaveTrail = false;
                    }
                    return;
            }
        }

        /// <summary>
        /// Handles all ground detection. Is called in Update().
        /// </summary>
        private void GroundDetection()
        {
            // Fix the ground detection for upside down
            Vector3 newGroundOffset = groundOffset;
            newGroundOffset.y *= UpsideDownMultiplier;

            // Detect if the player is on the ground
            OnGround = Physics.OverlapBox(_transform.position + newGroundOffset, groundDetectSize, Quaternion.identity, GamemodeHandler.GroundLayer).Length >= 1;

            // Detects if the player has landed back on the ground
            if (!_landedOnGround && OnGround)
            {
                _landedOnGround = true;
                OnLand();
            }
            // Detects when the player leaves the ground
            else if (_landedOnGround && !OnGround)
            {
                _landedOnGround = false;
                OnLeaveGround();
            }
        }

        /// <summary>
        /// Called when the player lands on the ground.
        /// </summary>
        public virtual void OnLand()
        {

        }

        /// <summary>
        /// Called when the player leaves the ground.
        /// </summary>
        public virtual void OnLeaveGround()
        {

        }

        /// <summary>
        /// Fixed Update is called once per physics frame.
        /// </summary>
        public virtual void FixedUpdate()
        {
            // Gravity constant (do none if gravity is 0)
            if (gravity != 0)
            {
                Rigidbody.AddForce(gravity * UpsideDownMultiplier * Vector3.down);
            }

            // Clamp Y velocity between terminal velocity if it's not 0
            if (terminalVelocity != Vector2.zero)
            {
                YVelocity = Mathf.Clamp(YVelocity,
                    UpsideDown ? -terminalVelocity.y : -terminalVelocity.x,
                    UpsideDown ? terminalVelocity.x : terminalVelocity.y
                    );
            }
        }

        /// <summary>
        /// OnClick is called when the player presses the main gameplay button. <para/>
        /// <paramref name="mode"/> determines whether the button was just pressed, held or just released.
        /// </summary>
        public virtual void OnClick(PressMode mode)
        {

        }

        /// <summary>
        /// Fixed Update is called once per physics frame.
        /// </summary>
        public virtual void OnDeath()
        {

        }

        /// <summary>
        /// Fixed Update is called once per physics frame.
        /// </summary>
        public virtual void OnRespawn(bool inPracticeMode, Checkpoint checkpoint)
        {

        }

        /// <summary>
        /// Called when the player changes gravity. This can be from entering a gravity portal or from pressing a blue orb.
        /// </summary>
        public virtual void OnChangeGravity(bool upsideDown)
        {

        }

        /// <summary>
        /// Enum for determining in what way the players trail is shown.
        /// </summary>
        [Serializable]
        public enum TrailMode
        {
            /// <summary>
            /// Trail is never enabled
            /// </summary>
            never = 0,
            /// <summary>
            /// Trails is always enabled
            /// </summary>
            always = 1,
            /// <summary>
            /// Trails is only enabled when interacting with special objects, such as hitting a orb or entering a gravity portal.
            /// </summary>
            specialObjects = 2,
        }

#if UNITY_EDITOR
        public void DrawGroundDetectGizmo(Transform transform, bool upsideDown)
        {
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;

            // Fix the ground detection for upside down
            Vector3 newGroundOffset = groundOffset;
            newGroundOffset.y *= upsideDown ? -1 : 1;

            Gizmos.DrawWireCube(transform.position + newGroundOffset, groundDetectSize * 2);
        }
#endif
    }
}

// ############
// # Template #
// ############
/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Input;

namespace Game.Player
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class INSERTNAME : GamemodeScript
    {
        /// <summary>
        /// OnEnable is called when the gamemode is switched to this gamemode
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
        }

        /// <summary>
        /// OnDisable is called when the gamemode is switched from this gamemode
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        public override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public override void OnClick(PressMode mode)
        {
            base.OnClick(mode);
        }
    }
}
*/
