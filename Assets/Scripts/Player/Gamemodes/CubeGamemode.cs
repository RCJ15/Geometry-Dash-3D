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
        [Header("Jumping")]
        [SerializeField] private float jumpHeight = 20f;
        [SerializeField] private float jumpCooldown = 0.2f;
        private float jumpCooldownTimer;

        [Header("Ground Detection")]
        [SerializeField] private float groundDetectSize = 0.45f;
        [SerializeField] private LayerMask groundLayer;

        private bool onGround;
        private bool landedOnGround;

        [Header("Cube Rotation")]
        [SerializeField] private Transform objToRotate;
        [SerializeField] private float cubeSlerpSpeed = 0.1f;
        private Vector3 angularVelocity;
        private Vector3 targetRot;

        private Vector3 startRot;

        [Header("Effects")]
        [SerializeField] private ParticleSystem slideParticles;
        [SerializeField] private ParticleSystem jumpParticles;
        [SerializeField] private ParticleSystem landParticles;

        // Time in the air is 0.44 seconds

        public override void Start()
        {
            startRot = targetRot;
        }

        /// <summary>
        /// OnEnable is called when the gamemode is switched to this gamemode
        /// </summary>
        public override void OnEnable()
        {
            
        }

        /// <summary>
        /// OnDisable is called when the gamemode is switched from this gamemode
        /// </summary>
        public override void OnDisable()
        {
            // Stop slide particles
            slideParticles.Stop();
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        public override void Update()
        {
            // Don't do anything if the player is dead
            if (dead)
                return;

            // Decrease the jump cooldown whilst it's above 0
            if (jumpCooldownTimer > 0)
            {
                jumpCooldownTimer -= Time.deltaTime;
            }

            GroundDetection();

            AngularVelocity();
        }

        /// <summary>
        /// Handles all ground detection
        /// </summary>
        private void GroundDetection()
        {
            // Detect if the player is on the ground
            onGround = Physics.OverlapBox(transform.position, Vector3.one * groundDetectSize, transform.rotation, groundLayer).Length >= 1;

            // Detects if the player has landed back on the ground
            if (!landedOnGround && onGround)
            {
                landedOnGround = true;
                OnLand();

                // Enable slide particles
                slideParticles.Play();
            }
            // Detects when the player leaves the ground
            else if (landedOnGround && !onGround)
            {
                landedOnGround = false;

                // Disable slide particles
                slideParticles.Stop();
            }
        }

        /// <summary>
        /// Handles all rotation angular velocity physics stuff. Is called in Update()
        /// </summary>
        private void AngularVelocity()
        {
            // Spin in the Z axis while in the air
            if (!onGround)
            {
                angularVelocity.z = -180 / 0.44f;
            }

            // Increase target rotation by angular velocity
            if (angularVelocity != Vector3.zero)
            {
                targetRot += angularVelocity * Time.deltaTime;
                targetRot.x = MathE.LoopValue(targetRot.x, 0, 360);
                targetRot.y = MathE.LoopValue(targetRot.y, 0, 360);
                targetRot.z = MathE.LoopValue(targetRot.z, 0, 360);
            }
        }

        private void OnLand()
        {
            // Reset angular velocity Z
            angularVelocity.z = 0;

            // Round the X and Z rotation to be a multiple of 90
            targetRot.x = Mathf.Round(targetRot.x / 90) * 90;
            targetRot.z = Mathf.Round(targetRot.z / 90) * 90;
        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public override void FixedUpdate()
        {
            // Don't do anything if the player is dead
            if (dead)
                return;

            // Set rotation
            objToRotate.rotation = Quaternion.Slerp(objToRotate.rotation, Quaternion.Euler(targetRot), cubeSlerpSpeed);
        }

        /// <summary>
        /// OnClick is called when the player presses the main gameplay button. <para/>
        /// <paramref name="mode"/> determines whether the button was just pressed, held or just released.
        /// </summary>
        public override void OnClick(PressMode mode)
        {
            // Can't jump if not on ground or if the jump is on cooldown
            if (!onGround || jumpCooldownTimer > 0)
            {
                return;
            }

            // Check the press mode
            switch (mode)
            {
                // The button was held down
                case PressMode.hold:

                    // Jump
                    YVelocity = jumpHeight;

                    // Restart the jump cooldown
                    jumpCooldownTimer = jumpCooldown;
                    break;
            }
        }

        public override void OnDeath()
        {
            // Stop slide particles
            slideParticles.Stop();

            // Reset rotation
            angularVelocity = Vector3.zero;
            targetRot = startRot;
            objToRotate.rotation = Quaternion.Euler(startRot);
        }
    }
}
