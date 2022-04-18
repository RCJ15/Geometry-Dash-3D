using System;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.Easing
{
    /// <summary>
    /// A struct that contains data for easing with ease rate and a custom curve
    /// </summary>
    [Serializable]
    public struct EaseData
    {
        public static readonly EaseData defaultValue = new EaseData(EasingType.none, 2, AnimationCurve.Linear(0, 0, 1, 1));

        public EasingType Type;

        [Range(0.1f, 4f)]
        public float EaseRate;

        public AnimationCurve CustomCurve;

        /// <summary>
        /// Evaluates the Y value on the curve <paramref name="x"/> position using this objects Easing Type, Ease Rate and Custom Curve.
        /// </summary>
        public float Evaluate(float x)
        {
            // Check the easing type
            switch (Type)
            {
                // Ease
                case EasingType.easeInOut:
                    return EaseMethods.EaseInOut(x, EaseRate);

                case EasingType.easeIn:
                    return EaseMethods.EaseIn(x, EaseRate);

                case EasingType.easeOut:
                    return EaseMethods.EaseOut(x, EaseRate);

                // Elastic
                case EasingType.elasticInOut:
                    return EaseMethods.ElasticInOut(x, EaseRate);

                case EasingType.elasticIn:
                    return EaseMethods.ElasticIn(x, EaseRate);

                case EasingType.elasticOut:
                    return EaseMethods.ElasticOut(x, EaseRate);

                // Bounce
                case EasingType.bounceInOut:
                    return EaseMethods.BounceInOut(x);

                case EasingType.bounceIn:
                    return EaseMethods.BounceIn(x);

                case EasingType.bounceOut:
                    return EaseMethods.BounceOut(x);

                // Exponential
                case EasingType.exponentialInOut:
                    return EaseMethods.ExponentialInOut(x);

                case EasingType.exponentialIn:
                    return EaseMethods.ExponentialIn(x);

                case EasingType.exponentialOut:
                    return EaseMethods.ExponentialOut(x);

                // Sine
                case EasingType.sineInOut:
                    return EaseMethods.SineInOut(x);

                case EasingType.sineIn:
                    return EaseMethods.SineIn(x);

                case EasingType.sineOut:
                    return EaseMethods.SineOut(x);

                // Back
                case EasingType.backInOut:
                    return EaseMethods.BackInOut(x);

                case EasingType.backIn:
                    return EaseMethods.BackIn(x);

                case EasingType.backOut:
                    return EaseMethods.BackOut(x);

                // Custom curve
                case EasingType.custom:
                    return CustomCurve.Evaluate(x);
            }

            // Return X by default (linear curve)
            return x;
        }

        public EaseData(EasingType type, float easeRate, AnimationCurve customCurve)
        {
            Type = type;
            EaseRate = easeRate;
            CustomCurve = customCurve;
        }
    }
}
