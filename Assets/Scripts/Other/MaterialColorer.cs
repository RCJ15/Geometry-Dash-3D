using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Takes a renderer and an array of colors to change the renderers material color to the ones in the array.
/// </summary>
public class MaterialColorer : MonoBehaviour
{
    //-- Renderer
    [SerializeField] internal RenderType renderType;

    [SerializeField] private Renderer rend;
    [SerializeField] private ParticleSystem particles;
    private ParticleSystemRenderer particlesRenderer;

    //-- Colors
    [SerializeField] internal ColorMode colorMode;

    public Color color = Color.white;
    [SerializeField] private int materialIndex;

    public Color[] colors = new Color[] { Color.white };

    [SerializeField] private MaterialColorer copyFrom;

    //-- Update mode
    [SerializeField] private UpdateMode updateMode;

    //-- Other settings
    [SerializeField] private bool updateEmmision;

    /// <summary>
    /// Finds the correct renderer
    /// </summary>
    /// <returns>The renderer found</returns>
    private Renderer GetRenderer
    {
        get
        {
            switch (renderType)
            {
                case RenderType.particleSystem:

                    // Set the particles renderer if it's null
                    if (particlesRenderer == null)
                    {
                        particlesRenderer = particles.GetComponent<ParticleSystemRenderer>();
                    }

                    return particlesRenderer;

                default:
                    return rend;
            }
        }
    }

    /// <summary>
    /// Returns the correct material index
    /// </summary>
    private int GetMaterialIndex
    {
        get
        {
            // Return this material index if it shouldn't copy from another
            if (colorMode != ColorMode.copyFromAnother)
            {
                return materialIndex;
            }
            // Return the copy froms material index otherwise
            else
            {
                return copyFrom.GetMaterialIndex;
            }
        }
    }

    /// <summary>
    /// Returns the correct color mode
    /// </summary>
    private ColorMode GetColorMode
    {
        get
        {
            // Return this color mode if it shouldn't copy from another
            if (colorMode != ColorMode.copyFromAnother)
            {
                return colorMode;
            }
            // Return the copy froms color mode otherwise
            else
            {
                return copyFrom.GetColorMode;
            }
        }
    }
    
    /// <summary>
    /// Returns the correct colors
    /// </summary>
    private Color GetColor
    {
        get
        {
            // Return this color if it shouldn't copy from another
            if (colorMode != ColorMode.copyFromAnother)
            {
                return color;
            }
            // Return the copy froms color otherwise
            else
            {
                return copyFrom.GetColor;
            }
        }
    }

    /// <summary>
    /// Returns the correct colors
    /// </summary>
    private Color[] GetColors
    {
        get
        {
            // Return this colors if it shouldn't copy from another
            if (colorMode != ColorMode.copyFromAnother)
            {
                return colors;
            }
            // Return the copy froms colors otherwise
            else
            {
                return copyFrom.GetColors;
            }
        }
    }

