using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GD3D.Player
{
    public class PlayerColors : PlayerScript, IMaterialColorable
    {
        public readonly static Color DefaultColor1 = new Color(0.4784314f, 0.9686275f, 0.003921569f);
        public readonly static Color DefaultColor2 = new Color(0.01568628f, 0.9529412f, 0.9686275f);

        [Header("Colors")]
        public Color Color1 = DefaultColor1;
        public Color Color2 = DefaultColor2;

        [SerializeField] private MaterialColorData[] materials;

        public Color[] GetColors { get => new Color[] { Color1, Color2 };
            set {
                if (value.Length < 2) return;

                Color1 = value[0];
                Color2 = value[1];
            }
        }
        public Color GetColor { get => Color1; set => Color1 = value; }

        public MaterialColorer.ColorMode GetColorMode => MaterialColorer.ColorMode.array;

        public int GetMaterialIndex => 0;

        public override void Start()
        {
            base.Start();

            UpdateMaterialColors(Color1, Color2);
        }

        /// <summary>
        /// Updates all the materials to have the correct player color
        /// </summary>
        public void UpdateMaterialColors(Color color1, Color color2)
        {
            // Loop through the color data
            foreach (MaterialColorData materialColorData in materials)
            {
                // Get the color depending on the index
                Color color = materialColorData.playerColorIndex == 2 ? color2 : color1;

                // Update color
                MaterialColorer.UpdateMaterialColor(materialColorData.Material, color, materialColorData.ChangeEmission, materialColorData.ChangeSpecular);
            }
        }

        /// <summary>
        /// Data class for which materials to color
        /// </summary>
        [System.Serializable]
        public class MaterialColorData
        {
            public Material Material;
            [Range(1, 2)]
            public int playerColorIndex;

            [Space]
            public bool ChangeEmission;
            public bool ChangeSpecular;
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(PlayerColors))]
        public class PlayerColorsEditor : Editor
        {
            PlayerColors colors;

            public override void OnInspectorGUI()
            {
                // Serialize colors
                Serialize("Color1");
                Serialize("Color2");

                // Space
                GUILayout.Space(3);

                // Color reset button
                if (GUILayout.Button("Reset colors to default"))
                {
                    Undo.RecordObject(this, "Reset colors to default");

                    colors.Color1 = DefaultColor1;
                    colors.Color2 = DefaultColor2;
                }

                // Space
                GUILayout.Space(10);

                // Serialize materials array
                Serialize("materials");

                // Space
                GUILayout.Space(3);

                // Color reset button
                if (GUILayout.Button("Update material colors"))
                {
                    colors.UpdateMaterialColors(colors.Color1, colors.Color2);
                }
            }

            private void Serialize(string name)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(name));
            }

            private void OnEnable()
            {
                // Get target
                colors = (PlayerColors)target;
            }
        }
#endif
    }
}
