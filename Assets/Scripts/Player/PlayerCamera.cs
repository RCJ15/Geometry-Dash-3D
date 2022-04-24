using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using GD3D.Camera;

namespace GD3D.Player
{
    /// <summary>
    /// Contains the camera and controls where the camera is looking. Also has methods for camera shake
    /// </summary>
    public class PlayerCamera : PlayerScript
    {
        //-- Instance
        private static PlayerCamera s_instance;
        
        private CameraBehaviour _cam;

        public override void Awake()
        {
            base.Awake();

            // Set instance
            s_instance = this;
        }

        /// <summary>
        /// Sets the camera target
        /// </summary>
        public static void SetCamTarget(Transform target)
        {
            s_instance._cam.Target = target;
        }

        public override void Start()
        {
            base.Start();

            // Get the main camera
            _cam = CameraBehaviour.Instance;

            // Subscribe to events
            player.OnDeath += OnDeath;
            player.OnRespawn += OnRespawn;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        /// <summary>
        /// Static shortcut for <see cref="CameraBehaviour.Shake(float, float, float)"/>
        /// </summary>
        public static void Shake(float strength, float frequency, float length)
        {
            s_instance._cam.Shake(strength, frequency, length);
        }

        private void OnDeath()
        {
            // Shake the camera
            Shake(1, 30, 0.5f);

            // Make the camera not follow the player
            SetCamTarget(null);
        }

        private void OnRespawn(bool inPracticeMode, Checkpoint checkpoint)
        {
            // Make the camera follow the player
            SetCamTarget(transform);
        }
    }
}
