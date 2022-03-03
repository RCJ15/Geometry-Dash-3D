using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.Player
{
    public class PlayerColors : PlayerScript, IMaterialColorable
    {
        // Default green & blue colors
        public Color playerColor1 = new Color(0.4784314f, 0.9686275f, 0.003921569f);
        public Color playerColor2 = new Color(0.01568628f, 0.9529412f, 0.9686275f);

        public Color[] GetColors { get => new Color[] { playerColor1, playerColor2 };
            set {
                if (value.Length < 2) return;

                playerColor1 = value[0];
                playerColor2 = value[1];
            }
        }
        public Color GetColor { get => playerColor1; set => playerColor1 = value; }

        public MaterialColorer.ColorMode GetColorMode => MaterialColorer.ColorMode.array;

        public int GetMaterialIndex => 0;

        public void ColorRenderer(Renderer renderer, int playerColorIndex)
        {
            Color color = GetColors[playerColorIndex];
        }
    }
}
