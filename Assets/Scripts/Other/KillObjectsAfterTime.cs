using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D
{
    public class KillObjectsAfterTime : MonoBehaviour
    {
        public float Lifetime;
        public GameObject[] ObjectsToKill = new GameObject[] { };
        public bool UnscaledTime;
        public bool KillThisObject = true;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            StartCoroutine(Despawn());
        }

        /// <summary>
        /// Starts a timer that'll destroy objects when it runs out
        /// </summary>
        private IEnumerator Despawn()
        {
            if (UnscaledTime)
            {
                yield return new WaitForSecondsRealtime(Lifetime);
            }
            else
            {
                yield return new WaitForSeconds(Lifetime);
            }

            foreach (GameObject g in ObjectsToKill)
            {
                Destroy(g);
            }

            if (KillThisObject)
            {
                Destroy(gameObject);
            }
        }
    }
}