using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.Objects
{
    /// <summary>
    /// Special script for making the jump orb particles rotate based on the cameras direction
    /// </summary>
    public class JumpOrbParticles : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 1;
        private Transform _cameraTransform;

        private void Start()
        {
            // Get camera transform
            _cameraTransform = Helpers.Camera.transform;
        }

        private void FixedUpdate()
        {
            // Rotate based on the cameras local space
            Vector3 newDir = _cameraTransform.InverseTransformDirection(rotationSpeed * Vector3.right);
            Debug.DrawRay(transform.position, newDir);

            transform.rotation = transform.rotation * Quaternion.Euler(newDir);
        }
    }
}
