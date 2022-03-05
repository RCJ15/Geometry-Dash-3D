using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Camera;

namespace GD3D
{
    /// <summary>
    /// Handles moving and spawning borders
    /// </summary>
    public class BorderManager : MonoBehaviour
    {
        //-- Important Stuff
        public const int MAX_HEIGHT = 100;
        public static bool BordersActive;
        public static BorderManager Instance;

        [Header("Borders")]
        [SerializeField] private Transform baseFloor;
        [SerializeField] private Collider baseFloorHitbox;

        private float _floorStartY;

        private Transform _roof;
        private Transform _floor;

        private Collider _roofHitbox;
        private Collider _floorHitbox;

        [Header("Animation")]
        [SerializeField] private AnimationCurve borderAppearCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        [SerializeField] private AnimationCurve borderDisappearCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

        [SerializeField] private float borderAppearTime;
        [SerializeField] private float borderDisappearTime;

        [SerializeField] private float borderMoveDistance;
        [SerializeField] private float borderSize;

        private Coroutine _roofMoveCoroutine;
        private Coroutine _floorMoveCoroutine;

        private CameraBehaviour _cam;

        private void Awake()
        {
            // Set instance
            Instance = this;

            _floorStartY = baseFloor.position.y;

            // Create roof duplicating the floor
            _roof = CreateRoofPart(baseFloor.gameObject);
            _floor = baseFloor;

            _roofHitbox = CreateRoofPart(baseFloorHitbox.gameObject).GetComponent<Collider>();
            _floorHitbox = baseFloorHitbox;
            
            _roofHitbox.transform.SetParent(_roof);
            _floorHitbox.transform.SetParent(_floor);

            // Set names for the newly created roof
            _roof.gameObject.name = "Border Roof";
            _roofHitbox.gameObject.name = "Border Roof (Hitbox)";

            // Disable roof by default
            _roof.gameObject.SetActive(false);
            _roofHitbox.enabled = false;

            _roof.position = new Vector3(0, borderMoveDistance, 0);

            BordersActive = false;
        }

        private Transform CreateRoofPart(GameObject template)
        {
            // Copy template
            GameObject newObj = Instantiate(template, template.transform.position, template.transform.rotation, null);
            Transform transform = newObj.transform;

            // Invert the Y scale (cuz it's the roof and we are using the floor as template)
            transform.localScale = new Vector3(1, -1, 1);

            return transform;
        }

        private void Start()
        {
            // Get the camera
            _cam = CameraBehaviour.Instance;
        }

        /// <summary>
        /// Makes borders appear from both the given <paramref name="min"/> Y position and the given <paramref name="max"/> Y position
        /// </summary>
        public static void ApplyBorders(float min, float max)
        {
            // Set the cam YLockPos
            Instance._cam.YLockPos = (min + max) / 2;

            // Make roof appear
            Instance.RoofAppear(max);
            Instance._roofHitbox.enabled = true;

            // Only make floor appear if the min is not underneath the ground level
            Instance.MoveFloorToYPos(min);

            BordersActive = true;
        }

        public static void RemoveBorders()
        {
            // Don't remove borders if they are already gone
            if (!BordersActive)
            {
                return;
            }

            // Make both roof disappear
            Instance.RoofDisappear();

            // Make floor move to start position
            Instance.MoveFloorToYPos(Instance._floorStartY);

            Instance._roofHitbox.enabled = false;

            BordersActive = false;
        }

        /// <summary>
        /// Makes the roof transition to the given <paramref name="yPosition"/> and enables the roof
        /// </summary>
        public void RoofAppear(float yPosition)
        {
            // Enable
            _roof.gameObject.SetActive(true);

            // Move
            MoveRoof(yPosition + (borderSize / 2), borderAppearTime, borderAppearCurve);
        }

        /// <summary>
        /// Makes the roof transition away from it's current position (and eventually disable itself)
        /// </summary>
        public void RoofDisappear()
        {
            // Disable but stored in a action
            Action onFinish = () => _roof.gameObject.SetActive(false);

            // Move
            MoveRoof(_roof.position.y + borderMoveDistance, borderDisappearTime, borderDisappearCurve, onFinish);
        }

        /// <summary>
        /// Makes the floor transition to the given <paramref name="yPosition"/>
        /// </summary>
        public void MoveFloorToYPos(float yPosition)
        {
            // Move
            MoveFloor(yPosition - (borderSize / 2), borderAppearTime, borderAppearCurve);
        }

        /// <summary>
        /// Moves the roof from it's current Y position to the given <paramref name="yPosition"/>
        /// </summary>
        public void MoveRoof(float yPosition, float time, AnimationCurve curve, Action onFinish = null)
        {
            // Stop old move coroutine
            if(_roofMoveCoroutine != null)
            {
                StopCoroutine(_roofMoveCoroutine);
            }

            // Start new move coroutine
            _roofMoveCoroutine = StartCoroutine(TransitionBorder(_roof, yPosition, time, curve, onFinish));
        }

        /// <summary>
        /// Moves the floor from it's current Y position to the given <paramref name="yPosition"/>
        /// </summary>
        public void MoveFloor(float yPosition, float time, AnimationCurve curve, Action onFinish = null)
        {
            // Stop old move coroutine
            if (_floorMoveCoroutine != null)
            {
                StopCoroutine(_floorMoveCoroutine);
            }

            // Start new move coroutine
            _floorMoveCoroutine = StartCoroutine(TransitionBorder(_floor, yPosition, time, curve, onFinish));
        }

        /// <summary>
        /// Coroutine for transitioning the given <paramref name="border"/> to the given <paramref name="yPosition"/> over the given <paramref name="time"/>
        /// </summary>
        /// <param name="border">What to move</param>
        /// <param name="yPosition">The end position</param>
        /// <param name="time">How long the move will take (in seconds)</param>
        /// <param name="onFinish">What happens when the object is done moving</param>
        private IEnumerator TransitionBorder(Transform border, float yPosition, float time, AnimationCurve curve, Action onFinish = null)
        {
            float currentTimer = time;
            float originalYPos = border.transform.position.y;

            // Pseudo Update() method
            while (currentTimer > 0)
            {
                float t = currentTimer / time;

                // Calculate the new Y position using lerp and the transition animation curve
                Vector3 newPos = border.transform.position;
                newPos.y = Mathf.Lerp(yPosition, originalYPos, curve.Evaluate(t));
                border.transform.position = newPos;

                // Wait for the next frame
                currentTimer -= Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            // Set the border to move to the final Y position
            Vector3 finalPos = border.transform.position;
            finalPos.y = yPosition;
            border.transform.position = finalPos;

            // Invoke event
            onFinish?.Invoke();
        }
    }
}
