using System.Runtime.Serialization;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Player;

namespace GD3D.Objects
{
    /// <summary>
    /// Handles all the IDs for objects like <see cref="JumpOrb"/>, <see cref="JumpPad"/> and <see cref="Portal"/> for example.
    /// </summary>
    public class ObjectIDHandler : MonoBehaviour
    {
        private static readonly ObjectIDGenerator idGenerator = new ObjectIDGenerator();

        //-- Instance
        public static ObjectIDHandler Instance;

        //-- Dictionaries
        public Dictionary<long, JumpOrb> JumpOrbs = new Dictionary<long, JumpOrb>();
        public Dictionary<long, JumpPad> JumpPads = new Dictionary<long, JumpPad>();
        public Dictionary<long, Portal> Portals = new Dictionary<long, Portal>();
        public Dictionary<long, Trigger> Triggers = new Dictionary<long, Trigger>();

        //-- Activated HashSets
        public HashSet<long> ActivatedJumpOrbs = new HashSet<long>();
        public HashSet<long> ActivatedJumpPads = new HashSet<long>();
        public HashSet<long> ActivatedPortals = new HashSet<long>();
        public HashSet<long> ActivatedTriggers = new HashSet<long>();

        private void Awake()
        {
            // Set the instance
            Instance = this;
        }

        private void Start()
        {
            PlayerMain player = PlayerMain.Instance;

            // Subscribe to events
            player.OnDeath += OnDeath;
            player.OnRespawn += OnRespawn;
        }

        private void Update()
        {

        }

        private void OnDeath()
        {
            // Clear all the activated HashSets since all the triggers and stuff get re enabled
            ActivatedJumpOrbs.Clear();
            ActivatedJumpPads.Clear();
            ActivatedPortals.Clear();
            ActivatedTriggers.Clear();
        }

        private void OnRespawn(bool inPracticeMode, Checkpoint checkpoint)
        {
            // Check if we are in practice mode
            if (inPracticeMode)
            {
                // Set to the checkpoints HashSets
                ActivatedJumpOrbs = new HashSet<long>(checkpoint.ActivatedTriggers);
                ActivatedJumpPads = new HashSet<long>(checkpoint.ActivatedJumpPads);
                ActivatedPortals = new HashSet<long>(checkpoint.ActivatedPortals);
                ActivatedTriggers = new HashSet<long>(checkpoint.ActivatedTriggers);
            }
        }

        #region GetID
        /// <summary>
        /// Generates a new ID for a <see cref="JumpOrb"/> object.
        /// </summary>
        /// <returns>The newly generated ID.</returns>
        public long GetID(JumpOrb jumpOrb)
        {
            long id = idGenerator.GetId(jumpOrb, out bool _);

            JumpOrbs.Add(id, jumpOrb);

            return id;
        }

        /// <summary>
        /// Generates a new ID for a <see cref="JumpPad"/> object.
        /// </summary>
        /// <returns>The newly generated ID.</returns>
        public long GetID(JumpPad jumpPad)
        {
            long id = idGenerator.GetId(jumpPad, out bool _);

            JumpPads.Add(id, jumpPad);

            return id;
        }

        /// <summary>
        /// Generates a new ID for a <see cref="Portal"/> object.
        /// </summary>
        /// <returns>The newly generated ID.</returns>
        public long GetID(Portal portal)
        {
            long id = idGenerator.GetId(portal, out bool _);

            Portals.Add(id, portal);

            return id;
        }

        /// <summary>
        /// Generates a new ID for a <see cref="Trigger"/> object.
        /// </summary>
        /// <returns>The newly generated ID.</returns>
        public long GetID(Trigger trigger)
        {
            long id = idGenerator.GetId(trigger, out bool _);

            Triggers.Add(id, trigger);

            return id;
        }
        #endregion

        #region ActivateID
        /// <summary>
        /// Activates the ID associated with the <see cref="JumpOrb"/> object.
        /// </summary>
        public void ActivateID(JumpOrb jumpOrb)
        {
            ActivatedJumpOrbs.Add(jumpOrb.ID);
        }

        /// <summary>
        /// Activates the ID associated with the <see cref="JumpPad"/> object.
        /// </summary>
        public void ActivateID(JumpPad jumpPad)
        {
            ActivatedJumpPads.Add(jumpPad.ID);
        }

        /// <summary>
        /// Activates the ID associated with the <see cref="Portal"/> object.
        /// </summary>
        public void ActivateID(Portal portal)
        {
            ActivatedPortals.Add(portal.ID);
        }

        /// <summary>
        /// Activates the ID associated with the <see cref="Trigger"/> object.
        /// </summary>
        public void ActivateID(Trigger trigger)
        {
            ActivatedTriggers.Add(trigger.ID);
        }
        #endregion

        #region IsActivated
        /// <summary>
        /// Checks if the <see cref="JumpOrb"/> object is activated.
        /// </summary>
        public bool IsActivated(JumpOrb jumpOrb)
        {
            return ActivatedJumpOrbs.Contains(jumpOrb.ID);
        }

        /// <summary>
        /// Checks if the <see cref="JumpPad"/> object is activated.
        /// </summary>
        public bool IsActivated(JumpPad jumpPad)
        {
            return ActivatedJumpPads.Contains(jumpPad.ID);
        }

        /// <summary>
        /// Checks if the <see cref="Portal"/> object is activated.
        /// </summary>
        public bool IsActivated(Portal portal)
        {
            return ActivatedPortals.Contains(portal.ID);
        }

        /// <summary>
        /// Checks if the <see cref="Trigger"/> object is activated.
        /// </summary>
        public bool IsActivated(Trigger trigger)
        {
            return ActivatedTriggers.Contains(trigger.ID);
        }
        #endregion
    }
}
