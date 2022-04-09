using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Player;

namespace GD3D.Objects
{
    /// <summary>
    /// A object that contains data about the player during a certain state
    /// </summary>
    public class Checkpoint : MonoBehaviour
    {
        // Position
        public float Distance;
        public float Offset;
        public float YPosition;

        // Player Data
        public Gamemode PlayerGamemode = Gamemode.cube;
        public GameSpeed PlayerSpeed = GameSpeed.normalSpeed;

        public Vector3 PlayerVelocity;
        public float PlayerOffsetVelocity;

        /// <summary>
        /// Sets this checkpoints data based on the players current gamemode, position, velocity and speed
        /// </summary>
        public void SetCheckpoint()
        {
            // Get the player
            PlayerMain player = PlayerMain.Instance;

            // 
        }
    }
}
