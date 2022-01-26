using Game.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// Stores which gamemode the player is in and handles when gamemodes are switched
    /// </summary>
    public class PlayerGamemodeHandler : PlayerScript
    {
        public Gamemode currentGamemode;

        [Header("Gamemodes")]
        public CubeGamemode cube;
        public ShipGamemode ship;
        public UfoGamemode ufo;
        public BallGamemode ball;
        public RobotGamemode robot;

        private GamemodeScript activeGamemodeScript;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        public override void Start()
        {
            base.Start();

            // Setup all the gamemode scripts
            SetupGamemodeScript(cube);
            SetupGamemodeScript(ship);
            SetupGamemodeScript(ufo);
            SetupGamemodeScript(ball);
            SetupGamemodeScript(robot);

            // Update the start gamemode
            ChangeGamemode(currentGamemode);
        }

        private void SetupGamemodeScript(GamemodeScript script)
        {
            script.gh = this;
            script.rb = rb;
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        public override void Update()
        {
            base.Update();

            // Call Update() in activeGamemodeScript (Unless it's null)
            activeGamemodeScript?.Update();
        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            // Call FixedUpdate() in activeGamemodeScript (Unless it's null)
            activeGamemodeScript?.FixedUpdate();
        }

        public override void OnClick(PressMode mode)
        {
            base.OnClick(mode);

            // Call OnClick() in activeGamemodeScript (Unless it's null)
            activeGamemodeScript?.OnClick(mode);
        }

        /// <summary>
        /// Updates the current gamemode and changes it to be <paramref name="newGamemode"/>
        /// </summary>
        public void ChangeGamemode(Gamemode newGamemode)
        {
            // Change gamemode enum
            currentGamemode = newGamemode;

            // Call OnDisable() in the old activeGamemodeScript (Unless it's null)
            activeGamemodeScript?.OnDisable();

            // Set the new activeGamemodeScript
            activeGamemodeScript = TypeToGamemode(newGamemode);

            // Call OnEnable() in the new activeGamemodeScript (Unless it's null)
            activeGamemodeScript?.OnEnable();
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
                    return cube;

                case Gamemode.ship:
                    return ship;

                case Gamemode.ufo:
                    return ufo;

                case Gamemode.ball:
                    return ball;

                case Gamemode.robot:
                    return robot;

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
