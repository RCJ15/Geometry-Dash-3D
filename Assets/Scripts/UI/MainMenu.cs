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
            // Set the last active menu scene index
            MenuData.LastActiveMenuSceneIndex = (int)Transition.SceneIndex.mainMenu;
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
                OpenWindow(url);
            }
            else
            {
                Application.OpenURL(url);
            }
        }

        // Need to use a special dll and jslib file for this to work properly
        [DllImport("__Internal")]
        private static extern void OpenWindow(string url);

        /// <summary>
        /// Transitions to the given menu scene <paramref name="index"/>.
        /// </summary>
        public void GotoMenu(Transition.SceneIndex index)
        {
            // Don't transition if we are already transitioning
            if (Transition.IsTransitioning)
            {
                return;
            }

            Transition.TransitionToScene((int)index);
        }

        /// <summary>
        /// Transitions to the given scene <paramref name="index"/>.
        /// </summary>
        public void GotoScene(int index)
        {
            // Don't transition if we are already transitioning
            if (Transition.IsTransitioning)
            {
                return;
            }

            Transition.TransitionToScene(index);
        }
    }
}
