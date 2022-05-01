using GD3D.CustomInput;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GD3D.Player
{
    /// <summary>
    /// Stores which gamemode the player is in and handles when gamemodes are switched
    /// </summary>
    public class PlayerGamemodeHandler : PlayerScript
    {
        private Gamemode _startGamemode;
        public Gamemode CurrentGamemode;

        /// <summary>
        /// HashSet of airborne gamemodes.
        /// </summary>
        public static readonly HashSet<Gamemode> AirborneGamemodes = new HashSet<Gamemode>()
        {
            Gamemode.ship,
            Gamemode.ufo,
            Gamemode.wave,
        };

        //-- Upside down
        public bool UpsideDown;
        private bool _startUpsideDown;
        private bool _oldUpsideDown;

        public Action<bool> OnChangeGravity;

        //-- Small
        public bool IsSmall;
        private bool _startSmall;
        private bool _oldIsSmall;

        [Space]
        public LayerMask GroundLayer;

        [Header("Gamemodes")]
        public CubeGamemode Cube;
        public ShipGamemode Ship;
        public BallGamemode Ball;
        public UfoGamemode Ufo;
        public WaveGamemode Wave;
        public RobotGamemode Robot;
        public SpiderGamemode Spider;

        private GamemodeScript _activeGamemodeScript;
        public GamemodeScript CurrentGamemodeScript => _activeGamemodeScript;

        /// <summary>
        /// Subscribe to this event to be notified whenever the player changes gamemode
        /// </summary>
        public Action<Gamemode> OnChangeGamemode;

        public override void Start()
        {
            base.Start();

            // Setup all the gamemode scripts
            SetupGamemodeScript(Cube);
            SetupGamemodeScript(Ship);
            SetupGamemodeScript(Ball);
            SetupGamemodeScript(Ufo);
            SetupGamemodeScript(Wave);
            SetupGamemodeScript(Robot);
            SetupGamemodeScript(Spider);

            // Change to the start gamemode
            _startGamemode = CurrentGamemode;
            ChangeGamemode(_startGamemode);

            // Set start values
            _startUpsideDown = UpsideDown;
            _startSmall = IsSmall;

            // Subscribe to events
            player.OnDeath += ResetValues;
            player.OnDeath += OnDeath;

            player.OnRespawn += OnRespawn;

            if (player.InMainMenu)
            {
                // Randomize gamemode when the player teleports in the main menu
                player.Movement.OnMainMenuTeleport += () =>
                {
                    // Wait 1 frame before doing this so no weird trail stretch stuff happens
                    Helpers.TimerEndOfFrame(this, () =>
                    {
                        int randomGamemode = Random.Range(0, 2);

                        ChangeGamemode((Gamemode)randomGamemode);
                    });
                };
            }
        }

        /// <summary>
        /// Called when the player dies
        /// </summary>
        private void OnDeath()
        {
            _activeGamemodeScript.OnDeath();
        }

        /// <summary>
        /// Called when the player respawns
        /// </summary>
        private void OnRespawn(bool inPracticeMode, Checkpoint checkpoint)
        {
            _activeGamemodeScript.OnRespawn(inPracticeMode, checkpoint);

            // Check if we are not in practice mode
            if (!inPracticeMode)
            {
                ResetValues();
                ChangeGamemode(_startGamemode);
            }
            else
            {
                UpsideDown = checkpoint.PlayerUpsideDown;
                IsSmall = checkpoint.PlayerIsSmall;

                ChangeGamemode(checkpoint.PlayerGamemode);
            }
        }

        /// <summary>
        /// Resets some values to their start value.
        /// </summary>
        private void ResetValues()
        {
            UpsideDown = _startUpsideDown;
            IsSmall = _startSmall;
        }

        /// <summary>
        /// Sets important variables and invokes Start() on the given <see cref="GamemodeScript"/>. <para/>
        /// As the name implies, this is used to setup the scripts before the game begins
        /// </summary>
        private void SetupGamemodeScript(GamemodeScript script)
        {
            script.GamemodeHandler = this;
            script.Player = player;
            script.PlayerMovement = player.Movement;
            script.Rigidbody = rb;

            script.Start();
        }

        public override void Update()
        {
            base.Update();

            // Return if the player is dead
            if (player.IsDead)
            {
                return;
            }

            // Check if the players gravity has changed
            if (_oldUpsideDown != UpsideDown)
            {
                // If it has, then invoke the OnChangeGravity events
                _oldUpsideDown = UpsideDown;

                OnChangeGravity?.Invoke(UpsideDown);
                _activeGamemodeScript.OnChangeGravity(UpsideDown);
            }

            // Check if the players size has changed
            if (_oldIsSmall != IsSmall)
            {
                // If it has, then invoke the OnChangeGravity events
                _oldIsSmall = IsSmall;

                OnChangeGravity?.Invoke(UpsideDown);
                _activeGamemodeScript.OnChangeGravity(UpsideDown);
            }

            // Call Update() in activeGamemodeScript
            _activeGamemodeScript?.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            // Return if the player is dead
            if (player.IsDead)
            {
                return;
            }

            // Call FixedUpdate() in activeGamemodeScript
            _activeGamemodeScript?.FixedUpdate();
        }

        public override void OnClickKey(PressMode mode)
        {
            base.OnClickKey(mode);

            // Return if the player is dead
            if (player.IsDead)
            {
                return;
            }

            // Call OnClick() in activeGamemodeScript
            _activeGamemodeScript?.OnClick(mode);
        }

        /// <summary>
        /// Updates the current gamemode and changes it to be the <paramref name="newGamemode"/>
        /// </summary>
        public void ChangeGamemode(Gamemode newGamemode)
        {
            // Change gamemode enum
            CurrentGamemode = newGamemode;

            // Call OnDisable() in the old activeGamemodeScript
            _activeGamemodeScript?.OnDisable();

            // Set the new activeGamemodeScript
            _activeGamemodeScript = TypeToGamemode(newGamemode);

            // Call OnEnable() in the new activeGamemodeScript (Unless it's null)
            _activeGamemodeScript?.OnEnable();

            // Invoke event
            OnChangeGamemode?.Invoke(newGamemode);
        }

        /// <summary>
        /// Converts the given <see cref="GamemodeScript"/> to <see cref="Gamemode"/>
        /// </summary>
        public Gamemode GamemodeToType(GamemodeScript g)
        {
            // Self explanatory how this works
            switch (g)
            {
                case CubeGamemode _:
                    return Gamemode.cube;

                case ShipGamemode _:
                    return Gamemode.ship;

                case BallGamemode _:
                    return Gamemode.ball;

                case UfoGamemode _:
                    return Gamemode.ufo;

                case WaveGamemode _:
                    return Gamemode.wave;

                case RobotGamemode _:
                    return Gamemode.robot;

                case SpiderGamemode _:
                    return Gamemode.spider;

                default:
                    return Gamemode.none;
            }
        }

        /// <summary>
        /// Converts the given <see cref="Gamemode"/> to <see cref="GamemodeScript"/>
        /// </summary>
        public GamemodeScript TypeToGamemode(Gamemode type)
        {
            // Self explanatory how this works
            switch (type)
            {
                case Gamemode.cube:
                    return Cube;

                case Gamemode.ship:
                    return Ship;

                case Gamemode.ball:
                    return Ball;

                case Gamemode.ufo:
                    return Ufo;

                case Gamemode.wave:
                    return Wave;

                case Gamemode.robot:
                    return Robot;

                case Gamemode.spider:
                    return Spider;

                default:
                    return null;
            }
        }

#if UNITY_EDITOR
        // Draw current gamemode ground detection gizmo
        private void OnDrawGizmosSelected()
        {
            GamemodeScript currentGamemodeScript = TypeToGamemode(CurrentGamemode);

            if (currentGamemodeScript == null)
            {
                return;
            }

            Gizmos.color = Color.red;
            currentGamemodeScript.DrawGroundDetectGizmo(transform, UpsideDown);
        }
#endif
    }

    /// <summary>
    /// An enum for the different player gamemodes
    /// </summary>
    public enum Gamemode
    {
        /// <summary>
        /// ?????
        /// </summary>
        none = -1,
        /// <summary>
        /// Jumps
        /// </summary>
        cube = 0,
        /// <summary>
        /// Flies up
        /// </summary>
        ship = 1,
        /// <summary>
        /// Changes gravity
        /// </summary>
        ball = 2,
        /// <summary>
        /// Flappy birb
        /// </summary>
        ufo = 3,
        /// <summary>
        /// I hate this gamemode (mad cuz bad)
        /// </summary>
        wave = 4,
        /// <summary>
        /// Jumps (But different)
        /// </summary>
        robot = 5,
        /// <summary>
        /// Ball gamemode, but way cooler
        /// </summary>
        spider = 6,
    }

    /// <summary>
    /// Contains data about which Y Velocity to set depending on the players size
    /// </summary>
    [Serializable]
    public class GamemodeSizedFloat
    {
        public float BigValue;
        public float SmallValue;

        /// <summary>
        /// Returns one either the big or small value, depends on if <paramref name="isSmall"/> is true or not
        /// </summary>
        public float GetValue(bool isSmall)
        {
            return isSmall ? SmallValue : BigValue;
        }

        public GamemodeSizedFloat(float big, float small)
        {
            BigValue = big;
            SmallValue = small;
        }
    }
}
