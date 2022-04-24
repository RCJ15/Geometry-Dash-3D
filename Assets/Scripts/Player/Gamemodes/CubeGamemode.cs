using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.CustomInput;

namespace GD3D.Player
{
    /// <summary>
    /// Jumps
    /// </summary>
    [System.Serializable]
    public class CubeGamemode : GamemodeScript
    {
        [Header("Jumping")]
        [SerializeField] private GamemodeSizedFloat jumpHeight = new GamemodeSizedFloat(20f, 12.75f);
        [SerializeField] protected GamemodeSizedFloat timeInjump = new GamemodeSizedFloat(0.44f, 0.2805f);
        [SerializeField] private float jumpCooldown = 0.2f;
        private float _jumpCooldownTimer;

        [Header("Cube Rotation")]
        [SerializeField] private Transform objToRotate;
        [SerializeField] private float rotateSlerpSpeed = 0.55f;
        private Vector3 _angularVelocity;
        private Vector3 _targetRot;

        [Header("Effects")]
        [SerializeField] private ParticleSystem slideParticles;
        [SerializeField] private ParticleSystem jumpParticles;
        [SerializeField] private ParticleSystem landParticles;

        // The time spent in the air per jump is about 0.44 seconds

        public override void Start()
        {
            base.Start();
        }

        public override void OnEnable()
        {
            base.OnEnable();

            ResetRotation();

            _jumpCooldownTimer = 0;

            // Play Slide particles if on the ground
            if (onGround)
            {
                slideParticles.Play();
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();

            // Stop slide particles
            slideParticles.Stop();
        }

        public override void Update()
        {
            // Don't do anything if the player is dead
            if (Dead)
            {
                return;
            }

            base.Update();

            // Decrease the jump cooldown whilst it's above 0
            if (_jumpCooldownTimer > 0)
            {
                _jumpCooldownTimer -= Time.deltaTime;
            }

            AngularVelocity();

            // Jump when the input buffer is above 0, the player is on the ground and the jump cooldown has ran out
            if ((Player.KeyHold || InputBufferAbove0) && onGround && _jumpCooldownTimer <= 0)
            {
                Jump();

                // Reset buffer time
                InputBuffer = 0;
            }
        }

        /// <summary>
        /// Handles all rotation angular velocity physics stuff. Is called in Update()
        /// </summary>
        private void AngularVelocity()
        {
            // Check if we are in the air
            if (!onGround)
            {
                _angularVelocity = Vector3.zero;

                // Spin -180 degrees per 0.44 seconds in the Z axis
                float force = 180 / timeInjump.GetValue(IsSmall);

                _angularVelocity.z = -force;

                // Set the angular velocity X to the XRot
                _targetRot.x = XRot;
            }

            // Increase target rotation by angular velocity
            if (_angularVelocity != Vector3.zero)
            {
                _targetRot += Time.deltaTime * UpsideDownMultiplier * _angularVelocity;
                _targetRot.x %= 360;
                _targetRot.y %= 360;
                _targetRot.z %= 360;
            }
        }

        public override void OnLand()
        {
            // Reset the X and Z angular velocity 
            _angularVelocity = Vector3.zero;

            // Snap the X and Z rotation to be a multiple of 90 (0, 90, 180, 270, 360 etc...)
            _targetRot.x = Mathf.Round(_targetRot.x / 90) * 90;
            _targetRot.y = Mathf.Round(_targetRot.y / 90) * 90;
            _targetRot.z = Mathf.Round(_targetRot.z / 90) * 90;

            landParticles.Play();

            // Enable slide particles
            slideParticles.Play();
        }

        public override void OnLeaveGround()
        {
            // Disable slide particles
            slideParticles.Stop();
        }

        public override void FixedUpdate()
        {
            // Don't do anything if the player is dead
            if (Dead)
                return;

            // Set rotation
            Quaternion slerp = Quaternion.Slerp(objToRotate.localRotation, Quaternion.Euler(_targetRot), rotateSlerpSpeed);

            objToRotate.localRotation = slerp;

            // Do gravity & terminal velocity
            base.FixedUpdate();
        }

        /// <summary>
        /// Called when the player is both on ground and presses the click key
        /// </summary>
        private void Jump()
        {
            // Set Y velocity
            YVelocity = jumpHeight.GetValue(IsSmall) * UpsideDownMultiplier;

            // Restart the jump cooldown
            _jumpCooldownTimer = jumpCooldown;

            jumpParticles.Play();

            IncreaseJumpCount();
        }

        public override void OnDeath()
        {
            base.OnDeath();

            // Stop slide particles
            slideParticles.Stop();

            ResetRotation();
        }

        private void ResetRotation()
        {
            _angularVelocity = Vector3.zero;
            _targetRot = Vector3.zero;
            objToRotate.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }
}
