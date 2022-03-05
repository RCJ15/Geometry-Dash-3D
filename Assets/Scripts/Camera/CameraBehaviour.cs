using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.Camera
{
    // Must have this here cuz otherwise it'll think I'm referencing the namespace instead
    using Camera = UnityEngine.Camera;

    /// <summary>
    /// Controls how the camera should behave and move about during the game
    /// </summary>
    public class CameraBehaviour : MonoBehaviour
    {
        //-- Instance
        public static CameraBehaviour Instance;

        [Header("Camera Settings")]
        [SerializeField] private Vector3 generalOffset = new Vector3(6f, 3.5f, 0f);

        [Space]
        public float XOffset;
        public float ZOffset = -10;
        public Vector3 Rotation = new Vector3(15, 0, 0);

        [Space]
        public Transform Target;
        private Vector3 _position;
        private Vector3 _targetPosition;

        [Header("Y Limits")]
        [SerializeField] private float limitYMin;
        [SerializeField] private float limitYMax;

        [HideInInspector]
        public float YLockPos;

        [Header("Interpolation Values")]
        [SerializeField] private float yMaxDelta = .2f;
        [SerializeField] private float yLerpDelta = .3f;
        [SerializeField] private float slerpRotationValue;

        //-- References
        private Camera _cam;

        //-- Shake
        private float _strength;
        private float _frequency;
        private float _frequencyTimer;

        private float _length;
        private float _lengthTimer;

        private Vector3 _shakeOffset;

        //-- Start values
        private Vector3 startPos;
        private Quaternion startRot;

        private Transform _transform;

        private void Awake()
        {
            // Set instance
            Instance = this;

            _transform = transform;
        }

        private void Start()
        {
            // Get references
            _cam = MathE.Camera;

            startPos = _transform.position;
            startRot = _transform.rotation;

            _position = _transform.position;
        }

        private void Update()
        {
            UpdateShake();
        }

        private void UpdateShake()
        {
            // Check if the timer is 0 or less
            if (_lengthTimer <= 0)
            {
                _shakeOffset = Vector3.zero;
                return;
            }

            // Decrease timer
            _lengthTimer -= Time.deltaTime;

            // Check if the frequency timer is 0 or less
            if (_frequencyTimer <= 0)
            {
                // Shake the camera
                float shakePower = MathE.Map(0, _length, 0, 1, _lengthTimer);
                _shakeOffset = Random.insideUnitSphere * _strength * shakePower;

                // Reset timer
                _frequencyTimer = 1f / _frequency;
            }
            // If not, decrease the timer
            else
            {
                _frequencyTimer -= Time.deltaTime;
            }
        }

        private void LateUpdate()
        {
            // Calculate the new position
            if (Target != null)
            {
                _targetPosition.x = Target.position.x;
                _targetPosition.z = Target.position.z;
            }

            _position.x = _targetPosition.x;
            _position.z = _targetPosition.z;

            // Set position
            _transform.position = _position + generalOffset + new Vector3(XOffset, 0, ZOffset) + _shakeOffset;
        }

        private void FixedUpdate()
        {
            if (Target == null)
            {
                return;
            }

            // If the borders are active, lock the Y position to the YLockPos (which is set to be the center between the borders)
            // Otherwise just set it to the target position
            float targetYPos = BorderManager.BordersActive ? YLockPos : Target.position.y;

            // Check if the targetYPos is out of the Y limits (or the borders are active)
            if ((targetYPos > _position.y + limitYMax || targetYPos < _position.y + limitYMin) || BorderManager.BordersActive)
            {
                // If so, move the targetY towards the targetYPos
                float maxDelta = yMaxDelta;

                _targetPosition.y = Mathf.MoveTowards(_targetPosition.y, targetYPos, maxDelta);
            }

            // Lerp towards targetY and targetZ
            _position.y = Mathf.Lerp(_position.y, _targetPosition.y, yLerpDelta);

            // Rotate
            _transform.rotation = Quaternion.Slerp(_transform.rotation, Quaternion.Euler(Rotation), slerpRotationValue * Time.deltaTime);
        }

        /// <summary>
        /// Shakes the camera
        /// </summary>
        /// <param name="strength">The strength of the shake</param>
        /// <param name="frequency">How often the camera will shake. This is in shakes per second</param>
        /// <param name="length">How long the shake will last</param>
        public void Shake(float strength, float frequency, float length)
        {
            _strength = strength;
            _frequency = frequency;
            _frequencyTimer = 0;

            _length = length;
            _lengthTimer = length;
        }

#if UNITY_EDITOR
        // Draw gizmos only in the editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            Vector3 position = transform.position;

            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
            Gizmos.matrix = matrix;

            position.y += limitYMin;
            Gizmos.DrawLine(position - new Vector3(10, 0, 0), position + new Vector3(10, 0, 0));

            position.y = transform.position.y + limitYMax;
            Gizmos.DrawLine(position - new Vector3(10, 0, 0), position + new Vector3(10, 0, 0));
        }
#endif
    }
}
