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

        private ObjectPool<PlayerTrail> pool;

        private bool _haveTrail = false;
        private PlayerTrail _currentTrail;

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
        public void SetTrail(bool enable)
        {
            // We don't have a trail and the value is true, so spawn a trail
            if (enable && !_haveTrail)
            {
                // Only spawn trail if the pool isn't empty
                if (!pool.IsEmpty())
                {
                    _currentTrail = pool.SpawnFromPool();
                }
            }
            // We have a trail currently and the value is false, so remove the current trail
            else if (!enable && _haveTrail && _currentTrail != null)
            {
                _currentTrail.DisableTrail();

                _currentTrail = null;
            }

            // Set it to the given value regardless
            _haveTrail = enable;
        }

        private void Awake()
        {
            // Set instance
            Instance = this;
        }

        public override void Start()
        {
            base.Start();

            // Subscribe to events
            player.OnDeath += OnDeath;

            print("Reminder to use icon customization for this");
            PlayerTrail trailToCopy = trailCopyables[0];

            pool = new ObjectPool<PlayerTrail>(trailToCopy, trailAmount, 
                (obj) =>
                {
                    obj.UpdateColor(PlayerColor2);
                    obj.ParentTransform = _transform;
                }
            );
        }

        private void OnDeath()
        {
            // Disable the players trail when they die
            HaveTrail = false;
        }
    }
}
