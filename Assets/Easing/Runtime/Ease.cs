using System;
using UnityEngine;

namespace GD3D.Easing
{
    /// <summary>
    /// This class contains various different methods used to create <see cref="EaseObject"/>s.
    /// </summary>
    public static class Ease
    {
        #region Transform Extensions
        /// <summary>
        /// Moves this object from it's current location to the <paramref name="destination"/> position. (World space)
        /// </summary>
        /// <returns>The newly created <see cref="EaseObject"/> that moves this object. <para/>
        /// Use this to set stuff like the <see cref="EaseObject.EaseData"/> by calling <see cref="EaseObject.SetEaseData"/> for example.</returns>
        public static EaseObject EaseMove(this Transform transform, Vector3 destination, float time)
        {
            Vector3 startPos = transform.position;

            return new EaseObject(0, 1, time).SetOnUpdate(
            (obj) =>
            {
                transform.position = startPos.EaseVector(destination, obj);
            });
        }

        /// <summary>
        /// Moves this object from it's current location to the <paramref name="destination"/> position. (Local space)
        /// </summary>
        /// <returns>The newly created <see cref="EaseObject"/> that moves this object. <para/>
        /// Use this to set stuff like the <see cref="EaseObject.EaseData"/> by calling <see cref="EaseObject.SetEaseData"/> for example.</returns>
        public static EaseObject EaseMoveLocal(this Transform transform, Vector3 destination, float time)
        {
            Vector3 startPos = transform.localPosition;

            return new EaseObject(0, 1, time).SetOnUpdate(
            (obj) =>
            {
                transform.localPosition = startPos.EaseVector(destination, obj);
            });
        }

        /// <summary>
        /// Scales this objects local scale from it's current scale to the <paramref name="destination"/> scale.
        /// </summary>
        /// <returns>The newly created <see cref="EaseObject"/> that scales this object. <para/>
        /// Use this to set stuff like the <see cref="EaseObject.EaseData"/> by calling <see cref="EaseObject.SetEaseData"/> for example.</returns>
        public static EaseObject EaseScale(this Transform transform, Vector3 destination, float time)
        {
            Vector3 startScale = transform.localScale;

            return new EaseObject(0, 1, time).SetOnUpdate(
            (obj) =>
            {
                transform.localScale = startScale.EaseVector(destination, obj);
            });
        }

        /// <summary>
        /// Rotates this object from it's current rotation to the <paramref name="destination"/> rotation. (<see cref="Vector3"/> edition)
        /// </summary>
        /// <returns>The newly created <see cref="EaseObject"/> that moves this object. <para/>
        /// Use this to set stuff like the <see cref="EaseObject.EaseData"/> by calling <see cref="EaseObject.SetEaseData"/> for example.</returns>
        public static EaseObject EaseRotation(this Transform transform, Vector3 destination, float time)
        {
            Quaternion startRot = transform.rotation;

            return new EaseObject(0, 1, time).SetOnUpdate(
            (obj) =>
            {
                transform.rotation = startRot.EaseQuaternion(destination, obj);
            });
        }

        /// <summary>
        /// Rotates this object from it's current rotation to the <paramref name="destination"/> rotation. (<see cref="Quaternion"/> edition)
        /// </summary>
        /// <returns>The newly created <see cref="EaseObject"/> that rotates this object. <para/>
        /// Use this to set stuff like the <see cref="EaseObject.EaseData"/> by calling <see cref="EaseObject.SetEaseData"/> for example.</returns>
        public static EaseObject EaseRotation(this Transform transform, Quaternion destination, float time)
        {
            Quaternion startRot = transform.rotation;

            return new EaseObject(0, 1, time).SetOnUpdate(
            (obj) =>
            {
                transform.rotation = startRot.EaseQuaternion(destination, obj);
            });
        }

