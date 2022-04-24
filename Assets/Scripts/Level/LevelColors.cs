using GD3D.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GD3D.Easing;

namespace GD3D.Level
{
    /// <summary>
    /// Contains all the information about the level colors, like background or ground colors for example.
    /// </summary>
    public class LevelColors : MonoBehaviour
    {
        //-- Constants (Static + Readonly = basically constants)
        public static readonly Color DEFAULT_BACKGROUND_COLOR = new Color(0.1568628f, 0.4901961f, 1);   // 287DFF in hexadecimal
        public static readonly Color DEFAULT_GROUND_COLOR = new Color(0, 0.4f, 1);                      // 0066FF in hexadecimal
        public static readonly Color DEFAULT_FOG_COLOR = new Color(0.2745098f, 0.5137255f, 0.8862745f); // 4683E2 in hexadecimal

        //-- Instance
        public static LevelColors Instance;

        //-- Dictionaries
        private Dictionary<ColorType, MaterialColorData> _colorDataDictionary = new Dictionary<ColorType, MaterialColorData>();
        private Dictionary<ColorType, long> _colorDataEasings = new Dictionary<ColorType, long>();

        //-- Color Data
        [SerializeField] private MaterialColorTypeData[] colorData;

        private PlayerMain player;

        private void Awake()
        {
            // Set the instance
            Instance = this;

            // Update the material colors to be the starting colors
            foreach (MaterialColorTypeData colorTypeData in colorData)
            {
                // Store variables
                ColorType type = colorTypeData.Type;
                MaterialColorData colorData = colorTypeData.ColorData;

                // Add to dictionaries
                _colorDataDictionary.Add(type, colorData);

                // Change the materials to the start color
                ChangeColor(type, GetStartColor(type));
            }
        }

        private void Start()
        {
            // Get the player instance
            player = PlayerMain.Instance;

            // Subcribe to events
            player.OnRespawn += (a, b) => StopAllEasings();
            player.OnRespawn += OnRespawn;
            EasingManager.Instance.OnEaseObjectRemove += OnEaseObjectRemove;
        }

        #region Scene Unloaded
        private void OnEnable()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnSceneUnloaded(Scene scene)
        {
            // Reset colors when the scene unloads
            ResetColors();
        }
        #endregion

        #region Saving & loading from checkpoint
        /// <summary>
        /// Creates a new dictionary that contains the current colors for every color type.
        /// </summary>
        /// <returns>The newly created dictionary.</returns>
        public Dictionary<ColorType, Color> Save()
        {
            // Create a new dictionary
            Dictionary<ColorType, Color> colorDictionary = new Dictionary<ColorType, Color>();

            // Loop through all the current color data
            foreach (var pair in _colorDataDictionary)
            {
                // Add to dictionary
                colorDictionary.Add(pair.Key, pair.Value.Color);
            }

            // Return the dictionary
            return colorDictionary;
        }

        private void OnRespawn(bool inPracticeMode, Checkpoint checkpoint)
        {
            // Reset colors if not in practice mode
            if (!inPracticeMode)
            {
                ResetColors();
            }
            else
            {
                // Loop through the checkpoint color data
                foreach (var pair in checkpoint.LevelColorData)
                {
                    // Set data accordingly
                    ChangeColor(pair.Key, pair.Value);
                }
            }
        }
        #endregion

        /// <summary>
        /// Called when a <see cref="EaseObject"/> is removed.
        /// </summary>
        private void OnEaseObjectRemove(long id)
        {
            // Create a nullable type which will be set to null by default
            ColorType? typeToRemove = null;

            // Loop through the easings
            foreach (var pair in _colorDataEasings)
            {
                // Check if the id matches the value
                if (pair.Value == id)
                {
                    // Set the type to the key
                    typeToRemove = pair.Key;
                    break;
                }
            }

            // Remove the value in the dictionary if the nullable key is not null
            if (typeToRemove.HasValue)
            {
                _colorDataEasings.Remove(typeToRemove.Value);
            }
        }

        /// <summary>
        /// Stops all the current active color easings.
        /// </summary>
        public static void StopAllEasings()
        {
            Queue<long> easeObjectsToRemove = new Queue<long>();

            // Make all the easings stop
            foreach (var pair in Instance._colorDataEasings)
            {
                // Get the ease object id
                long id = pair.Value;

                // Add to queue
                easeObjectsToRemove.Enqueue(id);
            }

            for (; easeObjectsToRemove.Count > 0;)
            {
                // Cancel the current ease using try remove
                EasingManager.TryRemoveEaseObject(easeObjectsToRemove.Dequeue());
            }
        }

        /// <summary>
        /// Changes all the the colors and materials to their starting color.
        /// </summary>
        public static void ResetColors()
        {
            foreach (var pair in Instance._colorDataDictionary)
            {
                // Change the materials to the regular color
                ChangeColor(pair.Key, pair.Value.StartColor);
            }
        }

