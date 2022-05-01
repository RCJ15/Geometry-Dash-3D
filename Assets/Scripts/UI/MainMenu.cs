using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GD3D.UI
{
    /// <summary>
    /// The main menu.
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        

        private void Start()
        {
        
        }

        private void Update()
        {

        }

        /// <summary>
        /// Opens a hyperlink. Works as expected in WEBGL as well.
        /// </summary>
        public void OpenLink(string url)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                openWindow(url);
            }
            else
            {
                Application.OpenURL(url);
            }
        }

        [DllImport("__Internal")]
        private static extern void openWindow(string url);

        /// <summary>
        /// Transitions to the level select scene.
        /// </summary>
        public void GotoLevelSelect()
        {
            // Don't transition if we are already transitioning
            if (Transition.IsTransitioning)
            {
                return;
            }

            // Hard coded >:(
            Transition.TransitionToScene(1);
        }

        /// <summary>
        /// Transitions to the icon kit scene.
        /// </summary>
        public void GotoIconKit()
        {
            // Don't transition if we are already transitioning
            if (Transition.IsTransitioning)
            {
                return;
            }

            // Hard coded >:(
            Transition.TransitionToScene(2);
        }
    }
}
