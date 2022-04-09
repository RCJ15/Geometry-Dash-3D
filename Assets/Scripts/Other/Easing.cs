using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GD3D
{
    /// <summary>
    /// Custom easing library for easing values between 2 points. <para/>
    /// Almost all values are from https://easings.net/ (Thanks)
    /// </summary>
    public static class Easing
    {
        #region Ease
        /// <summary>
        /// The "EaseInOut" <see cref="EasingType"/>. <para/>
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
                return EaseIn(x, easeRate) * 2;
            }
            else
            {
                return EaseOut(x, easeRate) / 2;
            }
        }

        /// <summary>
        /// The "EaseIn" <see cref="EasingType"/>. <para/>
        /// Change the <paramref name="easeRate"/> for different kinds of curves. <para/>
        /// 1.0 <paramref name="easeRate"/> = Linear. <para/>
        /// 2.0 <paramref name="easeRate"/> = Quad. (Default) <para/>
        /// 3.0 <paramref name="easeRate"/> = Cubic. <para/>
        /// 4.0 <paramref name="easeRate"/> = Quart.
        /// </summary>
        public static float EaseIn(float x, float easeRate = 2)
        {
            float val = Mathf.Pow(x, easeRate);

            return val;
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
            float val = Mathf.Pow(1 - x, easeRate);

            return 1 - val;
        }
        #endregion

        #region Elastic
        /// <summary>
        /// The "ElasticInOut" <see cref="EasingType"/>. <para/>
        /// Elastic easings in GD are weird as hell.
        /// </summary>
        public static float ElasticInOut(float x, float easeRate = 2)
        {
            float c5 = (2 * Mathf.PI) / 4.5f;

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
                    return -(Mathf.Pow(2, 20 * x - 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2;
                }
                else
                {
                    return (Mathf.Pow(2, -20 * x + 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2 + 1;
                }
            }
        }

        /// <summary>
        /// The "ElasticIn" <see cref="EasingType"/>. <para/>
        /// Elastic easings in GD are weird as hell.
        /// </summary>
        public static float ElasticIn(float x, float easeRate = 2)
        {
            float c4 = (2 * Mathf.PI) / 3;

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
                return -Mathf.Pow(2, 10 * x - 10) * Mathf.Sin((x * 10 - 10.75f) * c4);
            }
        }

        /// <summary>
        /// The "ElasticOut" <see cref="EasingType"/>. <para/>
        /// Elastic easings in GD are weird as hell.
        /// </summary>
        public static float ElasticOut(float x, float easeRate = 2)
        {
            float c4 = (2 * Mathf.PI) / 3;

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
                return Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10 - 0.75f) * c4) + 1;
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
        /// The "ExponentialInOut" <see cref="EasingType"/>.
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
        /// The "ExponentialIn" <see cref="EasingType"/>.
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
        /// The "ExponentialOut" <see cref="EasingType"/>.
        /// </summary>
        public static float ExponentialOut(float x)
        {
            if (x == 1)
            {
                return 1;
            }
            else
            {
                return Mathf.Pow(2, -10 * x);
            }
        }
        #endregion

        #region Sine
        /// <summary>
        /// The "SineInOut" <see cref="EasingType"/>.
        /// </summary>
        public static float SineInOut(float x)
        {
            float val = Mathf.Cos(Mathf.PI * x);

            return -(val - 1) / 2;
        }

        /// <summary>
        /// The "SineIn" <see cref="EasingType"/>.
        /// </summary>
        public static float SineIn(float x)
        {
            float val = Mathf.Cos((x * Mathf.PI) / 2);

            return 1 - val;
        }

        /// <summary>
        /// The "SineOut" <see cref="EasingType"/>.
        /// </summary>
        public static float SineOut(float x)
        {
            float val = Mathf.Sin((x * Mathf.PI) / 2);

            return val;
        }
        #endregion

        #region Back
        /// <summary>
        /// The "BackInOut" <see cref="EasingType"/>.
        /// </summary>
        public static float BackInOut(float x)
        {
            float c1 = 1.70158f;
            float c2 = c1 * 1.525f;

            if (x < 0.5f)
            {
                return Mathf.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2) / 2;
            }
            else
            {
                return (Mathf.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
            }
        }

        /// <summary>
        /// The "BackIn" <see cref="EasingType"/>.
        /// </summary>
        public static float BackIn(float x)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1;

            return c3 * x * x * x - c1 * x * x;
        }

        /// <summary>
        /// The "BackOut" <see cref="EasingType"/>.
        /// </summary>
        public static float BackOut(float x)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1;

            return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
        }
        #endregion

        /// <summary>
        /// Evaluates the curve on the <paramref name="easeData"/> using it's Easing Type, Ease Rate and Custom Curve.
        /// </summary>
        /// <param name="easeData">The ease data used.</param>
        /// <param name="x">The position on the curve. It's recommended that this is between 0 and 1.</param>
        /// <returns>The value on the <paramref name="x"/> position of the <paramref name="easeData"/> curve.</returns>
        public static float Evaluate(this EaseData easeData, float x)
        {
            // Check the easing type
            switch (easeData.Type)
            {
                // Ease
                case EasingType.easeInOut:
                    return EaseInOut(x, easeData.EaseRate);

                case EasingType.easeIn:
                    return EaseIn(x, easeData.EaseRate);

                case EasingType.easeOut:
                    return EaseOut(x, easeData.EaseRate);

                // Elastic
                case EasingType.elasticInOut:
                    return ElasticInOut(x, easeData.EaseRate);

                case EasingType.elasticIn:
                    return ElasticIn(x, easeData.EaseRate);

                case EasingType.elasticOut:
                    return ElasticOut(x, easeData.EaseRate);

                // Bounce
                case EasingType.bounceInOut:
                    return BounceInOut(x);

                case EasingType.bounceIn:
                    return BounceIn(x);

                case EasingType.bounceOut:
                    return BounceOut(x);

                // Exponential
                case EasingType.exponentialInOut:
                    return ExponentialInOut(x);

                case EasingType.exponentialIn:
                    return ExponentialIn(x);

                case EasingType.exponentialOut:
                    return ExponentialOut(x);

                // Sine
                case EasingType.sineInOut:
                    return SineInOut(x);

                case EasingType.sineIn:
                    return SineIn(x);

                case EasingType.sineOut:
                    return SineOut(x);

                // Back
                case EasingType.backInOut:
                    return BackInOut(x);

                case EasingType.backIn:
                    return BackIn(x);

                case EasingType.backOut:
                    return BackOut(x);

                // Custom curve
                case EasingType.custom:
                    return easeData.CustomCurve.Evaluate(x);
            }

            // Return X by default (linear curve)
            return x;
        }
    }

    /// <summary>
    /// Class that contains data for easing with ease rate and a custom curve
    /// </summary>
    [Serializable]
    public class EaseData
    {
        public EasingType Type;

        [Range(0.1f, 4f)]
        public float EaseRate = 2;

        public AnimationCurve CustomCurve = AnimationCurve.Linear(0, 0, 1, 1);
    }

