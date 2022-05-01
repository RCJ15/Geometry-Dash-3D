using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.ObjectPooling;

namespace GD3D.Player
{
    /// <summary>
    /// Handles spawning and removing all player trails. <para/>
    /// Use this to toggle on/off the players trail.
    /// </summary>
    public class PlayerTrailManager : PlayerScript
    {
        //-- Instance
        private static PlayerTrailManager Instance;

        [Header("Trails")]
        [SerializeField] private int trailAmount = 10;
        [SerializeField] private PlayerTrail[] trailCopyables; // For the different types of trails

        private ObjectPool<PlayerTrail> _pool;

        private bool _haveTrail = false;
        private PlayerTrail _currentTrail;

        private Transform _trailPosition;

        /// <summary>
        /// Returns if the player has a trail currently or not. <para/>
        /// When set, it will toggle if the player has a trail or not.
        /// </summary>
        public static bool HaveTrail
        {
            get => Instance._haveTrail;
            set => Instance.SetTrail(value);
        }

        /// <summary>
        /// Toggles if the player has a trail or not depending on if <paramref name="enable"/> is true or not.
        /// </summary>
        public void SetTrail(bool enable, bool forced = false)
        {
            // We don't have a trail and the value is true, so spawn a trail
            if (enable && (!_haveTrail || forced))
            {
                // Disable old trail
                if (_currentTrail != null)
                {
                    _currentTrail.DisableTrail();

                    _currentTrail = null;
                }

                // Only spawn trail if the pool isn't empty
                if (!_pool.IsEmpty())
                {
                    _currentTrail = _pool.SpawnFromPool();

                    _currentTrail.transform.SetParent(_trailPosition);
                    _currentTrail.transform.localPosition = Vector3.zero;
                }
            }
            // We have a trail currently and the value is false, so remove the current trail
            else if (!enable && (_haveTrail || forced) && _currentTrail != null)
            {
                if (_currentTrail != null)
                {
                    _currentTrail.DisableTrail();

                    _currentTrail = null;
                }
            }

            // Set it to the given value regardless
            _haveTrail = enable;
        }

        public override void Awake()
        {
            base.Awake();

            // Set instance
            Instance = this;
        }

        public override void Start()
        {
            base.Start();

            // Subscribe to events
            player.OnDeath += OnDeath;
            player.OnRespawn += OnRespawn;
            player.GamemodeHandler.OnChangeGamemode += (gamemode) => OnChangeGamemode(gamemode, true);

            // Subscribe to teleport event if we are in the main menu
            if (player.InMainMenu)
            {
                player.Movement.OnMainMenuTeleport += () =>
                {
                    // Disable the trail
                    HaveTrail = false;

                    // Wait for 1 frame
                    Helpers.TimerEndOfFrame(this, () =>
                    {
                        // Change all trails to have the correct color
                        foreach (PlayerTrail trail in _pool)
                        {
                            trail.UpdateColor(PlayerColor2);
                        }
                    });
                };
            }

            print("Reminder to use icon customization for this");
            PlayerTrail trailToCopy = trailCopyables[0];

            _pool = new ObjectPool<PlayerTrail>(trailToCopy, trailAmount, 
                (obj) =>
                {
                    obj.UpdateColor(PlayerColor2);
                    obj.ParentTransform = _transform;
                }
            );

            // Update gamemode manually
            OnChangeGamemode(player.GamemodeHandler.CurrentGamemode, false);
        }

        private void OnDeath()
        {
            // Disable the trail
            HaveTrail = false;
        }

        private void OnRespawn(bool inPracticeMode, Checkpoint checkpoint)
        {
            // Set if trail is enabled or not based on the checkpoint data
            if (inPracticeMode)
            {
                HaveTrail = checkpoint.TrailEnabled;
            }
            else
            {
                // Disabled if we are not in practice mode
                HaveTrail = false;
            }
        }

        private void OnChangeGamemode(Gamemode gamemode, bool resetTrail)
        {
            // Set the position
            _trailPosition = player.Mesh.CurrentTrailPosition;

            // Set it to this object if it's null
            if (_trailPosition == null)
            {
                _trailPosition = _transform;
            }

            // Reset trail
            if (resetTrail)
            {
                SetTrail(true, true);
            }
        }
    }
}
