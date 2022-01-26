using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Input;

namespace Game.Player
{
    //=========================================================================
    /// <summary>
    /// Handles all of the player input. Does not extend <see cref="PlayerScript"/>. <para/>
    /// Btw this is a script I copy from game to game, so that's why some stuff is pretty unnecessary.
    /// </summary>
    //=========================================================================
    public class PlayerInput : MonoBehaviour
    {
        public const int keycodeLength = 510;

        private static Key[] defaultKeys = new Key[]
        {
            //-- The main gameplay button
            new Key("Click",
                Key.NewKeys(KeyboardKey.UpArrow, KeyboardKey.W, KeyboardKey.LeftMouseButton, KeyboardKey.Space, KeyboardKey.Return, KeyboardKey.KeypadEnter),
                Key.NewKeys(GamepadKey.AButton, GamepadKey.BButton, GamepadKey.XButton, GamepadKey.YButton),
                3),

            //-- Other
            new Key("Pause",
                Key.NewKeys(KeyboardKey.Escape),
                Key.NewKeys(GamepadKey.Start),
                3),

            //-- UI Navigation Keys
            new Key("Left",
                Key.NewKeys(KeyboardKey.A, KeyboardKey.LeftArrow),
                Key.NewKeys(GamepadKey.LeftStickLeft, GamepadKey.DpadLeft),
                3),

            new Key("Right",
                Key.NewKeys(KeyboardKey.D, KeyboardKey.RightArrow),
                Key.NewKeys(GamepadKey.LeftStickRight, GamepadKey.DpadRight),
                3),

            new Key("Up",
                Key.NewKeys(KeyboardKey.W, KeyboardKey.UpArrow),
                Key.NewKeys(GamepadKey.LeftStickUp, GamepadKey.DpadUp),
                3),

            new Key("Down",
                Key.NewKeys(KeyboardKey.S, KeyboardKey.DownArrow),
                Key.NewKeys(GamepadKey.LeftStickDown, GamepadKey.DpadDown),
                3),

            new Key("Submit",
                Key.NewKeys(KeyboardKey.Return),
                Key.NewKeys(GamepadKey.AButton),
                3),

            new Key("Cancel",
                Key.NewKeys(KeyboardKey.Backspace),
                Key.NewKeys(GamepadKey.BButton),
                3),
        };

        public static Key[] DefaultKeys { get { return defaultKeys; } }

        public static Key[] keys = DefaultKeys;

        public static float triggerDeadZone = 0.5f;
        public static float joystickDeadzone = 0.5f;
        public static float dpadDeadzone = 0.5f;

        //-- Special Gamepad Input (For the different press modes)
        private static float badTriggerInput;
        private static float leftTriggerInput;
        private static float rightTriggerInput;
        private static Vector2 leftStickInput;
        private static Vector2 rightStickInput;
        private static Vector2 dpadInput;

        private static float exactBadTriggerInput;
        private static float exactLeftTriggerInput;
        private static float exactRightTriggerInput;
        private static Vector2 exactLeftStickInput;
        private static Vector2 exactRightStickInput;
        private static Vector2 exactDpadInput;

        //-- Old Special Gamepad Input
        private static float oldBadTriggerInput;
        private static float oldLeftTriggerInput;
        private static float oldRightTriggerInput;
        private static Vector2 oldLeftStickInput;
        private static Vector2 oldRightStickInput;
        private static Vector2 oldDpadInput;

        //-- Public Shortcuts
        public static float LeftTriggerInput { get { return exactLeftTriggerInput; } }
        public static float RightTriggerInput { get { return exactRightTriggerInput; } }
        public static Vector2 LeftStickInput { get { return exactLeftStickInput; } }
        public static Vector2 RightStickInput { get { return exactRightStickInput; } }
        public static Vector2 DpadInput { get { return exactDpadInput; } }

