using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GD3D
{
    /// <summary>
    /// Takes a renderer and an array of colors to change the renderers material color to the ones in the array. <para/>
    /// Also has options for a single color or copying another MaterialColorer's colors.
    /// </summary>
    public class MaterialColorer : MonoBehaviour, IMaterialColorable
    {
        //-- Constants
        public const string EMISSION_NAME = "_EmissionColor";
        public const string SPECULAR_NAME = "_SpecColor";

        //-- Renderer
        [SerializeField] internal RenderType _renderType;

        [SerializeField] private Renderer _renderer;
        [SerializeField] private ParticleSystem _particles;
        private ParticleSystemRenderer _particlesRenderer;

        //-- Colors
        [SerializeField] internal ColorMode _colorMode;

        [SerializeField] private Color _color = Color.white;
        [SerializeField] private int _materialIndex;

        [SerializeField] private Color[] _colors = new Color[] { Color.white };

        [SerializeField] private GameObject _copyFromObject;
        private IMaterialColorable _copyFrom;

        //-- Update mode
        [SerializeField] private UpdateMode _updateMode;

        //-- Other settings
        [SerializeField] private bool _updateEmmision;
        [SerializeField] private bool _updateSpecular;

        /// <summary>
        /// Finds the correct renderer
        /// </summary>
        /// <returns>The renderer found</returns>
        private Renderer GetRenderer
        {
            get
            {
                switch (_renderType)
                {
                    case RenderType.particleSystem:

                        // Set the particles renderer if it's null
                        if (_particlesRenderer == null)
                        {
                            _particlesRenderer = _particles.GetComponent<ParticleSystemRenderer>();
                        }

                        return _particlesRenderer;

                    default:
                        return _renderer;
                }
            }
        }

        /// <summary>
        /// Returns the correct material index
        /// </summary>
        public int GetMaterialIndex
        {
            get
            {
                // Return this material index if it shouldn't copy from another
                if (_colorMode != ColorMode.copyFromAnother)
                {
                    return _materialIndex;
                }
                // Return the copy froms material index otherwise
                else
                {
                    return _copyFrom.GetMaterialIndex;
                }
            }
        }

        /// <summary>
        /// Returns the correct color mode
        /// </summary>
        public ColorMode GetColorMode
        {
            get
            {
                // Return this color mode if it shouldn't copy from another
                if (_colorMode != ColorMode.copyFromAnother)
                {
                    return _colorMode;
                }
                // Return the copy froms color mode otherwise
                else
                {
                    return _copyFrom.GetColorMode;
                }
            }
        }

        /// <summary>
        /// Returns the correct colors
        /// </summary>
        public Color GetColor
        {
            get
            {
                // Return this color if it shouldn't copy from another
                if (_colorMode != ColorMode.copyFromAnother)
                {
                    return _color;
                }
                // Return the copy froms color otherwise
                else
                {
                    return _copyFrom.GetColor;
                }
            }
            set
            {
                // Set this color if it shouldn't copy from another
                if (_colorMode != ColorMode.copyFromAnother)
                {
                    _color = value;
                }
                // Set the copy froms color otherwise
                else
                {
                    _copyFrom.GetColor = value;
                }
            }
        }

        /// <summary>
        /// Returns the correct colors
        /// </summary>
        public Color[] GetColors
        {
            get
            {
                // Return this colors if it shouldn't copy from another
                if (_colorMode != ColorMode.copyFromAnother)
                {
                    return _colors;
                }
                // Return the copy froms colors otherwise
                else
                {
                    return _copyFrom.GetColors;
                }
            }
            set
            {
                // Set this colors if it shouldn't copy from another
                if (_colorMode != ColorMode.copyFromAnother)
                {
                    _colors = value;
                }
                // Set the copy froms colors otherwise
                else
                {
                    _copyFrom.GetColors = value;
                }
            }
        }

        /// <summary>
        /// Returns the correct renderers materials
        /// </summary>
        private Material[] materials
        {
            get { return GetRenderer.materials; }
            set { GetRenderer.materials = value; }
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded
        /// </summary>
        void Awake()
        {
            // Get the interface from the _copyFromObject
            if (_copyFromObject != null)
            {
                _copyFrom = _copyFromObject.GetComponent<IMaterialColorable>();

                // THROW ERROR >:(
                if (_copyFrom == null)
                {
                    throw new System.Exception($"{_copyFromObject.name} does not have a {nameof(IMaterialColorable)} attached to it");
                }
            }

            // Create a new list of materials
            List<Material> newMaterials = new List<Material>();

            // Change the newly created materials list
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

            // Always update colors on awake so nothing ugly is seen on the first frame
            UpdateColors();
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        private void Update()
        {
            // Update every frame if the correct update mode is set
            if (_updateMode == UpdateMode.everyFrame)
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
            if (_updateMode == UpdateMode.everyFixedFrame)
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
        /// Sets the colors of the renderer using an array of colors
        /// </summary>
        private void SetArrayColor(Color[] colors)
        {
            UpdateRendererMaterials(GetRenderer, colors, _updateEmmision, _updateSpecular);
        }

        /// <summary>
        /// Sets the colors of the renderer using a single color
        /// </summary>
        private void SetSingleColor(Color color)
        {
            UpdateMaterialColor(materials[_materialIndex], color, _updateEmmision, _updateSpecular);
        }

        /// <summary>
        /// Updates the given materials color to match the given color
        /// </summary>
        public static void UpdateMaterialColor(Material mat, Color color, bool updateEmission, bool updateSpecular)
        {
            // Set the color
            mat.color = color;

            // Set the emission color if updateEmmision is true
            if (updateEmission && mat.HasProperty(EMISSION_NAME))
            {
                mat.SetColor(EMISSION_NAME, color);
            }

            // Set the specular color if updateSpecular is true
            if (updateSpecular && mat.HasProperty(SPECULAR_NAME))
            {
                mat.SetColor(SPECULAR_NAME, color);
            }
        }

        /// <summary>
        /// Updates each of the given renderers materials to match the given color
        /// </summary>
        public static void UpdateRendererMaterials(Renderer renderer, Color color, bool updateEmission, bool updateSpecular)
        {
            // Loop through the amount of materials the mesh has
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                // Update each material to have the correct color
                UpdateMaterialColor(renderer.materials[i], color, true, true);
            }
        }

        /// <summary>
        /// Updates each of the given renderers materials to match the correct color in the given color array
        /// </summary>
        public static void UpdateRendererMaterials(Renderer renderer, Color[] colors, bool updateEmission, bool updateSpecular)
        {
            // Loop through the length of either colors or the matrials
            // The one that's the smallest in length will be used
            for (int i = 0; i < Mathf.Min(colors.Length, renderer.materials.Length); i++)
            {
                // Update each material to have the correct color
                UpdateMaterialColor(renderer.materials[i], colors[i], updateEmission, updateSpecular);
            }
        }

        /// <summary>
        /// Enum for what update mode a material colorer can have
        /// </summary>
        [System.Serializable]
        public enum RenderType
        {
            normal = 0,
            particleSystem = 1,
        }

        /// <summary>
        /// Enum for what color mode a material colorer can have
        /// </summary>
        [System.Serializable]
        public enum ColorMode
        {
            single = 0,
            array = 1,
            copyFromAnother = 2,
        }

        /// <summary>
        /// Enum for what update mode a material colorer can have
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

            Serialize("_renderType");

            switch (materialColorer._renderType)
            {
                case MaterialColorer.RenderType.particleSystem:
                    Serialize("_particles");
                    break;

                default:
                    Serialize("_renderer");
                    break;
            }

            //-- Colors
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Colors", EditorStyles.boldLabel);

            Serialize("_colorMode");

            switch (materialColorer._colorMode)
            {
                case MaterialColorer.ColorMode.single:
                    Serialize("_color");
                    Serialize("_materialIndex");
                    break;
                case MaterialColorer.ColorMode.array:
                    Serialize("_colors");
                    break;
                case MaterialColorer.ColorMode.copyFromAnother:
                    Serialize("_copyFromObject");
                    break;
            }

            //-- Update mode
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Update mode", EditorStyles.boldLabel);

            Serialize("_updateMode");

            //-- Other settings
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Other settings", EditorStyles.boldLabel);

            Serialize("_updateEmmision");
            Serialize("_updateSpecular");

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
}