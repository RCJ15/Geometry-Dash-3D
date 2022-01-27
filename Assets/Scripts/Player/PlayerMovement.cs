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
        // Multiplier 10.95813172043011
        // For this website https://gdforum.freeforums.net/thread/48749/p1kachu-presents-physics-geometry-dash

        //-- Speed constants (Blocks per second)
        public const float slowSpeed = 8.4f;
        public const float normalSpeed = 10.25f;
        public const float doubleSpeed = 12.8f;
        public const float tripleSpeed = 15.625f;
        public const float quadrupleSpeed = 18.73f;

        [Header("Stats")]
        [SerializeField] private GameSpeed currentSpeed;
        public static float speed;

        public bool onGround;
        public LayerMask groundLayer;

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
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        public override void Update()
        {
            base.Update();

            // Set onGround
            onGround = Physics.OverlapBox(transform.position, transform.localScale / 2 + (Vector3.one / 15), transform.rotation, groundLayer).Length >= 1;

            // Move the player
            targetX += Time.deltaTime * speed;
        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            // Lerp towards target X
            float newX = Mathf.Lerp(transform.position.x, targetX, 0.5f);

            transform.position = new Vector3(newX, transform.position.y, transform.position.z);

            // Clamp Y velocity between terminal velocity
            YVelocity = Mathf.Clamp(YVelocity, -terminalVelocity, terminalVelocity);
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