        // Start is called before the first frame update
        void Start()
        {
            // Reset all of the Special Gamepad Inputs because they are static
            badTriggerInput = 0;
            leftTriggerInput = 0;
            rightTriggerInput = 0;
            leftStickInput = Vector2.zero;
            rightStickInput = Vector2.zero;
            dpadInput = Vector2.zero;

            // Also reset the Old Special Gamepad Input
            oldBadTriggerInput = 0;
            oldLeftTriggerInput = 0;
            oldRightTriggerInput = 0;
            oldLeftStickInput = Vector2.zero;
            oldRightStickInput = Vector2.zero;
            oldDpadInput = Vector2.zero;
        }

        // Update is called once per frame
        void Update()
        {
            DoAxisInput();
        }

        /// <summary>
        /// Is called every frame in <see cref="Update"/> <para/>
        /// Sets values for all of the special axis inputs like gamepad trigger input and dpad input for example.
        /// </summary>
        private void DoAxisInput()
        {
            // Update the old inputs to be the current inputs
            oldBadTriggerInput = badTriggerInput;
            oldLeftTriggerInput = leftTriggerInput;
            oldRightTriggerInput = rightTriggerInput;
            oldLeftStickInput = leftStickInput;
            oldRightStickInput = rightStickInput;
            oldDpadInput = dpadInput;

            // Update the inputs to be new so the old ones become old
            exactBadTriggerInput = UnityEngine.Input.GetAxis("Trigger");
            exactLeftTriggerInput = Mathf.Abs(UnityEngine.Input.GetAxis("Left Trigger"));
            exactRightTriggerInput = Mathf.Abs(UnityEngine.Input.GetAxis("Right Trigger"));
            exactLeftStickInput = new Vector2(UnityEngine.Input.GetAxis("Joystick Horizontal"), UnityEngine.Input.GetAxis("Joystick Vertical"));
            exactRightStickInput = new Vector2(UnityEngine.Input.GetAxis("Right Joystick Horizontal"), UnityEngine.Input.GetAxis("Right Joystick Vertical"));
            exactDpadInput = new Vector2(UnityEngine.Input.GetAxis("Dpad X"), UnityEngine.Input.GetAxis("Dpad Y"));

            // Make the inputs 0 if they are within their dead zone
            // Also round the input values whilst the exact input values will stay the same
            // Triggers
            exactBadTriggerInput = LockAxis(exactBadTriggerInput, triggerDeadZone);
            badTriggerInput = Mathf.Round(exactBadTriggerInput);
            exactLeftTriggerInput = LockAxis(exactLeftTriggerInput, triggerDeadZone);
            leftTriggerInput = Mathf.Round(exactLeftTriggerInput);
            exactRightTriggerInput = LockAxis(exactRightTriggerInput, triggerDeadZone);
            rightTriggerInput = Mathf.Round(exactRightTriggerInput);

            // Left stick
            exactLeftStickInput = LockVector2Axis(exactLeftStickInput, joystickDeadzone);
            leftStickInput = MathE.RoundVector2(exactLeftStickInput);

            // Right stick
            exactRightStickInput = LockVector2Axis(exactRightStickInput, joystickDeadzone);
            rightStickInput = MathE.RoundVector2(exactRightStickInput);

            // Dpad
            exactDpadInput = LockVector2Axis(exactDpadInput, dpadDeadzone);
            dpadInput = MathE.RoundVector2(exactDpadInput);
        }

        /// <summary>
        /// Will check if <paramref name="axisValue"/> is within the <paramref name="deadZone"/> and set it to 0 if it's inside the range.
        /// </summary>
        /// <param name="axisValue">The given axis.</param>
        /// <param name="deadZone">The zone where input should be completely ignored/set to 0.</param>
        /// <returns>The <paramref name="axisValue"/> unless it's inside the <paramref name="deadZone"/> range, then it'll return 0.</returns>
        private float LockAxis(float axisValue, float deadZone)
        {
            bool setTo0 = MathE.ValueWithinRange(axisValue, -deadZone, deadZone);

            float lockedAxisValue = setTo0 ? 0 : axisValue;

            return lockedAxisValue;
        }

