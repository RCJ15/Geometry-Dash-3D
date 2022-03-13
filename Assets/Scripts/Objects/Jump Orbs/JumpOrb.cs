using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Player;
using GD3D.CustomInput;
using GD3D.ObjectPooling;

namespace GD3D
{
    /// <summary>
    /// Jump orb base class that all jump orbs inherit from
    /// </summary>
    public abstract class JumpOrb : MonoBehaviour
    {
        public static bool CantHitOrbs = false;
        private static bool s_doneStaticUpdate = false;

        [Header("Jump Orb Settings")]
        [SerializeField] private bool multiTrigger;
        private bool _cantBePressed = false;

        private bool ButtonPressed => _player.KeyHold || _player.InputBuffer > 0;

        [Space]
        [SerializeField] private ParticleSystemRenderer particles;

        [Space]
        [SerializeField] private PoolObject triggerEffect;
        [SerializeField] private int triggerPoolSize = 3;
        private ObjectPool<PoolObject> _triggerPool;

        [Space]
        [SerializeField] private PoolObject ringEffect;
        [SerializeField] private int ringPoolSize = 5;
        private ObjectPool<PoolObject> _ringPool;

        [Header("Jump Orb Color")]
        [SerializeField] private Color orbColor = Color.white;
        [SerializeField] private float particleAlpha = 0.4f;
        [SerializeField] private bool updateColors = true;

        [Space]
        [SerializeField] protected MeshRenderer meshRenderer;

        //-- Player Related
        protected PlayerMain _player;
        protected bool _touchingPlayer; // Pretty sus ngl

        private LayerMask _playerLayer;

        //-- Other Stuff
        protected Transform _transform;

        public virtual void Awake()
        {
            // Set transform
            _transform = transform;

            // Setup the triggerEffect and ringEffect by creating a copy and setting the copy for both objects
            GameObject triggerObj = Instantiate(triggerEffect.gameObject, _transform.position, Quaternion.identity);
            triggerObj.name = triggerEffect.gameObject.name;
            triggerObj.transform.localPosition = Vector3.zero;

            if (updateColors)
            {
                // Change the MaterialColorer to match the player colors
                MaterialColorer colorer = triggerObj.GetComponentInChildren<MaterialColorer>();

                // Make sure to have the same alpha value
                Color playerColor = orbColor;
                playerColor.a = colorer.GetColor.a;

                colorer.GetColor = playerColor;

                // Also set colors for the particles
                Color particleColor = orbColor;
                particleColor.a = particleAlpha;

                MaterialColorer.UpdateMaterialColor(particles.material, particleColor, true, true);

                // Also update colors of the mesh
                MaterialColorer.UpdateRendererMaterials(meshRenderer, orbColor, false, false);
            }

            // Create pools
            _triggerPool = new ObjectPool<PoolObject>(triggerObj, triggerPoolSize);
            _ringPool = new ObjectPool<PoolObject>(ringEffect, ringPoolSize);

            // Destroy the newly created objects because we have no use out of them anymore
            Destroy(triggerObj);
        }

        public virtual void Start()
        {
            // Set CantHitOrbs to false
            if (CantHitOrbs)
            {
                CantHitOrbs = false;
            }

            // Get player instance
            _player = PlayerMain.Instance;

            // Set player layer
            _playerLayer = _player.GetLayer;

            // Subscribe to events
            _player.OnDeath += OnDeath;
        }

        public virtual void Update()
        {
            // Return if the static update has already been done
            if (s_doneStaticUpdate)
            {
                return;
            }

            // Buffering orbs in the air logic
            if (ButtonPressed && !CantHitOrbs && !_touchingPlayer)
            {
                GamemodeScript script = _player.gamemode.CurrentGamemodeScript;

                if (script.BufferOrbs)
                {
                    // If the player lands then they are not allowed to buffer anymore
                    if (script.onGround)
                    {
                        CantHitOrbs = true;
                    }
                }
                // Do not allow the player to buffer at all
                else
                {
                    CantHitOrbs = true;
                }
            }

            // Check if the Key is up and if the player can't hit orbs
            if (_player.KeyUp && CantHitOrbs)
            {
                // Make so the player can hit orbs again
                CantHitOrbs = false;
            }

            // Set doneStaticUpdate to true
            s_doneStaticUpdate = true;
        }

        public virtual void LateUpdate()
        {
            // Reset so the static update will work next frame
            if (s_doneStaticUpdate)
            {
                s_doneStaticUpdate = false;
            }
        }

        /// <summary>
        /// Implement this to determine what happens when the player uses the jump orb
        /// </summary>
        public abstract void OnPressed();

        /// <summary>
        /// Override this to determine a custom jump orb condition that has to be met in order for the player to use the jump orb. <para/>
        /// So this must return true in order for the jump orb to be usable.
        /// </summary>
        public virtual bool CustomJumpOrbCondition()
        {
            return true;
        }

        public virtual void OnDeath()
        {
            // Set CantHitOrbs to false
            if (CantHitOrbs)
            {
                CantHitOrbs = false;
            }
        }

        public virtual void OnTriggerEnter(Collider col)
        {
            // Check if it's the player we are colliding with
            if (col.gameObject.layer == _playerLayer)
            {
                // Spawn ring effect
                if (!_ringPool.IsEmpty())
                {
                    PoolObject ringEffect = _ringPool.SpawnFromPool(_transform.position, _transform.rotation);
                    ringEffect.RemoveAfterTime(0.5f);
                }

                // Set touching
                _touchingPlayer = true;
            }
        }

        public virtual void OnTriggerExit(Collider col)
        {
            // Check if it's the player we are colliding with
            if (col.gameObject.layer == _playerLayer)
            {
                // Set not touching
                _touchingPlayer = false;
            }
        }

        public virtual void OnTriggerStay(Collider col)
        {
            // Check if it's the player we are colliding with
            if (col.gameObject.layer == _playerLayer)
            {
                // Return if the correct conditions haven't been met
                if (!ButtonPressed || _player.dead || CantHitOrbs || _cantBePressed || !CustomJumpOrbCondition())
                {
                    return;
                }

                // Make it so the player can't hit any other orbs
                CantHitOrbs = true;

                // Make it so this orb cant be pressed afterwards if it's not multiTrigger
                if (!multiTrigger)
                {
                    _cantBePressed = true;
                }

                // Spawn trigger effect
                if (!_triggerPool.IsEmpty())
                {
                    PoolObject triggerEffect = _triggerPool.SpawnFromPool(_transform.position, _transform.rotation);
                    triggerEffect.RemoveAfterTime(0.5f);
                }

                // Enable the player trail
                PlayerTrailManager.HaveTrail = true;

                // Call OnPressed method
                OnPressed();
            }
        }
    }
}