        /// <summary>
        /// Rotates this object from it's current rotation to the <paramref name="destination"/> rotation. (2D Z axis ONLY edition)
        /// </summary>
        /// <returns>The newly created <see cref="EaseObject"/> that rotates this object. <para/>
        /// Use this to set stuff like the <see cref="EaseObject.EaseData"/> by calling <see cref="EaseObject.SetEaseData"/> for example.</returns>
        public static EaseObject EaseRotation(this Transform transform, float destination, float time)
        {
            Quaternion startRot = transform.rotation;

            return new EaseObject(0, 1, time).SetOnUpdate(
            (obj) =>
            {
                transform.rotation = startRot.EaseQuaternion(Quaternion.Euler(0, 0, destination), obj);
            });
        }
        #endregion

        #region Easing Values
        #region Vector
        /// <summary>
        /// Eases this <see cref="Vector3"/> to the <paramref name="end"/> <see cref="Vector3"/> using the given <see cref="EaseObject"/>.
        /// </summary>
        /// <returns>The eased <see cref="Vector3"/>.</returns>
        public static Vector3 EaseVector(this Vector3 start, Vector3 end, EaseObject obj)
        {
            Vector3 newVector;

            newVector.x = obj.GetValue(start.x, end.x);
            newVector.y = obj.GetValue(start.y, end.y);
            newVector.z = obj.GetValue(start.y, end.z);

            return newVector;
        }

        /// <summary>
        /// Uses this <see cref="EaseObject"/> to ease the <paramref name="start"/> <see cref="Vector3"/> to the <paramref name="end"/> <see cref="Vector3"/>.
        /// </summary>
        /// <returns>The eased <see cref="Vector3"/>.</returns>
        public static Vector3 EaseVector(this EaseObject obj, Vector3 start, Vector3 end)
        {
            return start.EaseVector(end, obj);
        }

        /// <summary>
        /// Eases this <see cref="Vector2"/> to the <paramref name="end"/> <see cref="Vector2"/> using the given <see cref="EaseObject"/>.
        /// </summary>
        /// <returns>The eased <see cref="Vector2"/>.</returns>
        public static Vector2 EaseVector(this Vector2 start, Vector2 end, EaseObject obj)
        {
            Vector2 newVector;

            newVector.x = obj.GetValue(start.x, end.x);
            newVector.y = obj.GetValue(start.y, end.y);

            return newVector;
        }

        /// <summary>
        /// Uses this <see cref="EaseObject"/> to ease the <paramref name="start"/> <see cref="Vector2"/> to the <paramref name="end"/> <see cref="Vector2"/>.
        /// </summary>
        /// <returns>The eased <see cref="Vector2"/>.</returns>
        public static Vector2 EaseVector(this EaseObject obj, Vector2 start, Vector2 end)
        {
            return start.EaseVector(end, obj);
        }
        #endregion

        #region Quaternions
        /// <summary>
        /// Eases this <see cref="Quaternion"/> to the <paramref name="end"/> <see cref="Quaternion"/> using the given <see cref="EaseObject"/>.
        /// </summary>
        /// <returns>The eased <see cref="Quaternion"/>.</returns>
        public static Quaternion EaseQuaternion(this Quaternion start, Quaternion end, EaseObject obj)
        {
            Quaternion newQuaternion;

            newQuaternion.x = obj.GetValue(start.x, end.x);
            newQuaternion.y = obj.GetValue(start.y, end.y);
            newQuaternion.z = obj.GetValue(start.y, end.z);
            newQuaternion.w = obj.GetValue(start.w, end.w);

            return newQuaternion;
        }

        /// <summary>
        /// Uses this <see cref="EaseObject"/> to ease the <paramref name="start"/> <see cref="Quaternion"/> to the <paramref name="end"/> <see cref="Quaternion"/>.
        /// </summary>
        /// <returns>The eased <see cref="Quaternion"/>.</returns>
        public static Quaternion EaseQuaternion(this EaseObject obj, Quaternion start, Quaternion end)
        {
            return start.EaseQuaternion(end, obj);
        }

