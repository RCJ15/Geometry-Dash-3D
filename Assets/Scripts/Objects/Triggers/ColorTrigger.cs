using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Level;
using GD3D.Easing;

namespace GD3D.Objects
{
    /// <summary>
    /// Changes a color linearly over time when triggered
    /// </summary>
    public class ColorTrigger : Trigger
    {
        [Header("Color Settings")]
        [LevelSave] [SerializeField] private Color color = Color.white;
        [LevelSave] [SerializeField] private LevelColors.ColorType colorType;
        [LevelSave] [SerializeField] private EaseSettings easeSettings = EaseSettings.defaultValue;

        protected override void OnTriggered()
        {
            // Create a ease object
            EaseObject obj = easeSettings.CreateEase();

            // Add it to level colors
            LevelColors.AddEase(colorType, color, obj);
        }

#if UNITY_EDITOR
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            // Draw duration line
            DrawDurationLine(easeSettings.Time);
        }
#endif
    }
}
