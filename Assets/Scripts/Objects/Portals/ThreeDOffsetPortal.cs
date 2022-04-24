using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Player;
using GD3D.Easing;
using PathCreation;

namespace GD3D.Objects
{
    /// <summary>
    /// It's the 3D Offset Portal, but I can't spell with numbers so it is ThreeDOffsetPortal :( <para/>
    /// Anyways, changes the players 3D offset when entered
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class ThreeDOffsetPortal : Portal
    {
        [Header("3D Mode Offset Settings")]
        [SerializeField] private int vertexAmount = 15;
        [SerializeField] private float newOffset = 0;

        [Space]
        [SerializeField] private EaseSettings easeSettings = EaseSettings.defaultValue;

        [Header("Line")]
        [SerializeField] private float lineDistOffset = -0.5f;
        [SerializeField] private MeshRenderer endOfLineArrow;
        [SerializeField] private float endOfLineArrowOffset;

        [Space]
        [SerializeField] private Gradient lineGradient;
        [SerializeField] private float lineGradientCycleSpeed;
        [SerializeField] private float lineGradientEndColOffset = 0.3f;

        private float _currentLineGradientPos;

        private float _totalDistance;

        private LineRenderer _line;

        private AttachToPath _attachToPath;
        private PlayerMovement _playerMovement;

        private VertexPath _path;

        public override void Start()
        {
            base.Start();

            // Get components
            _line = GetComponent<LineRenderer>();
            _attachToPath = GetComponent<AttachToPath>();
            _playerMovement = _player.Movement;

            _path = _attachToPath.PathCreator.path;

            // Calculate total distance
            _totalDistance = _attachToPath.Distance;

            float addAmount = (float)easeSettings.Time / (float)vertexAmount;

            for (int i = 0; i < vertexAmount; i++)
            {
                _totalDistance += _playerMovement.GetSpeedAtDistance(_totalDistance) * addAmount;
            }

            UpdateLines();
            UpdateColors();
        }

        public override void OnEnterPortal()
        {
            // Create a easing that will be used by the player movement script
            EaseObject obj = easeSettings.CreateEase();

            _playerMovement.Ease3DOffset(newOffset, obj);
        }

        private void FixedUpdate()
        {
            // Reset gradient pos to 0 if it's above 1
            if (_currentLineGradientPos > 1)
            {
                _currentLineGradientPos = 0;
            }

            // Add to the gradient pos
            _currentLineGradientPos += lineGradientCycleSpeed * Time.fixedDeltaTime;

            UpdateColors();
        }

        private void UpdateLines()
        {
            // Add points to the line
            List<Vector3> newPositions = new List<Vector3>();
            
            for (int i = 0; i <= vertexAmount; i++)
            {
                // Calculate t
                float t = (float)i / (float)vertexAmount;

                // Calculate the value using the ease data
                float val = easeSettings.EaseData.Evaluate(t);

                // Calculate distance and offset
                float dist = Mathf.Lerp(_attachToPath.Distance, _totalDistance, t) + lineDistOffset;

                float offset = Mathf.Lerp(_attachToPath.ZOffset, newOffset, val);

                // Calculate pos and direction
                Vector3 pos = _path.GetPointAtDistance(dist, EndOfPathInstruction.Stop);
                Vector3 direction = _path.GetNormalAtDistance(dist, EndOfPathInstruction.Stop);

                // Add the offset
                pos += direction * offset;

                // Reset Y Position
                pos.y = transform.position.y;

                // Add to list
                newPositions.Add(pos);

                // Check if this is the last position
                if (i == vertexAmount)
                {
                    // Get transform
                    Transform endOfLineArrow = this.endOfLineArrow.transform;

                    // Set arrow position
                    endOfLineArrow.position = pos;

                    // Set arrow direction
                    endOfLineArrow.right = direction;

                    endOfLineArrow.position += endOfLineArrowOffset * endOfLineArrow.forward;
                }
            }

            // Set line renderers positions
            _line.positionCount = vertexAmount;
            _line.SetPositions(newPositions.ToArray());
        }

        private void UpdateColors()
        {
            // Set start color
            Color newStartColor = lineGradient.Evaluate(_currentLineGradientPos);

            // Make sure to have same as old alpha
            newStartColor.a = _line.startColor.a;

            _line.startColor = newStartColor;

            // Set end color
            float endColTime = _currentLineGradientPos + lineGradientEndColOffset;

            if (endColTime > 1)
            {
                endColTime -= 1;
            }

            Color newEndColor = lineGradient.Evaluate(endColTime);

            // Make sure to have same as old alpha
            newEndColor.a = _line.endColor.a;

            _line.endColor = newEndColor;

            // Set arrow material color to the end of line color
            MaterialColorer.UpdateMaterialColor(endOfLineArrow.material, newEndColor, true, true);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            try
            {
                if (_attachToPath == null)
                {
                    _attachToPath = GetComponent<AttachToPath>();
                }

                if (_path == null)
                {
                    _path = _attachToPath.PathCreator.path;
                }
            }
            catch (System.Exception)
            {
                return;
            }

            // Draw start
            Gizmos.color = Color.green;

            float dist = _attachToPath.Distance + lineDistOffset;

            Vector3 pos = _path.GetPointAtDistance(dist, EndOfPathInstruction.Stop);
            Vector3 direction = _path.GetNormalAtDistance(dist, EndOfPathInstruction.Stop);

            // Add the offset
            pos += direction * _attachToPath.ZOffset;

            // Reset Y Position
            pos.y = transform.position.y;

            Gizmos.DrawWireSphere(pos, 0.4f);

            // Draw end
            Gizmos.color = Color.red;

            dist = dist + easeSettings.Time * PlayerMovement.NORMAL_SPEED;

            pos = _path.GetPointAtDistance(dist, EndOfPathInstruction.Stop);
            direction = _path.GetNormalAtDistance(dist, EndOfPathInstruction.Stop);

            // Add the offset
            pos += direction * newOffset;

            // Reset Y Position
            pos.y = transform.position.y;

            Gizmos.DrawWireSphere(pos, 0.4f);

        }
#endif
    }
}
