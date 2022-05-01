using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using GD3D.CustomInput;
using GD3D.Easing;

namespace GD3D.Player
{
    /// <summary>
    /// Handles the players constant movement and detects when the player is on ground or not. <para/>
    /// Also takes care of all movement on the second axis when 3D mode is enabled
    /// </summary>
    public class PlayerMovement : PlayerScript
    {
        //-- Speed constants (Blocks per second)
        // Numbers from: https://gdforum.freeforums.net/thread/55538/easy-speed-maths-numbers-speeds?page=1
        public const float SLOW_SPEED = 8.36820083682f; // Actual multiplier: 0.80648535564x
        public const float NORMAL_SPEED = 10.3761348898f; // Actual multiplier: 1x
        public const float DOUBLE_SPEED = 12.9032258065f; // Actual multiplier: 1.2435483871x
        public const float TRIPLE_SPEED = 15.5945419103f; // Actual multiplier: 1.5029239766x
        public const float QUADRUPLE_SPEED = 19.1846522782f; // Actual multiplier: 1.8489208633x

        [SerializeField] private PathCreator pathCreator;
        private VertexPath path => pathCreator.path;

        [Header("Main Menu")]
        [SerializeField] private float mainMenuMaxTravelAmount;

        public System.Action OnMainMenuTeleport;

        [Header("Stats")]
        [SerializeField] private GameSpeed currentSpeed = GameSpeed.normalSpeed;
        private GameSpeed _startSpeed;

        public static GameSpeed CurrentSpeed;
        public static float Speed;

        private float _travelAmount;
        private float _startTravelAmount;

        public float TravelAmount => _travelAmount;
        public float StartTravelAmount => _startTravelAmount;

        [Header("3D Mode (Moving on second axis)")]
        [SerializeField] private float speed3D = 1;

        [Space]
        [Range(0, 1)] [SerializeField] private float damp3DMoving = 0;
        [Range(0, 1)] [SerializeField] private float damp3DTurning = 0;
        [Range(0, 1)] [SerializeField] private float damp3DStopping = 0;

        [HideInInspector] public float OffsetVelocity;

        private Key leftKey;
        private Key rightKey;

        private Transform _cam;

        private bool _in3DMode;
        public bool In3DMode
        {
            get => _in3DMode;
            set
            {
                // Trigger effects if we are entering 3D mode
                if (value && !_in3DMode && enter3DModeParticles != null)
                {
                    enter3DModeParticles.Play();
                }
                // Trigger effects if we are exiting 3D mode
                else if (!value && _in3DMode && exit3DModeParticles != null)
                {
                    exit3DModeParticles.Play();
                }

                _in3DMode = value;
            }
        }

        [Header("Effects")]
        [SerializeField] private ParticleSystem enter3DModeParticles;
        [SerializeField] private ParticleSystem exit3DModeParticles;

        private float _3DOffset;
        public float Current3DOffset => _3DOffset;

        //-- Ease ID
        private long? _current3DOffsetEaseId = null;

        public override void Awake()
        {
            base.Awake();

            ChangeSpeed(currentSpeed);
        }

