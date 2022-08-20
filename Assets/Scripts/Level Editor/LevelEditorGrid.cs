using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.LevelEditor
{
    /// <summary>
    /// The grid in the level editor.
    /// </summary>
    public class LevelEditorGrid : MonoBehaviour
    {
        public static LevelEditorGrid Instance;

        public static float GridDistance
        {
            get => Instance._plane.distance;
            set => Instance._plane.distance = value;
        }

        [SerializeField] private GameObject gridLine;
        [SerializeField] private Vector2Int amount = new Vector2Int(1000, 1000);

        private string _lineName;
        private int _lineAmount;

        private Camera _cam;

        private Plane _plane;
        private static readonly Vector3 _centerViewportPos = new Vector3(0.5f, 0.5f, 0);

        private void Awake()
        {
            Instance = this;

            _cam = Helpers.Camera;

            _plane = new Plane(Vector3.forward, 0);

            _lineName = gridLine.name;

            // Create grid lines by amount
            for (int x = -amount.x / 2; x < amount.x / 2; x++)
            {
                CreateLine(x, false);
            }

            for (int y = -amount.y / 2; y < amount.y / 2; y++)
            {
                CreateLine(y, true);
            }

            gridLine.SetActive(false);

            // Subscribe to update the 
            LevelEditorCamera.OnChanged += UpdateGridPos;
        }

        private void CreateLine(int offset, bool vertical)
        {
            Transform line;
            if (_lineAmount == 0)
            {
                line = gridLine.transform;
            }
            else
            {
                line = Instantiate(gridLine, transform).transform;
            }

            line.gameObject.name = $"{_lineName} ({_lineAmount})";

            _lineAmount++;

            if (vertical)
            {
                line.localPosition = new Vector3(0, offset + 0.5f);
                line.rotation = Quaternion.Euler(0, 0, 90);
            }
            else
            {
                line.localPosition = new Vector3(offset + 0.5f, 0);
                line.rotation = Quaternion.identity;
            }
        }

        private void UpdateGridPos()
        {
            Ray ray = _cam.ViewportPointToRay(_centerViewportPos);

            // Return if try get fails
            if (!TryGetGridPos(ray, out Vector3 result))
            {
                return;
            }

            transform.position = result;
        }

        private void OnDestroy()
        {
            // Unsubscribe so no null errors happen
            LevelEditorCamera.OnChanged -= UpdateGridPos;
        }

        #region Get Grid Pos Methods
        public Vector3 GetGridPos(bool roundResult = true)
        {
            return GetGridPos(_cam.ScreenPointToRay(Input.mousePosition), roundResult);
        }

        public bool TryGetGridPos(out Vector3 result, bool roundResult = true)
        {
            return TryGetGridPos(_cam.ScreenPointToRay(Input.mousePosition), out result, roundResult);
        }

        public Vector3 GetGridPos(Ray ray, bool roundResult = true)
        {
            if (TryGetGridPos(ray, out Vector3 result, roundResult))
            {
                return result;
            }

            return Vector3.zero;
        }

        public bool TryGetGridPos(Ray ray, out Vector3 result, bool roundResult = true)
        {
            if (_plane.Raycast(ray, out float distance))
            {
                result = ray.GetPoint(distance);

                if (roundResult)
                {
                    result.x = Mathf.Floor(result.x) + 0.5f;
                    result.y = Mathf.Floor(result.y) + 0.5f;
                    result.z = _plane.distance;
                }

                return true;
            }

            result = Vector3.zero;

            return false;
        }
        #endregion
    }
}
