using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Input;

namespace Game.Player
{
    /// <summary>
    /// Jumps
    /// </summary>
    [System.Serializable]
    public class CubeGamemode : GamemodeScript
    {
        public float jumpHeight = 18.6f;

        /// <summary>
        /// OnEnable is called when the gamemode is switched to this gamemode
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
        }

        /// <summary>
        /// OnDisable is called when the gamemode is switched from this gamemode
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
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

            // Add torque whilst in the air
            if (!OnGround)
            {
                rb.AddTorque(-10 * Vector3.forward, ForceMode.Force);
            }
        }

        /// <summary>
        /// OnClick is called when the player presses the main gameplay button. <para/>
        /// <paramref name="mode"/> determines whether the button was just pressed, held or just released.
        /// </summary>
        public override void OnClick(PressMode mode)
        {
            base.OnClick(mode);

            // Can't jump if not on ground
            if (!OnGround)
            {
                return;
            }

            // Check the press mode
            switch (mode)
            {
                // The button was just pressed
                case PressMode.hold:

                    YVelocity = jumpHeight;
                    rb.AddTorque(-2 * Vector3.forward, ForceMode.VelocityChange);

                    break;
            }
        }
    }
}
