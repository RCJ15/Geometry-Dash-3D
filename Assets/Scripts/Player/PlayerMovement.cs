using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// Handles the players constant movement and detects when the player is on ground or not.
    /// </summary>
    public class PlayerMovement : PlayerScript
    {
        //-- Speed constants (Blocks per second)
        // Numbers from: https://gdforum.freeforums.net/thread/55538/easy-speed-maths-numbers-speeds?page=1
        public const float slowSpeed = 8.36820083682f; // Actual multiplier: 0.80648535564x
        public const float normalSpeed = 10.3761348898f; // Actual multiplier: 1x
        public const float doubleSpeed = 12.9032258065f; // Actual multiplier: 1.2435483871x
        public const float tripleSpeed = 15.5945419103f; // Actual multiplier: 1.5029239766x
        public const float quadrupleSpeed = 19.1846522782f; // Actual multiplier: 1.8489208633x

        [Header("Stats")]
        [SerializeField] private GameSpeed currentSpeed;
        public static float speed;

        public float terminalVelocity = 28.4f;

        private float targetX;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        public override void Start()
        {
            base.Start();

            ChangeSpeed(currentSpeed);

            // Set target X start point
            targetX = transform.position.x;

            // Subscribe to the on respawn event
            p.OnRespawn += OnRespawn;
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        public override void Update()
        {
            base.Update();

            // Move the player
            targetX += Time.deltaTime * speed;
        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            // Go towards target X
            transform.position = new Vector3(targetX, transform.position.y, transform.position.z);

            // Clamp Y velocity between terminal velocity
            YVelocity = Mathf.Clamp(YVelocity, -terminalVelocity, terminalVelocity);

            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -Input.GetAxis("Horizontal") * 10);
        }

        public void ChangeSpeed(GameSpeed newSpeed)
        {
            currentSpeed = newSpeed;

            // Set moveSpeed based on the current speed
            switch (newSpeed)
            {
                case GameSpeed.slowSpeed:
                    speed = slowSpeed;
                    break;

                case GameSpeed.normalSpeed:
                    speed = normalSpeed;
                    break;

                case GameSpeed.doubleSpeed:
                    speed = doubleSpeed;
                    break;

                case GameSpeed.tripleSpeed:
                    speed = tripleSpeed;
                    break;

                case GameSpeed.quadrupleSpeed:
                    speed = quadrupleSpeed;
                    break;

                default:
                    speed = 0;
                    break;
            }
        }

        /// <summary>
        /// Is called when the player respawns
        /// </summary>
        private void OnRespawn()
        {
            // Reset the target X
            targetX = p.startPos.x;

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