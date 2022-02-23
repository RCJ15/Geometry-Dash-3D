using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.Audio.Pulsing
{
    /// <summary>
    /// Uses the current size on the <see cref="PulseSizeManager"/> with the correct <see cref="PulseType"/>
    /// </summary>
    public class PulseSize : MonoBehaviour
    {
        [SerializeField] private PulseType pulseType;

        private PulseSizeManager manager;
        private float oldSize;

        private void Start()
        {
            // Destroy itself if no manager with the same pusle type exists
            if (!PulseSizeManager.pulseSizeManagers.ContainsKey(pulseType))
            {
                Destroy(this);
                return;
            }

            // Get the manager
            manager = PulseSizeManager.pulseSizeManagers[pulseType];
        }

        private void FixedUpdate()
        {
            // No changes, thus return
            if (oldSize == manager.CurrentSize)
            {
                return;
            }

            // Set size
            SetSize(manager.CurrentSize);
            oldSize = manager.CurrentSize;
        }

        private void SetSize(float newSize)
        {
            transform.localScale = Vector3.one * newSize;
        }
    }
}
