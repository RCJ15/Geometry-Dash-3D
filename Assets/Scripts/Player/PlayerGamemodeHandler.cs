using GD3D.CustomInput;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.Player
{
    /// <summary>
    /// Stores which gamemode the player is in and handles when gamemodes are switched
    /// </summary>
    public class PlayerGamemodeHandler : PlayerScript
    {
        private Gamemode _startGamemode;
        public Gamemode CurrentGamemode;

        public bool upsideDown;
        private bool startUpsideDown;
        private bool oldUpsideDown;

        public Action<bool> OnChangeGravity;

        [Header("Gamemodes")]
        public CubeGamemode Cube;
        public ShipGamemode Ship;
        public BallGamemode Ball;
        public UfoGamemode Ufo;
        public WaveGamemode Wave;
        public RobotGamemode Robot;
        public SpiderGamemode Spider;

        private GamemodeScript _activeGamemodeScript;

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
            startUpsideDown = upsideDown;

            // Subscribe to events
            player.OnDeath += OnDeath;
            player.OnRespawn += OnRespawn;
        }

        /// <summary>
        /// Called when the player dies
        /// </summary>
        private void OnDeath()
        {
            upsideDown = startUpsideDown;
            
            _activeGamemodeScript.OnDeath();
        }

        /// <summary>
        /// Called when the player respawns
        /// </summary>
        private void OnRespawn()
        {
            _activeGamemodeScript.OnRespawn();

            ChangeGamemode(_startGamemode);
        }

        /// <summary>
        /// Sets important variables and invokes Start() on the given <see cref="GamemodeScript"/>. <para/>
        /// As the name implies, this is used to setup the scripts before the game begins
        /// </summary>
        private void SetupGamemodeScript(GamemodeScript script)
        {
            script.GamemodeHandler = this;
            script.Player = player;
            script.Rigidbody = rb;

            script.Start();
        }

        public override void Update()
        {
            base.Update();

            // Check if the players gravity has changed
            if (oldUpsideDown != upsideDown)
            {
                // If it has, then invoke the OnChangeGravity events
                oldUpsideDown = upsideDown;

                OnChangeGravity?.Invoke(upsideDown);
                _activeGamemodeScript.OnChangeGravity(upsideDown);
            }

            // Call Update() in activeGamemodeScript
            _activeGamemodeScript?.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            // Call FixedUpdate() in activeGamemodeScript
            _activeGamemodeScript?.FixedUpdate();
        }

        public override void OnClick(PressMode mode)
        {
            base.OnClick(mode);

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
        private Gamemode GamemodeToType(GamemodeScript g)
        {
            // Self explanatory how this works
            switch (g)
            {
                case CubeGamemode cube:
                    return Gamemode.cube;

                case ShipGamemode ship:
                    return Gamemode.ship;

                case BallGamemode ball:
                    return Gamemode.ball;

                case UfoGamemode ufo:
                    return Gamemode.ufo;

                case WaveGamemode wave:
                    return Gamemode.wave;

                case RobotGamemode robot:
                    return Gamemode.robot;

                case SpiderGamemode spider:
                    return Gamemode.spider;

                default:
                    return Gamemode.none;
            }
        }

        /// <summary>
        /// Converts the given <see cref="Gamemode"/> to <see cref="GamemodeScript"/>
        /// </summary>
        private GamemodeScript TypeToGamemode(Gamemode type)
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
            currentGamemodeScript.DrawGroundDetectGizmo(transform, upsideDown);
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
}