        /// <summary>
        /// Eases this <see cref="Quaternion"/> to the <paramref name="end"/> <see cref="Vector3"/> (which is converted to a <see cref="Quaternion"/>) using the given <see cref="EaseObject"/>.
        /// </summary>
        /// <returns>The eased <see cref="Quaternion"/>.</returns>
        public static Quaternion EaseQuaternion(this Quaternion start, Vector3 end, EaseObject obj)
        {
            Quaternion newQuaternion;
            Quaternion endQuaternion = Quaternion.Euler(end);

            newQuaternion.x = obj.GetValue(start.x, endQuaternion.x);
            newQuaternion.y = obj.GetValue(start.y, endQuaternion.y);
            newQuaternion.z = obj.GetValue(start.y, endQuaternion.z);
            newQuaternion.w = obj.GetValue(start.w, endQuaternion.w);

            return newQuaternion;
        }

        /// <summary>
        /// Uses this <see cref="EaseObject"/> to ease the <paramref name="start"/> <see cref="Quaternion"/> to the <paramref name="end"/> <see cref="Vector3"/> (which is converted to a <see cref="Quaternion"/>).
        /// </summary>
        /// <returns>The eased <see cref="Quaternion"/>.</returns>
        public static Quaternion EaseQuaternion(this EaseObject obj, Quaternion start, Vector3 end)
        {
            return start.EaseQuaternion(end, obj);
        }
        #endregion

        #region Color
        /// <summary>
        /// Eases this <see cref="Color"/> to the <paramref name="end"/> <see cref="Color"/> using the given <see cref="EaseObject"/>.
        /// </summary>
        /// <returns>The eased <see cref="Color"/>.</returns>
        public static Color EaseColor(this Color start, Color end, EaseObject obj)
        {
            Color newColor;

            // Use Mathf.Clamp01 because color channels can't be above 1 or below 0
            // Otherwise you get many angry errors
            newColor.r = Mathf.Clamp01(obj.GetValue(start.r, end.r));
            newColor.g = Mathf.Clamp01(obj.GetValue(start.g, end.g));
            newColor.b = Mathf.Clamp01(obj.GetValue(start.b, end.b));
            newColor.a = Mathf.Clamp01(obj.GetValue(start.a, end.a));

            return newColor;
        }

        /// <summary>
        /// Uses this <see cref="EaseObject"/> to ease the <paramref name="start"/> <see cref="Color"/> to the <paramref name="end"/> <see cref="Color"/>.
        /// </summary>
        /// <returns>The eased <see cref="Color"/>.</returns>
        public static Color EaseColor(this EaseObject obj, Color start, Color end)
        {
            return start.EaseColor(end, obj);
        }
        #endregion
        #endregion

        #region Ease Struct Extensions
        /// <summary>
        /// Creates a new <see cref="EaseObject"/> from the <see cref="EaseSettings"/>.
        /// </summary>
        /// <returns>The newly created <see cref="EaseObject"/>. <para/>
        /// Use this to set stuff like the <see cref="EaseObject.EaseData"/> by calling <see cref="EaseObject.SetEaseData"/> for example.</returns>
        public static EaseObject CreateEase(this EaseSettings easeSettings)
        {
            return new EaseObject(easeSettings);
        }

        /// <summary>
        /// Creates a new <see cref="EaseObject"/> from the <see cref="EaseData"/>.
        /// </summary>
        /// <returns>The newly created <see cref="EaseObject"/>. <para/>
        /// Use this to set stuff like the <see cref="EaseObject.EaseData"/> by calling <see cref="EaseObject.SetOnUpdate"/> for example.</returns>
        public static EaseObject CreateEase(this EaseData easeData)
        {
            return new EaseObject(easeData);
        }

        /// <summary>
        /// Creates a new <see cref="EaseObject"/> from the <see cref="EaseState"/>.
        /// </summary>
        /// <returns>The newly created <see cref="EaseObject"/>. <para/>
        /// Use this to set stuff like the <see cref="EaseObject.EaseData"/> by calling <see cref="EaseObject.SetEaseData"/> for example.</returns>
        public static EaseObject CreateEase(this EaseState easeState)
        {
            return new EaseObject(easeState);
        }
        #endregion
    }

    /// <summary>
    /// An enum that has the same easing types as the ones used in geometry dash. <para/>
    /// Also allows for a custom animation curve when using <see cref="custom"/>.
    /// </summary>
    [Serializable]
    public enum EasingType
    {
        /// <summary>
        /// Same as a linear curve.
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
    }
}
