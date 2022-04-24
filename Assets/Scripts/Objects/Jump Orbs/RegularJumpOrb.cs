using GD3D.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.Objects
{
    /// <summary>
    /// A regular jump orb which just sets the players Y velocity to a certain value
    /// </summary>
    public class RegularJumpOrb : JumpOrb
    {
        [Header("Velocity Setting")]
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
            // Get velocity
            GamemodeSizedFloat velocity = _velocityData[gamemodeHandler.CurrentGamemode];

            // Set velocity
            _player.YVelocity = velocity.GetValue(gamemodeHandler.IsSmall);
        }

        /// <summary>
        /// Contains data about a gamemodes velocity data
        /// </summary>
        [System.Serializable]
        public class GamemodeVelocityData
        {
            public Gamemode Gamemode;

            [Space]
            public GamemodeSizedFloat VelocityData;
        }
    }
}
