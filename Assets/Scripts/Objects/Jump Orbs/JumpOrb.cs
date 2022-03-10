using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Player;
using GD3D.CustomInput;

namespace GD3D
{
    /// <summary>
    /// Jump orb base class that all jump orbs inherit from
    /// </summary>
    public abstract class JumpOrb : MonoBehaviour
    {
        public static bool CantHitOrbs = false;
        private static bool s_doneStaticUpdate = false;

        [Header("Jump Orb Settings")]
        [SerializeField] private bool multiTrigger;
        private bool _cantBePressed = false;

        protected PlayerMain _player;
        protected bool _touchingPlayer; // Pretty sus ngl

        private LayerMask playerLayer;

        public virtual void Start()
        {
            // Set player layer
            playerLayer = LayerMask.NameToLayer("Player");

            // Set CantHitOrbs to false
            if (CantHitOrbs)
            {
                CantHitOrbs = false;
            }

            // Get player instance
            _player = PlayerMain.Instance;
        }

        public virtual void Update()
        {
            // Return if the static update has already been done
            if (s_doneStaticUpdate)
            {
                return;
            }

            // Buffering orbs in the air logic
            if (_player.KeyDown && !CantHitOrbs && !_touchingPlayer)
            {
                GamemodeScript script = _player.gamemode.CurrentGamemodeScript;

                if (script.BufferOrbs)
                {
                    // If the player lands then they are not allowed to buffer anymore
                    if (script.onGround)
                    {
                        CantHitOrbs = true;
                    }
                }
                // Do not allow the player to buffer at all
                else
                {
                    CantHitOrbs = true;
                }
            }

            // Check if the Key is up and if the player can't hit orbs
            if (_player.KeyUp && CantHitOrbs)
            {
                // Make so the player can hit orbs again
                CantHitOrbs = false;
            }

            // Set doneStaticUpdate to true
            s_doneStaticUpdate = true;
        }

        public virtual void LateUpdate()
        {
            // Reset so the static update will work next frame
            if (s_doneStaticUpdate)
            {
                s_doneStaticUpdate = false;
            }
        }

        /// <summary>
        /// Implement this to determine what happens when the player uses the jump orb
        /// </summary>
        public abstract void OnPressed();

        /// <summary>
        /// Override this to determine a custom jump orb condition that has to be met in order for the player to use the jump orb. <para/>
        /// So this must return true in order for the jump orb to be usable.
        /// </summary>
        public virtual bool CustomJumpOrbCondition()
        {
            return true;
        }

        public virtual void OnTriggerEnter(Collider col)
        {
            // Check if it's the player we are colliding with
            if (col.gameObject.layer == playerLayer)
            {
                // TODO: Make ring shrink animation thingy
                _touchingPlayer = true;
            }
        }

        public virtual void OnTriggerExit(Collider col)
        {
            // Check if it's the player we are colliding with
            if (col.gameObject.layer == playerLayer)
            {
                _touchingPlayer = false;
            }
        }

        public virtual void OnTriggerStay(Collider col)
        {
            // Check if it's the player we are colliding with
            if (col.gameObject.layer == playerLayer)
            {
                // Return if the correct conditions haven't been met
                if (CantHitOrbs || _cantBePressed || !CustomJumpOrbCondition())
                {
                    return;
                }

                // Return if the player is not holding any key
                if (!_player.KeyHold)
                {
                    return;
                }

                // Make it so the player can't hit any other orbs
                CantHitOrbs = true;

                // Make it so this orb cant be pressed if it's not multiTrigger
                if (!multiTrigger)
                {
                    _cantBePressed = true;
                }

                // Enable the player trail
                PlayerTrailManager.HaveTrail = true;

                // Call OnPressed method
                OnPressed();
            }
        }
    }
}
