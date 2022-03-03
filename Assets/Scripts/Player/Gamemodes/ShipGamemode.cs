using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.CustomInput;

namespace GD3D.Player
{
    /// <summary>
    /// Flies up
    /// </summary>
    [System.Serializable]
    public class ShipGamemode : GamemodeScript
    {
        [Header("Flying")]
        [SerializeField] private float flySpeed;

        private bool _holdingClickKey;

        [Header("Rotation")]
        [SerializeField] private Transform objToRotate;
        [SerializeField] private float rotateSlerpSpeed = 0.15f;
        [SerializeField] private float zRotationModifier = 4;
        private Vector3 _targetRot;

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void Update()
        {
            base.Update();

            // Go up/down based on if the click key is being held
            YVelocity += flySpeed * Time.deltaTime * (_holdingClickKey ? 1 : -1);

            AngularVelocity();
        }

        /// <summary>
        /// Handles all rotation angular velocity physics stuff. Is called in Update()
        /// </summary>
        private void AngularVelocity()
        {
            // Rotate towards the Y velocity while in the air
            if (!onGround)
            {
                _targetRot.z = Rigidbody.velocity.y * zRotationModifier;

                // Set X velocity
                _targetRot.x = XRot;
            }
            // Otherwise do not rotate at all
            else
            {
                _targetRot = Vector3.zero;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            // Set rotation
            Quaternion slerp = Quaternion.Slerp(objToRotate.rotation, Quaternion.Euler(_targetRot), rotateSlerpSpeed);

            objToRotate.rotation = slerp;
        }

        public override void OnClick(PressMode mode)
        {
            switch (mode)
            {
                case PressMode.down:
                    _holdingClickKey = true;
                    break;

                case PressMode.up:
                    _holdingClickKey = false;
                    break;
            }
        }
    }
}