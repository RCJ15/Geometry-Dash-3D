using System;
using UnityEngine;

namespace GD3D.Easing
{
    /// <summary>
    /// Contains methods for easing values between 2 points. <para/>
    /// Most values and math functions are from https://easings.net (Thanks!!!)
    /// </summary>
    public static class EaseMethods
    {
        #region Ease
        /// <summary>
        /// The "Ease In Out" <see cref="EasingType"/>. <para/>
        /// Change the <paramref name="easeRate"/> for different kinds of curves. <para/>
        /// 1.0 <paramref name="easeRate"/> = Linear. <para/>
        /// 2.0 <paramref name="easeRate"/> = Quad. (Default) <para/>
        /// 3.0 <paramref name="easeRate"/> = Cubic. <para/>
        /// 4.0 <paramref name="easeRate"/> = Quart.
        /// </summary>
        public static float EaseInOut(float x, float easeRate = 2)
        {
            if (x < 0.5f)
            {
                return Mathf.Pow(2, easeRate - 1) * Mathf.Pow(x, easeRate);
            }
            else
            {
                return 1 - Mathf.Pow(-2 * x + 2, easeRate) / 2;
            }
        }

        /// <summary>
        /// The "Ease In" <see cref="EasingType"/>. <para/>
        /// Change the <paramref name="easeRate"/> for different kinds of curves. <para/>
        /// 1.0 <paramref name="easeRate"/> = Linear. <para/>
        /// 2.0 <paramref name="easeRate"/> = Quad. (Default) <para/>
        /// 3.0 <paramref name="easeRate"/> = Cubic. <para/>
        /// 4.0 <paramref name="easeRate"/> = Quart.
        /// </summary>
        public static float EaseIn(float x, float easeRate = 2)
        {
            return Mathf.Pow(x, easeRate);
        }

        /// <summary>
        /// The "EaseOut" <see cref="EasingType"/>. <para/>
        /// Change the <paramref name="easeRate"/> for different kinds of curves. <para/>
        /// 1.0 <paramref name="easeRate"/> = Linear. <para/>
        /// 2.0 <paramref name="easeRate"/> = Quad. (Default) <para/>
        /// 3.0 <paramref name="easeRate"/> = Cubic. <para/>
        /// 4.0 <paramref name="easeRate"/> = Quart.
        /// </summary>
        public static float EaseOut(float x, float easeRate = 2)
        {
            return 1 - Mathf.Pow(1 - x, easeRate);
        }
        #endregion

        #region Elastic (Hurts my brain)
        /// <summary>
        /// The "Elastic In Out" <see cref="EasingType"/>. <para/>
        /// Elastic easings in GD are weird as hell so this is not 100% accurate but it's very close. <para/>
        /// Having <paramref name="easeRate"/> below ~1.2 will make the curve work like a regular elastic curve. <para/>
        /// Having <paramref name="easeRate"/> be above ~1.2 will make this curve work like an exponential curve. <para/>
        /// For the most accurate regular elastic curve, an <paramref name="easeRate"/> of 0.75 will do the trick.
        /// </summary>
        public static float ElasticInOut(float x, float easeRate = 2)
        {
            if (x == 0)
            {
                return 0;
            }
            else if (x == 1)
            {
                return 1;
            }
            else
            {
                float t = easeRate / 2;
                float invT = 1 - t; // "inv" means inverted, so inverted t essentially (0 and 1 is flipped)

                float sinIntensity = 4 * invT * Mathf.PI / 4.5f; // The lower "t" is, the more intense the sine wave will be

                if (x > 0.5f)
                {
                    sinIntensity *= -1;
                }

                float sin = Mathf.Sin((20 * x - 11.125f) * sinIntensity) * Mathf.Clamp01(invT);

                // Correct the exponential to always end on 1 if "t" is above 1
                float expoCorrection = t <= 1 ? 0 : (t - 1) / 10;
                float expoIn;

                if (x < 0.5f)
                {
                    expoIn = Mathf.Pow(2, 20 * (x - expoCorrection) - 10) * t;
                }
                else
                {
                    expoIn = Mathf.Pow(2, -20 * (x + expoCorrection) + 10) * t;
                }

                if (x < 0.5f)
                {
                    return -(Mathf.Pow(2, 20 * x - 10) * (sin - expoIn)) / 2;
                }
                else
                {
                    return Mathf.Pow(2, -20 * x + 10) * (sin - expoIn) / 2 + 1;
                }
            }
        }

