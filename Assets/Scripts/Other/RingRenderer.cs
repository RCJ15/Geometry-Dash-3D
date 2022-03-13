using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GD3D
{
    /// <summary>
    /// Uses a line renderer to draw a ring with adjustable resolution and size. <para/>
    /// Can even be used to draw wire polygons like triangles and squares.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class RingRenderer : MonoBehaviour
    {
        [Header("Ring Stats")]
        [SerializeField] private int points = 100;
        [SerializeField] private float ringSize = 1;

        [Header("Camera")]
        [SerializeField] private bool lookAtCamera = true;
        private Transform _cam;

        [Header("Color")]
        [SerializeField] private bool modifyColor = false;
        [SerializeField] private Color startColor = Color.white;
        [SerializeField] private Color endColor = Color.white;

        [Header("Update mode")]
        [SerializeField] private UpdateMode updateMode;

        private LineRenderer _lr;

        /// <summary>
        /// Awake is called when the script instance is being loaded
        /// </summary>
        void Awake()
        {
            // Get line renderer
            _lr = GetComponent<LineRenderer>();

            // Get the camera
            _cam = UnityEngine.Camera.main.transform;

            // Always update in awake regardless of the update mode
            DoUpdate();
        }

        private void OnEnable()
        {
            // Update lines if the update mode is not onAwake
            if (updateMode != UpdateMode.onAwake)
            {
                DoUpdate();
            }
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        private void Update()
        {
            // Update every frame if the update mode is set to every frame
            if (updateMode == UpdateMode.everyFrame)
            {
                DoUpdate();
            }
        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        private void FixedUpdate()
        {
            // Update every fixed frame if the update mode is set to every fixed frame
            if (updateMode == UpdateMode.everyFixedFrame)
            {
                DoUpdate();
            }
        }
        
        /// <summary>
        /// Shortcut so I don't have to type this out every single time I need to update something in this script
        /// </summary>
        private void DoUpdate()
        {
            UpdateLines();
            
            // Update colors
            if (modifyColor)
            {
                UpdateColor();
            }
        }

        /// <summary>
        /// Updates the line renderers lines to form the ring
        /// </summary>
        public void UpdateLines()
        {
            // Get the line renderer if it's (somehow) missing
            if (_lr == null)
            {
                _lr = GetComponent<LineRenderer>();
            }

            // Render no ring if there are no points or the size is 0
            if (points <= 0 || ringSize <= 0)
            {
                _lr.positionCount = 0;
                return;
            }

            // Look at the camera if we are supposed to do that
            if (lookAtCamera && _cam != null)
            {
                transform.LookAt(_cam, Vector3.up);
            }

            // Create list of Vector3
            List<Vector3> poses = new List<Vector3>();

            // Loop for the amount of points needed
            for (int i = 0; i < points; i++)
            {
                // Add the position to the list
                poses.Add(GetPos(i));
            }

            // Add the first pos
            poses.Add(GetPos(0));

            // Set the line renderers positions
            _lr.positionCount = poses.Count;

            _lr.SetPositions(poses.ToArray());
        }

        /// <summary>
        /// Updates the lines start and end color to match the <see cref="startColor"/> and the <see cref="endColor"/>
        /// </summary>
        public void UpdateColor()
        {
            _lr.startColor = startColor;
            _lr.endColor = endColor;
        }

        /// <summary>
        /// Returns the position of the ring at the given <paramref name="index"/>
        /// </summary>
        private Vector3 GetPos(int index)
        {
            // Calculate the angle
            float angle = Helpers.Map(0, points, 0, 360, index);
            angle = Helpers.LoopValue(angle, 0, 360);

            // Convert the angle into a normal
            Vector2 dir = Helpers.AngleToNormal(angle);

            // Calculate the new position
            Vector3 newPos = dir * ringSize;

            // Return the position
            return transform.TransformPoint(newPos);
        }

        /// <summary>
        /// Enum for what update mode a ring renderer can have
        /// </summary>
        [System.Serializable]
        public enum UpdateMode
        {
            onAwake = 0,
            everyFrame = 1,
            everyFixedFrame = 2,
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            // Get the line renderer if it's missing
            if (_lr == null)
            {
                _lr = GetComponent<LineRenderer>();
            }

            // Render no ring if there are no points or the size is 0
            if (points <= 0 || ringSize <= 0)
            {
                return;
            }

            // Simply draws the ring but with gizmos
            for (int i = 0; i < points; i++)
            {
                float t = (float)i / (float)points;

                // Get the color
                Gizmos.color = _lr.colorGradient.Evaluate(t);

                // Draw the lines
                Gizmos.DrawLine(GetPos(i), GetPos(i + 1));
            }
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RingRenderer))]
    public class RingRendererEditor : Editor
    {
        private RingRenderer ringRenderer;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editor Tools", EditorStyles.boldLabel);

            // Just renders the lines, but in the editor
            if (GUILayout.Button("Render in editor"))
            {
                Undo.RecordObject(ringRenderer, "Rendered ring in editor");
                ringRenderer.UpdateLines();
            }
        }

        private void OnEnable()
        {
            // Get the line renderer on the target object
            ringRenderer = (RingRenderer)target;
        }
    }
#endif
}