using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GD3D.CustomInput;
using GD3D.Player;

namespace GD3D.LevelEditor
{
    /// <summary>
    /// 
    /// </summary>
    public class LevelEditorCamera : MonoBehaviour
    {
        //-- Singleton Instance
        public static LevelEditorCamera Instance;

        //-- Static Changed Booleans
        /// <summary>
        /// Will be true if either the Cameras position or rotation was changed this frame.
        /// </summary>
        public static bool Changed => ChangedPos || ChangedRot;
        /// <summary>
        /// Will be true if the Cameras position was changed this frame.
        /// </summary>
        public static bool ChangedPos { get; private set; }
        /// <summary>
        /// Will be true if the Cameras rotation was changed this frame.
        /// </summary>
        public static bool ChangedRot { get; private set; }

        public static Action OnChanged;
        public static Action OnChangedPos;
        public static Action OnChangedRot;

        public Camera Cam { get; private set; }

        [SerializeField] private float moveSpeed = 100;
        [SerializeField] private float scrollSpeed = 10000;
        [SerializeField] private float rotateSpeed = 1000;

        private Vector3 _oldPos;
        private Quaternion _oldRot;

        private EventSystem _eventSystem;
        private bool _pointerOnGameObject;

        private void Awake()
        {
            Instance = this;

            Cam = GetComponent<Camera>();

            _eventSystem = EventSystem.current;

            ChangedPos = false;
            ChangedRot = false;
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            #region Static Changed Booleans
            if (transform.position != _oldPos)
            {
                ChangedPos = true;

                OnChanged?.Invoke();
                OnChangedPos?.Invoke();

                _oldPos = transform.position;
            }
            else if (ChangedPos)
            {
                ChangedPos = false;
            }

            if (transform.rotation != _oldRot)
            {
                ChangedRot = true;

                OnChanged?.Invoke();
                OnChangedRot?.Invoke();

                _oldRot = transform.rotation;
            }
            else if (ChangedRot)
            {
                ChangedRot = false;
            }
            #endregion

            if (Input.GetMouseButtonUp(0))
            {
                _pointerOnGameObject = false;
            }

            if (_eventSystem.IsPointerOverGameObject())
            {
                _pointerOnGameObject = true;
            }

            float mouseXDelta = Input.GetAxis("Mouse X");
            float mouseYDelta = Input.GetAxis("Mouse Y");
            float mouseScroll = Input.GetAxis("Mouse ScrollWheel");

            if (!_pointerOnGameObject && ((Input.GetMouseButton(0) && !LevelEditorBuildOptions.Swipe) || Input.GetMouseButton(2)))
            {
                transform.Translate(new Vector2(-mouseXDelta, -mouseYDelta) * moveSpeed * Time.deltaTime, Space.Self);
            }

            if (!_pointerOnGameObject && mouseScroll != 0)
            {
                transform.Translate(new Vector3(0, 0, mouseScroll * scrollSpeed * Time.deltaTime), Space.Self);
            }

            if (Input.GetMouseButton(1))
            {
                transform.Rotate(new Vector3(-mouseYDelta, mouseXDelta, 0) * rotateSpeed * Time.deltaTime, Space.World);
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
            }
        }
    }
}
