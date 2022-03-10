using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GD3D.ObjectPooling;

namespace GD3D.Player
{
    /// <summary>
    /// Controls the player spawning
    /// </summary>
    public class PlayerSpawn : PlayerScript
    {
        [SerializeField] private int poolSize = 4;
        private ObjectPool<PoolObject> pool;

        [SerializeField] private PoolObject respawnRing;

        [SerializeField] private float respawnTime;

        [SerializeField] private TMP_Text attemptText;
        private int _currentAttempt = 1;
        public int CurrentAttemp => _currentAttempt;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        public override void Start()
        {
            base.Start();

            // Subscribe to the OnDeath event
            player.OnDeath += OnDeath;

            // Setup the respawnRing obj by creating a copy and setting the copy
            GameObject obj = Instantiate(respawnRing.gameObject, transform.position, Quaternion.identity, transform);
            obj.transform.position = _transform.position;

            // Change the line renderers color
            LineRenderer lr = obj.GetComponent<LineRenderer>();
            lr.startColor = PlayerColor1;
            lr.endColor = PlayerColor1;

            // Create pool
            pool = new ObjectPool<PoolObject>(obj, poolSize,
                (poolObj) =>
                {
                    poolObj.transform.SetParent(_transform);
                    poolObj.transform.localPosition = Vector3.zero;
                }
            );

            // Destroy the newly created object because we have no use out of it anymore
            Destroy(obj);
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        public override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        /// <summary>
        /// Is called when the player dies
        /// </summary>
        private void OnDeath()
        {
            // Disable the mesh
            player.mesh.ToggleCurrentMesh(false);

            // Stop the currently active respawn coroutine
            if (currentRespawnCoroutine != null)
            {
                StopCoroutine(currentRespawnCoroutine);
            }

            // Start the respawn coroutine
            currentRespawnCoroutine = StartCoroutine(Respawn());
        }

        private Coroutine currentRespawnCoroutine;
        /// <summary>
        /// Makes the player flash on/off and spawn respawn rings
        /// </summary>
        public IEnumerator Respawn()
        {
            // Wait 1 second
            yield return new WaitForSeconds(1f);

            _currentAttempt++;
            attemptText.text = "Attempt " + _currentAttempt;

            // Invoke respawn event
            player.InvokeRespawnEvent();

            // Make the player flash on/off and spawn respawn rings every time the player is turned on
            // Do this 3 times total over the course of 0.6 seconds
            SpawnRespawnRing();
            ToggleMesh(true);

            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(0.05f);

                ToggleMesh(false);

                yield return new WaitForSeconds(0.05f);

                SpawnRespawnRing();
                ToggleMesh(true);
            }
        }

        /// <summary>
        /// Just a shortcut for <see cref="PlayerMesh.ToggleCurrentMesh(bool)"/>
        /// </summary>
        private void ToggleMesh(bool enable)
        {
            player.mesh.ToggleCurrentMesh(enable);
        }

        /// <summary>
        /// Spawns a respawn ring (duh)
        /// </summary>
        private void SpawnRespawnRing()
        {
            // Spawn the ring
            PoolObject obj = pool.SpawnFromPool(_transform.position);
            obj.RemoveAfterTime(0.5f);
        }
    }
}