        /// <summary>
        /// The "Elastic In" <see cref="EasingType"/>. <para/>
        /// Elastic easings in GD are weird as hell so this is not 100% accurate but it's very close. <para/>
        /// Having <paramref name="easeRate"/> below ~1.2 will make the curve work like a regular elastic curve. <para/>
        /// Having <paramref name="easeRate"/> be above ~1.2 will make this curve work like an exponential curve. <para/>
        /// For the most accurate regular elastic curve, an <paramref name="easeRate"/> of 0.75 will do the trick.
        /// </summary>
        public static float ElasticIn(float x, float easeRate = 2)
        {
            if (x == 0)
            {
                return 0;
            }
            else if (x == 1)
            {
                return 1;
            }
            else
            {
                // Original "ElasticIn" function:
                // -Mathf.Pow(2, 10 * x - 10) * Mathf.Sin((x * 10 - 10.75f) * ((2 * Mathf.PI) / 3));
                
                float t = easeRate / 2;
                float invT = 1 - t; // "inv" means inverted, so inverted t essentially (0 and 1 is flipped)

                float sinIntensity = 4 * invT * Mathf.PI / 3; // The lower "t" is, the more intense the sine wave will be

                float sin = Mathf.Sin((x * 10 - 10.75f) * sinIntensity) * Mathf.Clamp01(invT);

                // Correct the exponential to always end on 1 if "t" is above 1
                float expoCorrection = t <= 1 ? 0 : (t - 1) / 10;
                float expoIn = ExponentialIn(x - expoCorrection) * t;

                return -Mathf.Pow(2, 10 * x - 10) * (sin - expoIn);

            }
        }

        /// <summary>
        /// The "Elastic Out" <see cref="EasingType"/>. <para/>
        /// Elastic easings in GD are weird as hell so this is not 100% accurate but it's very close. <para/>
        /// Having <paramref name="easeRate"/> below ~1.2 will make the curve work like a regular elastic curve. <para/>
        /// Having <paramref name="easeRate"/> be above ~1.2 will make this curve work like an exponential curve. <para/>
        /// For the most accurate regular elastic curve, an <paramref name="easeRate"/> of 0.75 will do the trick.
        /// </summary>
        public static float ElasticOut(float x, float easeRate = 2)
        {
            if (x == 0)
            {
                return 0;
            }
            else if (x == 1)
            {
                return 1;
            }
            else
            {
                // Original "ElasticOut" function:
                // Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10 - 0.75f) * (2 * Mathf.PI / 3)) + 1;

                float t = easeRate / 2;
                float invT = 1 - t; // "inv" means inverted, so inverted t essentially (0 and 1 is flipped)

                float sinIntensity = 4 * invT * Mathf.PI / 3; // The lower "t" is, the more intense the sine wave will be

                float sin = Mathf.Sin((x * 10 - 0.75f) * -sinIntensity) * Mathf.Clamp01(invT);

                // Correct the exponential to always end on 1 if "t" is above 1
                float expoCorrection = t <= 1 ? 0 : (t - 1) / 10;
                float expoIn = Mathf.Pow(2, -10 * (x + expoCorrection)) * t;

                return Mathf.Pow(2, -10 * x) * (sin - expoIn) + 1;
            }
        }
        #endregion

        #region Bounce
        /// <summary>
        /// The "BounceInOut" <see cref="EasingType"/>.
        /// </summary>
        public static float BounceInOut(float x)
        {
            if (x < 0.5f)
            {
                return (1 - BounceOut(1 - 2 * x)) / 2;
            }
            else
            {
                return (1 + BounceOut(2 * x - 1)) / 2;
            }
        }

