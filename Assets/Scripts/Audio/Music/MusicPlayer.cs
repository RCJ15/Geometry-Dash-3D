using GD3D.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace GD3D.Audio
{
    public class MusicPlayer : MonoBehaviour
    {
        //-- Static Variables
        public static MusicPlayer Instance;

        [Header("Important Stuff")]
        [SerializeField] private AudioClip song;

        //-- Other References
        private AudioSource source;
        private PlayerMain player;

        private void Awake()
        {
            // Set instance
            Instance = this;
        }

        private void Start()
        {
            // Get references
            source = GetComponent<AudioSource>();
            player = PlayerMain.Instance;

            // Subscribe to events
            player.OnDeath += OnDeath;
            player.OnRespawn += OnRespawn;

            // Setup the audio source
            source.clip = song;
            source.Play();
        }

        private void OnDeath()
        {
            source.Stop();
        }

        private void OnRespawn()
        {
            source.Play();
        }
    }
}
