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

        [Space]
        [SerializeField] private float mainMenuFlyTimerMin;
        [SerializeField] private float mainMenuFlyTimerMax;
        private float _currentMainMenuFlyTimer;

        [Space]
        [SerializeField] private float mainMenuHoldTimerMin;
        [SerializeField] private float mainMenuHoldTimerMax;
        private float _currentMainMenuHoldTimer;

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

            // Randomize random timers if we are in the main menu
            if (InMainMenu)
            {
                _currentMainMenuFlyTimer = Random.Range(mainMenuFlyTimerMin, mainMenuFlyTimerMax);
                _currentMainMenuHoldTimer = Random.Range(mainMenuHoldTimerMin, mainMenuHoldTimerMax);
            }
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

            // Check if we are not in the main menu
            if (!InMainMenu)
            {
                // Fly based on if the click key is being held
                Fly(KeyHold);
            }
            else
            {
                // Do not allow the ship to fly further if above a certain Y position
                if (_transform.position.y > 7.5f)
                {
                    _currentMainMenuHoldTimer = 0;
                }

                // Let 2 random timers decide our flight
                Fly(_currentMainMenuHoldTimer > 0);

                // The hold timer decides when the ship actually flies
                // The fly timer decides when the hold timer (and itself) are reset randomly
                if (_currentMainMenuHoldTimer > 0)
                {
                    _currentMainMenuHoldTimer -= Time.deltaTime;
                }

                if (_currentMainMenuFlyTimer > 0)
                {
                    _currentMainMenuFlyTimer -= Time.deltaTime;
                }
                else
                {
                    _currentMainMenuFlyTimer = Random.Range(mainMenuFlyTimerMin, mainMenuFlyTimerMax);
                    _currentMainMenuHoldTimer = Random.Range(mainMenuHoldTimerMin, mainMenuHoldTimerMax);
                }
            }
            
            HandleParticles();

            AngularVelocity();
        }

        /// <summary>
        /// Adds to the YVelocity if <paramref name="input"/> is true, otherwise we subtract.
        /// </summary>
        private void Fly(bool input)
        {
            YVelocity += flySpeed * Time.deltaTime * (input ? 1 : -1) * UpsideDownMultiplier;
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
            if (OnGround && !KeyHold && !slideParticles.isPlaying)
            {
                slideParticles.Play();
            }
            else if ((!OnGround || KeyHold) && slideParticles.isPlaying)
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
            if (!OnGround)
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