        public override void Start()
        {
            base.Start();

            // Set the transform
            _transform = transform;

            // Set the starting values
            _travelAmount = path.GetClosestDistanceAlongPath(transform.position);
            _startTravelAmount = _travelAmount;

            _startSpeed = currentSpeed;

            _cam = Helpers.Camera.transform;

            // Subscribe to events
            player.OnRespawn += (a, b) => Cancel3DOffsetEase();
            player.OnRespawn += OnRespawn;
            EasingManager.Instance.OnEaseObjectRemove += OnEaseObjectRemove;

            leftKey = PlayerInput.GetKey("3D Move Left");
            rightKey = PlayerInput.GetKey("3D Move Right");

            // Check if we are in the main menu
            if (player.InMainMenu)
            {
                // Subscribe to own teleport event
                OnMainMenuTeleport += () =>
                {
                    // Randomize movement speed
                    int speed = Random.Range(-1, 4);

                    ChangeSpeed((GameSpeed)speed);
                };

                // We will also add an extra event to when we die that'll teleport to us to the start
                player.OnDeath += () =>
                {
                    _travelAmount = _startTravelAmount;
                    PlayerTrailManager.HaveTrail = false;
                    UpdatePosition();

                    OnMainMenuTeleport?.Invoke();
                };

                // Invoke teleport event 1 frame later
                // We do it 1 frame later so that other subscribers that subscribe at start will get properly called
                Helpers.TimerEndOfFrame(this, () =>
                {
                    OnMainMenuTeleport?.Invoke();
                });
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            // Do nothing if the player is dead
            if (player.IsDead)
            {
                return;
            }

            Extra3DModeMovement();

            // Move the player
            _travelAmount += Time.fixedDeltaTime * Speed;

            // If we are in the main menu, then we will teleport back to the start travel amount after travelling over a certain amount
            if (player.InMainMenu && _travelAmount >= mainMenuMaxTravelAmount)
            {
                _travelAmount = _startTravelAmount;

                // Disable the trail and teleport so that the trail doesn't do a big stretchy stretch across the entire map
                PlayerTrailManager.HaveTrail = false;

                // Invoke main menu teleport event
                OnMainMenuTeleport?.Invoke();
            }

            UpdatePosition();

            // Rotate the correct way
            Vector3 newRot = path.GetRotationAtDistance(_travelAmount, EndOfPathInstruction.Stop).eulerAngles;

            newRot.x = transform.rotation.eulerAngles.x;
            newRot.z = transform.rotation.eulerAngles.z;

            transform.rotation = Quaternion.Euler(newRot);
        }

        /// <summary>
        /// Sets the transform position to match the correct <see cref="_travelAmount"/> and <see cref="_3DOffset"/>.
        /// </summary>
        private void UpdatePosition()
        {
            // Calculate target position
            Vector3 targetPos = path.GetPointAtDistance(_travelAmount, EndOfPathInstruction.Stop);
            Vector3 direction = path.GetNormalAtDistance(_travelAmount, EndOfPathInstruction.Stop);

            targetPos += direction * _3DOffset;

            // Ignore Y
            targetPos.y = _transform.position.y;

            _transform.position = targetPos;
        }

        /// <summary>
        /// Called in <see cref="FixedUpdate"/> every frame. This is just here for cleaner code (hopefully).
        /// </summary>
        private void Extra3DModeMovement()
        {
            // Guard clause
            if (!_in3DMode || _current3DOffsetEaseId.HasValue)
            {
                if (OffsetVelocity != 0)
                {
                    OffsetVelocity = 0;
                }

                return;
            }

            // Input
            float input = Key.GetAxis(leftKey, rightKey);

            // Transform the input to be relative to the camera
            //Vector3 test1 = _transform.TransformDirection(new Vector3(0, 0, input));
            //Vector3 test2 = _cam.TransformDirection(new Vector3(0, 0, input));

            //print(test1 + " | " + test2);

            // By default, damping is moving
            float damping = damp3DMoving;

            // Turning
            if (Mathf.Sign(input) != Mathf.Sign(OffsetVelocity))
            {
                damping = damp3DTurning;
            }
            // Stopping
            else if (input == 0)
            {
                damping = damp3DStopping;
            }

            // Apply velocity
            OffsetVelocity += input * (speed3D / 10);
            OffsetVelocity *= Mathf.Pow(damping, Time.fixedDeltaTime * 10);

            // Change offset
            _3DOffset += OffsetVelocity;
            _3DOffset = Mathf.Clamp(_3DOffset, -4.5f, 4.5f);
        }

        /// <summary>
        /// Returns what the players speed is going to be at the given <paramref name="distance"/>. <para/>
        /// Note: This currently just returns the players speed as we have no speed portals in the game.
        /// </summary>
        public float GetSpeedAtDistance(float distance)
        {
            switch (currentSpeed)
            {
                case GameSpeed.slowSpeed:
                    return SLOW_SPEED;

                case GameSpeed.normalSpeed:
                    return NORMAL_SPEED;

                case GameSpeed.doubleSpeed:
                    return DOUBLE_SPEED;

                case GameSpeed.tripleSpeed:
                    return TRIPLE_SPEED;

                case GameSpeed.quadrupleSpeed:
                    return QUADRUPLE_SPEED;
            }

            // Return the current speed by default
            return Speed;
        }

        /// <summary>
        /// Changes the players speed to match the <paramref name="newSpeed"/>.
        /// </summary>
        public void ChangeSpeed(GameSpeed newSpeed)
        {
            currentSpeed = newSpeed;
            CurrentSpeed = newSpeed;

            // Set moveSpeed based on the current speed
            switch (newSpeed)
            {
                case GameSpeed.slowSpeed:
                    Speed = SLOW_SPEED;
                    break;

                case GameSpeed.normalSpeed:
                    Speed = NORMAL_SPEED;
                    break;

                case GameSpeed.doubleSpeed:
                    Speed = DOUBLE_SPEED;
                    break;

                case GameSpeed.tripleSpeed:
                    Speed = TRIPLE_SPEED;
                    break;

                case GameSpeed.quadrupleSpeed:
                    Speed = QUADRUPLE_SPEED;
                    break;

                default:
                    Speed = 0;
                    break;
            }
        }

        #region Easing
        /// <summary>
        /// Called when a <see cref="EaseObject"/> is removed.
        /// </summary>
        private void OnEaseObjectRemove(long id)
        {
            // Set the ID to null if it was removed
            if (_current3DOffsetEaseId.HasValue && _current3DOffsetEaseId.Value == id)
            {
                _current3DOffsetEaseId = null;
            }
        }

        /// <summary>
        /// Cancels the current 3D offset easing if there is one currently active.
        /// </summary>
        public void Cancel3DOffsetEase()
        {
            // Remove the current ease using try remove
            bool removedEase = EasingManager.TryRemoveEaseObject(_current3DOffsetEaseId);

            // If it was removed, set the ease ID to null
            if (removedEase)
            {
                _current3DOffsetEaseId = null;
            }
        }

        /// <summary>
        /// Will set the given easing <paramref name="obj"/> to change the 3D offset to the given <paramref name="target"/>. <para/>
        /// Will also call <see cref="Cancel3DOffsetEase"/> to remove any active easing.
        /// </summary>
        public void Ease3DOffset(float target, EaseObject obj)
        {
            Cancel3DOffsetEase();

            // Cache the current offset
            float startValue = _3DOffset;

            // Set the easing on update method to change the field of view over time
            obj.OnUpdate = (obj) =>
            {
                _3DOffset = obj.GetValue(startValue, target);
            };

            // Set the ID
            _current3DOffsetEaseId = obj.ID;
        }
        #endregion

        /// <summary>
        /// Is called when the player respawns
        /// </summary>
        private void OnRespawn(bool inPracticeMode, Checkpoint checkpoint)
        {
            // Check if we are not in practice mode
            if (!inPracticeMode)
            {
                // Reset values
                _travelAmount = _startTravelAmount;
                _3DOffset = 0;
                OffsetVelocity = 0;
                ChangeSpeed(_startSpeed);

                _in3DMode = false;

                // Reset rigidbody
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            else
            {
                // Set values to the checkpoint values
                _travelAmount = checkpoint.PlayerDistance;
                _3DOffset = checkpoint.PlayerOffset;
                OffsetVelocity = checkpoint.PlayerOffsetVelocity;
                ChangeSpeed(checkpoint.PlayerSpeed);

                _in3DMode = checkpoint.PlayerIn3DMode;

                // Set rigidbody to the checkpoint values
                rb.velocity = checkpoint.PlayerVelocity;
                rb.angularVelocity = checkpoint.PlayerAngularVelocity;
            }
        }
    }

    public enum GameSpeed
    {
        none = -2,
        slowSpeed = -1,
        normalSpeed = 0,
        doubleSpeed = 1,
        tripleSpeed = 2,
        quadrupleSpeed = 3,
    }
}