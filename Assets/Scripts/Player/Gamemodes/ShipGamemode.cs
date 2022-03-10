using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.CustomInput;

namespace GD3D.Player
{
    /// <summary>
    /// Flies up
    /// </summary>
    [System.Serializable]
    public class ShipGamemode : GamemodeScript
    {
        [Header("Flying")]
        [SerializeField] private float flySpeed;

        private bool _holdingClickKey;

        [Header("Rotation")]
        [SerializeField] private Transform objToRotate;
        [SerializeField] private float rotateSlerpSpeed = 0.15f;
        [SerializeField] private float xRotationModifier = 4;
        private Vector3 _targetRot;

        [Header("Effects")]
        [SerializeField] private ParticleSystem flyParticles;
        [SerializeField] private ParticleSystem slideParticles;
        [SerializeField] private ParticleSystem constantParticles;

        [Space]
        [SerializeField] private Transform particlesParent;

        [Header("Upside Down Scaling")]
        [SerializeField] private Transform objToScale;

        public override void OnEnable()
        {
            base.OnEnable();

            _holdingClickKey = GamemodeHandler._clickKey.Pressed(PressMode.hold);

            // Make constant particles constantly play whilst in ship gamemode
            constantParticles.Play();
        }

        public override void OnDisable()
        {
            base.OnDisable();

            // Make all particles stop
            flyParticles.Stop();
            slideParticles.Stop();
            constantParticles.Stop();
        }

        public override void Update()
        {
            base.Update();

            // Go up/down based on if the click key is being held
            YVelocity += (flySpeed * Time.deltaTime * (_holdingClickKey ? 1 : -1)) * upsideDownMultiplier;
            
            HandleParticles();

            AngularVelocity();
        }

        /// <summary>
        /// Handles the fly and slide particles. Is called in Update()
        /// </summary>
        private void HandleParticles()
        {
            // Fly particles
            if (_holdingClickKey && !flyParticles.isPlaying)
            {
                flyParticles.Play();
            }
            else if (!_holdingClickKey && flyParticles.isPlaying)
            {
                flyParticles.Stop();
            }

            // Slide particles
            if (onGround && !_holdingClickKey && !slideParticles.isPlaying)
            {
                slideParticles.Play();
            }
            else if ((!onGround || _holdingClickKey) && slideParticles.isPlaying)
            {
                slideParticles.Stop();
            }
        }

        /// <summary>
        /// Handles all rotation angular velocity physics stuff. Is called in Update()
        /// </summary>
        private void AngularVelocity()
        {
            // Rotate towards the Y velocity while in the air
            if (!onGround)
            {
                _targetRot.z = Rigidbody.velocity.y * xRotationModifier;

                // Set X velocity
                _targetRot.x = XRot;
            }
            // Otherwise do not rotate at all
            else
            {
                _targetRot = Vector3.zero;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            // Set rotation
            Quaternion slerp = Quaternion.Slerp(objToRotate.localRotation, Quaternion.Euler(_targetRot * upsideDownMultiplier), rotateSlerpSpeed);

            objToRotate.localRotation = slerp;

            // Set particles rotation
            particlesParent.localRotation = objToRotate.localRotation;
        }

        public override void OnClick(PressMode mode)
        {
            switch (mode)
            {
                case PressMode.down:
                    _holdingClickKey = true;
                    break;

                case PressMode.up:
                    _holdingClickKey = false;
                    break;
            }
        }

        public override void OnChangeGravity(bool upsideDown)
        {
            // Set the scale to make sure the ship turns upside down when it's upside down
            objToScale.transform.localScale = new Vector3(1, upsideDown ? -1 : 1, 1);
        }
    }
}