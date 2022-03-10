using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.ObjectPooling;

namespace GD3D.Player
{
    /// <summary>
    /// A trail that will follow the player for a certain while, then detatch
    /// </summary>
    public class PlayerTrail : PoolObject
    {
        private TrailRenderer _trail;
        private TrailRenderer Trail
        {
            get
            {
                if (_trail == null)
                {
                    _trail = GetComponent<TrailRenderer>();
                }

                return _trail;
            }
        }

        private Transform _transform;
        private Transform Transform
        {
            get
            {
                if (_transform == null)
                {
                    _transform = transform;
                }

                return _transform;
            }
        }

        public Transform ParentTransform;

        /// <summary>
        /// Sets both the start and end color of the trail to the given <paramref name="color"/>
        /// </summary>
        public void UpdateColor(Color color)
        {
            color.a = Trail.startColor.a;
            Trail.startColor = color;

            color.a = Trail.endColor.a;
            Trail.endColor = color;
        }

        public override void OnSpawn()
        {
            base.OnSpawn();

            // Attach to parent and reset position
            Transform.SetParent(ParentTransform);
            Transform.localPosition = Vector3.zero;

            // Clear trail
            Trail.Clear();
        }

        public void DisableTrail()
        {
            Transform.SetParent(null);

            // Remove this object after waiting for the Trail.time
            RemoveAfterTime(Trail.time);
        }
    }
}
