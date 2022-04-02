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
        [SerializeField] private float reset3DOffsetTime = 1;
        [SerializeField] private EasingType reset3DOffsetEasing = EasingType.sineInOut;

        private AttachToPath _attachToPath;
        private PlayerMovement _playerMovement;

        private bool In3DMode
        {
            get => _player.movement.In3DMode;
            set => _player.movement.In3DMode = value;
        }

        public override void Start()
        {
            base.Start();

            // Get components
            _attachToPath = GetComponent<AttachToPath>();
            _playerMovement = _player.movement;
        }

        public override void OnEnterPortal()
        {
            In3DMode = enable3DMode;

            // Cancel current tween if we are enabling 3D mode
            if (enable3DMode)
            {
                _playerMovement.Cancel3DOffsetTween();
            }
            // Start new 3D offset tween if we are disabling 3D mode
            else
            {
                _playerMovement.Tween3DOffset(_attachToPath.ZOffset, reset3DOffsetTime, reset3DOffsetEasing);
            }
        }

        public override bool CustomPortalCondition()
        {
            // This condition is true if the player is not in 3D Mode if this portal is supposed to enable it
            return In3DMode != enable3DMode;
        }
    }
}
