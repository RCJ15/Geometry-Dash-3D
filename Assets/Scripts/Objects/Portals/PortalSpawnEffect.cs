using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.ObjectPooling;

namespace GD3D.Objects
{
    /// <summary>
    /// The portal spawning effect
    /// </summary>
    public class PortalSpawnEffect : MonoBehaviour
    {
        private MaterialColorer materialColorer;
        private Animator anim;

        private void Start()
        {
            // Get components
            materialColorer = GetComponent<MaterialColorer>();
            anim = GetComponent<Animator>();
        }

        /// <summary>
        /// Will set the <see cref="MaterialColorer"/> on this object to have the same color as the given <paramref name="color"/>
        /// </summary>
        public void SetColors(Color color)
        {
            // Make sure the color has the same alpha value
            color.a = materialColorer.GetColor.a;
            materialColorer.GetColor = color;

            // Update colors
            materialColorer.UpdateColors();
        }

        /// <summary>
        /// Will make the <see cref="Animator"/> on this object to play the spawn animation
        /// </summary>
        public void Animate()
        {
            anim.SetTrigger("Portal Enter Effect");
        }
    }
}
