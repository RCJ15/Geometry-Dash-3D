using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.CustomInput;

namespace GD3D.Player
{
    //=========================================================================
    /// <summary>
    /// Handles all of the player input. Does not extend <see cref="PlayerScript"/>. <para/>
    /// This is a script I copy from game to game, so that's why some stuff is pretty unnecessary.
    /// </summary>
    //=========================================================================
    public class PlayerInput : MonoBehaviour
    {
        public const int KEYCODE_LENGTH = 510;

        private static Key[] s_defaultKeys = new Key[]
        {
            //-- The main gameplay button
            new Key("Click",
                Key.NewKeys(KeyboardKey.UpArrow, KeyboardKey.W, KeyboardKey.LeftMouseButton, KeyboardKey.Space, KeyboardKey.Return, KeyboardKey.KeypadEnter),
                Key.NewKeys(GamepadKey.AButton)),
            
            //-- Practice Mode
            new Key("Place Checkpoint Crystal",
                Key.NewKeys(KeyboardKey.Z),
                Key.NewKeys(GamepadKey.XButton)),

            new Key("Remove Checkpoint Crystal",
                Key.NewKeys(KeyboardKey.X),
                Key.NewKeys(GamepadKey.BButton)),

            //-- 3D Mode
            new Key("3D Move Left",
                Key.NewKeys(KeyboardKey.LeftArrow, KeyboardKey.A),
                Key.NewKeys(GamepadKey.LeftStickLeft, GamepadKey.DpadLeft)),

            new Key("3D Move Right",
                Key.NewKeys(KeyboardKey.RightArrow, KeyboardKey.D),
                Key.NewKeys(GamepadKey.LeftStickRight, GamepadKey.DpadRight)),

            //-- Other
            new Key("Escape",
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

        public static Key[] DefaultKeys => s_defaultKeys;

        public static Key[] Keys = DefaultKeys;

        public static float TriggerDeadZone = 0.5f;
        public static float JoystickDeadzone = 0.5f;
        public static float DpadDeadzone = 0.5f;

        //-- Special Gamepad Input (For the different press modes)
        private static float s_badTriggerInput;
        private static float s_leftTriggerInput;
        private static float s_rightTriggerInput;
        private static Vector2 s_leftStickInput;
        private static Vector2 s_rightStickInput;
        private static Vector2 s_dpadInput;

        private static float s_exactBadTriggerInput;
        private static float s_exactLeftTriggerInput;
        private static float s_exactRightTriggerInput;
        private static Vector2 s_exactLeftStickInput;
        private static Vector2 s_exactRightStickInput;
        private static Vector2 s_exactDpadInput;

        //-- Old Special Gamepad Input
        private static float s_oldBadTriggerInput;
        private static float s_oldLeftTriggerInput;
        private static float s_oldRightTriggerInput;
        private static Vector2 s_oldLeftStickInput;
        private static Vector2 s_oldRightStickInput;
        private static Vector2 s_oldDpadInput;

        //-- Public Shortcuts
        public static float LeftTriggerInput => s_exactLeftTriggerInput;
        public static float RightTriggerInput => s_exactRightTriggerInput;
        public static Vector2 LeftStickInput => s_exactLeftStickInput;
        public static Vector2 RightStickInput => s_exactRightStickInput;
        public static Vector2 DpadInput => s_exactDpadInput;

        private void Start()
        {
            // Reset all of the Special Gamepad Inputs because they are static
            s_badTriggerInput = 0;
            s_leftTriggerInput = 0;
            s_rightTriggerInput = 0;
            s_leftStickInput = Vector2.zero;
            s_rightStickInput = Vector2.zero;
            s_dpadInput = Vector2.zero;

            // Also reset the Old Special Gamepad Input
            s_oldBadTriggerInput = 0;
            s_oldLeftTriggerInput = 0;
            s_oldRightTriggerInput = 0;
            s_oldLeftStickInput = Vector2.zero;
            s_oldRightStickInput = Vector2.zero;
            s_oldDpadInput = Vector2.zero;
        }

        private void Update()
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
            s_oldBadTriggerInput = s_badTriggerInput;
            s_oldLeftTriggerInput = s_leftTriggerInput;
            s_oldRightTriggerInput = s_rightTriggerInput;
            s_oldLeftStickInput = s_leftStickInput;
            s_oldRightStickInput = s_rightStickInput;
            s_oldDpadInput = s_dpadInput;

            // Update the inputs to be new so the old ones become old
            s_exactBadTriggerInput = Input.GetAxis("Trigger");
            s_exactLeftTriggerInput = Mathf.Abs(Input.GetAxis("Left Trigger"));
            s_exactRightTriggerInput = Mathf.Abs(Input.GetAxis("Right Trigger"));
            s_exactLeftStickInput = new Vector2(Input.GetAxis("Joystick Horizontal"), Input.GetAxis("Joystick Vertical"));
            s_exactRightStickInput = new Vector2(Input.GetAxis("Right Joystick Horizontal"), Input.GetAxis("Right Joystick Vertical"));
            s_exactDpadInput = new Vector2(Input.GetAxis("Dpad X"), Input.GetAxis("Dpad Y"));

            // Make the inputs 0 if they are within their dead zone
            // Also round the input values whilst the exact input values will stay the same
            // Triggers
            s_exactBadTriggerInput = LockAxis(s_exactBadTriggerInput, TriggerDeadZone);
            s_badTriggerInput = Mathf.Round(s_exactBadTriggerInput);
            s_exactLeftTriggerInput = LockAxis(s_exactLeftTriggerInput, TriggerDeadZone);
            s_leftTriggerInput = Mathf.Round(s_exactLeftTriggerInput);
            s_exactRightTriggerInput = LockAxis(s_exactRightTriggerInput, TriggerDeadZone);
            s_rightTriggerInput = Mathf.Round(s_exactRightTriggerInput);

            // Left stick
            s_exactLeftStickInput = LockVector2Axis(s_exactLeftStickInput, JoystickDeadzone);
            s_leftStickInput = Helpers.RoundVector2(s_exactLeftStickInput);

            // Right stick
            s_exactRightStickInput = LockVector2Axis(s_exactRightStickInput, JoystickDeadzone);
            s_rightStickInput = Helpers.RoundVector2(s_exactRightStickInput);

            // Dpad
            s_exactDpadInput = LockVector2Axis(s_exactDpadInput, DpadDeadzone);
            s_dpadInput = Helpers.RoundVector2(s_exactDpadInput);
        }

        /// <summary>
        /// Will check if <paramref name="axisValue"/> is within the <paramref name="deadZone"/> and set it to 0 if it's inside the range.
        /// </summary>
        /// <param name="axisValue">The given axis.</param>
        /// <param name="deadZone">The zone where input should be completely ignored/set to 0.</param>
        /// <returns>The <paramref name="axisValue"/> unless it's inside the <paramref name="deadZone"/> range, then it'll return 0.</returns>
        private float LockAxis(float axisValue, float deadZone)
        {
            bool setTo0 = Helpers.ValueWithinRange(axisValue, -deadZone, deadZone);

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
        /// Loops through <see cref="Keys"/> until it finds a key with a name that matches <paramref name="name"/> exactly. (Unless <paramref name="caseSensitive"/> is set to false)
        /// </summary>
        /// <param name="name">The name of the key that we should search for.</param>
        /// <param name="caseSensitive">True by defualt.<para/>
        /// If true, then the names must match perfectly, so "hello" and "HeLlO" won't match. (Notice the capital letters)</param>
        /// <returns>The key that was found. Will be null if no key was found.</returns>
        public static Key GetKey(string name, bool caseSensitive = true)
        {
            // Loop through all of the keys
            foreach (Key key in Keys)
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
            if ((int)key >= KEYCODE_LENGTH - 1)
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
            if ((int)key >= KEYCODE_LENGTH - 1)
            {
                // Check all of the special gamepad keys
                switch (key)
                {
                    // Triggers
                    case GamepadKey.LeftTrigger:
                        bool leftTriggerPressed = ValuePressed(s_leftTriggerInput, s_oldLeftTriggerInput, mode, false);

                        if (!leftTriggerPressed)
                            leftTriggerPressed = ValuePressed(s_badTriggerInput, s_oldBadTriggerInput, mode, true);

                        return leftTriggerPressed;

                    case GamepadKey.RightTrigger:
                        bool rightTriggerPressed = ValuePressed(s_rightTriggerInput, s_oldRightTriggerInput, mode, false);

                        if (!rightTriggerPressed)
                            rightTriggerPressed = ValuePressed(s_badTriggerInput, s_oldBadTriggerInput, mode, false);

                        return rightTriggerPressed;

                    // Left Stick
                    case GamepadKey.LeftStickUp:
                        return ValuePressed(s_leftStickInput.y, s_oldLeftStickInput.y, mode, false);
                    case GamepadKey.LeftStickDown:
                        return ValuePressed(s_leftStickInput.y, s_oldLeftStickInput.y, mode, true);
                    case GamepadKey.LeftStickLeft:
                        return ValuePressed(s_leftStickInput.x, s_oldLeftStickInput.x, mode, true);
                    case GamepadKey.LeftStickRight:
                        return ValuePressed(s_leftStickInput.x, s_oldLeftStickInput.x, mode, false);

                    // Right Stick
                    case GamepadKey.RightStickUp:
                        return ValuePressed(s_rightStickInput.y, s_oldRightStickInput.y, mode, false);
                    case GamepadKey.RightStickDown:
                        return ValuePressed(s_rightStickInput.y, s_oldRightStickInput.y, mode, true);
                    case GamepadKey.RightStickLeft:
                        return ValuePressed(s_rightStickInput.x, s_oldRightStickInput.x, mode, true);
                    case GamepadKey.RightStickRight:
                        return ValuePressed(s_rightStickInput.x, s_oldRightStickInput.x, mode, false);

                    // Dpad
                    case GamepadKey.DpadUp:
                        return ValuePressed(s_dpadInput.y, s_oldDpadInput.y, mode, false);
                    case GamepadKey.DpadDown:
                        return ValuePressed(s_dpadInput.y, s_oldDpadInput.y, mode, true);
                    case GamepadKey.DpadLeft:
                        return ValuePressed(s_dpadInput.x, s_oldDpadInput.x, mode, true);
                    case GamepadKey.DpadRight:
                        return ValuePressed(s_dpadInput.x, s_oldDpadInput.x, mode, false);

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
