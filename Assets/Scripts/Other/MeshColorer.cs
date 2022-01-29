using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Takes a mesh renderer and an array of colors to change the mesh material color to the one in the array.
/// </summary>
public class MeshColorer : MonoBehaviour
{
    [Header("Main Stuff")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Color[] colors = new Color[] { Color.white };

    [Header("Other Stuff")]
    [SerializeField] private bool updateOnAwake = true;
    [SerializeField] private bool updateEmmision;


    /// <summary>
    /// Awake is called when the script instance is being loaded
    /// </summary>
    void Awake()
    {
        // Update the colors on awake if updateOnAwake is true
        if (updateOnAwake)
        {
            UpdateColors();
        }
    }

    /// <summary>
    /// Updates the mesh renderers materials to have the same colors as the colors array.
    /// </summary>
    public void UpdateColors()
    {
        // Create a new list of materials
        List<Material> newMaterials = new List<Material>();

        // Loop through all existing materials
        for (int i = 0; i < Mathf.Clamp(colors.Length, 0, meshRenderer.materials.Length); i++)
        {
            Material mat = meshRenderer.materials[i];

            // Create a new material that copies the orignal material
            Material newMat = new Material(mat);

            // Set the color
            newMat.color = colors[i];

            // Set the emission color if updateEmmision is true
            if (updateEmmision)
            {
                newMat.SetColor("_EmissionColor", colors[i]);
            }

            // Add the material to the list
            newMaterials.Add(newMat);
        }

        // If the new materials list is shorter than the old list,
        // then add the old materials to the list
        if (newMaterials.Count < meshRenderer.materials.Length)
        {
            for (int i = newMaterials.Count; i < meshRenderer.materials.Length; i++)
            {
                newMaterials.Add(meshRenderer.materials[i]);
            }
        }

        // Set the new materials
        meshRenderer.materials = newMaterials.ToArray();
    }
}
