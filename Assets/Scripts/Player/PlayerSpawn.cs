using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Game.Player
{
    /// <summary>
    /// 
    /// </summary>
    public class PlayerSpawn : PlayerScript
    {
        [SerializeField] private GameObject respawnRing;
        [SerializeField] private MaterialColorer playerColorer;

        [SerializeField] private float respawnTime;

        [SerializeField] private TMP_Text attemptText;
        private int currentAttemp = 1;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        public override void Start()
        {
            base.Start();

            // Subscribe to the OnDeath event
            p.OnDeath += OnDeath;
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
            p.mesh.ToggleMesh(false);

            StartCoroutine(Respawn());
        }

        /// <summary>
        /// Makes the player flash on/off and spawn respawn rings
        /// </summary>
        public IEnumerator Respawn()
        {
            // Wait 1 second
            yield return new WaitForSeconds(1f);

            currentAttemp++;
            attemptText.text = "Attemp " + currentAttemp;

            // Invoke respawn event
            p.InvokeRespawnEvent();

            // Make the player flash on/off and spawn respawn rings every time the player is turned on
            // Do this 3 times total over the course of 0.6 seconds
            SpawnRespawnRing();
            ToggleMesh(true);

            for (int i = 0; i < 4; i++)
            {
                yield return new WaitForSeconds(0.05f);

                ToggleMesh(false);

                yield return new WaitForSeconds(0.05f);

                SpawnRespawnRing();
                ToggleMesh(true);
            }
        }

        /// <summary>
        /// Just a shortcut for <see cref="PlayerMesh.ToggleMesh(bool)"/>
        /// </summary>
        private void ToggleMesh(bool enable)
        {
            p.mesh.ToggleMesh(enable);
        }

        /// <summary>
        /// Spawns a respawn ring with the right color
        /// </summary>
        private void SpawnRespawnRing()
        {
            // Create the ring
            GameObject obj = Instantiate(respawnRing, transform.position, Quaternion.identity, transform);
            obj.transform.localPosition = Vector3.zero;

            // Get the player color
            Color playerColor1 = playerColorer.colors[0];

            // Change the line renderers color
            LineRenderer lr = obj.GetComponent<LineRenderer>();
            lr.startColor = playerColor1;
            lr.endColor = playerColor1;
        }
    }
}
