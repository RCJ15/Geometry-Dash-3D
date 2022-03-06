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
        private bool _cantBeEntered;

        [Space]
        [SerializeField] private GameObject spawnOnEnter;
        [SerializeField] private UnityEvent onEnterPortal;

        [Header("Portal Color")]
        [SerializeField] private bool updateColors = true;
        [SerializeField] protected Color color = Color.white;

        [Space]
        [SerializeField] protected MeshRenderer mesh;
        [SerializeField] private int materialIndex;

        protected PlayerMain _player;

        public virtual void Start()
        {
            _player = PlayerMain.Instance;

            // Update the mesh color to match the portal color
            if (updateColors && mesh != null)
            {
                // Clone the material at the correct index
                Material newMat = new Material(mesh.materials[materialIndex]);

                // Update color
                MaterialColorer.UpdateMaterialColor(newMat, color, true, true);

                // Re-apply the newly colored material
                // Have to this weird thing because "mesh.materials[materialIndex] = newMat;" doesn't work for some reason
                List<Material> materialList = new List<Material>();

                // Loop through the materials
                for (int i = 0; i < mesh.materials.Length; i++)
                {
                    // Add the new material if the index is the same as the material index
                    if ( i == materialIndex)
                    {
                        materialList.Add(newMat);
                    }
                    // Otherwise add regular materials to the list
                    else
                    {
                        materialList.Add(mesh.materials[i]);
                    }
                }

                // Set the materials
                mesh.materials = materialList.ToArray();
            }
        }

        /// <summary>
        /// Implement this to determine what happens when the player enters this portal
        /// </summary>
        public abstract void OnEnterPortal();

        /// <summary>
        /// Override this to determine a custom portal condition that has to be met in order for the player to enter the portal
        /// </summary>
        public virtual bool CustomPortalCondition()
        {
            return true;
        }

        public virtual void OnTriggerEnter(Collider col)
        {
            // Return if the portal can't be entered or the custom condition is false
            if (_cantBeEntered || !CustomPortalCondition())
            {
                return;
            }

            // Player entered portal
            if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                OnEnterPortal();

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
                    _cantBeEntered = true;
                }
            }
        }
    }
}
