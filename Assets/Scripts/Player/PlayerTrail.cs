using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.Player
{
    /// <summary>
    /// A trail that will follow the player for a certain while, then detatch
    /// </summary>
    public class PlayerTrail : MonoBehaviour
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

        private Coroutine _trailDisableCoroutine;

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

        /// <summary>
        /// Clears the trail and attaches itself to the given <paramref name="transform"/>
        /// </summary>
        public void Attach(Transform transform)
        {
            // Attach and reset position
            Transform.SetParent(transform);
            Transform.localPosition = Vector3.zero;

            // Clear and enable trail
            Trail.Clear();
            ToggleTrail(true);

            // Stop the current trailDisableCoroutine if there is one active currently
            if (_trailDisableCoroutine != null)
            {
                StopCoroutine(_trailDisableCoroutine);
            }
        }

        /// <summary>
        /// Makes the trail detach from any parent and then disables the trail
        /// </summary>
        public void Detach()
        {
            Transform.SetParent(null);

            // Start the disable trail after time coroutine
            _trailDisableCoroutine = StartCoroutine(DisableTrailAfterTime(_trail.time));
        }

        /// <summary>
        /// Waits the given <paramref name="time"/> and then disables the trail when it's finished.
        /// </summary>
        private IEnumerator DisableTrailAfterTime(float time)
        {
            yield return new WaitForSeconds(time);

            ToggleTrail(false);
        }

        /// <summary>
        /// Toggles the trail on/off depending on if <paramref name="enable"/> is true or not
        /// </summary>
        public void ToggleTrail(bool enable)
        {
            Trail.enabled = enable;
        }
    }
}
