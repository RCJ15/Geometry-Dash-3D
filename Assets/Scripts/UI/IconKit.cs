using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.UI
{
    /// <summary>
    /// The icon kit select screen.
    /// </summary>
    public class IconKit : MonoBehaviour
    {


        private void Start()
        {
            // Set the last active menu scene index
            MenuData.LastActiveMenuSceneIndex = (int)Transition.SceneIndex.levelSelect;
        }

        private void Update()
        {

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