        /// <summary>
        /// Will add and set the given easing <paramref name="obj"/> to change the given color <paramref name="type"/> to the given <paramref name="color"/>. <para/>
        /// Will remove and replace the old easing if one already exists for the given color <paramref name="type"/>.
        /// Example: a type of <see cref="ColorType.background"/> will change the background color over time.
        /// </summary>
        public static void AddEase(ColorType type, Color color, EaseObject obj)
        {
            // Get the current color
            Color currentColor = GetCurrentColor(type);

            // Set the easing on update method to change color over time
            obj.OnUpdate = (obj) =>
            {
                Color newColor = obj.EaseColor(currentColor, color);

                ChangeColor(type, newColor);
            };

            // Check if an easing does NOT exist for the color type
            if (!Instance._colorDataEasings.ContainsKey(type))
            {
                // Add the new easing to the dictionary
                Instance._colorDataEasings.Add(type, obj.ID);
            }
            else
            {
                long oldId = Instance._colorDataEasings[type];
                
                // Replace the old easing in the dictionary
                Instance._colorDataEasings[type] = obj.ID;

                // Remove the old easing in the dictionary
                EasingManager.RemoveEaseObject(oldId);
            }
        }

        /// <summary>
        /// Instantly changes the given color <paramref name="type"/> to the given <paramref name="color"/>. <para/>
        /// Example: a type of <see cref="ColorType.background"/> will change the background color.
        /// </summary>
        public static void ChangeColor(ColorType type, Color color)
        {
            // Get the color data from the dictionary
            MaterialColorData colorData = GetColorData(type);
            colorData.Color = color;

            // Fog is a special case where the actual fog color is changed too
            if (type == ColorType.fog)
            {
                RenderSettings.fogColor = color;
            }

            // Change all the materials colors
            foreach (RendererMaterialData matData in colorData.RenderMaterialData)
            {
                // Modify all the materials on the renderer
                if (matData.ModifyAllMaterials)
                {
                    MaterialColorer.UpdateRendererMaterials(matData.Renderer, color, true, true);
                }
                // Modify a single material
                else
                {
                    Material mat = matData.Renderer.materials[matData.MaterialIndex];
                    MaterialColorer.UpdateMaterialColor(mat, color, true, true);
                }
            }
        }

        /// <summary>
        /// Returns the current color that the given color <paramref name="type"/> has. <para/>
        /// Example: a type of <see cref="ColorType.background"/> will give you the backgrounds current color.
        /// </summary>
        public static Color GetCurrentColor(ColorType type)
        {
            return GetColorData(type).Color;
        }

        /// <summary>
        /// Returns the start color that the given color <paramref name="type"/> has. <para/>
        /// Example: a type of <see cref="ColorType.background"/> will give you the backgrounds start color.
        /// </summary>
        public static Color GetStartColor(ColorType type)
        {
            return GetColorData(type).StartColor;
        }

        /// <summary>
        /// Returns the color data that the given color <paramref name="type"/> has. <para/>
        /// Example: a type of <see cref="ColorType.background"/> will give you the backgrounds color data.
        /// </summary>
        public static MaterialColorData GetColorData(ColorType type)
        {
            // Throw error if the given color type doesn't exist
            if (!Instance._colorDataDictionary.ContainsKey(type))
            {
                throw new NullReferenceException($"The color type of \"{type}\" does not exist.");
            }

            // Return the color data
            return Instance._colorDataDictionary[type];
        }

        /// <summary>
        /// Holds information about what type of color type the color data is.
        /// </summary>
        [Serializable]
        public class MaterialColorTypeData
        {
            public ColorType Type;
            public MaterialColorData ColorData;
        }

        /// <summary>
        /// Holds information about what color the material should be colored
        /// </summary>
        [Serializable]
        public class MaterialColorData
        {
            public Color StartColor = Color.white;
            public Color Color = Color.white;

            [Space]
            public List<RendererMaterialData> RenderMaterialData = new List<RendererMaterialData>();
        }

        /// <summary>
        /// Holds information about what materials that should be changed on the renderer.
        /// </summary>
        [Serializable]
        public class RendererMaterialData
        {
            public Renderer Renderer;
            public int MaterialIndex;
            public bool ModifyAllMaterials;

            public RendererMaterialData(Renderer renderer, int materialIndex, bool modifyAllMaterials = false)
            {
                Renderer = renderer;
                MaterialIndex = materialIndex;
                ModifyAllMaterials = modifyAllMaterials;
            }
        }

        /// <summary>
        /// An enum used to determine what kind of color the color is.
        /// </summary>
        [Serializable]
        public enum ColorType
        {
            background = 0,
            ground = 1,
            fog = 2,
        }
    }
}
