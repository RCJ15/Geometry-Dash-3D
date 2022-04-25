using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using GD3D.Level;
using GD3D.Player;
using GD3D.CustomInput;

namespace GD3D.UI
{
    /// <summary>
    /// Pause menu of the game. Handles pausing the game. (of course) <para/>
    /// Also handles enabling auto retry, auto checkpoints, progress bar and show percent as well as music and SFX volume.
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        //-- Instance
        public static PauseMenu Instance;

        //-- Static variables
        public static bool IsPaused;
        public static bool CanPause = true;

        //-- Instance references
        private LevelData _levelData;
        private ProgressBar _progressBar;
        private PlayerMain _player;

        [Header("Toggle Object")]
        [SerializeField] private GameObject toggleObj;

        [Header("Pause Menu Assets")]
        [SerializeField] private TMP_Text levelText;

        [Space]
        [SerializeField] private Slider normalProgressBar;
        [SerializeField] private TMP_Text normalProgressText;

        [Space]
        [SerializeField] private Slider practiceProgressBar;
        [SerializeField] private TMP_Text practiceProgressText;

        [Space]
        [SerializeField] private AudioMixer audioMixer; 

        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        [Space]
        [SerializeField] private Toggle autoRetryToggle;
        [SerializeField] private Toggle autoCheckpointsToggle;
        [SerializeField] private Toggle progressBarToggle;
        [SerializeField] private Toggle showPercentToggle;

        [Space]
        [SerializeField] private Image practiceButtonImage;
        [SerializeField] private Sprite exitPracticeSprite;
        private Sprite enterPracticeSprite;

        [Header("Other Objects")]
        [SerializeField] private GameObject practiceUI;

        private UIClickable[] _UIClickables;

        //-- Events
        public Action OnPause;
        public Action OnResume;

        //-- Other stuff
        private Key _pauseKey;
        private SaveFile _saveFile;

        private void Awake()
        {
            // Set the instance
            Instance = this;
        }

        private void Start()
        {
            // Get instances
            _levelData = LevelData.Instance;
            _progressBar = ProgressBar.Instance;
            _player = PlayerMain.Instance;

            // Get pause key
            _pauseKey = PlayerInput.GetKey("Pause");

            // Set static variables
            IsPaused = false;
            CanPause = true;

            // Get all UI Clickables in the children of this object
            _UIClickables = GetComponentsInChildren<UIClickable>();

            // Get save file
            _saveFile = SaveData.SaveFile;

            // Disable practice UI
            practiceUI.SetActive(false);

            // Set the enterPracticeSprite to the starting sprite on the practice button
            enterPracticeSprite = practiceButtonImage.sprite;

            UpdateMenu();
        }

        private void Update()
        {
            bool pressedPause = _pauseKey.Pressed(PressMode.down);

            // Return if the pause button is not pressed or if the transition is transitioning
            if (!pressedPause || Transition.IsTransitioning)
            {
                return;
            }

            // Pause
            if (!IsPaused && CanPause)
            {
                Pause();
            }
            // Resume
            else
            {
                Resume();
            }
        }

        /// <summary>
        /// Call this to pause the game if it's not currently paused.
        /// </summary>
        public void Pause()
        {
            IsPaused = true;

            Time.timeScale = 0;

            // Invoke event
            OnPause?.Invoke();

            UpdateMenu();
        }

        /// <summary>
        /// Call this to resume the game if it's currently paused.
        /// </summary>
        public void Resume()
        {
            IsPaused = false;

            Time.timeScale = 1;

            // Invoke event
            OnResume?.Invoke();

            UpdateMenu();
        }

