using GD3D.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GD3D.Level
{
    /// <summary>
    /// Contains all the information about the level colors, like background or ground colors for example
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
        private Dictionary<ColorType, Coroutine> _activeColorChangeCoroutines = new Dictionary<ColorType, Coroutine>();

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
                _activeColorChangeCoroutines.Add(type, null);

                // Change the materials to the start color
                ChangeColor(type, GetStartColor(type));
            }
        }

        private void Start()
        {
            // Get the player instance
            player = PlayerMain.Instance;

            // Subcribe to events
            player.OnDeath += OnDeath;
            player.OnRespawn += ResetColors;
        }

        #region Scene Unloaded Schenanigans
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

        private void OnDeath()
        {
            // Make all the coroutines stop
            foreach (var pair in _activeColorChangeCoroutines)
            {
                // Get coroutine
                Coroutine coroutine = pair.Value;

                // If it isn't null, stop it
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
            }
        }

        /// <summary>
        /// Changes all the the colors and materials to their starting color
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
        /// Will linearly change the color of the given color <paramref name="type"/> to the given <paramref name="color"/> over the given <paramref name="time"/>. <para/>
        /// Example: a type of <see cref="ColorType.background"/> will change the background color
        /// </summary>
        public static void ChangeColorOverTime(ColorType type, Color color, float time)
        {
            // Shortcut for instance cuz I'm lazy
            LevelColors I = Instance;

            // Throw error if the given color type doesn't exist
            if (!I._colorDataDictionary.ContainsKey(type))
            {
                throw new Exception($"The color type of \"{type}\" does not exist.");
            }

            // Stop the currently active coroutine
            Coroutine currentCoroutine = I._activeColorChangeCoroutines[type];
            if (currentCoroutine != null)
            {
                I.StopCoroutine(currentCoroutine);
            }

            // Start the new coroutine
            Coroutine coroutine = I.StartCoroutine(I.ChangeColorOverTimeCoroutine(type, color, time));
            I._activeColorChangeCoroutines[type] = coroutine;
        }

        /// <summary>
        /// Will linearly change the color of the given color <paramref name="type"/> to the given <paramref name="color"/> over the given <paramref name="time"/>. <para/>
        /// Example: a type of <see cref="ColorType.background"/> will change the background color
        /// </summary>
        private IEnumerator ChangeColorOverTimeCoroutine(ColorType type, Color color, float time)
        {
            Color startColor = GetStartColor(type);

            float currentTimer = time;

            // Pseudo FIXED update method
            while (currentTimer > 0)
            {
                float t = currentTimer / time;

                // Change the color
                Color targetColor = Color.Lerp(color, startColor, t);
                ChangeColor(type, targetColor);

                // Wait for next frame
                currentTimer -= Time.fixedDeltaTime;

                yield return new WaitForFixedUpdate();
            }

            // Set to the given color so it's 100% the correct color when this runs out
            ChangeColor(type, color);
        }

        /// <summary>
        /// Instantly changes the given color <paramref name="type"/> to the given <paramref name="color"/>. <para/>
        /// Example: a type of <see cref="ColorType.background"/> will change the background color
        /// </summary>
        public static void ChangeColor(ColorType type, Color color)
        {
            // Get the color data from the dictionary
            MaterialColorData colorData = GetColorData(type);
            colorData.Color = color;

            // Fog is a special case where RenderSettings.fogColor is changed too
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
        /// Example: a type of <see cref="ColorType.background"/> will give you the backgrounds current color
        /// </summary>
        public static Color GetCurrentColor(ColorType type)
        {
            return GetColorData(type).Color;
        }

        /// <summary>
        /// Returns the start color that the given color <paramref name="type"/> has. <para/>
        /// Example: a type of <see cref="ColorType.background"/> will give you the backgrounds start color
        /// </summary>
        public static Color GetStartColor(ColorType type)
        {
            return GetColorData(type).StartColor;
        }

        /// <summary>
        /// Returns the color data that the given color <paramref name="type"/> has. <para/>
        /// Example: a type of <see cref="ColorType.background"/> will give you the backgrounds color data
        /// </summary>
        public static MaterialColorData GetColorData(ColorType type)
        {
            // Throw error if the given color type doesn't exist
            if (!Instance._colorDataDictionary.ContainsKey(type))
            {
                throw new Exception($"The color type of \"{type}\" does not exist.");
            }

            // Return the color data
            return Instance._colorDataDictionary[type];
        }

        /// <summary>
        /// Holds information about what type of color type the color data is
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
        /// Holds information about what materials that should be changed on the renderer
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
        /// An enum used to determine what kind of color the color is
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
