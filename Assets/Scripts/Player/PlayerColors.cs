using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// Stores the colors of the player and handles when the player switches colors
    /// </summary>
    public class PlayerColors : PlayerScript
    {
        public Color color1 =
            new Color(0.4784314f, 0.9686275f, 0.003921569f, 1); // Default green color
        public Color color2 =
            new Color(0.01568628f, 0.9529412f, 0.9686275f, 1); // Default light blue color

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        public override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        public override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
    }
}