#if UNITY_EDITOR
    /// <summary>
    /// The custom property drawer for <see cref="EaseData"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(EaseData), true)]
    public class EaseDataPropertyDrawer : PropertyDrawer
    {
        private float extraHeight = 0;
        private static bool foldoutOpen = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Get properties
            var type = GetProperty(property, "Type");
            var easeRate = GetProperty(property, "EaseRate");
            var customCurve = GetProperty(property, "CustomCurve");

            extraHeight = 0;

            // Reset and cache indent level for use later
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect foldoutRect = new Rect(position.x, position.y, position.width, 20);

            // Create foldout
            foldoutOpen = EditorGUI.Foldout(foldoutRect, foldoutOpen, label);

            // Do not draw anything else if the foldout is closed
            if (!foldoutOpen)
            {
                return;
            }

            // Calculate rects
            Rect typeRect = new Rect(position.x, position.y + 20, position.width, 20);
            Rect extraRect = new Rect(position.x, position.y + 40, position.width, 20);

            EditorGUI.indentLevel++;

            Serialize(typeRect, type);

            // Serialize easeRate and customCurve
            EasingType easingType = (EasingType)type.enumValueIndex;

            switch (easingType)
            {
                case EasingType.easeInOut:
                case EasingType.easeIn:
                case EasingType.easeOut:
                case EasingType.elasticInOut:
                case EasingType.elasticIn:
                case EasingType.elasticOut:
                    Serialize(extraRect, easeRate);
                    break;

                case EasingType.custom:
                    // Draw a animation curve that is clamped between 0 and 1
                    customCurve.animationCurveValue = EditorGUI.CurveField(extraRect, "Custom Curve", customCurve.animationCurveValue, Color.green, new Rect(0, 0, 1, 1));
                    extraHeight += extraRect.height;
                    break;
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        private void Serialize(Rect rect, SerializedProperty property)
        {
            EditorGUI.PropertyField(rect, property);
            extraHeight += rect.height;
        }

        private SerializedProperty GetProperty(SerializedProperty prop, string name)
        {
            return prop.FindPropertyRelative(name);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + extraHeight;
        }

        /*
        #region Context Menu and Copy Paste stuff
        private void ContextMenu(Rect position, SerializedProperty property)
        {
            Event e = Event.current;

            if (e.type == EventType.MouseDown && e.button == 1 && position.Contains(e.mousePosition))
            {
                GenericMenu context = new GenericMenu();
                
                context.AddItem(new GUIContent("Expand"), false, () => AddToClipboard(property));
                context.AddItem(new GUIContent("Unexpand"), false, () => );
                
                context.ShowAsContext();
            }
        }

        private void AddToClipboard(SerializedProperty property)
        {
            // Get properties
            var type = GetProperty(property, "Type");
            var easeRate = GetProperty(property, "EaseRate");
            var customCurve = GetProperty(property, "CustomCurve");

            // Convert to string
        }
        #endregion
        */
    }
#endif

    /// <summary>
    /// An enum that has the same easing types the ones used in geometry dash.
    /// </summary>
    [Serializable]
    public enum EasingType
    {
        /// <summary>
        /// Linear curve
        /// </summary>
        none,

        // Ease
        easeInOut,
        easeIn,
        easeOut,

        // Elastic
        elasticInOut,
        elasticIn,
        elasticOut,

        // Bounce
        bounceInOut,
        bounceIn,
        bounceOut,

        // Exponential
        exponentialInOut,
        exponentialIn,
        exponentialOut,

        // Sine
        sineInOut,
        sineIn,
        sineOut,

        // Back
        backInOut,
        backIn,
        backOut,

        /// <summary>
        /// Use a custom animation curve instead of a default ease type
        /// </summary>
        custom,
    }
}
