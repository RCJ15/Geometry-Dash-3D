using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Level;

namespace GD3D.Objects
{
    /// <summary>
    /// Changes a color linearly over time when triggered
    /// </summary>
    public class ColorTrigger : TimedTrigger
    {
        [Header("Color Settings")]
        [SerializeField] private Color color = Color.white;
        [SerializeField] private LevelColors.ColorType colorType;
        [SerializeField] private EaseData easeData;

        public override void OnTriggered()
        {
            // IMPLEMENT EASING HERE

        }

        public override void OnUpdate(float time)
        {
            
        }

        public override void OnFinish()
        {

        }
    }
}
