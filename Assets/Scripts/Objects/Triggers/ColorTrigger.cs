using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Level;

namespace GD3D.Objects
{
    /// <summary>
    /// Changes a color linearly over time when triggered
    /// </summary>
    public class ColorTrigger : Trigger
    {
        [Header("Color Settings")]
        [SerializeField] private Color color = Color.white;
        [SerializeField] private LevelColors.ColorType colorType;

        [Space]
        [SerializeField] private float fadeTime;

        public override void OnTriggered()
        {
            LevelColors.ChangeColorOverTime(colorType, color, fadeTime);
        }


#if UNITY_EDITOR
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            DrawDurationLine(fadeTime);
        }
#endif
    }
}