        /// <summary>
        /// Will check if <paramref name="axisValue"/> is within the <paramref name="deadZone"/> and set it to 0 if it's inside the range.<para/>
        /// Basically the same as <see cref="LockAxis(float, float)"/> but with <see cref="Vector2"/> instead.
        /// </summary>
        /// <param name="axisValue">The given axis.</param>
        /// <param name="deadZone">The zone where input should be completely ignored/set to 0.</param>
        /// <returns>The <paramref name="axisValue"/> unless it's inside the <paramref name="deadZone"/> range, then it'll return 0.</returns>
        private Vector2 LockVector2Axis(Vector2 axisValue, float deadZone)
        {
            return new Vector2(LockAxis(axisValue.x, deadZone), LockAxis(axisValue.y, deadZone));
        }

        /// <summary>
        /// Loops through <see cref="keys"/> until it finds a key with a name that matches <paramref name="name"/> exactly. (Unless <paramref name="caseSensitive"/> is set to false)
        /// </summary>
        /// <param name="name">The name of the key that we should search for.</param>
        /// <param name="caseSensitive">True by defualt.<para/>
        /// If true, then the names must match perfectly, so "hello" and "HeLlO" won't match. (Notice the capital letters)</param>
        /// <returns>The key that was found. Will be null if no key was found.</returns>
        public static Key GetKey(string name, bool caseSensitive = true)
        {
            // Loop through all of the keys
            foreach (Key key in keys)
            {
                // Check if the names match (Case sensitive)
                if (key.name == name && caseSensitive)
                {
                    // Return the key if the names match
                    return key;
                }

                // Check if the names match (Not case sensitive)
                if (key.name.ToLower() == name.ToLower() && !caseSensitive)
                {
                    // Return the key if the names match
                    return key;
                }
            }

            // If no key was found, return null
            return null;
        }

        /// <summary>
        /// Checks if <paramref name="key"/> is being held down, has just been pressed down or if the value has just been released. (Depends on <paramref name="mode"/>) <para/>
        /// This one is only for <seealso cref="KeyboardKey"/> input.
        /// </summary>
        /// <param name="key">The <seealso cref="KeyboardKey"/> that'll be checked.</param>
        /// <param name="mode">Is <seealso cref="PressMode.down"/> by default. The press mode to check.</param>
        /// <returns>True if <paramref name="key"/> was held/just pressed/just released. (Depends on <paramref name="mode"/>)</returns>
        public static bool KeyPressed(KeyboardKey key, PressMode mode = PressMode.down)
        {
            // Return false if the key is none
            if (key == KeyboardKey.None)
                return false;

            // If the key is outside the keycode length then return false
            if ((int)key >= keycodeLength - 1)
            {
                return false;
            }

            // Check if the key is pressed but in KeyCode form instead
            return KeyPressed((KeyCode)key, mode);
        }

