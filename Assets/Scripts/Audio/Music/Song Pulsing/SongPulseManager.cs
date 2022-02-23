using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GD3D.Audio.Pulsing
{
    /// <summary>
    /// Handles all the special objects that will pulse to the music <para/>
    /// The pulsing is based on the songs current loudness <para/>
    /// Use <see cref="Loudness"/> to get the current loudness
    /// </summary>
    public class SongPulseManager : MonoBehaviour
    {
        public static float Loudness;
        public static Action<float> OnLoudnessUpdate;
        public const float MaxVolume = 0.3f;
        
        private AudioSource _source;

        private float[] _clipSampleData;

        private int _sampleAmount;
        private float _frequencyMultiplier;

        void Start()
        {
            // Get audio source
            _source = GetComponent<AudioSource>();

            // Calculate the frequency multiplier
            // We do this so the pulsing system works on any audio frequency
            // This is orignally designed for 44100 hz which is why we use it here
            _frequencyMultiplier = (float)44100 / (float)_source.clip.frequency;

            // Get the correct sample amount for the duration of updateStep
            int totalSamples = _source.clip.samples * _source.clip.channels;

            float samplesPerSecond = (float)totalSamples / (float)_source.clip.length;

            float samplesPerUpdateStep = (float)samplesPerSecond * (float)Time.fixedDeltaTime;

            _sampleAmount = Mathf.CeilToInt(samplesPerUpdateStep);

            _clipSampleData = new float[_sampleAmount];
        }

        private void FixedUpdate()
        {
            UpdateLoudness();
        }

        private void UpdateLoudness()
        {
            if (!_source.isPlaying)
            {
                return;
            }

            // Get sample data and insert it into the _clipSampleData array.
            // _source.timeSamples is here as the offset and where we begin reading.
            // The sample data is fed into _clipSampleData until it's full.
            // This means the length of the _clipSampleData is the ending of our read.

            // TIP: 1024 samples is about 80 ms on a 44khz stereo clip.
            _source.clip.GetData(_clipSampleData, _source.timeSamples);

            // Get the mean of all the samples volume combined
            float vol = 0;
            foreach (float sample in _clipSampleData)
            {
                vol += Mathf.Abs(sample);
            }

            vol /= _clipSampleData.Length;

            // Fix it so it works on all frequencies
            vol *= _frequencyMultiplier;

            // Invoke event with the new volume
            Loudness = vol;
            OnLoudnessUpdate?.Invoke(vol);
        }
    }

    /// <summary>
    /// An enum that represents what type of pulse that is being used
    /// </summary>
    public enum PulseType
    {
        /// <summary>
        /// Used by most objects for pulsing
        /// </summary>
        normal = 0,
        /// <summary>
        /// Used by big objects, like the pulsing arrow and pulsing X
        /// </summary>
        big = 1,
        /// <summary>
        /// Used by all orbs
        /// </summary>
        orb = 2,
        /// <summary>
        /// Used by the wave trail
        /// </summary>
        waveTrail = 3,
    }
}
