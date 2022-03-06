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

        [Header("Borders")]
        [SerializeField] private bool haveBorders;
        [SerializeField] private int borderDistance = 10;

        public override void OnEnterPortal()
        {
            // Change the gamemode
            _player.gamemode.ChangeGamemode(gamemode);

            // Apply borders or remove them, depening on if haveBorders is true or not
            if (haveBorders)
            {
                BorderManager.ApplyBorders(GetBorderMinY(), GetBorderMaxY());
            }
            else
            {
                // Remove borders
                BorderManager.RemoveBorders();
            }
        }

        public override bool CustomPortalCondition()
        {
            // This condition is true if the player is not in the same gamemode
            return _player.gamemode.CurrentGamemode != gamemode;
        }

        /// <summary>
        /// Returns the border max Y position
        /// </summary>
        public float GetBorderMaxY()
        {
            float maxY = transform.position.y + (borderDistance / 2);

            maxY = Mathf.Clamp(maxY, 0 + borderDistance, BorderManager.MAX_HEIGHT);

            return Mathf.Round(maxY);
        }

        /// <summary>
        /// Returns the border min Y position
        /// </summary>
        public float GetBorderMinY()
        {
            float minY = transform.position.y - (borderDistance / 2);

            minY = Mathf.Clamp(minY, 0, BorderManager.MAX_HEIGHT - borderDistance);

            return Mathf.Round(minY);
        }

#if UNITY_EDITOR
        // Draw borders in editor
        private void OnDrawGizmosSelected()
        {
            if (!haveBorders)
            {
                return;
            }

            Gizmos.color = Color.blue;

            Vector3 pos = transform.position;

            pos.y = GetBorderMaxY();
            Gizmos.DrawLine(pos - new Vector3(10, 0, 0), pos + new Vector3(10, 0, 0));

            pos.y = GetBorderMinY();
            Gizmos.DrawLine(pos - new Vector3(10, 0, 0), pos + new Vector3(10, 0, 0));
        }
#endif
    }
}
