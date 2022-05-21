using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.Audio
{
    /// <summary>
    /// The music that plays in the main menu.
    /// </summary>
    public class MainMenuMusic : MonoBehaviour
    {
        public static MainMenuMusic Instance;

        private AudioSource _source;

        private void Awake()
        {
            // Check if instance is null
            if (Instance == null)
            {
                // Set the instance to this object
                Instance = this;

                // Dont destroy on load
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // Destroy self since there already exists an instance of this object
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Get components
            _source = GetComponent<AudioSource>();

            // Play music
            _source.Play();
        }

        /// <summary>
        /// Stops the currently playing instance.
        /// </summary>
        public static void StopInstance()
        {
            // Destroy the instance
            Destroy(Instance.gameObject);

            // Reset instance so a new one can s
            Instance = null;
        }
    }
}
