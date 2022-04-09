using GD3D.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

namespace GD3D.Objects
{
    /// <summary>
    /// A trigger is an invisble object that will make something happen at a specific point during the level, like changing the background color gradually. <para/>
    /// IMPORTANT NOTE: If you're going to create a new trigger that changes a value overtime, inherit from <see cref="TimedTrigger"/> instead as it will be able to be saved in checkpoints.
    /// </summary>
    [RequireComponent(typeof(AttachToPath))]
    public abstract class Trigger : MonoBehaviour
    {
        [Header("General Trigger Options")]
        [SerializeField] private bool isTouchTriggered;

        private bool _hasBeenTriggered;
        private bool _playerHasPassed;

        public bool HasBeenTriggered => _hasBeenTriggered;

        //-- References
        protected AttachToPath _attachToPath;
        protected PlayerMain _player;

        //-- Properties
        public float Distance => _attachToPath.Distance;
        protected bool CanTrigger => !_hasBeenTriggered && !_player.dead && CustomTriggerCondition();

        public virtual void Start()
        {
            // Get references
            _attachToPath = GetComponent<AttachToPath>();
            _player = PlayerMain.Instance;

            // Subsribe to events
            _player.OnRespawn += OnRespawn;
        }

        /// <summary>
        /// Override this to do stuff when the player dies
        /// </summary>
        public virtual void OnRespawn()
        {
            // Execute this one frame later using this epic timer thingy I wrote :D
            Helpers.TimerEndOfFrame(this, () =>
            {
                _hasBeenTriggered = false;
                _playerHasPassed = false;
            }
            );
        }

        public virtual void Update()
        {
            // Return if this trigger is touch triggered (cuz then we check in OnCollisionEnter())
            if (isTouchTriggered || _playerHasPassed)
            {
                return;
            }

            // Check if the player has gone past the trigger
            if (_player.movement.TravelAmount > Distance)
            {
                // Make sure this is set to true so the trigger won't trigger again
                _playerHasPassed = true;

                // Trigger the trigger (if we can trigger)
                if (CanTrigger)
                {
                    OnTriggered();
                    _hasBeenTriggered = true;
                }
            }
        }

        public virtual void OnCollisionEnter(Collision col)
        {
            // Return if this trigger is NOT touch triggered (cuz then we check in Update())
            // Also return if this trigger cant trigger
            if (!isTouchTriggered || !CanTrigger)
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
        /// Override this to determine a custom trigger condition that has to be met in order for the player to trigger this trigger. <para/>
        /// So this must return true in order for the trigger to be triggered.
        /// </summary>
        public virtual bool CustomTriggerCondition()
        {
            return true;
        }

#if UNITY_EDITOR
        // Draw a trigger line in the editor
        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2705882f, 0.8784314f, 1, 0.5f);

            Vector3 pos1 = transform.position;
            pos1.y = -100;

            Vector3 pos2 = transform.position;
            pos2.y = 100;

            Gizmos.DrawLine(pos1, pos2);
        }

        /// <summary>
        /// Will draw a gizmo duration line that curves along the current path creator.
        /// </summary>
        protected void DrawDurationLine(float time, int linesPerTile = 3)
        {
            Gizmos.color = new Color(0.8f, 0.8f, 0.8f, 0.5f);
            
            float totalDistance = time * PlayerMovement.NORMAL_SPEED;

            // Get path and AttachToPath component
            PathCreator pathCreator = FindObjectOfType<PathCreator>();
            VertexPath path = pathCreator.path;

            _attachToPath = GetComponent<AttachToPath>();

            float startDistance = _attachToPath.Distance;

            // Draw the lines
            float addedDistance = 0;

            Vector3 oldPos = transform.position;

            // Calculate amount of lines
            int amount = Mathf.CeilToInt((float)totalDistance * (float)linesPerTile);

            for (int i = 0; i < amount; i++)
            {
                // Get position at the current distance
                Vector3 posAtDistance = path.GetPointAtDistance(startDistance + addedDistance);
                posAtDistance.y = oldPos.y;

                // Draw the line
                Gizmos.DrawLine(oldPos, posAtDistance);

                // Set old pos
                oldPos = posAtDistance;

                // Add distance
                addedDistance += (float)totalDistance / (float)amount;
            }

        }
#endif
    }
}
