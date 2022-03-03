using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Player;

namespace GD3D.Objects
{
    /// <summary>
    /// Changes the players gamemode when entered
    /// </summary>
    public class GamemodePortal : Portal
    {
        [Header("Gamemode Settings")]
        [SerializeField] private Gamemode gamemode;

        public override void OnEnterPortal(PlayerMain player)
        {
            // Change the gamemode
            player.gamemode.ChangeGamemode(gamemode);
        }
    }
}
