using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.Audio.Pulsing
{
    /// <summary>
    /// This object will trigger a event when the <see cref="SongPulseManager"/> updates it's loudness. <para/>
    /// Is used as a base class for most objects that will pulse to the song.
    /// </summary>
    public abstract class SongPulseObject : MonoBehaviour
    {
        /// <summary>
        /// Called whenever new loudness is set in the <see cref="SongPulseManager"/>
        /// </summary>
        /// <param name="vol">The new loudness as a parameter</param>
        public abstract void OnUpdateLoudness(float vol);

        private void OnEnable() => SongPulseManager.OnLoudnessUpdate += OnUpdateLoudness;

        private void OnDisable() => SongPulseManager.OnLoudnessUpdate -= OnUpdateLoudness;
    }
}
