using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D
{
    /// <summary>
    /// Controls the ground in the main menu.
    /// </summary>
    public class MainMenuGround : MonoBehaviour
    {
        [SerializeField] private Space space = Space.x;
        [SerializeField] private float teleportAmount = 120;
        [SerializeField] private float moveSpeed = Player.PlayerMovement.NORMAL_SPEED;

        private void FixedUpdate()
        {
            float moveAmount = Time.fixedDeltaTime * moveSpeed;

            ChangeAxis(moveAmount, space);

            // Check if the axis is above the teleport amount
            if (Mathf.Abs(GetAxis(transform.position, space)) >= Mathf.Abs(teleportAmount))
            {
                // Change by negative teleport amount
                ChangeAxis(-teleportAmount, space);
            }
        }

        /// <summary>
        /// Changes the correct axis according to <paramref name="space"/> by <paramref name="amount"/>.
        /// </summary>
        private void ChangeAxis(float amount, Space space)
        {
            switch (space)
            {
                case Space.x:
                    transform.position += new Vector3(amount, 0, 0);
                    break;

                case Space.y:
                    transform.position += new Vector3(0, amount, 0);
                    break;

                case Space.z:
                    transform.position += new Vector3(0, 0, amount);
                    break;
            }
        }

        /// <summary>
        /// Returns a float on <paramref name="pos"/> which is the axis according to <paramref name="space"/>.
        /// </summary>
        private float GetAxis(Vector3 pos, Space space)
        {
            switch (space)
            {
                case Space.x:
                    return pos.x;

                case Space.y:
                    return pos.y;

                case Space.z:
                    return pos.z;

                default:
                    return 0;
            }
        }

        private enum Space
        {
            x, y, z,
        }
    }
}
