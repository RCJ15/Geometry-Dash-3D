using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GD3D.Player;

namespace GD3D.Objects
{
    /// <summary>
    /// Portal base class that all portals inherit from
    /// </summary>
    public abstract class Portal : MonoBehaviour
    {
        [Header("Base Portal Settings")]
        [SerializeField] private bool multiTrigger;
        private bool cantBeEntered;

        [Space]
        [SerializeField] private GameObject spawnOnEnter;
        [SerializeField] private UnityEvent onEnterPortal;

        [Space]
        [SerializeField] private bool updateColors = true;
        [SerializeField] private Color color = Color.white;

        /// <summary>
        /// Override this to determine what happens when the player enters this portal
        /// </summary>
        public abstract void OnEnterPortal(PlayerMain player);

        private void OnTriggerEnter(Collider col)
        {
            // Return
            if (cantBeEntered)
            {
                return;
            }

            // Player entered portal
            if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                OnEnterPortal(PlayerMain.Instance);

                // Trigger event
                onEnterPortal?.Invoke();

                // Spawn object
                if (spawnOnEnter != null)
                {
                    GameObject newObj = Instantiate(spawnOnEnter, transform.position, Quaternion.identity);
                    
                    // Change the color of the newly spawned object
                    if (updateColors)
                    {
                        Renderer newObjRend = newObj.GetComponent<Renderer>();

                        if (newObjRend != null)
                        {
                            MaterialColorer.UpdateRendererMaterials(newObjRend, color, true, true);
                        }
                    }
                }

                // Disable if not multi trigger
                if (!multiTrigger)
                {
                    cantBeEntered = true;
                }
            }
        }
    }
}
