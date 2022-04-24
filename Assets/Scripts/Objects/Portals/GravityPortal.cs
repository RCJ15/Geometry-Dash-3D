using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Player;

namespace GD3D.Objects
{
    /// <summary>
    /// Switches gravity when entered
    /// </summary>
    public class GravityPortal : Portal
    {
        [Header("Gravity Portal")]
        [SerializeField] private bool upsideDown;

        public bool UpsideDown => upsideDown;

        public override void OnEnterPortal()
        {
            // Switch to be upside down
            _player.GamemodeHandler.UpsideDown = upsideDown;
            _player.YVelocity /= 2;

            // Enable the players trail
            PlayerTrailManager.HaveTrail = true;
        }

        // This condition is true if the player is not the same upside down as this portal is
        public override bool CustomPortalCondition => _player.GamemodeHandler.UpsideDown != upsideDown;
    }
}
