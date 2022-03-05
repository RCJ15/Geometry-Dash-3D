using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GD3D
{
    /// <summary>
    /// Component that allows a object to be attached to the given path.
    /// </summary>
    public class AttachToPath : MonoBehaviour
    {
        [SerializeField] private PathCreator pathCreator;
        private VertexPath path => pathCreator.path;

        [Header("Position Settings")]
        [SerializeField] private float distance;
        [SerializeField] private float zOffset;

        [Header("Rotation")]
        [SerializeField] private float yRotationOffset;

        private Transform _transform;
        private Transform Transform
        {
            get
            {
                if (_transform == null)
                {
                    _transform = transform;
                }

                return _transform;
            }
        }

#if UNITY_EDITOR
        [SerializeField, HideInInspector]
        private bool justCreated = true;
        [SerializeField, HideInInspector]
        private bool subcribed = false;

        [SerializeField, HideInInspector]
        private float oldDistance = 0;

        [SerializeField, HideInInspector]
        private float oldZOffset = 0;
        [SerializeField, HideInInspector]
        private Vector3 oldPos = Vector3.zero;
#endif

        public void UpdatePosition()
        {
            // -- Set position --
            // Get the position at distance
            Vector3 targetPos = path.GetPointAtDistance(distance, EndOfPathInstruction.Stop);
            Vector3 direction = path.GetNormalAtDistance(distance, EndOfPathInstruction.Stop);

            // Apply offset
            targetPos += direction * zOffset;

            // Neutrilize Y
            targetPos.y = Transform.position.y;

            Transform.position = targetPos;

            // -- Set rotation --
            // Get rotation at distance
            Vector3 targetRot = path.GetRotationAtDistance(distance, EndOfPathInstruction.Stop).eulerAngles;

            // Neutrilize X and Z
            targetRot.x = transform.rotation.eulerAngles.x;
            targetRot.z = transform.rotation.eulerAngles.z;

            // Apply offset
            targetRot.y += yRotationOffset;

            Transform.rotation = Quaternion.Euler(targetRot);
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(AttachToPath))]
        public class AttachToPathEditor : Editor
        {
            private AttachToPath attachToPath;

            public override void OnInspectorGUI()
            {
                EditorGUI.BeginChangeCheck();

                base.OnInspectorGUI();

                if (EditorGUI.EndChangeCheck() || attachToPath.Transform.position != attachToPath.oldPos)
                {
                    if (attachToPath.zOffset == attachToPath.oldZOffset && attachToPath.distance == attachToPath.oldDistance)
                    {
                        Update();
                    }
                    else
                    {
                        attachToPath.oldZOffset = attachToPath.zOffset;
                        attachToPath.oldDistance = attachToPath.distance;

                        attachToPath.UpdatePosition();
                    }

                    attachToPath.oldPos = attachToPath.Transform.position;
                }
            }

            private void OnEnable()
            {
                attachToPath = (AttachToPath)target;

                if (attachToPath.justCreated)
                {
                    attachToPath.pathCreator = FindObjectOfType<PathCreator>();

                    if (attachToPath.pathCreator == null)
                    {
                        Debug.Log("There is no Path Creator in the scene!");
                        return;
                    }
                    
                    attachToPath.oldPos = attachToPath.Transform.position;
                    attachToPath.justCreated = false;

                    attachToPath.oldZOffset = attachToPath.zOffset;
                    attachToPath.oldDistance = attachToPath.distance;

                    Update();
                }

                if (attachToPath.pathCreator != null && !attachToPath.subcribed)
                {
                    attachToPath.subcribed = true;
                    attachToPath.pathCreator.pathUpdated += attachToPath.UpdatePosition;
                }
            }

            private void Update()
            {
                attachToPath.distance = attachToPath.path.GetClosestDistanceAlongPath(attachToPath.Transform.position);
                attachToPath.UpdatePosition();
            }

            private void OnDestroy()
            {
                if (attachToPath.pathCreator != null && attachToPath.subcribed)
                {
                    attachToPath.subcribed = false;
                    attachToPath.pathCreator.pathUpdated -= attachToPath.UpdatePosition;
                }
            }
        }
#endif
    }
}