using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GD3D.LevelEditor
{
    /// <summary>
    /// The darkness that appears whenever a window is opened in the level editor. <para/>
    /// Use <see cref="Appear"/> and <see cref="Disappear"/>. <para/>
    /// NOTE: The overlay WILL block raycasts for everything layered behind it, use Appear() with a false parameter to disable this.
    /// </summary>
    public class LevelEditorDarknessOverlay : MonoBehaviour
    {
        public static GameObject GameObject;
        public static Image Image;

        private static Color _startColor; // Based color
        private static int _timesOpen; // This will allow the overlay to stack :D

        private void Awake()
        {
            // Set static variables
            GameObject = gameObject;
            Image = GetComponent<Image>();

            _startColor = Image.color;

            // Be deactivated by default
            GameObject.SetActive(false);
            _timesOpen = 0;
        }

        /// <summary>
        /// Makes the darkness overlay appear above everything (except for things layered above this) <para/>
        /// NOTE: The overlay WILL block raycasts. Set <paramref name="blockRaycasts"/> to false to prevent this.
        /// </summary>
        public static void Appear(bool blockRaycasts = true)
        {
            _timesOpen++;

            // Update block raycasts
            Image.raycastTarget = blockRaycasts;

            UpdateAppearance();
        }

        /// <summary>
        /// Makes the darkness overlay disappear. (Wow no way) 
        /// </summary>
        public static void Disappear()
        {
            _timesOpen--;

            UpdateAppearance();
        }

        /// <summary>
        /// Updates the appearance of the darkness overlay. (Wow no way)
        /// </summary>
        private static void UpdateAppearance()
        {
            GameObject?.SetActive(_timesOpen > 0);

            // Update image if times open is greater than 0 (Also make sure the image exists)
            if (_timesOpen > 0 && Image != null)
            {
                // Make the darkness more intense the more times it's open to give the illusion that we have multiple darkness overlays
                Color col = _startColor;
                col.a = Mathf.Clamp01(col.a * _timesOpen);

                Image.color = col;
            }
        }
    }
}
