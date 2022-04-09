using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Objects;
using GD3D.CustomInput;

namespace GD3D.Player
{
    /// <summary>
    /// Handles going into and going out of practice mode. <para/>
    /// Also handles placing and removing checkpoint crystals in practice mode.
    /// </summary>
    public class PlayerPracticeMode : PlayerScript
    {
        [SerializeField] private GameObject checkpointCrystal;

        private List<Checkpoint> checkpoints = new List<Checkpoint>();

        private Key placeKey;
        private Key removeKey;

        public override void Start()
        {
            base.Start();

            // Get keys
            placeKey = PlayerInput.GetKey("Place Checkpoint Crystal");
            removeKey = PlayerInput.GetKey("Remove Checkpoint Crystal");
        }

        public override void Update()
        {
            base.Update();

            // Place checkpoint crystal
            if (placeKey.Pressed(PressMode.down))
            {

            }

            // Remove checkpoint crystal if there is more than 0 crystals existing
            if (removeKey.Pressed(PressMode.down) && checkpoints.Count > 0)
            {

            }
        }

        /// <summary>
        /// Places a checkpoint crystal
        /// </summary>
        private void PlaceCheckpointCrystal()
        {

        }
    }
}