        /// <summary>
        /// Updates the menus sliders and text
        /// </summary>
        private void UpdateMenu()
        {
            // Set the toggle object to be active if the game is paused and be disabled if the game is unpaused
            toggleObj.SetActive(IsPaused);

            // Update the other assets to reflect their proper data
            levelText.text = _levelData.LevelName;

            // We use ProgressBar.ToPercent() here to conver the percent into a nice formated string
            // You can also check out what the method does internally if you're curious
            normalProgressBar.normalizedValue = _levelData.NormalPercent;
            normalProgressText.text = ProgressBar.ToPercent(_levelData.NormalPercent);

            practiceProgressBar.normalizedValue = _levelData.PracticePercent;
            practiceProgressText.text = ProgressBar.ToPercent(_levelData.PracticePercent);

            musicSlider.value = _saveFile.MusicVolume;
            sfxSlider.value = _saveFile.SFXVolume;

            // Stop scaling on all the UI Clickables
            foreach (UIClickable clickable in _UIClickables)
            {
                clickable.StopScaling();
            }

            // Set the toggles to reflect the save file data
            autoRetryToggle.isOn = _saveFile.AutoRetryEnabled;
            autoCheckpointsToggle.isOn = _saveFile.AutoCheckpointsEnabled;
            progressBarToggle.isOn = _saveFile.ProgressBarEnabled;
            showPercentToggle.isOn = _saveFile.ShowPercentEnabled;
        }

        /// <summary>
        /// Sets the music volume to the given <paramref name="volume"/>.
        /// </summary>
        public void ChangeMusicVolume(float volume)
        {
            SetMixerVolume(volume, "Music Volume");

            _saveFile.MusicVolume = volume;
        }

        /// <summary>
        /// Sets the SFX volume to the given <paramref name="volume"/>.
        /// </summary>
        public void ChangeSFXVolume(float volume)
        {
            SetMixerVolume(volume, "SFX Volume");

            _saveFile.SFXVolume = volume;
        }

        /// <summary>
        /// Sets the audio parameter called <paramref name="name"/> to the given <paramref name="volume"/> converted to the logarithmic scale.
        /// </summary>
        private void SetMixerVolume(float volume, string name)
        {
            // Scale the volume to use the logarithmic scale because decibel is stupid
            float scaledVolume = Mathf.Log10(volume) * 20;

            if (volume <= 0.0f)
            {
                scaledVolume = -80.0f;
            }

            audioMixer.SetFloat(name, scaledVolume);
        }

        /// <summary>
        /// Toggles practice mode on/off.
        /// </summary>
        public void TogglePracticeMode()
        {
            PlayerPracticeMode.InPracticeMode = !PlayerPracticeMode.InPracticeMode;

            bool inPracticeMode = PlayerPracticeMode.InPracticeMode;

            // Toggle the practice UI
            practiceUI.SetActive(inPracticeMode);

            // Respawn the player if we are exiting practice mode
            if (!inPracticeMode)
            {
                _player.Spawn.Respawn();
            }

            practiceButtonImage.sprite = inPracticeMode ? exitPracticeSprite : enterPracticeSprite;
        }

        /// <summary>
        /// Transitions to the main menu.
        /// </summary>
        public void QuitToMenu()
        {
            Transition.TransitionToMainMenu();
        }

        #region Toggles
        /// <summary>
        /// Toggles Auto Retry on/off based on <paramref name="enable"/>.
        /// </summary>
        public void ToggleAutoRetry(bool enable)
        {
            _saveFile.AutoRetryEnabled = enable;
        }

        /// <summary>
        /// Toggles Auto Checkpoints on/off based on <paramref name="enable"/>.
        /// </summary>
        public void ToggleAutoCheckpoints(bool enable)
        {
            _saveFile.AutoCheckpointsEnabled = enable;
        }

        /// <summary>
        /// Toggles Progress Bar on/off based on <paramref name="enable"/>.
        /// </summary>
        public void ToggleProgressBar(bool enable)
        {
            _saveFile.ProgressBarEnabled = enable;

            // Also update the progress bar
            _progressBar.UpdateToggles();
            _progressBar.UpdatePercent();
        }

        /// <summary>
        /// Toggles Show Percent on/off based on <paramref name="enable"/>.
        /// </summary>
        public void ToggleShowPercent(bool enable)
        {
            _saveFile.ShowPercentEnabled = enable;

            // Also update the progress bar
            _progressBar.UpdateToggles();
            _progressBar.UpdatePercent();
        }
        #endregion
    }
}
