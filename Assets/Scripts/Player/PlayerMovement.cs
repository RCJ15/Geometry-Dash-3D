using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.Player
{
    /// <summary>
    /// Handles the players constant movement and detects when the player is on ground or not.
    /// </summary>
    public class PlayerMovement : PlayerScript
    {
        //-- Speed constants (Blocks per second)
        // Numbers from: https://gdforum.freeforums.net/thread/55538/easy-speed-maths-numbers-speeds?page=1
        public const float SLOW_SPEED = 8.36820083682f; // Actual multiplier: 0.80648535564x
        public const float NORMAL_SPEED = 10.3761348898f; // Actual multiplier: 1x
        public const float DOUBLE_SPEED = 12.9032258065f; // Actual multiplier: 1.2435483871x
        public const float TRIPLE_SPEED = 15.5945419103f; // Actual multiplier: 1.5029239766x
        public const float QUADRUPLE_SPEED = 19.1846522782f; // Actual multiplier: 1.8489208633x

        [Header("Stats")]
        [SerializeField] private GameSpeed currentSpeed = GameSpeed.normalSpeed;
        public static float Speed;

        private float _targetX;

        [Header("Z Movement")]
        [SerializeField] private float timeToAccelerate = 0.2f;
        [SerializeField] private float timeToDecelerate = 0.3f;
        [SerializeField] private float maxZSpeed = 7;

        [SerializeField] private AnimationCurve speedCurve = AnimationCurve.Linear(0, 0, 1, 1);
        private float _currentZSpeedTime;

        public bool _canMoveOnZ;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        public override void Start()
        {
            base.Start();

            // Set the transform
            _transform = transform;

            ChangeSpeed(currentSpeed);

            // Set target X start point
            _targetX = _transform.position.x;

            // Subscribe to the on respawn event
            player.OnRespawn += OnRespawn;
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        public override void Update()
        {
            base.Update();
            
            // Move the player
            _targetX += Time.deltaTime * Speed;
        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            // Go towards target X
            _transform.position = new Vector3(_targetX, _transform.position.y, _transform.position.z);

            ZAxisMovement();
        }

        private void ZAxisMovement()
        {
            if (!_canMoveOnZ)
                return;

            // Z Speed
            float zInput = Input.GetAxisRaw("Horizontal");

            float targetSpeed = zInput * maxZSpeed;

            bool accelerate = zInput != 0;

            // Accelerate
            if (accelerate)
            {
                _currentZSpeedTime = Mathf.MoveTowards(_currentZSpeedTime, zInput, Time.deltaTime / timeToAccelerate);
            }
            // Decelerate
            else
            {
                _currentZSpeedTime = Mathf.MoveTowards(_currentZSpeedTime, 0, Time.deltaTime / timeToDecelerate);
            }

            // Set speed
            Vector3 newVelocity = rb.velocity;

            newVelocity.z = speedCurve.Evaluate(Mathf.Abs(_currentZSpeedTime)) * Mathf.Sign(_currentZSpeedTime * -1) * maxZSpeed;

            rb.velocity = newVelocity;
        }

        public void ChangeSpeed(GameSpeed newSpeed)
        {
            currentSpeed = newSpeed;

            // Set moveSpeed based on the current speed
            switch (newSpeed)
            {
                case GameSpeed.slowSpeed:
                    Speed = SLOW_SPEED;
                    break;

                case GameSpeed.normalSpeed:
                    Speed = NORMAL_SPEED;
                    break;

                case GameSpeed.doubleSpeed:
                    Speed = DOUBLE_SPEED;
                    break;

                case GameSpeed.tripleSpeed:
                    Speed = TRIPLE_SPEED;
                    break;

                case GameSpeed.quadrupleSpeed:
                    Speed = QUADRUPLE_SPEED;
                    break;

                default:
                    Speed = 0;
                    break;
            }
        }

        /// <summary>
        /// Is called when the player respawns
        /// </summary>
        private void OnRespawn()
        {
            // Reset the target X
            _targetX = player.startPos.x;

            // Reset rigidbody components aswell
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public enum GameSpeed
    {
        none = -2,
        slowSpeed = -1,
        normalSpeed = 0,
        doubleSpeed = 1,
        tripleSpeed = 2,
        quadrupleSpeed = 4,
    }
}