        /// <summary>
        /// The "BounceIn" <see cref="EasingType"/>.
        /// </summary>
        public static float BounceIn(float x)
        {
            return 1 - BounceOut(1 - x);
        }

        /// <summary>
        /// The "BounceOut" <see cref="EasingType"/>. <para/>
        /// This one is actually disgusting to look at in code.
        /// </summary>
        public static float BounceOut(float x)
        {
            float n1 = 7.5625f;
            float d1 = 2.75f;

            if (x < 1 / d1)
            {
                return n1 * x * x;
            }
            else if (x < 2 / d1)
            {
                return n1 * (x -= 1.5f / d1) * x + 0.75f;
            }
            else if (x < 2.5f / d1)
            {
                return n1 * (x -= 2.25f / d1) * x + 0.9375f;
            }
            else
            {
                return n1 * (x -= 2.625f / d1) * x + 0.984375f;
            }
        }
        #endregion

        #region Exponential
        /// <summary>
        /// The "Exponential In Out" <see cref="EasingType"/>.
        /// </summary>
        public static float ExponentialInOut(float x)
        {
            if (x == 0)
            {
                return 0;
            }
            else if (x == 1)
            {
                return 1;
            }
            else
            {
                if (x < 0.5f)
                {
                    return Mathf.Pow(2, 20 * x - 10) / 2;
                }
                else
                {
                    return (2 - Mathf.Pow(2, -20 * x + 10)) / 2;
                }
            }
        }

        /// <summary>
        /// The "Exponential In" <see cref="EasingType"/>.
        /// </summary>
        public static float ExponentialIn(float x)
        {
            if (x == 0)
            {
                return 0;
            }
            else
            {
                return Mathf.Pow(2, 10 * x - 10);
            }
        }

        /// <summary>
        /// The "Exponential Out" <see cref="EasingType"/>.
        /// </summary>
        public static float ExponentialOut(float x)
        {
            if (x == 1)
            {
                return 1;
            }
            else
            {
                return 1 - Mathf.Pow(2, -10 * x);
            }
        }
        #endregion

        #region Sine
        /// <summary>
        /// The "Sine In Out" <see cref="EasingType"/>.
        /// </summary>
        public static float SineInOut(float x)
        {
            return -(Mathf.Cos(Mathf.PI * x) - 1) / 2;
        }

        /// <summary>
        /// The "Sine In" <see cref="EasingType"/>.
        /// </summary>
        public static float SineIn(float x)
        {
            return 1 - Mathf.Cos((x * Mathf.PI) / 2);
        }

        /// <summary>
        /// The "Sine Out" <see cref="EasingType"/>.
        /// </summary>
        public static float SineOut(float x)
        {
            return Mathf.Sin((x * Mathf.PI) / 2);
        }
        #endregion

        #region Back
        // Just leaving these constants here because they are used a lot in back easings
        // Blame https://easings.net for the names of the constants
        private const float C1 = 1.70158f;
        private const float C2 = C1 * 1.525f;
        private const float C3 = C1 + 1;

        /// <summary>
        /// The "Back In Out" <see cref="EasingType"/>.
        /// </summary>
        public static float BackInOut(float x)
        {
            if (x < 0.5f)
            {
                return Mathf.Pow(2 * x, 2) * ((C2 + 1) * 2 * x - C2) / 2;
            }
            else
            {
                return (Mathf.Pow(2 * x - 2, 2) * ((C2 + 1) * (x * 2 - 2) + C2) + 2) / 2;
            }
        }

        /// <summary>
        /// The "Back In" <see cref="EasingType"/>.
        /// </summary>
        public static float BackIn(float x)
        {
            return C3 * x * x * x - C1 * x * x;
        }

        /// <summary>
        /// The "Back Out" <see cref="EasingType"/>.
        /// </summary>
        public static float BackOut(float x)
        {
            return 1 + C3 * Mathf.Pow(x - 1, 3) + C1 * Mathf.Pow(x - 1, 2);
        }
        #endregion
    }
}
