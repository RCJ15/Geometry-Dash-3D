using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D
{
    /// <summary>
    /// Sucks particles into a certain suck point. It's very sucky (sus)
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class SuckingParticles : MonoBehaviour
    {
        [Header("Sucky")]
        [SerializeField] private Transform suckPoint;
        [SerializeField] private float suckKillRadius = 0.5f;
        [SerializeField] private float suckSpeed;

        //-- Particles
        private ParticleSystem _particleSystem;
        private ParticleSystem.Particle[] _particles;

        private void Start()
        {
            // Set particles
            _particleSystem = GetComponent<ParticleSystem>();
            _particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
        }

        private void FixedUpdate()
        {
            // If particle system == null return
            if (_particleSystem == null)
            {
                return;
            }

            // The target pos is the suckPoint in local space
            Vector3 targetPos = transform.InverseTransformPoint(suckPoint.transform.position);

            // Get particles
            _particleSystem.GetParticles(_particles);

            // Modify the particles
            int particleLength = _particles.Length;
            for (int i = 0; i < particleLength; i++)
            {
                // Get the particle
                ParticleSystem.Particle p = _particles[i];
                float distance = Vector3.Distance(p.position, targetPos);

                // If the particle is too close to the center, then go to the next particle
                if (distance < suckKillRadius)
                {
                    p.velocity = Vector3.zero;

                    _particles[i] = p;
                    
                    continue;
                }

                // Move towards suck point
                p.velocity += (targetPos - p.position) * suckSpeed;

                // Set the particle back
                _particles[i] = p;
            }

            // Set particles to update them
            _particleSystem.SetParticles(_particles);
        }

#if UNITY_EDITOR
        // Draw kill radius in the editor
        private void OnDrawGizmosSelected()
        {
            if (suckPoint == null)
            {
                return;
            }

            Gizmos.color = Color.blue;

            Gizmos.DrawWireSphere(suckPoint.position, suckKillRadius);
        }
#endif
    }
}
