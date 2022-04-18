using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.Easing
{
    /// <summary>
    /// In every scene, there'll be an object which will have an <see cref="EasingManager"/> attached to it. <para/>
    /// An easing manager, as it's name suggests, will manage and handle all easings in the scene. <para/>
    /// It is also responsible for saving and loading easing when the player dies with a checkpoint or such.
    /// </summary>
    public class EasingManager : MonoBehaviour
    {
        private static EasingManager s_instance;

        /// <summary>
        /// Returns the current active instance where all the easings are ran.
        /// </summary>
        public static EasingManager Instance
        {
            get
            {
                // Create a new easing manager if there is not one currently
                if (s_instance == null)
                {
                    GameObject newObj = new GameObject("Easing Manager", typeof(EasingManager));

                    s_instance = newObj.GetComponent<EasingManager>();
                }

                return s_instance;
            }
        }

        private Dictionary<long, EaseObject> _easeObjects = new Dictionary<long, EaseObject>();
        private Queue<long> _easeObjectsToRemove = new Queue<long>();

        public Action<long> OnEaseObjectAdd;
        public Action<long> OnEaseObjectRemove;

        private void Awake()
        {
            // Set instance if this is not already the instance
            if (s_instance != this)
            {
                s_instance = this;
            }
        }

        private void Update()
        {
            // Update each active easeObject
            foreach (var pair in _easeObjects)
            {
                EaseObject ease = pair.Value;

                // Skip this ease if it's not active
                if (!ease.Active)
                {
                    continue;
                }

                // Increase time and update
                ease.Time += Time.deltaTime * (ease.Reverse ? -1 : 1);
                ease.Update();
            }

            // Remove all ease objects in the easeObjectsToRemove queue
            int length = _easeObjectsToRemove.Count;
            for (int i = 0; i < length; i++)
            {
                _easeObjects.Remove(_easeObjectsToRemove.Dequeue());
            }
        }

        /// <summary>
        /// Adds an <see cref="EaseObject"/> to the active list of easings. <para/>
        /// Also calls <see cref="OnEaseObjectAdd"/>.
        /// </summary>
        public static void AddEaseObject(EaseObject obj)
        {
            long id = obj.ID;

            Instance.OnEaseObjectAdd?.Invoke(id);

            Instance._easeObjects.Add(id, obj);
        }

        /// <summary>
        /// Removes an <see cref="EaseObject"/>. <para/>
        /// Also calls <see cref="OnEaseObjectRemove"/> using the <see cref="EaseObject"/> id.
        /// </summary>
        public static void RemoveEaseObject(EaseObject obj)
        {
            RemoveEaseObject(obj.ID);
        }

        /// <summary>
        /// Removes an <see cref="EaseObject"/> using it's <paramref name="id"/>. <para/>
        /// Also calls <see cref="OnEaseObjectRemove"/>.
        /// </summary>
        public static void RemoveEaseObject(long id)
        {
            Instance.OnEaseObjectRemove?.Invoke(id);

            Instance._easeObjectsToRemove.Enqueue(id);
        }

        /// <summary>
        /// Returns true if the given <paramref name="id"/> of an <see cref="EaseObject"/> exists.
        /// </summary>
        public static bool HasEaseObject(long id)
        {
            return Instance._easeObjects.ContainsKey(id);
        }

        /// <summary>
        /// Returns the <see cref="EaseObject"/> with the given <paramref name="id"/>.
        /// </summary>
        public static EaseObject GetEaseObject(long id)
        {
            return Instance._easeObjects[id];
        }

        /// <summary>
        /// Sets <paramref name="obj"/> as the <see cref="EaseObject"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <returns>True or false depending on if the operation was successful, meaning that the <see cref="EaseObject"/> exists.</returns>
        public static bool TryGetEaseObject(long id, out EaseObject obj)
        {
            return Instance._easeObjects.TryGetValue(id, out obj);
        }

        /// <summary>
        /// Will attempt to remove the current <see cref="EaseObject"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <returns>True or false depending on if the operation was successful, meaning that the <see cref="EaseObject"/> exists and was removed.</returns>
        public static bool TryRemoveEaseObject(long id)
        {
            // Use try get to check if the ease object exists
            if (TryGetEaseObject(id, out EaseObject obj))
            {
                // If it exists, remove it
                RemoveEaseObject(obj);

                // Return true if the ease object exists and was removed
                return true;
            }

            // Return false if the ease object does not exist
            return false;
        }

        /// <summary>
        /// Will attempt to remove the current <see cref="EaseObject"/> with the given <paramref name="id"/>. (<see cref="Nullable"/> <see cref="long"/> version)
        /// </summary>
        /// <returns>True or false depending on if the operation was successful, meaning that the <see cref="EaseObject"/> exists and was removed.</returns>
        public static bool TryRemoveEaseObject(long? id)
        {
            // Check if the ID has a value (if it's not null)
            if (id.HasValue)
            {
                // Use regular try remove
                return TryRemoveEaseObject(id.Value);
            }

            // If it's null, return false since the try failed
            return false;
        }
    }
}
