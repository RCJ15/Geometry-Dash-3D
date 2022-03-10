using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Player;

namespace GD3D.Objects
{
    /// <summary>
    /// Jump pad base class that all jump pads inherit from
    /// </summary>
    public abstract class JumpPad : MonoBehaviour
    {
        [Header("Jump Pad Settings")]
        [SerializeField] private bool multiTrigger;
        private bool _cantBeTouched = false;

        protected PlayerMain _player;

        private LayerMask playerLayer;

        public virtual void Start()
        {
            // Set player layer
            playerLayer = LayerMask.NameToLayer("Player");

            // Get player instance
            _player = PlayerMain.Instance;
        }

        /// <summary>
        /// Implement this to determine what happens when the player uses the jump orb
        /// </summary>
        public abstract void OnTouched();

        /// <summary>
        /// Override this to determine a custom jump pad condition that has to be met in order for the player to use the jump pad. <para/>
        /// So this must return true in order for the jump pad to be usable.
        /// </summary>
        public virtual bool CustomJumpPadCondition()
        {
            return true;
        }

        public virtual void OnTriggerEnter(Collider col)
        {
            // Check if it's the player we are colliding with
            if (col.gameObject.layer == playerLayer)
            {
                // Return if the correct conditions haven't been met
                if (_cantBeTouched || !CustomJumpPadCondition())
                {
                    return;
                }

                // Make it so this pad cant be touched if it's not multiTrigger
                if (!multiTrigger)
                {
                    _cantBeTouched = true;
                }

                // Enable the player trail
                PlayerTrailManager.HaveTrail = true;

                // Call OnTouched method
                OnTouched();
            }
        }
    }
}
