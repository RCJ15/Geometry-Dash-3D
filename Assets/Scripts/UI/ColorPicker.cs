using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GD3D.UI
{
    /// <summary>
    /// A color picker that utilizes HSV (Hue, Saturation & Value)
    /// </summary>
    public class ColorPicker : MonoBehaviour
    {
        [SerializeField] private bool setStartingColor = true;
        [SerializeField] private Color startingColor = Color.white;

        [Header("HSV Sliders")]
        [SerializeField] private Slider hSlider;
        [SerializeField] private Slider sSlider;
        [SerializeField] private Slider vSlider;

        [Space]
        [Header("Color Updating")]
        [SerializeField] private Image sBackground;
        [SerializeField] private Image sGradient;

        [SerializeField] private Image vGradient;

        [Header("Event")]
        [SerializeField] private UnityEvent<Color> onUpdateColor;
        public UnityEvent<Color> OnUpdateColor => onUpdateColor;

        //-- Actual Color Variable
        private Color _color;
        public Color GetColor => _color;

        private void Start()
        {
            // Subscribe to events on all sliders
            hSlider.onValueChanged.AddListener((val) => UpdateColor());
            sSlider.onValueChanged.AddListener((val) => UpdateColor());
            vSlider.onValueChanged.AddListener((val) => UpdateColor());

            // Set starting color
            if (setStartingColor && startingColor != _color)
            {
                SetColor(startingColor);
                UpdateColor();
            }
        }

        /// <summary>
        /// Sets the color of this color picker to the new color.
        /// </summary>
        public void SetColor(Color newColor)
        {
            _color = newColor;

            // Set HSV
            Color.RGBToHSV(newColor, out float h, out float s, out float v);

            hSlider.normalizedValue = h;
            sSlider.normalizedValue = s;
            vSlider.normalizedValue = v;

            UpdateColor();
        }

        /// <summary>
        /// Updates the color variable and the appearance of all sliders to reflect the current color this color picker has.
        /// </summary>
        public void UpdateColor()
        {
            // Get HSV
            float h = hSlider.normalizedValue;
            float s = sSlider.normalizedValue;
            float v = vSlider.normalizedValue;

            // Set color
            _color = Color.HSVToRGB(hSlider.normalizedValue, sSlider.normalizedValue, vSlider.normalizedValue);

            // Update appearance
            sBackground.color = Color.HSVToRGB(h, 0, v);
            sGradient.color = Color.HSVToRGB(h, 1, v);

            vGradient.color = Color.HSVToRGB(h, s, 1);

            onUpdateColor?.Invoke(_color);
        }
    }
}