        /// <summary>
        /// Checks if <paramref name="key"/> is being held down, has just been pressed down or if the value has just been released. (Depends on <paramref name="mode"/>) <para/>
        /// This one is only for <seealso cref="GamepadKey"/> input.
        /// </summary>
        /// <param name="key">The <seealso cref="GamepadKey"/> that'll be checked.</param>
        /// <param name="mode">Is <seealso cref="PressMode.down"/> by default. The press mode to check.</param>
        /// <returns>True if <paramref name="key"/> was held/just pressed/just released. (Depends on <paramref name="mode"/>)</returns>
        public static bool KeyPressed(GamepadKey key, PressMode mode = PressMode.down)
        {
            // Return false if the key is none
            if (key == GamepadKey.None)
                return false;

            // If the key is outside the keycode length then it's one of the special gamepad keys so check those
            if ((int)key >= keycodeLength - 1)
            {
                // Check all of the special gamepad keys
                switch (key)
                {
                    // Triggers
                    case GamepadKey.LeftTrigger:
                        bool leftTriggerPressed = ValuePressed(leftTriggerInput, oldLeftTriggerInput, mode, false);

                        if (!leftTriggerPressed)
                            leftTriggerPressed = ValuePressed(badTriggerInput, oldBadTriggerInput, mode, true);

                        return leftTriggerPressed;

                    case GamepadKey.RightTrigger:
                        bool rightTriggerPressed = ValuePressed(rightTriggerInput, oldRightTriggerInput, mode, false);

                        if (!rightTriggerPressed)
                            rightTriggerPressed = ValuePressed(badTriggerInput, oldBadTriggerInput, mode, false);

                        return rightTriggerPressed;

                    // Left Stick
                    case GamepadKey.LeftStickUp:
                        return ValuePressed(leftStickInput.y, oldLeftStickInput.y, mode, false);
                    case GamepadKey.LeftStickDown:
                        return ValuePressed(leftStickInput.y, oldLeftStickInput.y, mode, true);
                    case GamepadKey.LeftStickLeft:
                        return ValuePressed(leftStickInput.x, oldLeftStickInput.x, mode, true);
                    case GamepadKey.LeftStickRight:
                        return ValuePressed(leftStickInput.x, oldLeftStickInput.x, mode, false);

                    // Right Stick
                    case GamepadKey.RightStickUp:
                        return ValuePressed(rightStickInput.y, oldRightStickInput.y, mode, false);
                    case GamepadKey.RightStickDown:
                        return ValuePressed(rightStickInput.y, oldRightStickInput.y, mode, true);
                    case GamepadKey.RightStickLeft:
                        return ValuePressed(rightStickInput.x, oldRightStickInput.x, mode, true);
                    case GamepadKey.RightStickRight:
                        return ValuePressed(rightStickInput.x, oldRightStickInput.x, mode, false);

                    // Dpad
                    case GamepadKey.DpadUp:
                        return ValuePressed(dpadInput.y, oldDpadInput.y, mode, false);
                    case GamepadKey.DpadDown:
                        return ValuePressed(dpadInput.y, oldDpadInput.y, mode, true);
                    case GamepadKey.DpadLeft:
                        return ValuePressed(dpadInput.x, oldDpadInput.x, mode, true);
                    case GamepadKey.DpadRight:
                        return ValuePressed(dpadInput.x, oldDpadInput.x, mode, false);

                    // Return false if no valid GamepadKey was given. (Which shouldn't happen)
                    default:
                        return false;
                }
            }

            // Check if the key is pressed but in KeyCode form instead
            return KeyPressed((KeyCode)key, mode);
        }

