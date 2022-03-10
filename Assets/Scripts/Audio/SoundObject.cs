using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.ObjectPooling;

namespace GD3D.Audio
{
    /// <summary>
    /// A object that plays sound. Is used as the pool object in the <see cref="SoundManager"/>
    /// </summary>
    public class SoundObject : PoolObject
    {
        private AudioSource source;
        public AudioSource GetSource => source;

        public override void OnCreated()
        {
            // Get component
            source = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Plays the sound and starts a timer that'll remove this object from the pool when the sound is finished playing
        /// </summary>
        public void Play()
        {
            // Play the sound
            source.Play();

            // Remove from pool when the sound has finished playing
            RemoveAfterTime(source.clip.length);
        }
    }
}
