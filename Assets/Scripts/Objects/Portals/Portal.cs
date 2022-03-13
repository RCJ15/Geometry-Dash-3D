using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GD3D.Player;
using GD3D.ObjectPooling;

namespace GD3D.Objects
{
    /// <summary>
    /// Portal base class that all portals inherit from
    /// </summary>
    public abstract class Portal : MonoBehaviour
    {
        [Header("Base Portal Settings")]
        [SerializeField] private bool multiTrigger;
        private bool _cantBeEntered;

        [Space]
        [SerializeField] private Animator spawnEffect;
        [SerializeField] private float flashTime = 0.5f;
        [SerializeField] private UnityEvent onEnterPortal;

        [Header("Portal Color")]
        [SerializeField] private bool updateColors = true;
        [SerializeField] protected Color color = Color.white;

        [Space]
        [SerializeField] protected MeshRenderer mesh;
        [SerializeField] protected ParticleSystemRenderer particles;
        [SerializeField] private int materialIndex;

        protected Material _mainMat => mesh.materials[materialIndex];
        protected PlayerMain _player;

        public virtual void Start()
        {
            _player = PlayerMain.Instance;

            // Update the mesh color to match the portal color
            if (updateColors && mesh != null && particles != null)
            {
                // Update the color
                MaterialColorer.UpdateMaterialColor(_mainMat, color, true, true);

                Color newColor = color;
                newColor.a = particles.material.color.a;

                MaterialColorer.UpdateMaterialColor(particles.material, newColor, true, true);
            }

            // Subsribe to events
            _player.OnRespawn += OnRespawn;
        }

        /// <summary>
        /// Override this to do stuff when the player dies
        /// </summary>
        public virtual void OnRespawn()
        {
            _cantBeEntered = false;
        }

        /// <summary>
        /// Implement this to determine what happens when the player enters this portal
        /// </summary>
        public abstract void OnEnterPortal();

        /// <summary>
        /// Override this to determine a custom portal condition that has to be met in order for the player to enter the portal. <para/>
        /// So this must return true in order for the portal to be entered.
        /// </summary>
        public virtual bool CustomPortalCondition()
        {
            return true;
        }

        public virtual void OnTriggerEnter(Collider col)
        {
            // Return if the portal can't be entered or the custom condition is false
            if (_cantBeEntered || !CustomPortalCondition())
            {
                return;
            }

            // Player entered portal
            if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                OnEnterPortal();

                // Flash Color
                StartCoroutine(FlashWhite(flashTime));

                // Trigger event
                onEnterPortal?.Invoke();

                // Trigger spawn effect
                if (spawnEffect != null)
                {
                    spawnEffect.SetTrigger("Reset");
                }

                // Disable if not multi trigger
                if (!multiTrigger)
                {
                    _cantBeEntered = true;
                }

                // invoke player event
                _player.OnEnterPortal?.Invoke(this);
            }
        }

        /// <summary>
        /// Makes the portals main color flash white during the given <paramref name="time"/>
        /// </summary>
        private IEnumerator FlashWhite(float time)
        {
            float currentTimer = time;

            // Pseudo update method
            while (currentTimer > 0)
            {
                float t = currentTimer / time;

                // Change the color
                Color targetColor = Color.Lerp(color, Color.white, Helpers.Map(0, 1, 0.2f, 1, t));
                MaterialColorer.UpdateMaterialColor(_mainMat, targetColor, true, true);

                // Wait for next frame
                currentTimer -= Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            // Reset the color
            MaterialColorer.UpdateMaterialColor(_mainMat, color, true, true);
        }
    }
}
