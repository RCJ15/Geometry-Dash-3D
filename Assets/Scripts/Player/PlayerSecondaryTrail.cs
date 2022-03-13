using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.ObjectPooling;

namespace GD3D.Player
{
    /// <summary>
    /// The secondary trail is a special trail that spawns copies of the players model. See <see cref="PlayerSecondaryTrailManager"/>
    /// </summary>
    public class PlayerSecondaryTrail : PoolObject
    {
        [SerializeField] private Material regularMat;
        [SerializeField] private Material invisibleMat;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private MaterialColorer _materialColorer;

        public override void OnCreated()
        {
            base.OnCreated();

            // Get components
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _materialColorer = GetComponent<MaterialColorer>();
        }

        /// <summary>
        /// Updates the mesh filer on this trail to use the given <paramref name="mesh"/>
        /// </summary>
        public void UpdateMesh(Mesh mesh, int mainMatIndex)
        {
            // Change mesh only if it's different
            if (_meshFilter.mesh != mesh)
            {
                _meshFilter.mesh = mesh;

                // Expand or shrink material size if the materials or index are different
                if (mesh.subMeshCount != _meshRenderer.materials.Length || mainMatIndex != _materialColorer.GetMaterialIndex)
                {
                    // Create a list of materials that we will fill later
                    List<Material> materials = new List<Material>();

                    for (int i = 0; i < mesh.subMeshCount; i++)
                    {
                        // Add regular material if this is the mainMatIndex
                        if (i == mainMatIndex)
                        {
                            materials.Add(regularMat);
                        }
                        // Add invisible material
                        else
                        {
                            materials.Add(invisibleMat);
                        }
                    }

                    // Set materials
                    _meshRenderer.materials = materials.ToArray();
                }

                // Set material index
                _materialColorer.SetMaterialIndex = mainMatIndex;
                _materialColorer.UpdateColors();
            }
        }
    }
}
