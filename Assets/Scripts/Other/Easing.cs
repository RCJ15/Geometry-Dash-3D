using System;
using UnityEngine;

namespace GD3D.Easing
{
    public static class EasingHelp
    {
        // I have to use these because LeanTween does not have a easing type which I can use >:(
        public static readonly AnimationCurve easeInOut = new AnimationCurve(
            new Keyframe(0, 0),
            new Keyframe(1, 1));

        public static readonly AnimationCurve easeIn = new AnimationCurve(
            new Keyframe(0, 0),
            new Keyframe(1, 1, 2, 2));

        public static readonly AnimationCurve easeOut = new AnimationCurve(
            new Keyframe(0, 0, 2, 2),
            new Keyframe(1, 1));

        /// <summary>
        /// Sets the easing of the tween to use a GD <see cref="EasingType"/>
        /// </summary>
        public static LTDescr SetGDEase(this LTDescr tween, EasingType type)
        {
            if (type.IsEaseType())
            {
                return tween.setEase(type.GetCurve());
            }

            return tween.setEase(type.ToLeanTweenType());
        }

        /// <summary>
        /// Makes the tween stop. <para/>
        /// Had to make this myself because LeanTween decided to make their old method obsolete
        /// </summary>
        public static LTDescr cancel(this LTDescr tween, bool callOnComplete = false)
        {
            LeanTween.cancel(tween.uniqueId, callOnComplete);

            return tween;
        }

        /// <summary>
        /// Returns the animation curve for the <see cref="EasingType"/>. <para/>
        /// Will only work for easings that are any kind of <see cref="EasingType.easeInOut"/> type.
        /// </summary>
        public static AnimationCurve GetCurve(this EasingType type)
        {
            switch (type)
            {
                case EasingType.easeInOut:
                    return easeInOut;
                case EasingType.easeIn:
                    return easeIn;
                case EasingType.easeOut:
                    return easeOut;
            }

            return null;
        }

        /// <summary>
        /// True if the <see cref="EasingType"/> is any kind of <see cref="EasingType.easeInOut"/> type
        /// </summary>
        public static bool IsEaseType(this EasingType type)
        {
            return
                type == EasingType.easeInOut ||
                type == EasingType.easeIn ||
                type == EasingType.easeOut;
        }

        /// <summary>
        /// Converts <see cref="EasingType"/> to <see cref="LeanTweenType"/> for use with <see cref="LeanTween"/>
        /// </summary>
        public static LeanTweenType ToLeanTweenType(this EasingType type)
        {
            switch (type)
            {
                case EasingType.none:
                    return LeanTweenType.linear;

                case EasingType.easeInOut:
                    return LeanTweenType.animationCurve;
                case EasingType.easeIn:
                    return LeanTweenType.animationCurve;
                case EasingType.easeOut:
                    return LeanTweenType.animationCurve;

                case EasingType.elasticInOut:
                    return LeanTweenType.easeInOutElastic;
                case EasingType.elasticIn:
                    return LeanTweenType.easeInElastic;
                case EasingType.elasticOut:
                    return LeanTweenType.easeOutElastic;

                case EasingType.bounceInOut:
                    return LeanTweenType.easeInOutBounce;
                case EasingType.bounceIn:
                    return LeanTweenType.easeInBounce;
                case EasingType.bounceOut:
                    return LeanTweenType.easeOutBounce;

                case EasingType.exponentialInOut:
                    return LeanTweenType.easeInOutExpo;
                case EasingType.exponentialIn:
                    return LeanTweenType.easeInExpo;
                case EasingType.exponentialOut:
                    return LeanTweenType.easeOutExpo;

                case EasingType.sineInOut:
                    return LeanTweenType.easeInOutSine;
                case EasingType.sineIn:
                    return LeanTweenType.easeInSine;
                case EasingType.sineOut:
                    return LeanTweenType.easeOutSine;

                case EasingType.backInOut:
                    return LeanTweenType.easeInOutBack;
                case EasingType.backIn:
                    return LeanTweenType.easeInBack;
                case EasingType.backOut:
                    return LeanTweenType.easeOutBack;
            }

            return LeanTweenType.notUsed;
        }
    }

    /// <summary>
    /// An enum that has the same easing types the ones used in geometry dash. <para/>
    /// Use <see cref=""/>
    /// </summary>
    [Serializable]
    public enum EasingType
    {
        none,

        easeInOut,
        easeIn,
        easeOut,

        elasticInOut,
        elasticIn,
        elasticOut,

        bounceInOut,
        bounceIn,
        bounceOut,

        exponentialInOut,
        exponentialIn,
        exponentialOut,

        sineInOut,
        sineIn,
        sineOut,

        backInOut,
        backIn,
        backOut,
    }
}
