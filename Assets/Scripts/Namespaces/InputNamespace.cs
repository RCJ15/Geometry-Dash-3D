using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Player;

namespace GD3D.CustomInput
{
    /// <summary>
    /// Is the enum for any press mode when taking input.
    /// </summary>
    [Serializable]
    public enum PressMode
    {
        /// <summary>
        /// True as long as the key is held down.
        /// </summary>
        hold = 0,
        /// <summary>
        /// True the frame the key is pressed.
        /// </summary>
        down = 1,
        /// <summary>
        /// True the frame the key is released.
        /// </summary>
        up = 2,
    }

    /// <summary>
    /// Contains data about a keybind within the game.
    /// </summary>
    [Serializable]
    public class Key
    {
        /// <summary>
        /// The name of the key.
        /// </summary>
        public string name;

        /// <summary>
        /// A list of all acceptable <see cref="KeyboardKey"/>s that can trigger this key as True.
        /// </summary>
        public KeyboardKey[] keyboardKeys;

        /// <summary>
        /// A list of all acceptable <see cref="GamepadKey"/>s that can trigger this key as True.
        /// </summary>
        public GamepadKey[] gamepadKeys;

        /// <summary>
        /// The max limit of keys for both <see cref="keyboardKeys"/> and <see cref="gamepadKeys"/>. <para/>
        /// If this is under or equal to 0 then no limit is set.
        /// </summary>
        public int maxKeys = 0;

        /// <summary>
        /// How the key should be checked when <see cref="Pressed(PressMode)"/> is called.
        /// </summary>
        public CheckMode checkMode;

        #region New Key() Methods
        /// <summary>
        /// Creates a new key with no keyboard or gamepad input. This is pretty useless.
        /// </summary>
        /// <param name="name">The <see cref="name"/> of the key.</param>
        /// <param name="maxKeys">Is 0 by default. The <see cref="maxKeys"/> of the key.</param>
        /// <param name="checkMode">Is <see cref="CheckMode.any"/> by default. How the key should be checked when <see cref="Pressed(PressMode)"/> is called.</param>
        public Key(string name, int maxKeys = 0, CheckMode checkMode = CheckMode.any)
        {
            // Set the name
            this.name = name;

            // Set the max keys
            this.maxKeys = maxKeys;

            // Set the key arrays
            keyboardKeys = new KeyboardKey[maxKeys];
            gamepadKeys = new GamepadKey[maxKeys];

            // Set the check mode
            this.checkMode = checkMode;

            FixArrays();
        }

        /// <summary>
        /// Creates a new key.
        /// </summary>
        /// <param name="name">The <see cref="name"/> of the key.</param>
        /// <param name="keyboardKey">The <see cref="KeyboardKey"/>s for the <see cref="keyboardKeys"/> array.</param>
        /// <param name="gamepadKey">The <see cref="GamepadKey"/>a for the <see cref="gamepadKeys"/> array.</param>
        /// <param name="checkMode">Is <see cref="CheckMode.any"/> by default. How the key should be checked when <see cref="Pressed(PressMode)"/> is called.</param>
        public Key(string name, KeyboardKey[] keyboardKey, GamepadKey[] gamepadKey, int maxKeys = 0, CheckMode checkMode = CheckMode.any)
        {
            // Set the name
            this.name = name;

            // Set the max keys
            this.maxKeys = maxKeys;

            // Set the key arrays
            keyboardKeys = keyboardKey;
            gamepadKeys = gamepadKey;

            // Set the check mode
            this.checkMode = checkMode;

            FixArrays();
        }

        /// <summary>
        /// Creates a new key with only one <see cref="KeyboardKey"/> and <see cref="GamepadKey"/>.
        /// </summary>
        /// <param name="name">The <see cref="name"/> of the key.</param>
        /// <param name="keyboardKey">The one and only <see cref="KeyboardKey"/> for the <see cref="keyboardKeys"/> array.</param>
        /// <param name="gamepadKey">The one and only <see cref="GamepadKey"/> for the <see cref="gamepadKeys"/> array.</param>
        /// <param name="checkMode">Is <see cref="CheckMode.any"/> by default. How the key should be checked when <see cref="Pressed(PressMode)"/> is called.</param>
        public Key(string name, KeyboardKey keyboardKey, GamepadKey gamepadKey, int maxKeys = 0, CheckMode checkMode = CheckMode.any)
        {
            // Set the name
            this.name = name;

            // Set the max keys
            this.maxKeys = maxKeys;

            // Create 2 empty arrays with only one entry in each
            keyboardKeys = NewKeys(keyboardKey);
            gamepadKeys = NewKeys(gamepadKey);

            // Set the check mode
            this.checkMode = checkMode;

            FixArrays();
        }

