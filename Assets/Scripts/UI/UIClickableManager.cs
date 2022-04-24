using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Easing;

namespace GD3D.UI
{
    /// <summary>
    /// A <see cref="GameObject"/> that contains data about <see cref="UIClickable"/>s scaling animations.
    /// </summary>
    public class UIClickableManager : MonoBehaviour
    {
        public static UIClickableManager Instance;

        //-- Manager Data
        public float SizeIncrease = 1.25f;

        public EaseSettings EaseSettingsIn = new EaseSettings(0.3f, new EaseData(EasingType.bounceOut), EaseObject.EaseCompleteMode.remove);
        public EaseSettings EaseSettingsOut = new EaseSettings(0.3f, new EaseData(EasingType.bounceOut), EaseObject.EaseCompleteMode.remove);

        private void Awake()
        {
            // Set instance
            Instance = this;
        }
    }
}
