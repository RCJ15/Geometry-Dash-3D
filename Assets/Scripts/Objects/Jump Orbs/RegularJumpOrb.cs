using GD3D.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.Objects
{
    /// <summary>
    /// A regular jump orb which just sets the players Y velocity to a certain value and can also invert the players gravity.
    /// </summary>
    public class RegularJumpOrb : JumpOrb
    {
        [Header("Velocity Setting")]
        [SerializeField] private bool invertGravity;
        [SerializeField] private float setVelocity = 0; // If 0, then gamemode velocity data is used instead

        [Space]
        [SerializeField] private GamemodeVelocityData[] gamemodeVelocityData;
        private Dictionary<Gamemode, GamemodeSizedFloat> _velocityData = new Dictionary<Gamemode, GamemodeSizedFloat>();

        private PlayerGamemodeHandler gamemodeHandler;

        public override void Start()
        {
            base.Start();

            // Create dictionary
            foreach (GamemodeVelocityData gamemodeData in gamemodeVelocityData)
            {
                // Continue if the gamemode already exists
                if (_velocityData.ContainsKey(gamemodeData.Gamemode))
                {
                    continue;
                }

                // Add to dictionary
                _velocityData.Add(gamemodeData.Gamemode, gamemodeData.VelocityData);
            }

            // Get gamemode handler
            gamemodeHandler = _player.GamemodeHandler;
        }

        public override void OnPressed()
        {
            if (setVelocity != 0)
            {
                _player.YVelocity = setVelocity;
            }
            else
            {
                // Get velocity
                GamemodeSizedFloat velocity = _velocityData[gamemodeHandler.CurrentGamemode];

                // Set velocity
                _player.YVelocity = velocity.GetValue(gamemodeHandler.IsSmall);
            }

            // Correct for upside down
            _player.YVelocity *= gamemodeHandler.UpsideDownMultiplier;

            // Invert gravity
            if (invertGravity)
            {
                gamemodeHandler.UpsideDown = !gamemodeHandler.UpsideDown;
            }
        }
    }
}
