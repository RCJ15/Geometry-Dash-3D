using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Camera;

namespace GD3D.Objects
{
    /// <summary>
    /// Shakes the camera when triggered
    /// </summary>
    public class ShakeTrigger : Trigger
    {
        [Header("Shake Settings")]
        [SerializeField] private float strength;
        [SerializeField] private float frequency;
        [SerializeField] private float length;

        //-- References
        private CameraBehaviour _cam;

        public override void Start()
        {
            base.Start();

            // Get camera instance
            _cam = CameraBehaviour.Instance;
        }

        public override void OnTriggered()
        {
            // Shake the camera
            _cam.Shake(strength, frequency, length);
        }

#if UNITY_EDITOR
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            DrawDurationLine(length);
        }
#endif
    }
}
