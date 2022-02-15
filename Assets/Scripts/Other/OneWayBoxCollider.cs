using UnityEngine;

namespace GD3D
{
    /// <summary>
    /// This was copied from: https://www.youtube.com/watch?v=qwwjwb7XlUc <para/>
    /// So credit goes to them.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class OneWayBoxCollider : MonoBehaviour
    {
        /// <summary> The direction that the other object should be coming from for entry. </summary>
        [SerializeField] private Vector3 entryDirection = Vector3.up;
        /// <summary> Should the entry direction be used as a local direction? </summary>
        [SerializeField] private bool localDirection = false;
        /// <summary> How large should the trigger be in comparison to the original collider? </summary>
        [SerializeField] private Vector3 triggerScale = Vector3.one * 1.25f;
        /// <summary> The original collider. Will always be present thanks to the RequireComponent attribute. </summary>
        private new BoxCollider collider = null;
        /// <summary> The trigger that we add ourselves once the game starts up. </summary>
        private BoxCollider collisionCheckTrigger = null;

        /// <summary> The entry direction, calculated accordingly based on whether it is a local direction or not. 
        /// This is pretty much what I've done in the video when copy-pasting the local direction check, but written as a property. </summary>
        public Vector3 Direction => localDirection ? transform.TransformDirection(entryDirection.normalized) : entryDirection.normalized;

        private void Awake()
        {
            collider = GetComponent<BoxCollider>();

            // Adding the BoxCollider and making sure that its sizes match the ones
            // of the OG collider.
            collisionCheckTrigger = gameObject.AddComponent<BoxCollider>();
            collisionCheckTrigger.size = new Vector3(
                collider.size.x * triggerScale.x,
                collider.size.y * triggerScale.y,
                collider.size.z * triggerScale.z
            );
            collisionCheckTrigger.center = collider.center;
            collisionCheckTrigger.isTrigger = true;
        }

        private void OnValidate()
        {
            // This bit of code exists only to prevent OnDrawGizmos from throwing
            // errors in the editor mode when it does not have reference to the
            // collider, if used.
            collider = GetComponent<BoxCollider>();
            collider.isTrigger = false;
        }

        private void OnTriggerStay(Collider other)
        {
            // Simulate a collision between our trigger and the intruder to check
            // the direction that the latter is coming from. The method returns true
            // if any collision has been detected.
            if (Physics.ComputePenetration(
                collisionCheckTrigger, transform.position, transform.rotation,
                other, other.transform.position, other.transform.rotation,
                out Vector3 collisionDirection, out float _))
            {
                float dot = Vector3.Dot(Direction, collisionDirection);

                // Opposite direction; passing not allowed.
                if (dot < 0)
                {
                    // Making sure that the two object are NOT ignoring each other.
                    Physics.IgnoreCollision(collider, other, false);
                }
                else
                {
                    // Making the colliders ignore each other, and thus allow passage
                    // from one way.
                    Physics.IgnoreCollision(collider, other, true);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Instead of directly using the transform.position, we're using the
            // collider center, converted into global position. The way I did it
            // in the video made it easier to write, but the on-screen drawing would
            // not take in consideration the actual offset of the collider.
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.TransformPoint(collider.center), Direction * 2);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.TransformPoint(collider.center), -Direction * 2);
        }
    }
}