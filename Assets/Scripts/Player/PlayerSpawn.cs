using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GD3D.ObjectPooling;
using GD3D.Easing;
using GD3D.Level;
using GD3D.UI;

namespace GD3D.Player
{
    /// <summary>
    /// Controls the player spawning and showing the respawn menu/new best popup.
    /// </summary>
    public class PlayerSpawn : PlayerScript
    {
        [SerializeField] private int poolSize = 4;
        private ObjectPool<PoolObject> _pool;

        [SerializeField] private PoolObject respawnRing;

        [SerializeField] private float respawnTime;

        [SerializeField] private TMP_Text attemptText;
        private int _currentAttempt = 1;
        public int CurrentAttemp => _currentAttempt;

        [Header("Respawn Menu")]
        [SerializeField] private GameObject respawnMenu;
        [SerializeField] private EaseSettings respawnMenuEaseSettings;

        [Space]
        [SerializeField] private TMP_Text respawnMenuLevelName;
        [SerializeField] private TMP_Text respawnMenuAttemptText;
        [SerializeField] private Slider respawnMenuProgressBar;
        [SerializeField] private TMP_Text respawnMenuProgressPercent;
        [SerializeField] private TMP_Text respawnMenuJumpText;
        [SerializeField] private TMP_Text respawnMenuTimeText;

        [Space]
        [SerializeField] private Button respawnButton;
        [SerializeField] private Button quitButton;

        private UIClickable[] _respawnMenuUIClickables;

        private long? _respawnSizeEaseID = null;
        private Vector3 _respawnMenuStartSize;
        private Transform _respawnMenuTransform;

        [Header("New Best Popup")]
        [SerializeField] private GameObject newBestPopup;
        [SerializeField] private TMP_Text newBestPercentText;

        [Space]
        [SerializeField] private EaseSettings newBestShowEaseSettings;
        [SerializeField] private EaseSettings newBestHideEaseSettings;

        private long? _newBestSizeEaseID = null;
        private Vector3 _newBestStartSize;
        private Transform _newBestTransform;

        //-- Other
        private Coroutine _currentRespawnCoroutine;
        private SaveFile _saveFile;
        private PlayerPracticeMode _practiceMode;

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public override void Start()
        {
            base.Start();

            // Set the save file
            _saveFile = SaveData.SaveFile;

            // Setup respawn menu
            _respawnMenuTransform = respawnMenu.transform;
            _respawnMenuStartSize = _respawnMenuTransform.localScale;

            respawnMenuLevelName.text = LevelData.Instance.LevelName;

            respawnButton.onClick.AddListener(Respawn);
            quitButton.onClick.AddListener(QuitToMenu);

            respawnMenu.SetActive(false);

            _respawnMenuUIClickables = respawnMenu.GetComponentsInChildren<UIClickable>();

            // Setup new best popup
            _newBestTransform = newBestPopup.transform;
            _newBestStartSize = _newBestTransform.localScale;

            newBestPopup.SetActive(false);

            // Get the player practice mode script
            _practiceMode = player.PracticeMode;

            // Subscribe to the OnDeath event
            player.OnDeath += OnDeath;
            EasingManager.Instance.OnEaseObjectRemove += OnEaseObjectRemove;

            // Setup the respawnRing obj by creating a copy and setting the copy
            GameObject obj = Instantiate(respawnRing.gameObject, transform.position, Quaternion.identity, transform);
            obj.transform.position = _transform.position;

            // Change the line renderers color
            LineRenderer lr = obj.GetComponent<LineRenderer>();
            lr.startColor = PlayerColor1;
            lr.endColor = PlayerColor1;

            // Create pool
            _pool = new ObjectPool<PoolObject>(obj, poolSize,
                (poolObj) =>
                {
                    poolObj.transform.SetParent(_transform);
                    poolObj.transform.localPosition = Vector3.zero;
                }
            );

            // Destroy the newly created object because we have no use out of it anymore
            Destroy(obj);
        }

        private void OnEaseObjectRemove(long id)
        {
            // Set respawn size ease ID to null if the respawn size ease got removed
            if (_respawnSizeEaseID.HasValue && id == _respawnSizeEaseID.Value)
            {
                _respawnSizeEaseID = null;
            }

            // Do the same thing for the new best size easing
            if (_newBestSizeEaseID.HasValue && id == _newBestSizeEaseID.Value)
            {
                _newBestSizeEaseID = null;
            }
        }

        #region Respawn Menu
        /// <summary>
        /// Makes the respawn menu appear with a scale easing.
        /// </summary>
        private void ShowRespawnMenu()
        {
            // Disable the pause menu so you can't pause
            PauseMenu.CanPause = false;

            // Set objects on the respawn menu
            respawnMenuAttemptText.text = attemptText.text;

            respawnMenuJumpText.text = $"Jumps: {PlayerMain.TimesJumped}";

            // Use TimeSpan here to format the text to look nice :)
            TimeSpan time = TimeSpan.FromSeconds(PlayerMain.TimeSpentPlaying);
            respawnMenuTimeText.text = $"Time: {time.ToString("mm':'ss")}";

            respawnMenuProgressBar.normalizedValue = ProgressBar.Percent;
            respawnMenuProgressPercent.text = ProgressBar.PercentString;

            // Enable respawn menu
            respawnMenu.SetActive(true);

            // Try remove ease object
            EasingManager.TryRemoveEaseObject(_respawnSizeEaseID);

            // Set the respawn menu scale to 0
            _respawnMenuTransform.localScale = Vector3.zero;

            // Create new scale easing
            EaseObject ease = _respawnMenuTransform.EaseScale(_respawnMenuStartSize, 1);

            // Set ease settings
            ease.SetSettings(respawnMenuEaseSettings);

            // Set ease ID
            _respawnSizeEaseID = ease.ID;
        }

