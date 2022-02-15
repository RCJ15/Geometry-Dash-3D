using GD3D.CustomInput;
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
        public Gamemode CurrentGamemode;

        [Header("Gamemodes")]
        public CubeGamemode Cube;
        public ShipGamemode Ship;
        public UfoGamemode Ufo;
        public BallGamemode Ball;
        public RobotGamemode Robot;

        private GamemodeScript _activeGamemodeScript;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        public override void Start()
        {
            base.Start();

            // Setup all the gamemode scripts
            SetupGamemodeScript(Cube);
            SetupGamemodeScript(Ship);
            SetupGamemodeScript(Ufo);
            SetupGamemodeScript(Ball);
            SetupGamemodeScript(Robot);

            // Update the start gamemode
            ChangeGamemode(CurrentGamemode);

            // Subscribe to events
            _player.OnDeath += OnDeath;
            _player.OnRespawn += OnRespawn;
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
        private void OnRespawn()
        {
            _activeGamemodeScript.OnRespawn();
        }

        private void SetupGamemodeScript(GamemodeScript script)
        {
            script.GamemodeHandler = this;
            script.Player = _player;
            script.Rigidbody = _rigidbody;

            script.Start();
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        public override void Update()
        {
            base.Update();

            // Call Update() in activeGamemodeScript (Unless it's null)
            _activeGamemodeScript?.Update();
        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            // Call FixedUpdate() in activeGamemodeScript (Unless it's null)
            _activeGamemodeScript?.FixedUpdate();
        }

        public override void OnClick(PressMode mode)
        {
            base.OnClick(mode);

            // Call OnClick() in activeGamemodeScript (Unless it's null)
            _activeGamemodeScript?.OnClick(mode);
        }

        /// <summary>
        /// Updates the current gamemode and changes it to be <paramref name="newGamemode"/>
        /// </summary>
        public void ChangeGamemode(Gamemode newGamemode)
        {
            // Change gamemode enum
            CurrentGamemode = newGamemode;

            // Call OnDisable() in the old activeGamemodeScript (Unless it's null)
            _activeGamemodeScript?.OnDisable();

            // Set the new activeGamemodeScript
            _activeGamemodeScript = TypeToGamemode(newGamemode);

            // Call OnEnable() in the new activeGamemodeScript (Unless it's null)
            _activeGamemodeScript?.OnEnable();
        }

        /// <summary>
        /// Converts <paramref name="g"/> to <see cref="Gamemode"/>
        /// </summary>
        private Gamemode GamemodeToType(GamemodeScript g)
        {
            // Self explanatory how this works
            switch (g)
            {
                case CubeGamemode c:
                    return Gamemode.cube;

                case ShipGamemode s:
                    return Gamemode.ship;

                case UfoGamemode u:
                    return Gamemode.ufo;

                case BallGamemode b:
                    return Gamemode.ball;

                case RobotGamemode r:
                    return Gamemode.robot;

                default:
                    return Gamemode.none;
            }
        }

        /// <summary>
        /// Converts <paramref name="type"/> to <see cref="GamemodeScript"/>
        /// </summary>
        private GamemodeScript TypeToGamemode(Gamemode type)
        {
            // Self explanatory how this works
            switch (type)
            {
                case Gamemode.none:
                    return null;

                case Gamemode.cube:
                    return Cube;

                case Gamemode.ship:
                    return Ship;

                case Gamemode.ufo:
                    return Ufo;

                case Gamemode.ball:
                    return Ball;

                case Gamemode.robot:
                    return Robot;

                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// An enum for the very different player gamemodes
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
        /// Flappy birb
        /// </summary>
        ufo = 2,
        /// <summary>
        /// Changes gravity
        /// </summary>
        ball = 3,
        /// <summary>
        /// Jumps (But different)
        /// </summary>
        robot = 4,
    }
}
