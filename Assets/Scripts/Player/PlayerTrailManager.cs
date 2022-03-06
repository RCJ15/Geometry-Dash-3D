using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        [SerializeField] private int trailAmount = 50;
        [SerializeField] private PlayerTrail[] trailCopyables; // For the different types of trails

        private bool _haveTrail = false;
        private PlayerTrail _currentTrail;

        private Queue<PlayerTrail> _trails = new Queue<PlayerTrail>();

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
                _currentTrail = _trails.Dequeue();
                _currentTrail.Attach(_transform);
            }
            // We have a trail currently and the value is false, so remove the current trail
            else if (!enable && _haveTrail && _currentTrail != null)
            {
                _currentTrail.Detach();
                _trails.Enqueue(_currentTrail);

                _currentTrail = null;
            }

            // Set it to the given value regardless
            _haveTrail = enable;
        }

        private void Awake()
        {
            // Set instance
            Instance = this;

            print("Reminder to use icon customization for this");
        }

        public override void Start()
        {
            base.Start();

            // Subscribe to events
            player.OnDeath += OnDeath;

            GenerateTrails(trailCopyables[0], trailAmount);
        }

        private void GenerateTrails(PlayerTrail copy, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject newObj = Instantiate(copy.gameObject, _transform.position, Quaternion.identity, null);
                PlayerTrail newTrail = newObj.GetComponent<PlayerTrail>();

                // Update color
                newTrail.UpdateColor(PlayerColor2);

                // Disable the trail
                newTrail.ToggleTrail(false);

                // Add to queue
                _trails.Enqueue(newTrail);
            }
        }

        private void OnDeath()
        {
            // Disable the players trail when they die
            HaveTrail = false;
        }
    }
}
