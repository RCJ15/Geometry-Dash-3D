using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using GD3D.Audio;

namespace GD3D.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class LevelSelectOption : MonoBehaviour
    {
        //-- Settings
        public LevelSelect.LevelData LevelData;
        private string _levelName;

        [Header("Objects")]
        [SerializeField] private TMP_Text levelNameText;
        [SerializeField] private TMP_Text starAmountText;
        public Image DifficultyFace;

        [Space]
        [SerializeField] private Slider normalProgressBar;
        [SerializeField] private Slider practiceProgressBar;

        [Space]
        [SerializeField] private TMP_Text normalProgressText;
        [SerializeField] private TMP_Text practiceProgressText;

        private void Start()
        {
            _levelName = LevelData.LevelName;

            levelNameText.text = _levelName;
            starAmountText.text = LevelData.StartAwarded.ToString();
            DifficultyFace.sprite = LevelData.DifficultyFace;

            // Update the progress bars with data from the JSON save file
            SaveFile.LevelSaveData levelSave = null;
            levelSave = SaveData.SaveFile.LevelData.FirstOrDefault((levelData) => levelData.name == _levelName);

            if (levelSave == null)
            {
                normalProgressBar.normalizedValue = 0;
                practiceProgressBar.normalizedValue = 0;

                normalProgressText.text = "0%";
                practiceProgressText.text = "0%";
            }
            else
            {
                normalProgressBar.normalizedValue = levelSave.normalPercent;
                practiceProgressBar.normalizedValue = levelSave.practicePercent;

                normalProgressText.text = ProgressBar.ToPercent(levelSave.normalPercent);
                practiceProgressText.text = ProgressBar.ToPercent(levelSave.practicePercent);
            }
        }

        private void Update()
        {
        
        }

        /// <summary>
        /// Plays the level.
        /// </summary>
        public void PlayLevel()
        {
            if (Transition.IsTransitioning)
            {
                return;
            }

            Transition.TransitionToScene(LevelData.LevelBuildIndex);

            // Stop the music
            MainMenuMusic.StopInstance();

            // Play sound effect
            SoundManager.PlaySound("Play Level", 1);
        }
    }
}
