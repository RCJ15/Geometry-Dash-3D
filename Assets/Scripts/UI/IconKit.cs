using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.CustomInput;

namespace GD3D.UI
{
    /// <summary>
    /// The icon kit select screen.
    /// </summary>
    public class IconKit : MonoBehaviour
    {
        private Key _quitKey;

        private void Start()
        {
            // Set the last active menu scene index
            MenuData.LastActiveMenuSceneIndex = (int)Transition.SceneIndex.levelSelect;

            // Get the quit key
            _quitKey = Player.PlayerInput.GetKey("Escape");
        }

        private void Update()
        {
            // Change the scene to the main menu if the quit key is pressed down
            if (!Transition.IsTransitioning && _quitKey.Pressed(PressMode.down))
            {
                GotoMenu(Transition.SceneIndex.mainMenu);
            }
        }

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
