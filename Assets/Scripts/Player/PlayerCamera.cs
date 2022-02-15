using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace GD3D.Player
{
    /// <summary>
    /// Contains the camera and controls where the camera is looking. Also has methods for camera shake
    /// </summary>
    public class PlayerCamera : PlayerScript
    {
        //-- Instance
        private static PlayerCamera s_instance;
        
        private Camera _cam;

        //-- Cinemachine
        private CinemachineVirtualCamera[] _cinemachineCams;
        private CinemachineBasicMultiChannelPerlin[] _perlins;

        //-- Shake
        private float _strength;
        private float _length;
        private float _lengthTimer;

        //-- Start values
        private Vector3[] startPos;
        private Quaternion[] startRot;

        private void Awake()
        {
            // Set instance
            s_instance = this;
        }

        /// <summary>
        /// Sets what all the cinemachine cameras should follow
        /// </summary>
        public static void SetCamFollow(Transform follow)
        {
            // Loop through all the cameras
            foreach (CinemachineVirtualCamera cinemachineCam in s_instance._cinemachineCams)
            {
                cinemachineCam.Follow = follow;
            }
        }

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        public override void Start()
        {
            base.Start();

            // Get the main camera
            _cam = Camera.main;

            // Find all cinemachine cameras
            _cinemachineCams = FindObjectsOfType<CinemachineVirtualCamera>();

            // Create temporary lists
            List<Vector3> startPosList = new List<Vector3>();
            List<Quaternion> startRotList = new List<Quaternion>();
            List<CinemachineBasicMultiChannelPerlin> perlinList = new List<CinemachineBasicMultiChannelPerlin>();

            int index = 0;
            foreach (CinemachineVirtualCamera cinemachineCam in s_instance._cinemachineCams)
            {
                print(cinemachineCam.name + " | " + index);
                index++;

                // Set start values for each camera
                startPosList.Add(cinemachineCam.transform.position);
                startRotList.Add(cinemachineCam.transform.rotation);

                // Also get the multi channel perlin for the camera shaking
                perlinList.Add(cinemachineCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>());
            }

            // Set the lists to be the arrays
            startPos = startPosList.ToArray();
            startRot = startRotList.ToArray();
            _perlins = perlinList.ToArray();

            // Subscribe to events
            _player.OnDeath += OnDeath;
            _player.OnRespawn += OnRespawn;
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        public override void Update()
        {
            base.Update();

            // Decrease the timer and set it to 0 if it's below 0
            if (_lengthTimer > 0)
            {
                _lengthTimer -= Time.deltaTime;
            }
            else if (_lengthTimer != 0)
            {
                _lengthTimer = 0;
            }
        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            // Update the shaking whilst the timer is above 0
            if (_lengthTimer > 0)
            {
                // Set the amplitude to decrease the lower the lengthTimer is
                float shakePower = MathE.Map(0, _length, 0, 1, _lengthTimer);

                foreach (CinemachineBasicMultiChannelPerlin perlin in _perlins)
                {
                    perlin.m_AmplitudeGain = _strength * shakePower;
                }
            }
            // Reset shake values when the timer runs out
            else
            {
                foreach (CinemachineBasicMultiChannelPerlin perlin in _perlins)
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
        }

        /// <summary>
        /// A static shortcut for <see cref="UpdateCam(float, float, float)"/>
        /// </summary>
        public static void Shake(float strength, float frequency, float length)
        {
            // Static shortcut cuz it's easier to access - Ruben
            s_instance.UpdateCam(strength, frequency, length);
        }

        /// <summary>
        /// Updates the cameras shake values
        /// </summary>
        /// <param name="strength">The strength of the shakes</param>
        /// <param name="frequency">The frequency of shakes</param>
        /// <param name="length">How long the shake will last</param>
        public void UpdateCam(float strength, float frequency, float length)
        {
            this._strength = strength;

            this._length = length;
            _lengthTimer = length;

            foreach (CinemachineBasicMultiChannelPerlin perlin in _perlins)
            {
                perlin.m_FrequencyGain = frequency;
            }
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
            // Reset all of the cameras positions and rotations
            for (int i = 0; i < _cinemachineCams.Length; i++)
            {
                _cinemachineCams[i].transform.position = startPos[i];
                _cinemachineCams[i].ForceCameraPosition(startPos[i], startRot[i]);
            }

            // Make the camera follow the player
            SetCamFollow(transform);
        }

        /// <summary>
        /// Changes the priority of all cameras to 0, except for the camera with the index given.
        /// </summary>
        /// <param name="cameraIndex"></param>
        public void ChangePriorityCamera(int cameraIndex)
        {
            // Loop through all cameras
            for (int i = 0; i < _cinemachineCams.Length; i++)
            {
                // Set their priority to 0 unless the current camera is the same as the camera we want to prioritise
                _cinemachineCams[i].Priority = i == cameraIndex ? 1 : 0;
            }
        }
    }
}
