using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Player;
using GD3D.ObjectPooling;
using GD3D.Level;

namespace GD3D.Objects
{
    /// <summary>
    /// Jump pad base class that all jump pads inherit from
    /// </summary>
    public abstract class JumpPad : MonoBehaviour
    {
        //-- ID
        protected ObjectIDHandler idHandler;
        [HideInInspector] public long ID;

        private bool _isActivated = false;

        [Header("Jump Pad Settings")]
        [LevelSave] [SerializeField] private bool multiTrigger;

        [Space]
        [SerializeField] private ParticleSystemRenderer particles;

        [Space]
        [SerializeField] private PoolObject triggerEffect;
        [SerializeField] private int triggerPoolSize = 3;
        private ObjectPool<PoolObject> _triggerPool;

        [SerializeField] private Transform triggerEffectPos;

        [Header("Jump Pad Color")]
        [SerializeField] private Color padColor = Color.white;
        [SerializeField] private float particleAlpha = 0.4f;
        [SerializeField] private bool updateColors = true;

        [Space]
        [SerializeField] protected MeshRenderer meshRenderer;

        //-- Player Related
        protected PlayerMain _player;

        private LayerMask playerLayer;

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
                Color playerColor = padColor;
                playerColor.a = colorer.GetColor.a;

                colorer.GetColor = playerColor;

                // Also set colors for the particles
                Color particleColor = padColor;
                particleColor.a = particleAlpha;

                MaterialColorer.UpdateMaterialColor(particles.material, particleColor, true, true);

                // Also update colors of the mesh
                MaterialColorer.UpdateRendererMaterials(meshRenderer, padColor, false, false);
            }

            // Create pools
            _triggerPool = new ObjectPool<PoolObject>(triggerObj, triggerPoolSize);

            // Destroy the newly created objects because we have no use out of them anymore
            Destroy(triggerObj);

            // Get the ID handler and generate ID
            idHandler = ObjectIDHandler.Instance;

            ID = idHandler.GetID(this);
        }

        public virtual void Start()
        {
            // Get player instance
            _player = PlayerMain.Instance;

            // Set player layer
            playerLayer = _player.GetLayer;

            // Subscribe to events
            _player.OnRespawn += OnRespawn;
        }

        /// <summary>
        /// Implement this to determine what happens when the player uses the jump orb
        /// </summary>
        public abstract void OnTouched();

        /// <summary>
        /// Override this to determine a custom jump pad condition that has to be met in order for the player to use the jump pad. <para/>
        /// So this must return true in order for the jump pad to be usable.
        /// </summary>
        public virtual bool CustomJumpPadCondition => true;

        public virtual void OnTriggerEnter(Collider col)
        {
            // Check if it's the player we are colliding with
            if (col.gameObject.layer == playerLayer)
            {
                // Return if the correct conditions haven't been met
                if (_player.IsDead || _isActivated || !CustomJumpPadCondition)
                {
                    return;
                }

                // Make it so this pad cant be touched afterwards if it's not multiTrigger
                if (!multiTrigger)
                {
                    idHandler.ActivateID(this);
                    _isActivated = true;
                }

                // Spawn trigger effect
                if (!_triggerPool.IsEmpty())
                {
                    PoolObject triggerEffect = _triggerPool.SpawnFromPool(triggerEffectPos.position, triggerEffectPos.rotation);
                    triggerEffect.RemoveAfterTime(0.3f);
                }

                // Enable the player trail
                PlayerTrailManager.HaveTrail = true;

                // Call OnTouched method
                OnTouched();
            }
        }

        /// <summary>
        /// Override this to decide what happens when the player dies
        /// </summary>
        public virtual void OnRespawn(bool inPracticeMode, Checkpoint checkpoint)
        {
            _isActivated = idHandler.IsActivated(this);
        }
    }
}
