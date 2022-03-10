using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper methods exist here. <para/>
/// I copy and paste this script from project to project so that's why this script has some unnecessary things in it - RCJ15
/// </summary>
public struct Helpers
{
    public static Camera Camera
    {
        get
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            return _camera;
        }
    }
    private static Camera _camera;

    /// <summary>
    /// Returns the current scene we are on.
    /// </summary>
    public static int CurrentScene => UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

    /// <summary>
    /// Returns the current screen size in pixels unit.
    /// </summary>
    public static Vector2 ScreenSize => new Vector2(Screen.width, Screen.height);

    /// <summary>
    /// Returns the entire screen size in world space
    /// </summary>
    public static Vector2 ScreenSizeWorld
    {
        get
        {
            // Get the screen size using this cool calculation
            Vector2 screenSize = Camera.ViewportToWorldPoint(new Vector3(1, 1, -Camera.transform.position.z));

            // Convert it into local space cuz we are getting size not position
            screenSize = Camera.transform.InverseTransformPoint(screenSize);

            // Return the screen size but twice as big (cuz it's only half)
            return screenSize * 2;
        }
    }

    /// <summary>
    /// Returns true if the value given is a prime number.
    /// </summary>
    public bool IsPrime(int value)
    {
        if (value == 1) return false; //Hard coded for 1 (False)
        if (value == 2) return true; //Hard coded for 2 (True)

        if (value % 2 == 0) return false; //It's a even number so not a prime number (False)

        for (int i = 2; i < value; i++)
        {
            //Advance from two to include correct calculation for '4'
            if (value % i == 0) return false; //If value can be divided by i (with decimals) then it's not a prime number! (False)
        }

        //If it passes all the tests then congrats! It's a prime number! (True)
        return true;
    }

    /// <summary>
    /// Returns true if the number is a number in the fibonacci sequence.
    /// </summary>
    public bool IsFibonacci(int value)
    {
        if (value == 0 || value == 1) return true; //Hard coded for 0 and 1 (True)

        int a = 0; //First number in the fibonacci sequence (0)
        int b = 1; //Second number in the fibonacci sequence (1)
        int c = a + b; //Both of the values added together
        
        //This will basically go through all the numbers in the fibonacci sequence that are below the given value
        while (c < value)
        {
            a = b; //Set A to the next in the sequence (B)
            b = c; //Set B to the next in the sequence (C)
            c = a + b; //Set C to A + B (fibonacci number)
        }

        if (c == value) return true; //If C is equal to value, then it's a number in the fibonacci sequence! (True)
        else return false; //If not, then it's not a number in the fibonacci sequence (False)
    }

    #region Trigonometry

    /// <summary>
    /// Returns C which is the square root of <paramref name="a"/>^2 + <paramref name="b"/>^2
    /// </summary>
    public static float PythagoreanTheorem(float a, float b)
    {
        return Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2));
    }

    #endregion

    /// <summary>
    /// Will return the given string except only the first letters are uppercase. Example: hEllO woRlD = Hello World.
    /// </summary>
    /// <param name="text">The text that will be converted</param>
    /// <param name="onlyFirstWord">If true, makes only the first words first letter uppercase and everything else lowercase</param>
    public static string FirstLettersUpper(string text, bool onlyFirstWord = false)
    {
        if (onlyFirstWord)
        {
            //Take the first letter and make it uppercase
            //Take the other letters and make them lowercase
            return char.ToUpper(text[0]) + text.Substring(1).ToLower();
        } else
        {
            string returnData = "";
            bool nextUpper = true;

            foreach (char c in text)
            {
                if (nextUpper)
                {
                    returnData += char.ToUpper(c);
                    nextUpper = false;
                } else
                {
                    returnData += char.ToLower(c);
                }

                //If the character was a space or period then the next character will be uppercase
                if (c.Equals(' ') || c.Equals('.'))
                {
                    nextUpper = true;
                }
            }

            return returnData;
        }
    }

    /// <summary>
    /// Useful for randomizing stuff but you don't want the same int twice in a row.
    /// Tries for the amount of times maxTimesTried has.
    /// </summary>
    /// <returns>A random int except that it can have a restriction, meaning it won't generate the restricted int</returns>
    public static int RandomNewInt(int min, int max, int restriction, int maxTimesTried = 100)
    {
        //If the minimum is equal to the maximum then return
        if (min == max)
        {
            return min;
        }

        int newInt = Random.Range(min, max);

        while (newInt == restriction && maxTimesTried > 0)
        {
            newInt = Random.Range(min, max);
            maxTimesTried--;
        }

        return newInt;
    }

    /// <summary>
    /// 1 dimensional distance calculation
    /// </summary>
    /// <returns>The distance between the 2 values</returns>
    public static float Distance(float value1, float value2)
    {
        return Mathf.Abs(Mathf.Max(value1, value2) - Mathf.Min(value1, value2));
    }

    /// <summary>
    /// Returns the given value looped around min and max (int)
    /// </summary>
    public static int LoopValue(int value, int min, int max)
    {
        // If the value is greater than the max then set it to the min
        if (value > max)
        {
            value = min;
        }
        // Else if the value is less than min then set it to the max
        else if (value < min)
        {
            value = max;
        }

        return value;
    }

    /// <summary>
    /// Returns the given value looped around min and max (float)
    /// </summary>
    public static float LoopValue(float value, float min, float max)
    {
        // If the value is greater than the max then set it to the min
        if (value > max)
        {
            value = min;
        }
        // Else if the value is less than min then set it to the max
        else if (value < min)
        {
            value = max;
        }

        return value;
    }

    /// <summary>
    /// Checks if the given value is within the <paramref name="min"/> and <paramref name="max"/> parameters. (int)
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="min">The minimum that <paramref name="value"/> has to be within.</param>
    /// <param name="max">The maximum that <paramref name="value"/> has to be within.</param>
    /// <param name="exact">Is false by default. Will also count the value as in range if it's exactly <paramref name="min"/> or <paramref name="max"/>.</param>
    /// <returns>If the value is within the range.</returns>
    public static bool ValueWithinRange(int value, int min, int max, bool exact = false)
    {
        return
            exact ?
            value >= min && value <= max
            :
            value > min && value < max;
    }

    /// <summary>
    /// Checks if the given value is within the <paramref name="min"/> and <paramref name="max"/> parameters. (float)
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="min">The minimum that <paramref name="value"/> has to be within.</param>
    /// <param name="max">The maximum that <paramref name="value"/> has to be within.</param>
    /// <param name="exact">Is false by default. Will also count the value as in range if it's exactly <paramref name="min"/> or <paramref name="max"/>.</param>
    /// <returns>If the value is within the range.</returns>
    public static bool ValueWithinRange(float value, float min, float max, bool exact = false)
    {
        return 
            exact ?
            value >= min && value <= max 
            : 
            value > min && value < max;
    }

    /// <summary>
    /// Copies the component <paramref name="original"/> to the <paramref name="destination"/> gameobject.
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    /// <param name="original">The component to copy.</param>
    /// <param name="destination">The gameobject that recieves the copies component.</param>
    /// <returns>The new copied component</returns>
    public static T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        // Store the component type
        System.Type type = original.GetType();

        // Create a new component on the destination gameobject
        Component copy = destination.AddComponent(type);

        // Copy the values
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }

        // Return the component
        return copy as T;
    }

    #region Array Methods
    /// <summary>
    /// Returns a random element in the given array
    /// </summary>
    public static T GetRandomArrayElement<T>(T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }


    /// <summary>
    /// Returns true if <paramref name="index"/> is above or equal to 0 and is below the length of <paramref name="array"/> - 1
    /// </summary>
    public static bool ValueInRangeOfArray<T>(int index, T[] array)
    {
        return index >= 0 && index <= array.Length - 1;
    }

    /// <summary>
    /// Returns and clamps the <paramref name="index"/> between 0 and the length of <paramref name="array"/> - 1
    /// </summary>
    public static int ClampIndexInArray<T>(int index, T[] array)
    {
        return Mathf.Clamp(index, 0, array.Length - 1);
    }

    /// <summary>
    /// Returns and loops the <paramref name="index"/> between 0 and the length of <paramref name="array"/> - 1
    /// </summary>
    public static int LoopIndexInArray<T>(int index, T[] array)
    {
        return LoopValue(index, 0, array.Length - 1);
    }
    #endregion

    #region Speed Calculation
    /// <summary>
    /// Methods useful for calculating speed. Or not useful cause they are all really easy. So I guess this is here for refreshing memory?
    /// </summary>
    public struct Speed
    {
        /// <summary>
        /// Distance / Speed = Time
        /// </summary>
        public static float GetTime(float distance, float speed)
        {
            return distance / speed;
        }
        /// <summary>
        /// Distance / Time = Speed
        /// </summary>
        public static float GetSpeed(float distance, float time)
        {
            return distance / time;
        }
        /// <summary>
        /// Speed * Time = Distance
        /// </summary>
        public static float GetDistance(float speed, float time)
        {
            return speed * time;
        }
    }
    #endregion

    #region Proportion Calculation
    /// <summary>
    /// Methods useful for calculating proportion. Or not useful cause they are all really easy. So I guess this is here for refreshing memory?
    /// </summary>
    public struct Proportion
    {
        /// <summary>
        /// Part / Whole = Proportion;
        /// </summary>
        public static float GetProportion(float part, float whole)
        {
            return part / whole;
        }
        /// <summary>
        /// Whole * Proportion = Part
        /// </summary>
        public static float GetPart(float whole, float proportion)
        {
            return whole * proportion;
        }
        /// <summary>
        /// Part / Proportion = Whole
        /// </summary>
        public static float GetWhole(float part, float proportion)
        {
            return part / proportion;
        }
    }
    #endregion

    #region Vector array conversion methods
    /// <summary>
    /// Converts a Vector3 array to a Vector2 array. 
    /// The Z position is discarded.
    /// </summary>
    public static Vector2[] ToVector2Array(Vector3[] v3)
    {
        return System.Array.ConvertAll(v3, GetV3fromV2);
    }
    /// <summary>
    /// (USELESS) Converts a Vector3 to a Vector2.
    /// Just use (Vector3)vector2 instead.
    /// </summary>
    public static Vector2 GetV3fromV2(Vector3 v3)
    {
        return new Vector2(v3.x, v3.y);
    }
    /// <summary>
    /// Converts a Vector2 array to a Vector4 array.
    /// The Z position is 0.
    /// </summary>
    public static Vector3[] ToVector3Array(Vector2[] v3)
    {
        return System.Array.ConvertAll(v3, GetV2fromV3);
    }
    /// <summary>
    /// (USELESS) Converts a Vector2 to a Vector3.
    /// Just use (Vector2)vector3 instead.
    /// </summary>
    public static Vector3 GetV2fromV3(Vector2 v2)
    {
        return new Vector3(v2.x, v2.y, 0);
    }
    #endregion

    #region Rotation methods
    /// <summary>
    /// Returns a quaternion with only the Z rotation set. Useful for 2D games where the other axes don't matter that much.
    /// </summary>
    public static Quaternion Get2DQuaternion(float zRot)
    {
        return Quaternion.Euler(0, 0, zRot);
    }
    /// <summary>
    /// Returns a Vector3 with only the Z rotation set. Useful for 2D games where the other axes don't matter that much.
    /// </summary>
    public static Vector3 Get2DVectorRotation(float zRot)
    {
        return new Vector3(0, 0, zRot);
    }

    /// <summary>
    /// Returns only the Z rotation of the euler angles. Useful for 2D games where the other axes don't matter that much.
    /// </summary>
    public static float GetZRot(Quaternion quaternion)
    {
        return quaternion.eulerAngles.z;
    }

    /// <summary>
    /// Rotates the given <paramref name="point"/> around the <paramref name="pivot"/> by the given <paramref name="angles"/>.
    /// </summary>
    /// <param name="point">The point that'll be rotated.</param>
    /// <param name="pivot">The pivot that the point will be rotated around.</param>
    /// <param name="angles">The amount the point will rotate around the pivot.</param>
    /// <returns>The rotated point.</returns>
    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        // Get the direction of the point and pivot
        Vector3 dir = point - pivot;

        // Rotate the point by angles
        dir = Quaternion.Euler(angles) * dir;

        // Calculate the new point by setting it to pivot and offseting it
        point = pivot + dir;

        // Return the new point
        return point;
    }
    #endregion

    #region Middle methods
    /// <summary>
    /// Returns the middle point of all values (int)
    /// </summary>
    public static int GetMiddle(params int[] values)
    {
        int totalValue = 0;

        // Loop through all the values and add them all together
        foreach (int value in values)
        {
            totalValue += value;
        }

        // Give the middle of all the values added together by dividing by the amount of values in the array
        return totalValue /= values.Length;
    }
    /// <summary>
    /// Returns the middle point of all values (float)
    /// </summary>
    public static float GetMiddle(params float[] values)
    {
        float totalValue = 0;

        // Loop through all the values and add them all together
        foreach (float value in values)
        {
            totalValue += value;
        }

        // Give the middle of all the values added together by dividing by the amount of values in the array
        return totalValue /= values.Length;
    }

    /// <summary>
    /// Returns the middle point of both vectors
    /// </summary>
    public static Vector2 GetMidPoint(Vector2 point1, Vector2 point2)
    {
        return new Vector2(GetMiddle(point1.x, point2.x), GetMiddle(point1.y, point2.y));
    }
    #endregion

    #region Rounding Vectors
    /// <summary>
    /// Rounds a vector3 on all axes
    /// </summary>
    /// <param name="vector">The vector that'll be rounded</param>
    /// <returns>A rounded version of the vector given</returns>
    public static Vector3 RoundVector3(Vector3 vector)
    {
        return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
    }
    /// <summary>
    /// Rounds a vector2 on all axes
    /// </summary>
    /// <param name="vector">The vector that'll be rounded</param>
    /// <returns>A rounded version of the vector given</returns>
    public static Vector2 RoundVector2(Vector2 vector)
    {
        return new Vector2(Mathf.Round(vector.x), Mathf.Round(vector.y));
    }
    #endregion

    #region Digitising Vectors
    /// <summary>
    /// Digitises a vector3 on all axes
    /// </summary>
    /// <param name="vector">The vector that'll be digitised</param>
    /// <returns>A digitised version of the vector given</returns>
    public static Vector2 DigitiseVector2(Vector2 vector, int slices)
    {
        return new Vector2(Digitise(vector.x, slices), Digitise(vector.y, slices));
    }
    /// <summary>
    /// Digitises a vector2 on all axes
    /// </summary>
    /// <param name="vector">The vector that'll be digitised</param>
    /// <returns>A digitised version of the vector given</returns>
    public static Vector3 DigitiseVector3(Vector3 vector, int slices)
    {
        return new Vector3(Digitise(vector.x, slices), Digitise(vector.y, slices), Digitise(vector.z, slices));
    }
    #endregion

    #region Timers
    public delegate void TimerEvent();

    /// <summary>
    /// Starts a timer coroutine on the given MonoBehaviour that is delayed by the specified time (Shortcut)
    /// </summary>
    /// <param name="executeOn">The script to execute the coroutine on</param>
    /// <param name="time">The time to wait</param>
    /// <param name="onComplete">Called when the timer is finished</param>
    /// <param name="unscaledTime">Will use unscaled time if true</param>
    /// <returns>The coroutine ran</returns>
    public static Coroutine TimerSeconds(MonoBehaviour executeOn, float time, TimerEvent onComplete = null, bool unscaledTime = false)
    {
        if (!unscaledTime)
        {
            return Timer(executeOn, new WaitForSeconds(time), onComplete);
        } else
        {
            return Timer(executeOn, new WaitForSecondsRealtime(time), onComplete);
        }
    }

    /// <summary>
    /// Starts a timer coroutine on the given MonoBehaviour that waits for the end of frame (Shortcut)
    /// </summary>
    /// <param name="executeOn">The script to execute the coroutine on</param>
    /// <param name="onComplete">Called when the timer is finished</param>
    /// <param name="fixedFrame">Will instead wait for fixed update if true</param>
    /// <returns>The coroutine ran</returns>
    public static Coroutine TimerEndOfFrame(MonoBehaviour executeOn, TimerEvent onComplete = null, bool fixedFrame = false)
    {
        if (!fixedFrame)
        {
            return Timer(executeOn, new WaitForEndOfFrame(), onComplete);
        }
        else
        {
            return Timer(executeOn, new WaitForFixedUpdate(), onComplete);
        }
    }

    /// <summary>
    /// Starts a timer coroutine on the given MonoBehaviour (YieldInstruction)
    /// </summary>
    /// <param name="executeOn">The script to execute the coroutine on</param>
    /// <param name="instruction">The instruction to preform</param>
    /// <param name="onComplete">Called when the timer is finished</param>
    /// <returns>The coroutine ran</returns>
    public static Coroutine Timer(MonoBehaviour executeOn, YieldInstruction instruction, TimerEvent onComplete = null)
    {
        return executeOn.StartCoroutine(TimerIEnumrator(instruction, onComplete));
    }
    public static IEnumerator TimerIEnumrator(YieldInstruction instruction, TimerEvent onComplete = null)
    {
        yield return instruction;

        onComplete?.Invoke();
    }

    /// <summary>
    /// Starts a timer coroutine on the given MonoBehaviour (CustomYieldInstruction)
    /// </summary>
    /// <param name="executeOn">The script to execute the coroutine on</param>
    /// <param name="instruction">The instruction to preform</param>
    /// <param name="onComplete">Called when the timer is finished</param>
    /// <returns>The coroutine ran</returns>
    public static Coroutine Timer(MonoBehaviour executeOn, CustomYieldInstruction instruction, TimerEvent onComplete = null)
    {
        return executeOn.StartCoroutine(TimerIEnumrator(instruction, onComplete));
    }
    public static IEnumerator TimerIEnumrator(CustomYieldInstruction instruction, TimerEvent onComplete = null)
    {
        yield return instruction;

        onComplete?.Invoke();
    }
    #endregion

    #region String Related Methods
    public static string FilterString(string str, char[] charsToRemove)
    {
        foreach (char c in charsToRemove)
        {
            str = str.Replace(c.ToString(), string.Empty);
        }

        return str;
    }
    #endregion

    //Really useful calculations inspired by game builder garage
    #region Game Builder Garage inspired methods
    /// <summary>
    /// Maps a value from one range to another.
    /// (From Game Builder Garage lololol)
    /// </summary>
    /// <param name="OldMin">The old min value range</param>
    /// <param name="OldMax">The old max value range</param>
    /// <param name="NewMin">The new min value range</param>
    /// <param name="NewMax">The new max value range</param>
    /// <param name="OldValue">The old value that'll be converted to the new range</param>
    /// <returns>The OldValue but in the new range</returns>
    public static float Map(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {
        float OldRange = OldMax - OldMin;
        float NewRange = NewMax - NewMin;
        float NewValue = ((OldValue - OldMin) * NewRange / OldRange) + NewMin;

        return NewValue;
    }

    /// <summary>
    /// A more advanced rounding function basically
    /// (From Game Builder Garage lololol)
    /// </summary>
    /// <returns>The value given but rounded to be the nearest sliced point</returns>
    public static float Digitise(float value, int slices = 2)
    {
        if (slices < 2)
        {
            return Mathf.Floor(value);
        }

        float scale = 1 / ((float)slices - 1);

        List<float> points = new List<float>();

        for (int i = 0; i < slices; i++)
        {
            points.Add(i * scale + Mathf.Floor(value));
        }

        float closestRange = float.PositiveInfinity;
        float currentValue = 0;
        foreach (float f in points)
        {
            float dist = Distance(f, value);
            if (dist < closestRange)
            {
                closestRange = dist;
                currentValue = f;
            }
        }

        return currentValue;
    }

    /// <summary>
    /// Converts an angle to a normal direction.
    /// (From Game Builder Garage lololol)
    /// <para>Use the <paramref name="convertToRadians"/> parameter to determine whether the angle should be converted to radians or not (is true by default)</para>
    /// </summary>
    /// <returns>A Vector2 where X is the Cos of the angle and the Y is the Sin of the angle</returns>
    public static Vector2 AngleToNormal(float angle, bool convertToRadians = true)
    {
        // Determine wheter or no the angle is in radians
        float multiplier = convertToRadians ? Mathf.Deg2Rad : 1;

        return new Vector2(Mathf.Cos(angle * multiplier), Mathf.Sin(angle * multiplier));
    }

    /// <summary>
    /// Converts a position into an angle (for 2D). 
    /// The origin will point towards the target.
    /// Add 90 more degrees to get the raw rotation.
    /// (From Game Builder Garage lololol)
    /// </summary>
    public static float PositionToAngle2D(Vector2 target, Vector2 origin)
    {
        //Subtract the target and origin positions from eachother
        target.x = target.x - origin.x;
        target.y = target.y - origin.y;

        //Get the angle using Mathf.Atan2
        //Convert it from Radians to Degrees by multiplying it with the Mathf.Rad2Deg constant
        //Also subtract 90 cause otherwise the angle is not working as expected
        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg - 90;
        return angle;
    }

    /// <summary>
    /// Basically returns true whenever the moment the value it's given is changed. 
    /// Uses generics so any type works!
    /// (From Game Builder Garage lololol)
    /// </summary>
    //Haha yes TriggerOnChange is one of my favourite new tools I have made
    //I am way too proud of this
    public class TriggerOnChange<T>
    {
        /// <summary>
        /// The old value
        /// </summary>
        public T oldValue;

        /// <summary>
        /// Invoked when the value is changed
        /// </summary>
        public delegate void TriggerEvent(T oldValue, T newValue);
        public event TriggerEvent OnValueChanged;

        /// <summary>
        /// Returns true if the newValue is changed.
        /// Also invokes the OnValueChanged event (If it's not null)
        /// </summary>
        public bool Check(T newValue)
        {
            //If the old value is NOT equal to the new value then it's changed!
            if (!newValue.Equals(oldValue))
            {
                //Set the old value to the new value and invoke the event (if it's not null)
                OnValueChanged?.Invoke(oldValue, newValue); 
                oldValue = newValue;

                //Return true
                return true;
            }

            //Return false
            return false;
        }

        public TriggerOnChange() { }

        public TriggerOnChange(T startValue)
        {
            oldValue = startValue;
        }
    }
    #endregion
}
