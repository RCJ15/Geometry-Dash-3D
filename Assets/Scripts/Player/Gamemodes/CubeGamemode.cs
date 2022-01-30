using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.CustomInput;

namespace Game.Player
{
    /// <summary>
    /// Jumps
    /// </summary>
    [System.Serializable]
    public class CubeGamemode : GamemodeScript
    {
        // Jump numbers from: https://www.youtube.com/watch?v=srkwnM5mRX8

        [Header("Jumping")]
        [SerializeField] private float jumpHeight = 20f;
        [SerializeField] private float jumpCooldown = 0.2f;
        private float jumpCooldownTimer;

        [Header("Ground Detection")]
        [SerializeField] private float groundDetectSize = 0.45f;
        public LayerMask groundLayer;

        [HideInInspector] public bool onGround;
        private bool landedOnGround;

        // Time in the air is 0.44 seconds

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

            // Decrease the jump cooldown whilst it's above 0
            if (jumpCooldownTimer > 0)
            {
                jumpCooldownTimer -= Time.deltaTime;
            }

            // Detect if the player is on the ground
            onGround = Physics.OverlapBox(transform.position, Vector3.one * groundDetectSize, transform.rotation, groundLayer).Length >= 1;

            // Detects if the player has landed
            if (!landedOnGround && onGround)
            {
                landedOnGround = true;
            }
            else if (landedOnGround && !onGround)
            {
                landedOnGround = false;
            }
        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public override void FixedUpdate()
        {
            base.FixedUpdate();


            // Add torque whilst in the air
            if (!onGround)
            {
                rb.AddTorque(-180 / 0.4f * Mathf.Deg2Rad * Vector3.forward, ForceMode.VelocityChange);
            }
            else
            {

            }
        }

        /// <summary>
        /// OnClick is called when the player presses the main gameplay button. <para/>
        /// <paramref name="mode"/> determines whether the button was just pressed, held or just released.
        /// </summary>
        public override void OnClick(PressMode mode)
        {
            base.OnClick(mode);

            // Can't jump if not on ground or the jump is on cooldown
            if (!onGround || jumpCooldownTimer > 0)
            {
                return;
            }

            // Check the press mode
            switch (mode)
            {
                // The button was held down
                case PressMode.hold:

                    YVelocity = jumpHeight;

                    // Reset the jump cooldown
                    jumpCooldownTimer = jumpCooldown;

                    Debug.Log("JUMPED");

                    float force = -350 * Mathf.Deg2Rad;

                    rb.AddTorque(force * Vector3.forward, ForceMode.Impulse);

                    break;
            }
        }
    }
}
