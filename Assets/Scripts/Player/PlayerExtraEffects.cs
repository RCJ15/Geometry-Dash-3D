using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Objects;

namespace GD3D.Player
{
    /// <summary>
    /// Handles any extra effects that will occur during the game, like the portal switching lines for example
    /// </summary>
    public class PlayerExtraEffects : PlayerScript
    {
        [SerializeField] private ParticleSystem regularGravityLines;
        [SerializeField] private Color reverseGravityColor = Color.white;
        private ParticleSystem _upsideDownGravityLines;

        private void Awake()
        {
            // Create a copy of the upsideDownGravityLines and invert it to create the other lines
            Transform tran = regularGravityLines.transform;

            Vector3 rot = tran.eulerAngles;

            // Invert Z
            rot.z += 180;

            GameObject newObj = Instantiate(regularGravityLines.gameObject, tran.position, Quaternion.Euler(rot), tran.parent);
            newObj.name = "Upside Down Gravity Lines";

            newObj.transform.localPosition = regularGravityLines.transform.localPosition;

            _upsideDownGravityLines = newObj.GetComponent<ParticleSystem>();

            // Get trails
            ParticleSystem.TrailModule trails = _upsideDownGravityLines.trails;

            // Set colors and make sure they have the same alpha as the original
            Color colorMin = reverseGravityColor;
            colorMin.a = trails.colorOverLifetime.colorMin.a;

            Color colorMax = reverseGravityColor;
            colorMax.a = trails.colorOverLifetime.colorMax.a;

            // Set new colors
            trails.colorOverLifetime = new ParticleSystem.MinMaxGradient(colorMin, colorMax);
        }

        public override void Start()
        {
            base.Start();

            // Subscribe to events
            player.OnEnterPortal += OnEnterPortal;
        }

        private void OnEnterPortal(Portal portal)
        {
            // Check if the portal is a gravity portal
            if (portal.GetType() == typeof(GravityPortal))
            {
                GravityPortal gravityPortal = (GravityPortal)portal;

                // Play reverse gravity lines
                if (gravityPortal.UpsideDown)
                {
                    _upsideDownGravityLines.Play();
                }
                // Play regular gravity lines
                else
                {
                    regularGravityLines.Play();
                }
            }
        }
    }
}