        /// <summary>
        /// Transitions to the main menu.
        /// </summary>
        public void QuitToMenu()
        {
            Transition.TransitionToLastActiveMenu();
        }
        #endregion

        #region New Best Popup
        /// <summary>
        /// Makes the new best popup appear with a scale easing. <para/>
        /// Is called in <see cref="LevelData"/>.
        /// </summary>
        public void ShowNewBest()
        {
            // Set the percent text on the new best popup
            newBestPercentText.text = ProgressBar.PercentString;

            // Enable new best popuo
            newBestPopup.SetActive(true);

            // Try remove ease object
            EasingManager.TryRemoveEaseObject(_newBestSizeEaseID);

            // Set the new best popup scale to 0
            _newBestTransform.localScale = Vector3.zero;

            // Create new scale easing
            EaseObject ease = _newBestTransform.EaseScale(_newBestStartSize, 1);

            // Set ease settings
            ease.SetSettings(newBestShowEaseSettings);

            // Set ease ID
            _newBestSizeEaseID = ease.ID;

            // Make the easing dissapear when it's complete
            ease.SetOnComplete((obj) =>
            {
                // Create new scale easing
                EaseObject ease = _newBestTransform.EaseScale(Vector3.zero, 1);

                // Set ease settings
                ease.SetSettings(newBestHideEaseSettings);

                // Set ease ID
                _newBestSizeEaseID = ease.ID;

            });
        }
        #endregion

        /// <summary>
        /// Is called when the player dies.
        /// </summary>
        private void OnDeath()
        {
            // Increase attempt count
            _currentAttempt++;
            SaveData.CurrentLevelData.totalAttempts++;

            // Disable the mesh
            player.Mesh.ToggleCurrentMesh(false);

            Coroutine coroutine;

            // Check if auto retry is enabled
            // But also auto retry if we are in practice mode since there is no respawn menu in practice mode
            if (PlayerPracticeMode.InPracticeMode || _saveFile.AutoRetryEnabled)
            {
                // Respawn after 1 second if auto retry is enabled or if we are in practice mode
                coroutine = Helpers.TimerSeconds(this, 1, Respawn);
            }
            else
            {
                // Bring up the respawn menu after 1 second if auto retry is disabled
                coroutine = Helpers.TimerSeconds(this, 1, ShowRespawnMenu);
            }

            StartRespawnCouroutine(coroutine);
        }

        private void StartRespawnCouroutine(Coroutine coroutine)
        {
            // Stop the currently active respawn coroutine
            if (_currentRespawnCoroutine != null)
            {
                StopCoroutine(_currentRespawnCoroutine);
            }

            // Set the new respawn coroutine to the given coroutine
            _currentRespawnCoroutine = coroutine;
        }

        /// <summary>
        /// Respawns the player.
        /// </summary>
        public void Respawn()
        {
            // Enable the pause menu so you can pause again
            PauseMenu.CanPause = true;

            // Disable the respawn menu and new best popups
            respawnMenu.SetActive(false);
            newBestPopup.SetActive(false);

            // Stop the scaling of all the UI Clickables on the respawn menu
            foreach (UIClickable clickable in _respawnMenuUIClickables)
            {
                clickable.StopScaling();
            }

            bool inPracticeMode = PlayerPracticeMode.InPracticeMode;
            Checkpoint checkpoint = null;

            if (inPracticeMode)
            {
                checkpoint = _practiceMode.LatestCheckpoint;
                checkpoint?.OnLoaded();
            }

            // Invoke respawn event
            player.InvokeRespawnEvent(inPracticeMode && checkpoint != null, checkpoint);

            // Set attempt text
            attemptText.text = $"Attempt  {_currentAttempt}";

            // Start the respawn coroutine
            Coroutine coroutine = StartCoroutine(RespawnCouroutine());
            StartRespawnCouroutine(coroutine);

            // Ignore input for this moment so the player won't instantly jump when respawning
            player.IgnoreInput();
        }
            
        /// <summary>
        /// Makes the player flash on/off and spawn respawn rings 3 times.
        /// </summary>
        private IEnumerator RespawnCouroutine()
        {
            // Make the player flash on/off and spawn respawn rings every time the player is turned on
            // Do this 3 times total over the course of 0.6 seconds
            SpawnRespawnRing();

            PlayerMesh mesh = player.Mesh;

            mesh.ToggleCurrentMesh(true);

            for (int i = 0; i < 3; i++)
            {
                yield return Helpers.GetWaitForSeconds(0.05f);

                mesh.ToggleCurrentMesh(false);

                yield return Helpers.GetWaitForSeconds(0.05f);

                SpawnRespawnRing();
                mesh.ToggleCurrentMesh(true);
            }
        }

        /// <summary>
        /// Spawns a respawn ring.
        /// </summary>
        private void SpawnRespawnRing()
        {
            // Spawn the ring
            PoolObject obj = _pool.SpawnFromPool(_transform.position);
            obj?.RemoveAfterTime(0.5f);
        }
    }
}
