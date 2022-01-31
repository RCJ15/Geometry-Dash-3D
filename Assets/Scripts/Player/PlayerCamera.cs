using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Game.Player
{
    /// <summary>
    /// Contains the camera and controls where the camera is looking. Also has methods for camera shake
    /// </summary>
    public class PlayerCamera : PlayerScript
    {
        private static PlayerCamera instance;
        
        private Camera cam;

        [Header("Cinemachine")]
        [SerializeField] private CinemachineVirtualCamera cinemachineCam;
        private CinemachineBasicMultiChannelPerlin perlin;

        private float strength;
        private float length;
        private float lengthTimer;

        private Vector3 startPos;
        private Quaternion startRot;

        private void Awake()
        {
            // Set instance
            instance = this;
        }

        /// <summary>
        /// Sets what the cinemachine camera should follow
        /// </summary>
        public static void SetCamFollow(Transform follow)
        {
            instance.cinemachineCam.Follow = follow;
        }

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        public override void Start()
        {
            base.Start();

            // Get the main camera
            cam = Camera.main;

            // Set start position
            startPos = cinemachineCam.transform.position;
            startRot = cinemachineCam.transform.rotation;

            // Get multi channel perlin for camera shake
            perlin = cinemachineCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            // Subscribe to events
            p.OnDeath += OnDeath;
            p.OnRespawn += OnRespawn;
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        public override void Update()
        {
            base.Update();

            // Decrease the timer and set it to 0 if it's below 0
            if (lengthTimer > 0)
            {
                lengthTimer -= Time.deltaTime;
            }
            else if (lengthTimer != 0)
            {
                lengthTimer = 0;
            }
        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            // Update the shaking whilst the timer is above 0
            if (lengthTimer > 0)
            {
                // Set the amplitude to decrease the lower the lengthTimer is
                float shakePower = MathE.Map(0, length, 0, 1, lengthTimer);
                perlin.m_AmplitudeGain = strength * shakePower;
            }
            // Reset shake values when the timer runs out
            else
            {
                if (perlin.m_AmplitudeGain != 0)
                {
                    perlin.m_AmplitudeGain = 0;
                }
                if (perlin.m_FrequencyGain != 0)
                {
                    perlin.m_FrequencyGain = 0;
                }
            }
        }

        /// <summary>
        /// A static shortcut for <see cref="UpdateCam(float, float, float)"/>
        /// </summary>
        public static void Shake(float strength, float frequency, float length)
        {
            // Static shortcut cuz it's easier to access - Ruben
            instance.UpdateCam(strength, frequency, length);
        }

        /// <summary>
        /// Updates the cameras shake values
        /// </summary>
        /// <param name="strength">The strength of the shakes</param>
        /// <param name="frequency">The frequency of shakes</param>
        /// <param name="length">How long the shake will last</param>
        public void UpdateCam(float strength, float frequency, float length)
        {
            this.strength = strength;

            this.length = length;
            lengthTimer = length;

            perlin.m_FrequencyGain = frequency;
        }

        private void OnDeath()
        {
            // Shake the camera
            Shake(5, 5, 0.5f);

            // Make the camera not follow the player
            SetCamFollow(null);
        }

        private void OnRespawn()
        {
            // Reset the camera position
            cinemachineCam.transform.position = startPos;
            cinemachineCam.ForceCameraPosition(startPos, startRot);

            // Make the camera follow the player
            SetCamFollow(transform);
        }
    }
}