    /// <summary>
    /// Returns the correct renderers materials
    /// </summary>
    private Material[] materials {
        get { return GetRenderer.materials; }
        set { GetRenderer.materials = value; }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded
    /// </summary>
    void Awake()
    {
        // Create a new list of materials
        List<Material> newMaterials = new List<Material>();

        // Change a list of materials
        if (GetColorMode == ColorMode.array)
        {
            // Loop through all existing materials
            for (int i = 0; i < Mathf.Clamp(GetColors.Length, 0, materials.Length); i++)
            {
                Material mat = materials[i];

                // Create a new material that copies the orignal material
                Material newMat = new Material(mat);

                // Add the material to the list
                newMaterials.Add(newMat);
            }

            // If the new materials list is shorter than the old list,
            // then add the old materials to the list
            if (newMaterials.Count < materials.Length)
            {
                for (int i = newMaterials.Count; i < materials.Length; i++)
                {
                    newMaterials.Add(materials[i]);
                }
            }
        }

        // Change one of the materials to be new
        // The material that is changed is controlled by the material index
        if (GetColorMode == ColorMode.single)
        {
            // Loop through all the materials
            for (int i = 0; i < materials.Length; i++)
            {
                Material mat = materials[i];

                // If the I matches the material index, then create a new material
                if (i == GetMaterialIndex)
                {
                    // Create a new material that copies the orignal material
                    Material newMat = new Material(mat);

                    // Add the material to the list
                    newMaterials.Add(newMat);
                }
                // Otherwise just add the material to the list
                else
                {
                    newMaterials.Add(mat);
                }
            }
        }

        // Set the new materials
        materials = newMaterials.ToArray();

        // Update the colors on awake if updateOnAwake is true
        UpdateColors();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        // Update every frame if the correct update mode is set
        if (updateMode == UpdateMode.everyFrame)
        {
            UpdateColors();
        }
    }

    /// <summary>
    /// Fixed Update is called once per physics frame
    /// </summary>
    private void FixedUpdate()
    {
        // Update every fixed frame if the correct update mode is set
        if (updateMode == UpdateMode.everyFixedFrame)
        {
            UpdateColors();
        }
    }

    /// <summary>
    /// Updates the mesh renderers materials to have the same colors as the colors array.
    /// </summary>
    public void UpdateColors()
    {
        // Change an entire list of material colors
        if (GetColorMode == ColorMode.array)
        {
            SetArrayColor(GetColors);

            return;
        }

        // Color only a single material in the list
        // The material is determined by the material index
        if (GetColorMode == ColorMode.single)
        {
            SetSingleColor(GetColor);

            return;
        }
    }

    /// <summary>
    /// Sets the color using an array
    /// </summary>
    private void SetArrayColor(Color[] colors)
    {
        // Loop through the amount of colors
        for (int i = 0; i < Mathf.Min(colors.Length, materials.Length); i++)
        {
            Material mat = materials[i];

            // Set the color
            mat.color = colors[i];

            // Set the emission color if updateEmmision is true
            if (updateEmmision)
            {
                mat.SetColor("_EmissionColor", colors[i]);
            }
        }
    }

    /// <summary>
    /// Sets the color using an array
    /// </summary>
    private void SetSingleColor(Color color)
    {
        // Get material
        Material mat = materials[materialIndex];

        // Set the color
        mat.color = color;

        // Set the emission color if updateEmmision is true
        if (updateEmmision)
        {
            mat.SetColor("_EmissionColor", color);
        }
    }

    /// <summary>
    /// Returns the current color array
    /// </summary>
    private Color[] GetArrayColor()
    {
        // Return normal colors
        if (colorMode != ColorMode.copyFromAnother)
        {
            return colors;
        }
        // Return copy from colors
        else
        {
            return copyFrom.colors;
        }
    }

    /// <summary>
    /// Returns the current single color
    /// </summary>
    private Color GetSingleColor()
    {
        // Return normal color
        if (colorMode != ColorMode.copyFromAnother)
        {
            return color;
        }
        // Return copy from color
        else
        {
            return copyFrom.color;
        }
    }

    /// <summary>
    /// Enum for what update mode a mesh colorer can have
    /// </summary>
    [System.Serializable]
    public enum RenderType
    {
        normal = 0,
        particleSystem = 1,
    }

    /// <summary>
    /// Enum for what color mode a mesh colorer can have
    /// </summary>
    [System.Serializable]
    public enum ColorMode
    {
        single = 0,
        array = 1,
        copyFromAnother = 2,
    }

    /// <summary>
    /// Enum for what update mode a mesh colorer can have
    /// </summary>
    [System.Serializable]
    public enum UpdateMode
    {
        onAwake = 0,
        everyFrame = 1,
        everyFixedFrame = 2,
    }
}

#if UNITY_EDITOR

/// <summary>
/// The custom editor for the Material Colorer.
/// </summary>
[CustomEditor(typeof(MaterialColorer))]
public class MaterialColorerEditor : Editor
{
    private MaterialColorer materialColorer;

    public override void OnInspectorGUI()
    {
        // Update the object
        serializedObject.Update();

        //-- Renderer
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Renderer", EditorStyles.boldLabel);

        Serialize("renderType");

        switch (materialColorer.renderType)
        {
            case MaterialColorer.RenderType.particleSystem:
                Serialize("particles");
                break;

            default:
                Serialize("rend");
                break;
        }

        //-- Colors
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Colors", EditorStyles.boldLabel);

        Serialize("colorMode");

        switch (materialColorer.colorMode)
        {
            case MaterialColorer.ColorMode.single:
                Serialize("color");
                Serialize("materialIndex");
                break;
            case MaterialColorer.ColorMode.array:
                Serialize("colors");
                break;
            case MaterialColorer.ColorMode.copyFromAnother:
                Serialize("copyFrom");
                break;
        }

        //-- Update mode
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Update mode", EditorStyles.boldLabel);

        Serialize("updateMode");

        //-- Other settings
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Other settings", EditorStyles.boldLabel);

        Serialize("updateEmmision");

        // Apply modified properties
        serializedObject.ApplyModifiedProperties();
    }

    private void Serialize(string name)
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty(name));
    }

    // Called when the object gets enabled
    private void OnEnable()
    {
        // Get the MaterialColorer component of this object
        materialColorer = (MaterialColorer)target;
    }
}
#endif
