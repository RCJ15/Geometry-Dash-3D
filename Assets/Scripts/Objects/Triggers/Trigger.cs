using GD3D.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.Objects
{
    /// <summary>
    /// A trigger is an invisble object that will make something happen at a specific point during the level, like changing the background color gradually.
    /// </summary>
    [RequireComponent(typeof(AttachToPath))]
    public abstract class Trigger : MonoBehaviour
    {
        [Header("General Trigger Options")]
        [SerializeField] private bool isTouchTriggered;

        private bool _hasBeenTriggered;

        //-- References
        private AttachToPath _attachToPath;
        private PlayerMain _player;

        //-- Properties
        private float Distance => _attachToPath.Distance;

        public virtual void Start()
        {
            // Get references
            _attachToPath = GetComponent<AttachToPath>();
            _player = PlayerMain.Instance;
        }

        public virtual void Update()
        {
            // Do not trigger if this trigger has already been triggered
            // Or if the custom condition hasn't been met
            // Or if this trigger is touch triggered (cuz then we check in OnCollisionEnter())
            if (isTouchTriggered || _hasBeenTriggered || !CustomTriggerCondition())
            {
                return;
            }

            // Trigger if the player has gone past the distance
            if (_player.movement.TravelAmount > Distance)
            {
                OnTriggered();
                _hasBeenTriggered = true;
            }
        }

        public virtual void OnCollisionEnter(Collision col)
        {
            // Do not trigger if this trigger has already been triggered
            // Or if the custom condition hasn't been met
            // Or if this trigger is NOT touch triggered (cuz then we check in Update())
            if (!isTouchTriggered || _hasBeenTriggered || !CustomTriggerCondition())
            {
                return;
            }

            // Player touched trigger
            if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                OnTriggered();
                _hasBeenTriggered = true;
            }
        }

        /// <summary>
        /// Implement this to determine what happens when this trigger is triggered
        /// </summary>
        public abstract void OnTriggered();

        /// <summary>
        /// Override this to determine a custom trigger condition that has to be met in order for the player to trigger this trigger
        /// </summary>
        public virtual bool CustomTriggerCondition()
        {
            return true;
        }
    }
}
