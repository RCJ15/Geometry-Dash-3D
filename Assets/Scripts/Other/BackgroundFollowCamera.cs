using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D
{
    /// <summary>
    /// Makes the object follow the camera in a stepped motion to allow for looping a object of a certain size.
    /// </summary>
    public class BackgroundFollowCamera : MonoBehaviour
    {
        [Header("Main Settings")]
        [SerializeField] private Vector3 _roundValue = new Vector3(16, 16, 16);

        [SerializeField] private bool _followX = true;
        [SerializeField] private bool _followY = true;
        [SerializeField] private bool _followZ = true;

        [SerializeField] private bool _useLocalScale = true;

        [Header("General Offset")]
        [SerializeField] private Vector3 _offset;

        [Header("Random Offset")]
        [SerializeField] private bool _haveRandomXOffset = false;
        private float _randomXOffset;

        [SerializeField] private bool _haveRandomYOffset = false;
        private float _randomYOffset;

        [SerializeField] private bool _haveRandomZOffset = false;
        private float _randomZOffset;

        [Header("Generate Background")]
        [SerializeField] private GameObject _backgroundObjectToCopy;
        [SerializeField] private Vector3Int _amount = new Vector3Int(1, 1, 1);

        private Transform _cam;
        private Vector3 _startPos;
        private Vector3 _pieceOffset;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            // Set the start position
            _startPos = transform.position;

            // Get the camera
            _cam = Camera.main.transform;

            // Generate a random X offset if it should have a random X offset
            if (_haveRandomXOffset)
            {
                _randomXOffset = Random.Range(-_roundValue.x, _roundValue.x);
            }

            // Generate a random Y offset if it should have a random Y offset
            if (_haveRandomYOffset)
            {
                _randomYOffset = Random.Range(-_roundValue.y, _roundValue.y);
            }

            // Generate a random Z offset if it should have a random Z offset
            if (_haveRandomZOffset)
            {
                _randomZOffset = Random.Range(-_roundValue.z, _roundValue.z);
            }

            // If there is no object to copy or the amount of objects to copy is below or equal to 0, then return
            if (_backgroundObjectToCopy == null || (_amount.x <= 0 && _amount.y <= 0 && _amount.z <= 0))
            {
                return;
            }

            int amountSpawned = 1;

            // Set the pieceOffset
            _pieceOffset = _backgroundObjectToCopy.transform.localPosition;

            // Loop for the amount of objects in the X
            for (int x = 1; x < _amount.x + 1; x++)
            {
                // Loop for the amount of objects in the Y
                for (int y = 0; y < _amount.y; y++)
                {
                    //Loop for the amount of objects in the Z
                    for (int z = 0; z < _amount.z; z++)
                    {
                        // Set the X pos
                        float xPos = _amount.x <= 1 ? // Check if the amount of objects in the X are less or equal to 1
                                                      // If true:
                            _backgroundObjectToCopy.transform.localPosition.x // Defaults to the local X position
                            : // Else
                            (x * _roundValue.x) - ((_amount.x * _roundValue.x) / 2); // Calculation for getting the middle pos for X

                        // Set the Y pos
                        float yPos = _amount.y <= 1 ? // Check if the amount of objects in the Y are less or equal to 1
                                                      // If true:
                            _backgroundObjectToCopy.transform.localPosition.y // Defaults to the local Y position
                            : // Else:
                            (y * _roundValue.y) - ((_amount.y * _roundValue.y) / 2); // Calculation for getting the middle pos for Y

                        // Set the Z pos
                        float zPos = _amount.z <= 1 ? // Check if the amount of objects in the Z are less or equal to 1
                                                      // If true:
                            _backgroundObjectToCopy.transform.localPosition.z // Defaults to the local Z position
                            : // Else:
                            (z * _roundValue.z) - ((_amount.z * _roundValue.z) / 2); // Calculation for getting the middle pos for Z

                        GameObject newObj = Instantiate(_backgroundObjectToCopy, Vector3.zero, _backgroundObjectToCopy.transform.rotation, transform);
                        newObj.transform.SetParent(transform);
                        newObj.transform.localPosition = new Vector3(xPos, yPos, zPos) + _pieceOffset + _offset;
                        newObj.name = _backgroundObjectToCopy.name + " (" + amountSpawned + ")";

                        // Increment amount spawned by 1
                        amountSpawned++;
                    }
                }
            }

            // Destroy the orignal object cuz it's useless now
            Destroy(_backgroundObjectToCopy);
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {
            // Set the position to follow the camera (with steps)
            transform.position = new Vector3(
                // X pos
                _followX ?
                (Mathf.Round(_cam.position.x / (_useLocalScale ? transform.localScale.x : 1) / _roundValue.x) // Round it after decreasing the value
                * _roundValue.x * (_useLocalScale ? transform.localScale.x : 1)) + _randomXOffset // Make the value big again
                :
                _startPos.x, // Otherwise if it's not supposed to follow the X then set it to just be at it's start X

                // Y pos
                _followY ?
                (Mathf.Round(_cam.position.y / (_useLocalScale ? transform.localScale.y : 1) / _roundValue.y) // Round it after decreasing the value
                * _roundValue.y * (_useLocalScale ? transform.localScale.y : 1)) + _randomYOffset // Make the value big again
                :
                _startPos.y, // Otherwise if it's not supposed to follow the Y then set it to just be at it's start Y

                // Z pos
                _followZ ?
                (Mathf.Round(_cam.position.z / (_useLocalScale ? transform.localScale.z : 1) / _roundValue.z) // Round it after decreasing the value
                * _roundValue.z * (_useLocalScale ? transform.localScale.z : 1)) + _randomZOffset // Make the value big again
                :
                _startPos.z) // Otherwise if it's not supposed to follow the Z then set it to just be at it's start Z

                // Add the offset
                + _offset;
        }
    }
}