        /// <summary>
        /// Creates a new key with only one <see cref="KeyboardKey"/>.
        /// </summary>
        /// <param name="name">The <see cref="name"/> of the key.</param>
        /// <param name="keyboardKey">The one and only <see cref="KeyboardKey"/> for the <see cref="keyboardKeys"/> array.</param>
        /// <param name="gamepadKey">The <see cref="GamepadKey"/>a for the <see cref="gamepadKeys"/> array.</param>
        /// <param name="checkMode">Is <see cref="CheckMode.any"/> by default. How the key should be checked when <see cref="Pressed(PressMode)"/> is called.</param>
        public Key(string name, KeyboardKey keyboardKey, GamepadKey[] gamepadKey, int maxKeys = 0, CheckMode checkMode = CheckMode.any)
        {
            // Set the name
            this.name = name;

            // Set the max keys
            this.maxKeys = maxKeys;

            // Set one of the key arrays and create a empty array with only one entry in the other
            keyboardKeys = NewKeys(keyboardKey);
            gamepadKeys = gamepadKey;

            // Set the check mode
            this.checkMode = checkMode;

            FixArrays();
        }

        /// <summary>
        /// Creates a new key with only one <see cref="GamepadKey"/>.
        /// </summary>
        /// <param name="name">The <see cref="name"/> of the key.</param>
        /// <param name="keyboardKey">The <see cref="KeyboardKey"/>s for the <see cref="keyboardKeys"/> array.</param>
        /// <param name="gamepadKey">The one and only <see cref="GamepadKey"/> for the <see cref="gamepadKeys"/> array.</param>
        /// <param name="checkMode">Is <see cref="CheckMode.any"/> by default. How the key should be checked when <see cref="Pressed(PressMode)"/> is called.</param>
        public Key(string name, KeyboardKey[] keyboardKey, GamepadKey gamepadKey, int maxKeys = 0, CheckMode checkMode = CheckMode.any)
        {
            // Set the name
            this.name = name;

            // Set the max keys
            this.maxKeys = maxKeys;

            // Set one of the key arrays and create a empty array with only one entry in the other
            keyboardKeys = keyboardKey;
            gamepadKeys = NewKeys(gamepadKey);

            // Set the check mode
            this.checkMode = checkMode;

            FixArrays();
        }
        #endregion

        /// <summary>
        /// Fixes the <see cref="keyboardKeys"/> and <see cref="gamepadKeys"/> array to match the size of <see cref="maxKeys"/>, unless it's 0 or less.
        /// </summary>
        public void FixArrays()
        {
            // IF the max keys are 0 or below, then return
            if (maxKeys <= 0)
                return;

            // Fix the keyboard keys first
            if (keyboardKeys.Length != maxKeys)
            {
                // Create a new list of KeyboardKey
                List<KeyboardKey> fixedKeyboardKeys = new List<KeyboardKey>();

                // Loop for the amount in maxKeys
                for (int i = 0; i < maxKeys; i++)
                {
                    // Add it to the list if it's in range of the old keyboardKeys array
                    if (Helpers.ValueInRangeOfArray(i, keyboardKeys))
                    {
                        fixedKeyboardKeys.Add(keyboardKeys[i]);
                    }
                    // Else just add a key of none to the list
                    else
                    {
                        fixedKeyboardKeys.Add(KeyboardKey.None);
                    }
                }

                // Set the old array to the new fixed list
                keyboardKeys = fixedKeyboardKeys.ToArray();
            }

            // Fix the gamepadKeys now
            if (gamepadKeys.Length != maxKeys)
            {
                // Create a new list of GamepadKeys
                List<GamepadKey> fixedGamepadKeys = new List<GamepadKey>();

                // Loop for the amount in maxKeys
                for (int i = 0; i < maxKeys; i++)
                {
                    // Add it to the list if it's in range of the old gamepadKeys array
                    if (Helpers.ValueInRangeOfArray(i, gamepadKeys))
                    {
                        fixedGamepadKeys.Add(gamepadKeys[i]);
                    }
                    // Else just add a key of none to the list
                    else
                    {
                        fixedGamepadKeys.Add(GamepadKey.None);
                    }
                }

                // Set the old array to the new fixed list
                gamepadKeys = fixedGamepadKeys.ToArray();
            }
        }

