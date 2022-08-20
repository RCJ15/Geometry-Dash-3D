using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Player;

namespace GD3D.Objects
{
    /// <summary>
    /// Will toggle the secondary trail on/off when triggered
    /// </summary>
    public class SecondaryTrailTrigger : Trigger
    {
        [Header("Trail Setting")]
        [SerializeField] private bool enableTrail = true;

        protected override void OnTriggered()
        {
            PlayerSecondaryTrailManager.HaveTrail = enableTrail;
        }
    }
}
