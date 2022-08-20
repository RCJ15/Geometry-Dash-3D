using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GD3D.UI;
using GD3D.Level;
using TMPro;

namespace GD3D.LevelEditor
{
    //-- Easier access :)
    using ColorType = LevelColors.ColorType;

    /// <summary>
    /// The color picker in the level editor.
    /// </summary>
    public class LevelEditorColorPicker : LevelEditorWindow<LevelEditorColorPicker>
    {
        //-- Singleton Instance
        private static Color? _copiedColor = null; // Nullable as we could have nothing :(
        private static readonly Color DEFAULT_COLOR = new Color(1, 1, 1, 1); // White with full opacity :D

        [Header("Color")]
        [SerializeField] private Image previousColor;
        [SerializeField] private ColorPicker colorPicker;

        [Header("Fade Time")]
        [SerializeField] private GameObject fadeTimeParent;
        [SerializeField] private TMP_InputField fadeTimeInputField;
        [SerializeField] private Slider fadeTimeSlider;
        private float _fadeTimeValue;

        [Header("Opacity")]
        [SerializeField] private TMP_Text opacityText;
        [SerializeField] private Slider opacitySlider;

        [Header("Color ID")]
        [SerializeField] private GameObject colorIDObj;
        [SerializeField] private TMP_Text colorIDText;

        private ColorType _currentColorID;
        private static readonly ColorType[] _allColorIDs = (ColorType[])Enum.GetValues(typeof(ColorType));
        private int _colorIDIndex
        {
            get => (int)_currentColorID;
            set => _currentColorID = (ColorType)value;
        }

        [Header("Toggle")]
        [SerializeField] private Toggle touchTriggerToggle;

        private Action<Color> _currentFinishAction;
        private Action<Color, float, bool, ColorType> _currentColorTriggerAction;

        #region Public Methods
        public void UpdateOpacityText(float val)
        {
            string text = val.ToString();

            int length = text.Length;

            if (length > 4)
            {
                text = text.Substring(0, 4);
            }
            else if (length < 4)
            {
                if (length <= 1)
                {
                    text += '.';
                }

                while (text.Length < 4)
                {
                    text += '0';
                }
            }

            // This whole process just makes the text always display 2 decimals and ONLY 2 decimal numbers
            opacityText.text = text;
        }

        public void ChangeColorID(int amount)
        {
            if (amount != 0)
            {
                int index = _colorIDIndex + amount;

                // Loop value around
                index = Helpers.LoopValue(index, 0, _allColorIDs.Length - 1);

                _colorIDIndex = index;
            }

            // Default to BG
            string text = "BG";

            switch (_currentColorID)
            {
                case ColorType.ground:
                    text = "G1";
                    break;

                case ColorType.fog:
                    text = "Fog";
                    break;
            }

            // Update text
            colorIDText.text = text;
        }

        public void UpdateFadeTimeInputField(string text)
        {
            // Return if parse fails
            if (!float.TryParse(text, out float val))
            {
                return;
            }

            // Update fade time on slider
            fadeTimeSlider.SetValueWithoutNotify(val);
            _fadeTimeValue = val;
        }

        public void UpdateFadeTime(float val)
        {
            // Set everything here without notify as to prevent an indefinite loop of infinity which would cause the destruction of the universe
            fadeTimeSlider.SetValueWithoutNotify(val);

            float roundedVal = Mathf.Round(val * 100) / 100;

            fadeTimeInputField.SetTextWithoutNotify(roundedVal.ToString());

            _fadeTimeValue = val;
        }

        public void CopyColor()
        {
            Color col = colorPicker.GetColor;
            col.a = opacitySlider.normalizedValue;

            _copiedColor = col;
        }

        public void PasteColor()
        {
            if (!_copiedColor.HasValue)
            {
                return;
            }

            SetColor(_copiedColor.Value);
        }

        public void SetToDefaultColor()
        {
            Color col = DEFAULT_COLOR;

            SetColor(col);
        }

        public void SetColor(Color col)
        {
            colorPicker.SetColor(col);

            opacitySlider.SetValueWithoutNotify(col.a);
            UpdateOpacityText(col.a);
        }

        public override void LocalClose()
        {
            base.LocalClose();

            Color col = colorPicker.GetColor;

            col.a = opacitySlider.normalizedValue;

            // Invoke finish actions
            _currentFinishAction?.Invoke(col);
            _currentColorTriggerAction?.Invoke(col, _fadeTimeValue, touchTriggerToggle.isOn, _currentColorID);

            // Set actions to null so they won't be called again on accident
            _currentFinishAction = null;
            _currentColorTriggerAction = null;
        }
        #endregion

        public static void SetData(Color startColor, Action<Color> onFinish)
        {
            Instance.LocalSetData(startColor, onFinish);
        }

        public static void SetData(Color startColor, float fadeTime, ColorType colorID, bool touchTriggered, Action<Color, float, bool, ColorType> onFinish)
        {
            Instance.LocalSetData(startColor, fadeTime, colorID, touchTriggered, onFinish);
        }

        private void LocalSetData(Color startColor, Action<Color> onFinish)
        {
            Color noA = startColor;
            noA.a = 1; // No A actually means 1 A (?)

            // Update previous color and color picker
            previousColor.color = noA;
            colorPicker.SetColor(noA);

            // Update opacity slider
            opacitySlider.normalizedValue = startColor.a;

            // Hide fade time and color ID
            fadeTimeParent.SetActive(false);
            colorIDObj.SetActive(false);
            touchTriggerToggle.gameObject.SetActive(false);

            // Set finish action
            _currentFinishAction = onFinish;
        }

        private void LocalSetData(Color startColor, float fadeTime, ColorType colorID, bool touchTriggered, Action<Color, float, bool, ColorType> onFinish)
        {
            Color noA = startColor;
            noA.a = 1; // No A actually means 1 A (?)

            // Update previous color and color picker
            previousColor.color = startColor;
            colorPicker.SetColor(startColor);

            // Update opacity slider
            opacitySlider.normalizedValue = startColor.a;

            // Show fade time, color ID and touch triggered toggle
            fadeTimeParent.SetActive(true);
            colorIDObj.SetActive(true);
            touchTriggerToggle.gameObject.SetActive(true);

            // Update fade time
            UpdateFadeTime(fadeTime);

            // Update color ID
            _currentColorID = colorID;
            ChangeColorID(0); // 0 will just update visuals without changing the value ;)

            // Update touch triggered toggle
            touchTriggerToggle.isOn = touchTriggered;

            // Set finish action
            _currentColorTriggerAction = onFinish;
        }
    }
}
