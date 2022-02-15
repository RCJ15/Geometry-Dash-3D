using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Player;

namespace GD3D
{
    public class FollowPlayer : MonoBehaviour
    {
        private Transform _player;

        [Header("Follow Axis")]
        [SerializeField] private bool _followX;
        [SerializeField] private bool _followY;
        [SerializeField] private bool _followZ;

        [Header("Offset")]
        [SerializeField] private Vector3 _offset;

        private Vector3 _startPos;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            // Set the start position
            _startPos = transform.position;

            // Get player instance
            _player = PlayerMain.Instance.transform;
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {
            // The new position this object will move to
            Vector3 newPos = _startPos + _player.transform.position + _offset;

            // Decide which axes to keep based on which axes bools are enabled
            newPos = new Vector3(
                _followX ? newPos.x : transform.position.x,
                _followY ? newPos.y : transform.position.y,
                _followZ ? newPos.y : transform.position.z
                );

            // Set the position
            transform.position = newPos;
        }
    }
}