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
        [SerializeField] private float jumpHeight = 20f;
        [SerializeField] private float jumpCooldown = 0.2f;
        private float _jumpCooldownTimer;

        [Header("Ground Detection")]
        [SerializeField] private float groundDetectSize = 0.54f;
        [SerializeField] private LayerMask groundLayer;

        private bool _onGround;
        private bool _landedOnGround;

        [Header("Cube Rotation")]
        [SerializeField] private Transform objToRotate;
        [SerializeField] private float cubeSlerpSpeed = 0.55f;
        private Vector3 _angularVelocity;
        private Vector3 _targetRot;

        [Header("Effects")]
        [SerializeField] private ParticleSystem slideParticles;
        [SerializeField] private GameObject jumpParticles;
        [SerializeField] private GameObject landParticles;

        // The time spent in the air per jump is about 0.44 seconds

        /// <summary>
        /// OnEnable is called when the gamemode is switched to this gamemode
        /// </summary>
        public override void OnEnable()
        {
            ResetRotation();
        }

        /// <summary>
        /// OnDisable is called when the gamemode is switched from this gamemode
        /// </summary>
        public override void OnDisable()
        {
            // Stop slide particles
            slideParticles.Stop();
        }

        public override void Update()
        {
            // Don't do anything if the player is dead
            if (dead)
                return;

            // Decrease the jump cooldown whilst it's above 0
            if (_jumpCooldownTimer > 0)
            {
                _jumpCooldownTimer -= Time.deltaTime;
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
            _onGround = Physics.OverlapBox(_transform.position, Vector3.one * groundDetectSize, _transform.rotation, groundLayer).Length >= 1;

            // Detects if the player has landed back on the ground
            if (!_landedOnGround && _onGround)
            {
                _landedOnGround = true;
                OnLand();

                // Enable slide particles
                slideParticles.Play();
            }
            // Detects when the player leaves the ground
            else if (_landedOnGround && !_onGround)
            {
                _landedOnGround = false;

                // Disable slide particles
                slideParticles.Stop();
            }
        }

        /// <summary>
        /// Handles all rotation angular velocity physics stuff. Is called in Update()
        /// </summary>
        private void AngularVelocity()
        {
            // Check if we are in the air
            if (!_onGround)
            {
                _angularVelocity = Vector3.zero;

                // Spin -180 degrees per 0.44 seconds in the Z axis
                float force = 180 / 0.44f;

                _angularVelocity.z = -force;

                // Set the angular velocity X to the rigidbodies current Z velocity
                _targetRot.x = Mathf.Clamp(Rigidbody.velocity.z, -1, 1) * 15;
            }

            // Increase target rotation by angular velocity
            if (_angularVelocity != Vector3.zero)
            {
                _targetRot += _angularVelocity * Time.deltaTime;
                _targetRot.x %= 360;
                _targetRot.y %= 360;
                _targetRot.z %= 360;
            }
        }

        private void OnLand()
        {
            // Reset the X and Z angular velocity 
            _angularVelocity = Vector3.zero;

            // Round the X and Z rotation to be a multiple of 90 (0, 90, 180 or 270)
            _targetRot.x = Mathf.Round(_targetRot.x / 90) * 90;
            _targetRot.y = Mathf.Round(_targetRot.y / 90) * 90;
            _targetRot.z = Mathf.Round(_targetRot.z / 90) * 90;

            SpawnParticles(landParticles);
        }

        public override void FixedUpdate()
        {
            // Don't do anything if the player is dead
            if (dead)
                return;

            // Set rotation
            Quaternion slerp = Quaternion.Slerp(objToRotate.rotation, Quaternion.Euler(_targetRot), cubeSlerpSpeed);

            objToRotate.rotation = slerp;

            // Do gravity
            base.FixedUpdate();
        }

        /// <summary>
        /// OnClick is called when the player presses the main gameplay button. <para/>
        /// <paramref name="mode"/> determines whether the button was just pressed, held or just released.
        /// </summary>
        public override void OnClick(PressMode mode)
        {
            // Can't jump if not on ground or if the jump is on cooldown
            if (!_onGround || _jumpCooldownTimer > 0)
            {
                return;
            }

            // Check the press mode
            switch (mode)
            {
                // The button was held down
                case PressMode.hold:
                    Jump();
                    break;
            }
        }

        private void Jump()
        {
            // Set Y velocity
            YVelocity = jumpHeight;

            // Restart the jump cooldown
            _jumpCooldownTimer = jumpCooldown;

            SpawnParticles(jumpParticles);
        }


        private void SpawnParticles(GameObject orignalObject)
        {
            // Create cube particles
            GameObject obj = Object.Instantiate(orignalObject, _transform.position, Quaternion.identity, _transform);
            obj.transform.localPosition = Vector3.zero;

            // Get the particle system renderer
            ParticleSystemRenderer jumpParticlesRenderer = obj.GetComponent<ParticleSystemRenderer>();
            Material newMaterial = Player.CloneMaterial(jumpParticlesRenderer.materials[0], 1, true, true);

            // Set the materials
            jumpParticlesRenderer.materials = new Material[] { newMaterial };
        }

        public override void OnDeath()
        {
            // Stop slide particles
            slideParticles.Stop();

            ResetRotation();
        }

        private void ResetRotation()
        {
            _angularVelocity = Vector3.zero;
            _targetRot = Vector3.zero;
            objToRotate.rotation = Quaternion.Euler(Vector3.zero);
        }
    }
}