        /// <summary>
        /// Creates an array of <see cref="KeyboardKey"/> using the params keyword. Basically is a shortcut.
        /// </summary>
        /// <param name="keys">The given keys.</param>
        /// <returns>The created array.</returns>
        public static KeyboardKey[] NewKeys(params KeyboardKey[] keys) { return keys; }

        /// <summary>
        /// Creates an array of <see cref="GamepadKey"/> using the params keyword. Basically is a shortcut.
        /// </summary>
        /// <param name="keys">The given keys.</param>
        /// <returns>The created array.</returns>
        public static GamepadKey[] NewKeys(params GamepadKey[] keys) { return keys; }

        /// <summary>
        /// Checks if the key is being held down, has just been pressed down or if the value has just been released. (Depends on <paramref name="mode"/>) <para/>
        /// 
        /// </summary>
        /// <param name="mode">Is <seealso cref="PressMode.down"/> by default. The press mode to check.</param>
        /// <returns>True if the value was held/just pressed/just released. (Depends on <paramref name="mode"/>)</returns>
        public bool Pressed(PressMode mode = PressMode.down)
        {
            // Change depending on the checkMode
            switch (checkMode)
            {
                // Check if any key in either keyboardKeys or gamepadKeys were pressed
                case CheckMode.any:

                    // Loop through all keyboard keys first
                    foreach (KeyboardKey key in keyboardKeys)
                    {
                        // Skip empty keys
                        if (key == KeyboardKey.None)
                            continue;

                        // Return true if one of the keyboard keys were pressed
                        if (PlayerInput.KeyPressed(key, mode))
                            return true;
                    }

                    // Loop through all the gamepad keys now
                    foreach (GamepadKey key in gamepadKeys)
                    {
                        // Skip empty keys
                        if (key == GamepadKey.None)
                            continue;

                        // Return true if one of the gamepad keys were pressed
                        if (PlayerInput.KeyPressed(key, mode))
                            return true;
                    }

                    // Return false if no key was pressed
                    return false;

                // Check if all the keys in either keyboardKeys or gamepadKeys were pressed
                case CheckMode.all:

                    bool checkGamepad = false;

                    // Loop through all keyboard keys first
                    foreach (KeyboardKey key in keyboardKeys)
                    {
                        // Skip empty keys
                        if (key == KeyboardKey.None)
                            continue;

                        // Check gamepad keys instead if one of the keyboard keys weren't pressed
                        if (!PlayerInput.KeyPressed(key, mode))
                        {
                            checkGamepad = true;
                            break;
                        }
                    }

                    // Check the gamepad keys if we should check the gamepad keys
                    if (checkGamepad)
                    {
                        // Loop through all the gamepad keys now
                        foreach (GamepadKey key in gamepadKeys)
                        {
                            // Skip empty keys
                            if (key == GamepadKey.None)
                                continue;

                            // Return false if one of the gamepad keys weren't pressed because we have already checked all the keyboard keys and they were false
                            if (!PlayerInput.KeyPressed(key, mode))
                                return false;
                        }
                    }

                    // Return true if all the keys in either keyboardKeys or gamepadKeys were pressed
                    return true;

                // Check if all the keys in BOTH keyboardKeys and gamepadKeys were pressed
                case CheckMode.everything:

                    // Loop through all keyboard keys first
                    foreach (KeyboardKey key in keyboardKeys)
                    {
                        // Skip empty keys
                        if (key == KeyboardKey.None)
                            continue;

                        // Return false if one of the keyboard keys weren't pressed
                        if (!PlayerInput.KeyPressed(key, mode))
                            return false;
                    }

                    // Loop through all the gamepad keys now
                    foreach (GamepadKey key in gamepadKeys)
                    {
                        // Skip empty keys
                        if (key == GamepadKey.None)
                            continue;

                        // Return false if one of the gamepad keys weren't pressed
                        if (!PlayerInput.KeyPressed(key, mode))
                            return false;
                    }

                    // Return true if all the keys in BOTH keyboardKeys and gamepadKeys were pressed
                    return true;

                // Return false if no valid CheckMode was given. (Which shouldn't happen)
                default:
                    return false;
            }
        }

