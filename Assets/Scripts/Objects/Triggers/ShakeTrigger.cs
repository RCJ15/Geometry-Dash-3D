using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.GDCamera;
using GD3D.Level;

namespace GD3D.Objects
{
    /// <summary>
    /// Shakes the camera when triggered
    /// </summary>
    public class ShakeTrigger : Trigger
    {
        [Header("Shake Settings")]
        [LevelSave] [SerializeField] private float strength;
        [LevelSave] [SerializeField] private float frequency;
        [LevelSave] [SerializeField] private float length;

        //-- References
        private CameraBehaviour _cam;

        protected override void Start()
        {
            base.Start();

            // Get camera instance
            _cam = CameraBehaviour.Instance;
        }

        protected override void OnTriggered()
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
