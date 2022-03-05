using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

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

        [SerializeField] private PathCreator pathCreator;
        private VertexPath path => pathCreator.path;

        [Header("Stats")]
        [SerializeField] private GameSpeed currentSpeed = GameSpeed.normalSpeed;
        public static float Speed;

        private float _travelAmount;

        [Header("Z Movement")]
        [SerializeField] private float zSpeed = 1;

        public bool _canMoveOnZ;
        private float _zOffset;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        public override void Start()
        {
            base.Start();

            // Set the transform
            _transform = transform;

            ChangeSpeed(currentSpeed);

            // Set the start travel amount
            _travelAmount = path.GetClosestDistanceAlongPath(transform.position);

            // Subscribe to the on respawn event
            player.OnRespawn += OnRespawn;
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

            ZAxisMovement();

            // Move the player
            _travelAmount += Time.fixedDeltaTime * Speed;

            // Calculate target position
            Vector3 targetPos = path.GetPointAtDistance(_travelAmount, EndOfPathInstruction.Stop);
            Vector3 direction = path.GetNormalAtDistance(_travelAmount, EndOfPathInstruction.Stop);

            targetPos += direction * _zOffset;

            // Ignore Y
            targetPos.y = _transform.position.y;

            _transform.position = targetPos;

            // Rotate the correct way
            Vector3 newRot = path.GetRotationAtDistance(_travelAmount, EndOfPathInstruction.Stop).eulerAngles;

            newRot.x = transform.rotation.eulerAngles.x;
            newRot.z = transform.rotation.eulerAngles.z;

            transform.rotation = Quaternion.Euler(newRot);
        }

        private void ZAxisMovement()
        {
            if (!_canMoveOnZ)
                return;

            // Z Speed
            float zInput = Input.GetAxisRaw("Horizontal");

            // Change offset
            _zOffset += (-zInput / 10) * zSpeed;
            _zOffset = Mathf.Clamp(_zOffset, -4.5f, 4.5f);
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
            _travelAmount = player.startPos.x;

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