        /// <summary>
        /// Is the enum for how all of the <see cref="Key"/>s should be checked when <see cref="Pressed(PressMode)"/> is called.
        /// </summary>
        [Serializable]
        public enum CheckMode
        {
            /// <summary>
            /// True if ANY of the keys in <see cref="keyboardKeys"/> or <see cref="gamepadKeys"/> are pressed.
            /// </summary>
            any = 0,
            /// <summary>
            /// True only if ALL the keys in <see cref="keyboardKeys"/> or <see cref="gamepadKeys"/> are pressed.
            /// </summary>
            all = 1,
            /// <summary>
            /// True only if EVERY key in <see cref="keyboardKeys"/> AND all the keys in <see cref="gamepadKeys"/> are pressed. (This one is pretty useless)
            /// </summary>
            everything = 2,
        }

        /// <summary>
        /// Basically works the same as <see cref="UnityEngine.Input.GetAxisRaw(string)"/> but with <see cref="Key"/> instead.
        /// </summary>
        /// <param name="positiveKey">If this key is held down then 1 is returned.</param>
        /// <param name="negativeKey">If this key is held down then -1 is returned.</param>
        /// <returns>Returns 1 if <paramref name="positiveKey"/> is held down, -1 if <paramref name="negativeKey"/> is held down and returns 0 if none or both are held down.</returns>
        public static float GetAxis(Key negativeKey, Key positiveKey)
        {
            // Create bools to use instead of the function
            bool negativeHeld = negativeKey.Pressed(PressMode.hold);
            bool positiveHeld = positiveKey.Pressed(PressMode.hold);

            // Return 1 if the positive key is held down but the negative one is not
            if (positiveHeld && !negativeHeld)
            {
                return 1;
            }
            // Return -1 if the negative key is held down but the positive one is not
            else if (negativeHeld && !positiveHeld)
            {
                return -1;
            }

            // Return 0 by default
            return 0;
        }
    }

    /// <summary>
    /// Basically is the same as <seealso cref="KeyCode"/> but without the joystick inputs.
    /// </summary>
    [Serializable]
    public enum KeyboardKey
    {
        None = 0,
        Backspace = 8,
        Tab = 9,
        Clear = 12,
        Return = 13,
        Pause = 19,
        Escape = 27,
        Space = 32,
        Exclaim = 33,
        DoubleQuote = 34,
        Hash = 35,
        Dollar = 36,
        Percent = 37,
        Ampersand = 38,
        Quote = 39,
        LeftParen = 40,
        RightParen = 41,
        Asterisk = 42,
        Plus = 43,
        Comma = 44,
        Minus = 45,
        Period = 46,
        Slash = 47,
        Alpha0 = 48,
        Alpha1 = 49,
        Alpha2 = 50,
        Alpha3 = 51,
        Alpha4 = 52,
        Alpha5 = 53,
        Alpha6 = 54,
        Alpha7 = 55,
        Alpha8 = 56,
        Alpha9 = 57,
        Colon = 58,
        Semicolon = 59,
        Less = 60,
        Equals = 61,
        Greater = 62,
        Question = 63,
        At = 64,
        LeftBracket = 91,
        Backslash = 92,
        RightBracket = 93,
        Caret = 94,
        Underscore = 95,
        BackQuote = 96,
        A = 97,
        B = 98,
        C = 99,
        D = 100,
        E = 101,
        F = 102,
        G = 103,
        H = 104,
        I = 105,
        J = 106,
        K = 107,
        L = 108,
        M = 109,
        N = 110,
        O = 111,
        P = 112,
        Q = 113,
        R = 114,
        S = 115,
        T = 116,
        U = 117,
        V = 118,
        W = 119,
        X = 120,
        Y = 121,
        Z = 122,
        LeftCurlyBracket = 123,
        Pipe = 124,
        RightCurlyBracket = 125,
        Tilde = 126,
        Delete = 127,
        Keypad0 = 256,
        Keypad1 = 257,
        Keypad2 = 258,
        Keypad3 = 259,
        Keypad4 = 260,
        Keypad5 = 261,
        Keypad6 = 262,
        Keypad7 = 263,
        Keypad8 = 264,
        Keypad9 = 265,
        KeypadPeriod = 266,
        KeypadDivide = 267,
        KeypadMultiply = 268,
        KeypadMinus = 269,
        KeypadPlus = 270,
        KeypadEnter = 271,
        KeypadEquals = 272,
        UpArrow = 273,
        DownArrow = 274,
        RightArrow = 275,
        LeftArrow = 276,
        Insert = 277,
        Home = 278,
        End = 279,
        PageUp = 280,
        PageDown = 281,
        F1 = 282,
        F2 = 283,
        F3 = 284,
        F4 = 285,
        F5 = 286,
        F6 = 287,
        F7 = 288,
        F8 = 289,
        F9 = 290,
        F10 = 291,
        F11 = 292,
        F12 = 293,
        F13 = 294,
        F14 = 295,
        F15 = 296,
        Numlock = 300,
        CapsLock = 301,
        ScrollLock = 302,
        RightShift = 303,
        LeftShift = 304,
        RightControl = 305,
        LeftControl = 306,
        RightAlt = 307,
        LeftAlt = 308,
        RightCommand = 309,
        RightApple = 309,
        LeftCommand = 310,
        LeftApple = 310,
        LeftWindows = 311,
        RightWindows = 312,
        AltGr = 313,
        Help = 315,
        Print = 316,
        SysReq = 317,
        Break = 318,
        Menu = 319,

