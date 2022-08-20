using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.GDCamera;
using GD3D.Level;
using GD3D.Player;
using GD3D.Easing;

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

        [Header("Easing")]
        [SerializeField] private EaseSettings appearEaseSettings = EaseSettings.defaultValue;
        [SerializeField] private EaseSettings disappearEaseSettings = EaseSettings.defaultValue;

        [SerializeField] private float borderMoveDistance;
        [SerializeField] private float borderSize;

        private long? _roofEaseID = null;
        private long? _floorEaseID = null;

        private CameraBehaviour _cam;

        public float RoofYPos
        {
            get => _roof.position.y;
            set
            {
                Vector3 newPos = _roof.position;
                newPos.y = value;
                _roof.position = newPos;
            }
        }
        public float FloorYPos
        {
            get => _floor.position.y;
            set
            {
                Vector3 newPos = _floor.position;
                newPos.y = value;
                _floor.position = newPos;
            }
        }

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

            // By default the borders are off
            BordersActive = false;
        }

        private Transform CreateRoofPart(GameObject template)
        {
            // Copy template
            GameObject newObj = Instantiate(template, template.transform.position, template.transform.rotation, null);
            Transform transform = newObj.transform;

            // Invert the Y scale (cuz it's the roof and we are using the floor as a template)
            transform.localScale = new Vector3(1, -1, 1);

            return transform;
        }

        private void Start()
        {
            // Get the camera
            _cam = CameraBehaviour.Instance;

            // Add the roofs renderer to the LevelColors object so it's colored properly
            Renderer renderer = _roof.GetComponent<Renderer>();

            LevelColors.GetColorData(LevelColors.ColorType.ground)
                .RenderMaterialData.Add(
                new LevelColors.RendererMaterialData(renderer, 0, true)
            );

            // Subscribe to events
            PlayerMain.Instance.OnRespawn += OnRespawn;
            EasingManager.Instance.OnEaseObjectRemove += OnEaseObjectRemove;
        }

        private void OnRespawn(bool inPracticeMode, Checkpoint checkpoint)
        {
            // Stop the easings
            EasingManager.TryRemoveEaseObject(_floorEaseID);
            EasingManager.TryRemoveEaseObject(_roofEaseID);

            _floorEaseID = null;
            _roofEaseID = null;

            // Local variables for setting borders
            bool bordersActive = false;
            float roofYPos = borderMoveDistance;
            float floorYPos = _floorStartY;

            // Check if we are in practice mode
            if (inPracticeMode)
            {
                // Set local variables
                bordersActive = checkpoint.BordersActive;
                roofYPos = checkpoint.BorderRoofYPos;
                floorYPos = checkpoint.BorderFloorYPos;
            }

            // Instantly disable the roof
            _roof.gameObject.SetActive(bordersActive);

            _roofHitbox.enabled = bordersActive;

            // Teleport roof and floor to their correct positions
            RoofYPos = roofYPos;
            FloorYPos = floorYPos;

            BordersActive = bordersActive;
        }

        private void OnEaseObjectRemove(long id)
        {
            // Set the ease IDs to null if that ease just got removed
            if (_roofEaseID.HasValue && id == _roofEaseID)
            {
                _roofEaseID = null;
            }

            if (_floorEaseID.HasValue && id == _floorEaseID)
            {
                _floorEaseID = null;
            }
        }

        /// <summary>
        /// Makes borders appear from both the given <paramref name="min"/> Y position and the given <paramref name="max"/> Y position
        /// </summary>
        public static void ApplyBorders(float min, float max)
        {
            BorderManager I = Instance;

            // Set the cam YLockPos
            I._cam.YLockPos = (min + max) / 2;

            // Make roof appear
            I._roof.gameObject.SetActive(true);
            I.RoofYPos += I.borderMoveDistance;
            EaseObject roofMove = I.MoveRoof(max + (I.borderSize / 2));

            // Enable roof hitbox
            I._roofHitbox.enabled = true;

            // Make floor appear
            EaseObject floorMove = I.MoveFloor(min);

            // Set ease settings
            roofMove.SetSettings(I.appearEaseSettings);
            floorMove.SetSettings(I.appearEaseSettings);

            BordersActive = true;
        }

        public static void RemoveBorders()
        {
            // Don't remove borders if they are already gone
            if (!BordersActive)
            {
                return;
            }

            BorderManager I = Instance;

            // Make roof disappear
            EaseObject roofMove = I.MoveRoof(I.RoofYPos + I.borderMoveDistance);

            // Disable gameobject and hitbox when the easing is finished
            roofMove.SetOnComplete((obj) =>
            {
                I._roof.gameObject.SetActive(false);
                I._roofHitbox.enabled = false;
            });

            // Make floor move to start position
            EaseObject floorMove = I.MoveFloor(I._floorStartY);

            // Set ease settings
            roofMove.SetSettings(I.disappearEaseSettings);
            floorMove.SetSettings(I.disappearEaseSettings);

            BordersActive = false;
        }

        /// <summary>
        /// Eases the floor to the given <paramref name="yPosition"/>.
        /// </summary>
        /// <returns>The newly created <see cref="EaseObject"/>. <para/>
        /// Use it to set stuff like <see cref="EaseObject.EaseData"/> with <see cref="EaseObject.SetEaseData"/>.</returns>
        private EaseObject MoveFloor(float yPosition)
        {
            // Try remove the current easing
            EasingManager.TryRemoveEaseObject(_floorEaseID);

            // Create new position easing
            EaseObject ease = new EaseObject(0, 1);

            // Set update method
            float startPos = FloorYPos;

            ease.OnUpdate = (obj) =>
            {
                float newYPos = obj.GetValue(startPos, yPosition);

                FloorYPos = newYPos;
            };

            // Set ID
            _floorEaseID = ease.ID;

            return ease;
        }

        /// <summary>
        /// Eases the roof to the given <paramref name="yPosition"/>.
        /// </summary>
        /// <returns>The newly created <see cref="EaseObject"/>. <para/>
        /// Use it to set stuff like <see cref="EaseObject.EaseData"/> with <see cref="EaseObject.SetEaseData"/>.</returns>
        private EaseObject MoveRoof(float yPosition)
        {
            // Try remove the current easing
            EasingManager.TryRemoveEaseObject(_roofEaseID);

            // Create new easing
            EaseObject ease = new EaseObject(0, 1);

            // Set update method
            float startPos = RoofYPos;

            ease.OnUpdate = (obj) =>
            {
                float newYPos = obj.GetValue(startPos, yPosition);

                RoofYPos = newYPos;
            };

            // Set ID
            _roofEaseID = ease.ID;

            return ease;
        }
    }
}
