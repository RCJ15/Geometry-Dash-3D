using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GD3D.Audio.Pulsing
{
    /// <summary>
    /// Changes 
    /// </summary>
    public class PulseSizeManager : SongPulseObject
    {
        [Header("General Info")]
        [SerializeField] private PulseType pulseType;
        public static Dictionary<PulseType, PulseSizeManager> pulseSizeManagers = new Dictionary<PulseType, PulseSizeManager>();

        [Header("Size")]
        [SerializeField] private float moveDelta = 0.1f;
        [Space]

        [SerializeField] private float minSize = 0.2f;
        [SerializeField] private float maxSize = 1;

        [Header("Volume")]
        [SerializeField] private float minVolume = 0;
        [SerializeField] private float maxVolume = SongPulseManager.MaxVolume;

        [Header("Volume Blind Spots")]
        [SerializeField] private bool haveBlindSpots;
        [SerializeField] private float blindMinVolume;
        [SerializeField] private float blindMaxVolume;

        // Current Size
        private float _currentSize;

        public float CurrentSize => _currentSize;

        private void Awake()
        {
            // Remove all empty slots in the dictionary
            pulseSizeManagers = pulseSizeManagers.Where(pair => pair.Value != null)
                              .ToDictionary(pair => pair.Key, pair => pair.Value);

            // Destroy itself if the pulse type already exists
            if (pulseSizeManagers.ContainsKey(pulseType))
            {
                Destroy(gameObject);
                return;
            }

            // Add itself to the dictionary
            pulseSizeManagers.Add(pulseType, this);
        }

        private void FixedUpdate()
        {
            if (_currentSize == minSize)
            {
                return;
            }

            // Decrease the current size to the minimum size
            _currentSize = Mathf.MoveTowards(_currentSize, minSize, moveDelta * maxSize);
        }

        public override void OnUpdateLoudness(float vol)
        {
            // First check blind spots
            if (haveBlindSpots)
            {
                if (vol < blindMinVolume)
                {
                    return;
                }

                if (vol > blindMaxVolume)
                {
                    vol = blindMaxVolume;
                }
            }

            // Map the new size
            float newSize = Helpers.Map(minVolume, maxVolume, minSize, maxSize, vol);
            
            // Only set the new size if it's bigger than the old size
            if (newSize > _currentSize)
            {
                _currentSize = newSize;
            }
        }
    }
}
