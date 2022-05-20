using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.UI
{
    /// <summary>
    /// The level select screen.
    /// </summary>
    public class LevelSelect : MonoBehaviour
    {
        [SerializeField] private string[] levels;

        [SerializeField] private GameObject levelObject;

        private void Start()
        {
            // Clone multiple copies of the level object to populate the level select
            foreach (string levelName in levels)
            {
                GameObject newObj =  Instantiate(levelObject);
            }
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
