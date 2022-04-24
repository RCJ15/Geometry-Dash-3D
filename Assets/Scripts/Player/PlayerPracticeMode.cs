using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Audio;
using GD3D.CustomInput;
using GD3D.UI;

namespace GD3D.Player
{
    /// <summary>
    /// Handles going into and going out of practice mode. <para/>
    /// Also handles placing and removing checkpoint crystals in practice mode.
    /// </summary>
    public class PlayerPracticeMode : PlayerScript
    {
        //-- Instance
        public static PlayerPracticeMode Instance;

        //-- Practice mode bool
        private static bool _inPracticeMode;
        public static bool InPracticeMode
        {
            get => _inPracticeMode;
            set
            {
                // Return if the value is the same as the current value
                // This is so we don't do some unnecessary stuff if the value is set to what it already is
                if (value == _inPracticeMode)
                {
                    return;
                }

                // Set to the value
                _inPracticeMode = value;

                // Play practice music or stop it depending on if we are entering or exiting practice mode
                MusicPlayer.TogglePracticeSong(value);

                // If we exit practice mode, destroy all current crystal and clear all checkpoints
                if (!value)
                {
                    foreach (GameObject obj in Instance.CheckpointCrystals)
                    {
                        Destroy(obj);
                    }

                    Instance.Checkpoints.Clear();
                    Instance.CheckpointCrystals.Clear();
                }
            }
        }

        //-- Checkpoint
        [SerializeField] private GameObject checkpointCrystal;

        public List<Checkpoint> Checkpoints = new List<Checkpoint>();
        private List<GameObject> CheckpointCrystals = new List<GameObject>();

        //-- Input
        private Key placeKey;
        private Key removeKey;

        /// <summary>
        /// Returns the latest placed checkpoint in the checkpoints list. <para/>
        /// Will return null if there are no checkpoints placed.
        /// </summary>
        public Checkpoint LatestCheckpoint
        {
            get
            {
                int checkpointCount = Checkpoints.Count;

                if (checkpointCount <= 0)
                {
                    return null;
                }

                return Checkpoints[checkpointCount - 1];
            }
        }

        public override void Awake()
        {
            base.Awake();

            // Set the instance
            Instance = this;
        }

        public override void Start()
        {
            base.Start();

            // Get keys
            placeKey = PlayerInput.GetKey("Place Checkpoint Crystal");
            removeKey = PlayerInput.GetKey("Remove Checkpoint Crystal");
        }

        public override void Update()
        {
            base.Update();

            // Return if we are not in practice mode or if the game is paused
            if (!InPracticeMode || PauseMenu.IsPaused)
            {
                return;
            }

            // Place checkpoint crystal if the place key was pressed and we are not dead
            if (!player.IsDead && placeKey.Pressed(PressMode.down))
            {
                PlaceCheckpointCrystal();
            }

            // Get the length of the checkpoint
            int checkpointCount = Checkpoints.Count;

            // Remove checkpoint crystal if the remove key was pressed and if there is more than 0 crystals existing
            if (checkpointCount > 0 && removeKey.Pressed(PressMode.down))
            {
                int index = checkpointCount - 1;

                // Get the latest placed checkpoint and remove it
                Checkpoints.RemoveAt(index);

                // Destroy checkpoint crystal as well
                Destroy(CheckpointCrystals[index]);

                CheckpointCrystals.RemoveAt(index);
            }
        }

        /// <summary>
        /// Places a checkpoint crystal
        /// </summary>
        private void PlaceCheckpointCrystal()
        {
            // Create a new checkpoint
            Checkpoint checkpoint = new Checkpoint();
            checkpoint.SetCheckpoint();

            // Add the checkpoint to the list
            Checkpoints.Add(checkpoint);

            // Create a new checkpoint crystal
            Quaternion crystalRot = Quaternion.Euler(0, player.transform.eulerAngles.y, 0);
            GameObject newCrystal = Instantiate(checkpointCrystal, player.transform.position, crystalRot);
            newCrystal.name = $"{checkpointCrystal.name} ({CheckpointCrystals.Count + 1})";
            
            CheckpointCrystals.Add(newCrystal);
        }
    }
}
