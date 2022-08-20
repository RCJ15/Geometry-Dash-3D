using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.GDCamera;
using GD3D.Easing;
using GD3D.Level;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GD3D.Objects
{
    /// <summary>
    /// Moves the cameras offset and rotates the camera over time when triggered
    /// </summary>
    public class CameraTrigger : Trigger
    {
        [Header("Camera Settings")]
        [LevelSave] [SerializeField] private CameraTriggerMode mode;

        [LevelSave] [SerializeField] private Vector3 offset = new Vector3(6, 3.5f, -10);
        [LevelSave] [SerializeField] private Vector3 rotation = new Vector3(15, 0, 0);
        [Range(1, 179)]
        [LevelSave] [SerializeField] private float fov = 60;

        [Space]
        [LevelSave] [SerializeField] private EaseSettings easeSettings = EaseSettings.defaultValue;

        //-- References
        private CameraBehaviour _cam;

        protected override void Start()
        {
            base.Start();

            // Get camera instance
            _cam = CameraBehaviour.Instance;
        }

        protected override void OnTriggered()
        {
            // Create a easing that will be used by the camera behaviour later
            EaseObject obj = easeSettings.CreateEase();

            // Change different camera values based on the mode
            switch (mode)
            {
                case CameraTriggerMode.offset:
                    _cam.EaseOffset(offset, obj);
                    return;

                case CameraTriggerMode.rotation:
                    _cam.EaseRotation(rotation, obj);
                    return;

                case CameraTriggerMode.FOV:
                    _cam.EaseFov(fov, obj);
                    return;

                default:
                    // Or not since something went wrong
                    EasingManager.RemoveEaseObject(obj);
                    return;
            }
        }

        /// <summary>
        /// An enum that determines what thing the camera trigger is going to modify
        /// </summary>
        [System.Serializable]
        public enum CameraTriggerMode
        {
            offset = 0,
            rotation = 1,
            FOV = 2,
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(CameraTrigger))]
        public class CameraTriggerEditor : Editor
        {
            private CameraTrigger _cameraTrigger;

            public override void OnInspectorGUI()
            {
                EditorGUI.BeginChangeCheck();

                Serialize("isTouchTriggered");
                Serialize("mode");

                GUILayout.Space(5);

                // Serialize the correct variable based on the current mode
                switch (_cameraTrigger.mode)
                {
                    case CameraTriggerMode.offset:
                        Serialize("offset");
                        break;
                    case CameraTriggerMode.rotation:
                        Serialize("rotation");
                        break;
                    case CameraTriggerMode.FOV:
                        Serialize("fov");
                        break;
                }

                Serialize("easeSettings");

                // Apply modified properties
                if (EditorGUI.EndChangeCheck())
                {
                    _cameraTrigger.fov = Mathf.Clamp(_cameraTrigger.fov, 1, 179);

                    serializedObject.ApplyModifiedProperties();
                }
            }

            private void Serialize(string name)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(name));
            }

            private void OnEnable()
            {
                _cameraTrigger = (CameraTrigger)target;
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            DrawDurationLine(easeSettings.Time);
        }
#endif
    }
}