        /// <summary>
        /// Checks if <paramref name="key"/> is being held down, has just been pressed down or if the value has just been released. (Depends on <paramref name="mode"/>) <para/>
        /// This one is only for <seealso cref="KeyCode"/> input.
        /// </summary>
        /// <param name="key">The <seealso cref="KeyCode"/> that'll be checked.</param>
        /// <param name="mode">Is <seealso cref="PressMode.down"/> by default. The press mode to check.</param>
        /// <returns>True if <paramref name="key"/> was held/just pressed/just released. (Depends on <paramref name="mode"/>)</returns>
        public static bool KeyPressed(KeyCode key, PressMode mode = PressMode.down)
        {
            // Use a switch statement to change the input method to be correct
            switch (mode)
            {
                // Check if the key is held down at all
                case PressMode.hold:
                    return UnityEngine.Input.GetKey(key);

                // Check if the key has just been pressed
                case PressMode.down:
                    return UnityEngine.Input.GetKeyDown(key);

                // Check if the key has just been released
                case PressMode.up:
                    return UnityEngine.Input.GetKeyUp(key);

                // Return false if no valid PressMode was given. (Which shouldn't happen)
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if <paramref name="value"/> is being held down, has just been pressed down or if the value has just been released. (Depends on <paramref name="mode"/>)
        /// </summary>
        /// <param name="value">The value that'll be checked.</param>
        /// <param name="oldValue">The value one frame before <paramref name="value"/> has been set. Is only used when <paramref name="mode"/> is <seealso cref="PressMode.down"/> or <seealso cref="PressMode.up"/>.</param>
        /// <param name="mode">The press mode to check.</param>
        /// <param name="lessThan0">If true, then we'll check if the values are less than 0. Otherwise we check if they are above 0.</param>
        /// <returns>True if the value was held/just pressed/just released. (Depends on <paramref name="mode"/>)</returns>
        public static bool ValuePressed(float value, float oldValue, PressMode mode, bool lessThan0)
        {
            // Check the different press modes
            switch (mode)
            {
                // Check if the value is held down at all
                case PressMode.hold:
                    return lessThan0 ? value < 0 : value > 0;

                // Check if the value has just been pressed
                case PressMode.down:
                    return (lessThan0 ? value < 0 : value > 0) && oldValue == 0;

                // Check if the value has just been released
                case PressMode.up:
                    return value == 0 && oldValue == (lessThan0 ? -1 : 1);

                // Return false if no valid PressMode was given. (Which shouldn't happen)
                default:
                    return false;
            }
        }

        /// <summary>
        /// Will check if any keys at all were pressed, so both keyboard and gamepad.
        /// </summary>
        /// <param name="mode">Is <seealso cref="PressMode.down"/> by default. The press mode to check.</param>
        /// <param name="checkSpecialKeys">True by default. If we should check the special gamepad keys.</param>
        /// <returns>If any keyboard or gamepad keys were pressed.</returns>
        public static bool PressedAny(PressMode mode = PressMode.down, bool checkSpecialKeys = true)
        {
            return PressedKeyboard(mode) || PressedGamepad(mode, checkSpecialKeys);
        }


        /// <summary>
        /// Will check if any keyboard buttons were pressed.
        /// </summary>
        /// <param name="mode">Is <seealso cref="PressMode.down"/> by default. The press mode to check.</param>
        /// <returns>If any keyboard buttons were pressed.</returns>
        public static bool PressedKeyboard(PressMode mode = PressMode.down)
        {
            // Loop through all the KeyboardKey in the KeyboardKey enum
            foreach (KeyboardKey item in Enum.GetValues(typeof(KeyboardKey)))
            {
                // If the key was pressed, then return true
                if (KeyPressed(item, mode))
                {
                    return true;
                }
            }

            // Return false if no KeyboardKey was pressed
            return false;
        }

        /// <summary>
        /// Will check if any gamepad buttons were pressed. <para/>
        /// This also includes the special gamepad keys. So if the triggers have been pressed, if the joysticks has been moved or if the dpad has moved. <para/>
        /// To not check the special gamepad keys, then set <paramref name="checkSpecialKeys"/> to false.
        /// </summary>
        /// <param name="mode">The press mode to check. Is <seealso cref="PressMode.down"/> by default.</param>
        /// <param name="checkSpecialKeys">True by default. If we should check the special gamepad keys.</param>
        /// <returns>If any gamepad buttons were pressed.</returns>
        public static bool PressedGamepad(PressMode mode = PressMode.down, bool checkSpecialKeys = true)
        {
            // First check all the regular joystick buttons from "JoystickButton0" to "Joystick8Button19"
            for (int i = (int)KeyCode.JoystickButton0; i <= (int)KeyCode.Joystick8Button19; i++)
            {
                // If the key was pressed, then return true
                if (KeyPressed((KeyCode)i, mode))
                {
                    return true;
                }
            }

            // Only check special keys if 
            if (checkSpecialKeys)
            {
                // Loop through all the special keys
                for (int i = (int)GamepadKey.LeftTrigger; i <= (int)GamepadKey.DpadRight; i++)
                {
                    // If the key was pressed, then return true
                    if (KeyPressed((GamepadKey)i, mode))
                    {
                        return true;
                    }
                }
            }

            // Return false if no GamepadKey was pressed
            return false;
        }
    }
}
