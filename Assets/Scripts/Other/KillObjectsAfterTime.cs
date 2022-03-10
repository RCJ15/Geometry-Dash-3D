using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D
{
    /// <summary>
    /// Kills an object or some objects after a certain amount of given time
    /// </summary>
    public class KillObjectsAfterTime : MonoBehaviour
    {
        public float Lifetime;
        public GameObject[] ObjectsToKill = new GameObject[] { };
        public bool UnscaledTime;
        public bool KillThisObject = true;

        private void Start()
        {
            StartCoroutine(Despawn());
        }

        /// <summary>
        /// Starts a timer that'll destroy the object or some objects when it runs out
        /// </summary>
        private IEnumerator Despawn()
        {
            // Wait
            if (UnscaledTime)
            {
                yield return new WaitForSecondsRealtime(Lifetime);
            }
            else
            {
                yield return new WaitForSeconds(Lifetime);
            }

            // Kill objects in the array (if it has any of course)
            if (ObjectsToKill.Length > 0)
            {
                foreach (GameObject g in ObjectsToKill)
                {
                    Destroy(g);
                }
            }

            // Kill this object
            if (KillThisObject)
            {
                Destroy(gameObject);
            }
        }
    }
}