        // Some of these have been renamed for easier access
        LeftMouseButton = 323,
        RightMouseButton = 324,
        MiddleMouseButton = 325,

        // These are side/thumb buttons on the side of the mouse that aren't used a lot
        BackMouseButton = 326,
        ForwardMouseButton = 327,

        // Idk how you can access the power of these mouse buttons
        // (My current mouse doesn't have these buttons)
        Mouse5 = 328,
        Mouse6 = 329,
    }

    /// <summary>
    /// Includes all the joystick inputs from <seealso cref="KeyCode"/> but with some extra added special gamepad inputs like the left and right triggers for example.
    /// </summary>
    [Serializable]
    public enum GamepadKey
    {
        None = 0,

        // KeyCode gamepad buttons.
        // These are normally named something like "JoystickButton5" but I have named them to be the proper button name so it's easier to use

        /// <summary>
        /// This is true for Xbox. It's B on nintendo switch and Cross on Playstation
        /// </summary>
        AButton = 330,
        /// <summary>
        /// This is true for Xbox. It's A on nintendo switch and Circle on Playstation
        /// </summary>
        BButton = 331,
        /// <summary>
        /// This is true for Xbox. It's Y on nintendo switch and Square on Playstation
        /// </summary>
        XButton = 332,
        /// <summary>
        /// This is true for Xbox. It's X on nintendo switch and Triangle on Playstation
        /// </summary>
        YButton = 333,

        /// <summary>
        /// This is true for Xbox. It's L on nintendo switch and L1 on playstation.
        /// </summary>
        LeftBumper = 334,
        /// <summary>
        /// This is true for Xbox. It's R on nintendo switch and R1 on playstation.
        /// </summary>
        RightBumper = 335,

        /// <summary>
        /// This is true for Playstation. It's - (Minus) on nintendo switch and Back on Xbox.
        /// </summary>
        Select = 336,
        /// <summary>
        /// This is true for Xbox and Playstation. It's + (Plus) on nintendo switch.
        /// </summary>
        Start = 337,

        /// <summary>
        /// Also known as L3 on Playstation.
        /// </summary>
        LeftStickPress = 338,
        /// <summary>
        /// Also known as R3 on Playstation.
        /// </summary>
        RightStickPress = 339,

        // Idk what these are used for but they are here cuz unity has these by default
        JoystickButton10 = 340,
        JoystickButton11 = 341,
        JoystickButton12 = 342,
        JoystickButton13 = 343,
        JoystickButton14 = 344,
        JoystickButton15 = 345,
        JoystickButton16 = 346,
        JoystickButton17 = 347,
        JoystickButton18 = 348,
        JoystickButton19 = 349,

        // Triggers
        /// <summary>
        /// This is true for Xbox. It's ZL on nintendo switch and L2 on playstation.
        /// </summary>
        LeftTrigger = 510,
        /// <summary>
        /// This is true for Xbox. It's ZR on nintendo switch and R2 on playstation.
        /// </summary>
        RightTrigger = 511,

        // Left Stick
        LeftStickUp = 512,
        LeftStickDown = 513,
        LeftStickLeft = 514,
        LeftStickRight = 515,

        // Right Stick
        RightStickUp = 516,
        RightStickDown = 517,
        RightStickLeft = 518,
        RightStickRight = 519,

        // Dpad
        DpadUp = 520,
        DpadDown = 521,
        DpadLeft = 522,
        DpadRight = 523,
    }
}