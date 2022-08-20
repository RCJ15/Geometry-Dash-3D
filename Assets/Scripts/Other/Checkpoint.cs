using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Player;
using GD3D.Easing;
using GD3D.Objects;
using GD3D.Level;
using GD3D.GDCamera;

namespace GD3D
{
    /// <summary>
    /// A class that contains data about the game during a certain state. <para/>
    /// Is used in practice mode for example.
    /// </summary>
    public class Checkpoint
    {
        //-- Player Position
        public float PlayerDistance;
        public float PlayerOffset;

        //-- Player Velocity
        public Vector3 PlayerVelocity;
        public Vector3 PlayerAngularVelocity;
        public float PlayerOffsetVelocity;

        //-- Player Transform
        public Vector3 PlayerPosition;
        public Vector3 PlayerScale;
        public Quaternion PlayerRotation;

        //-- Other Player Stuff
        public Gamemode PlayerGamemode = Gamemode.cube;
        public GameSpeed PlayerSpeed = GameSpeed.normalSpeed;

        public bool PlayerUpsideDown;
        public bool PlayerIsSmall;

        public bool PlayerIn3DMode;

        //-- Trails
        public bool TrailEnabled;
        public bool SecondaryTrailEnabled;

        //-- Ease states
        public List<EaseState> EaseStates = new List<EaseState>();

        //-- Borders
        public bool BordersActive;

        public float BorderFloorYPos;
        public float BorderRoofYPos;

        //-- Activated HashSets
        public HashSet<long> ActivatedJumpOrbs = new HashSet<long>();
        public HashSet<long> ActivatedJumpPads = new HashSet<long>();
        public HashSet<long> ActivatedPortals = new HashSet<long>();
        public HashSet<long> ActivatedTriggers = new HashSet<long>();

        //-- Colors
        public Dictionary<LevelColors.ColorType, Color> LevelColorData = new Dictionary<LevelColors.ColorType, Color>();

        //-- Camera
        public CameraBehaviour.CamState CamState;

        /// <summary>
        /// Sets this checkpoints data based on the players current gamemode, position, velocity and speed
        /// </summary>
        public void SetCheckpoint()
        {
            // Get the player
            PlayerMain player = PlayerMain.Instance;
            PlayerMovement playerMovement = player.Movement;
            PlayerGamemodeHandler gamemodeHandler = player.GamemodeHandler;

            // Set Player Position
            PlayerDistance = playerMovement.TravelAmount;
            PlayerOffset = playerMovement.Current3DOffset;

            // Set Player Velocity
            PlayerVelocity = player.Rigidbody.velocity;
            PlayerAngularVelocity = player.Rigidbody.angularVelocity;
            PlayerOffsetVelocity = playerMovement.OffsetVelocity;

            // Set Player Transform Data
            PlayerPosition = player.transform.position;
            PlayerScale = player.transform.localScale;
            PlayerRotation = player.transform.rotation;

            // Set Other Player Stuff
            PlayerGamemode = gamemodeHandler.CurrentGamemode;
            PlayerSpeed = PlayerMovement.CurrentSpeed;

            PlayerUpsideDown = gamemodeHandler.UpsideDown;
            PlayerIsSmall = gamemodeHandler.IsSmall;

            PlayerIn3DMode = playerMovement.In3DMode;

            // Set trail
            TrailEnabled = PlayerTrailManager.HaveTrail;
            SecondaryTrailEnabled = PlayerSecondaryTrailManager.HaveTrail;

            // Get the current ease states
            EaseStates = EasingManager.Save();

            // Get the borders current position
            BorderManager borderManager = BorderManager.Instance;

            BordersActive = BorderManager.BordersActive;
            BorderRoofYPos = borderManager.RoofYPos;
            BorderFloorYPos = borderManager.FloorYPos;

            // Get the id handler
            ObjectIDHandler idHandler = ObjectIDHandler.Instance;

            // Set hash sets to be clones
            ActivatedJumpOrbs = new HashSet<long>(idHandler.ActivatedJumpOrbs);
            ActivatedJumpPads = new HashSet<long>(idHandler.ActivatedJumpPads);
            ActivatedPortals = new HashSet<long>(idHandler.ActivatedPortals);
            ActivatedTriggers = new HashSet<long>(idHandler.ActivatedTriggers);

            // Get level color data
            LevelColors levelColors = LevelColors.Instance;

            LevelColorData = levelColors.Save();

            // Get camera data
            CameraBehaviour cam = CameraBehaviour.Instance;

            CamState = cam.Save();
        }

        /// <summary>
        /// Called when this checkpoint is being loaded. <para/>
        /// Is only called in <see cref="PlayerSpawn"/>.
        /// </summary>
        public void OnLoaded()
        {
            // Load the ease states
            EasingManager.Load(EaseStates);
        }
    }
}
