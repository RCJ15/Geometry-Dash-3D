using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GD3D.Player;
using GD3D.Audio;
using TMPro;

namespace GD3D.UI
{
    /// <summary>
    /// The progress bar at the top of the screen. <para/>
    /// Is also used to keep track of where the player is.
    /// </summary>
    public class ProgressBar : MonoBehaviour
    {
        //-- Instance
        public static ProgressBar Instance;

        [Header("Objects")]
        [SerializeField] private Slider progressBar;
        [SerializeField] private Image progressBarTexture;
        [SerializeField] private TMP_Text percentText;
        [SerializeField] private TMP_Text percentTextCentered;

        //-- Instance references
        private PlayerMain _player;
        private PlayerMovement _playerMovement;

        //-- Distance
        private float endDistance;

        public static float Percent;
        public static string PercentString;

        //-- Other stuff
        private SaveFile _saveFile;

        private void Awake()
        {
            // Set the instance
            Instance = this;
        }

        private void Start()
        {
            // Temporary end distance
            endDistance = MusicPlayer.Instance.EndDistance;
            print("Reminder to fix this end distance thing");

            // Get instances
            _player = PlayerMain.Instance;
            _playerMovement = _player.Movement;

            // Color the progress bar texture according to player color 1
            progressBarTexture.color = _player.PlayerColor1;

            // Set percent to 0 since it's static
            Percent = 0;
            PercentString = "0%";

            // Get save file
            _saveFile = SaveData.SaveFile;

            UpdateToggles();
        }

        private void Update()
        {
            // Only update if the player is alive
            if (_player.IsDead)
            {
                return;
            }

            UpdatePercent();
        }

        /// <summary>
        /// Updates the percent text and slider
        /// </summary>
        public void UpdatePercent()
        {
            // Calculate the percent of level the player has traversed
            Percent = Helpers.Map(_playerMovement.StartTravelAmount, endDistance, 0, 1, _playerMovement.TravelAmount);
            Percent = Mathf.Clamp01(Percent);

            PercentString = ToPercent(Percent);

            // Set slider and text
            progressBar.normalizedValue = Percent;

            // Only update if the objects are active
            if (percentText.gameObject.activeSelf)
            {
                percentText.text = PercentString;
            }

            if (percentTextCentered.gameObject.activeSelf)
            {
                percentTextCentered.text = PercentString;
            }
        }

        /// <summary>
        /// Updates the progress bar to be enabled/disabled based on save file data.
        /// </summary>
        public void UpdateToggles()
        {
            // Cache booleans
            bool haveProgressBar = _saveFile.ProgressBarEnabled;
            bool showPercent = _saveFile.ShowPercentEnabled;

            // Toggle the progress bar based on if we should have the progress bar or not
            progressBar.gameObject.SetActive(haveProgressBar);

            // Set percent text to be active if both the progress bar and percent is enabled
            percentText.gameObject.SetActive(showPercent && haveProgressBar);

            // Set centered percent text to be active if the progress bar is disabled and percent is enabled
            percentTextCentered.gameObject.SetActive(showPercent && !haveProgressBar);
        }

        /// <summary>
        /// Converts the given <paramref name="percentAsFloat"/> and makes it look like percent text. <para/>
        /// <code>Example: giving "0.14" will return "14%" as a string.</code>
        /// </summary>
        /// <returns>The <paramref name="percentAsFloat"/> but as a fancy percent string.</returns>
        public static string ToPercent(float percentAsFloat)
        {
            // We use floor here since otherwise the player could get 100% on a level without completing it
            // Floor will make sure the player must have a value above or equal to 1.0 in order to get a written value of 100%
            // Example: a value of 0.999999 gets rounded to a string of 99%
            percentAsFloat = Mathf.FloorToInt(percentAsFloat * 100);

            return $"{percentAsFloat}%";
        }
    }
}
