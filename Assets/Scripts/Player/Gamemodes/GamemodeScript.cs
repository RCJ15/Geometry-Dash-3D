using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.CustomInput;
using System;

namespace GD3D.Player
{
    /// <summary>
    /// The class all gameplay scripts inherit from.
    /// </summary>
    public class GamemodeScript
    {
        [Header("Gravity")]
        [SerializeField] internal float gravity = 85;

        [Tooltip("X = Min Terminal Velocity \nY = Max Terminal Velocity \nWhen upside down, these are swaped")]
        [SerializeField] internal Vector2 terminalVelocity = new Vector2(28.4f, 28.4f);

        [Header("Ground Detection")]
        [SerializeField] private Vector3 groundOffset;
        [SerializeField] private Vector3 groundDetectSize = new Vector3(0.54f, 0.54f, 0.54f);
        [SerializeField] private LayerMask groundLayer;

        [Header("Other")]
        [SerializeField] private TrailMode trailMode = TrailMode.never;

        internal bool onGround;
        private bool _landedOnGround;

        internal float XRot => Mathf.Clamp(Rigidbody.velocity.z, -1, 1) * 15;

        //-- Component references
        [HideInInspector] public PlayerGamemodeHandler GamemodeHandler;
        [HideInInspector] public PlayerMain Player;
        [HideInInspector] public Rigidbody Rigidbody;

        internal Transform _transform;
        internal GameObject _gameObject;

        /// <summary>
        /// Shortcut for getting "upsideDown"
        /// </summary>
        internal bool upsideDown => GamemodeHandler.upsideDown;
        internal float upsideDownMultiplier => upsideDown ? -1 : 1;

        /// <summary>
        /// Shortcut for setting and getting "rb.velocity.y"
        /// </summary>
        internal float YVelocity
        {
            get => GamemodeHandler.YVelocity;
            set => GamemodeHandler.YVelocity = value;
        }

        /// <summary>
        /// Shortcut for getting "p.dead"
        /// </summary>
        internal bool dead => Player.dead;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        public virtual void Start()
        {
            _transform = GamemodeHandler.transform;
            _gameObject = GamemodeHandler.gameObject;
        }

        /// <summary>
        /// OnEnable is called when the gamemode is switched to this gamemode
        /// </summary>
        public virtual void OnEnable()
        {
            // Call OnChangeGravity in case there is a upside down mishap
            OnChangeGravity(upsideDown);
        }

        /// <summary>
        /// OnDisable is called when the gamemode is switched from this gamemode
        /// </summary>
        public virtual void OnDisable()
        {

        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        public virtual void Update()
        {
            GroundDetection();
            UpdateTrail();
        }

        /// <summary>
        /// Handles the players trail. Is called in Update()
        /// </summary>
        private void UpdateTrail()
        {
            if (trailMode == TrailMode.never && PlayerTrailManager.HaveTrail)
            {
                PlayerTrailManager.HaveTrail = false;
            }
            else if (trailMode == TrailMode.always && !PlayerTrailManager.HaveTrail)
            {
                PlayerTrailManager.HaveTrail = true;
            }
        }

        /// <summary>
        /// Handles all ground detection. Is called in Update()
        /// </summary>
        private void GroundDetection()
        {
            // Fix the ground detection for upside down
            Vector3 newGroundOffset = groundOffset;
            newGroundOffset.y *= upsideDownMultiplier;

            // Detect if the player is on the ground
            onGround = Physics.OverlapBox(_transform.position + newGroundOffset, groundDetectSize, Quaternion.identity, groundLayer).Length >= 1;

            // Detects if the player has landed back on the ground
            if (!_landedOnGround && onGround)
            {
                _landedOnGround = true;
                OnLand();
            }
            // Detects when the player leaves the ground
            else if (_landedOnGround && !onGround)
            {
                _landedOnGround = false;
                OnLeaveGround();
            }
        }

        /// <summary>
        /// Called when the player lands on the ground
        /// </summary>
        public virtual void OnLand()
        {

        }

        /// <summary>
        /// Called when the player leaves the ground
        /// </summary>
        public virtual void OnLeaveGround()
        {

        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public virtual void FixedUpdate()
        {
            // Gravity constant (do none if gravity is 0)
            if (gravity != 0)
            {
                Rigidbody.AddForce(Vector3.down * gravity * upsideDownMultiplier);
            }

            // Clamp Y velocity between terminal velocity if it's not 0
            if (terminalVelocity != Vector2.zero)
            {
                YVelocity = Mathf.Clamp(YVelocity,
                    upsideDown ? -terminalVelocity.y : -terminalVelocity.x,
                    upsideDown ? terminalVelocity.x : terminalVelocity.y
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
        /// Fixed Update is called once per physics frame
        /// </summary>
        public virtual void OnDeath()
        {

        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public virtual void OnRespawn()
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
