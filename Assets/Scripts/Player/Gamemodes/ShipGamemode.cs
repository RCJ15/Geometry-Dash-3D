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

        private bool KeyHold => Player.KeyHold;

        public override void OnEnable()
        {
            base.OnEnable();

            // Make constant particles constantly play whilst in ship gamemode
            constantParticles.Play();
        }

        public override void OnDisable()
        {
            base.OnDisable();

            StopAllParticles();
        }

        public override void OnDeath()
        {
            base.OnDeath();

            StopAllParticles();
        }

        private void StopAllParticles()
        {
            flyParticles.Stop();
            slideParticles.Stop();
            constantParticles.Stop();
        }

        public override void Update()
        {
            base.Update();

            // Go up/down based on if the click key is being held
            YVelocity += flySpeed * Time.deltaTime * (KeyHold ? 1 : -1) * UpsideDownMultiplier;
            
            HandleParticles();

            AngularVelocity();
        }

        /// <summary>
        /// Handles the fly and slide particles. Is called in Update()
        /// </summary>
        private void HandleParticles()
        {
            // Fly particles
            if (KeyHold && !flyParticles.isPlaying)
            {
                flyParticles.Play();
            }
            else if (!KeyHold && flyParticles.isPlaying)
            {
                flyParticles.Stop();
            }

            // Slide particles
            if (onGround && !KeyHold && !slideParticles.isPlaying)
            {
                slideParticles.Play();
            }
            else if ((!onGround || KeyHold) && slideParticles.isPlaying)
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

                // Set the angular velocity X to the XRot
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
            Quaternion slerp = Quaternion.Slerp(objToRotate.localRotation, Quaternion.Euler(_targetRot * UpsideDownMultiplier), rotateSlerpSpeed);

            objToRotate.localRotation = slerp;

            // Set particles rotation
            particlesParent.localRotation = objToRotate.localRotation;
        }

        public override void OnChangeGravity(bool upsideDown)
        {
            // Set the scale to make sure the ship turns upside down when it's upside down
            objToScale.transform.localScale = new Vector3(1, upsideDown ? -1 : 1, 1);
        }
    }
}