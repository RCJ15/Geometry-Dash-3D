using GD3D.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Easing;

namespace GD3D.Objects
{
    /// <summary>
    /// It's the 3D Mode Portal, but I can't spell with numbers so it is ThreeDModePortal :( <para/>
    /// Anyways, changes if the player can or can't move on the second axis when entered
    /// </summary>
    public class ThreeDModePortal : Portal
    {
        [Header("3D Mode Portal Settings")]
        [SerializeField] private bool enable3DMode = true;

        [Space]
        [SerializeField] private EaseSettings easeSettings = EaseSettings.defaultValue;

        private AttachToPath _attachToPath;
        private PlayerMovement _playerMovement;

        private bool In3DMode
        {
            get => _player.Movement.In3DMode;
            set => _player.Movement.In3DMode = value;
        }

        public override void Start()
        {
            base.Start();

            // Get components
            _attachToPath = GetComponent<AttachToPath>();
            _playerMovement = _player.Movement;
        }

        public override void OnEnterPortal()
        {
            In3DMode = enable3DMode;

            // Cancel current tween if we are enabling 3D mode
            if (enable3DMode)
            {
                _playerMovement.Cancel3DOffsetEase();
            }
            // Start new 3D offset tween if we are disabling 3D mode
            else
            {
                // Create a easing that will be used by the player movement script
                EaseObject obj = easeSettings.CreateEase();

                _playerMovement.Ease3DOffset(_attachToPath.ZOffset, obj);
            }
        }

        // This condition is true if the player is not in 3D Mode if this portal is supposed to enable it
        public override bool CustomPortalCondition => In3DMode != enable3DMode;
    }
}
