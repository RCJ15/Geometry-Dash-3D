using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GD3D.CustomInput;
using GD3D.Player;

namespace GD3D.LevelEditor
{
    /// <summary>
    /// The UI for the 3 different build modes in the level editor. Those being: Build, Edit & Delete.
    /// </summary>
    public class LevelEditorBuildModes : MonoBehaviour
    {
        public static BuildMode Mode = BuildMode.build;

        [Header("Buttons")]
        [SerializeField] private Button buildButton;
        [SerializeField] private Button editButton;
        [SerializeField] private Button deleteButton;

        [Space]
        [SerializeField] private Sprite onSprite;
        private Image _buildImg, _editImg, _deleteImg;

        [Header("GameObjects")]
        [SerializeField] private GameObject buildArea;
        [SerializeField] private GameObject editArea;
        [SerializeField] private GameObject deleteArea;

        [Space]
        [SerializeField] private GameObject categoryTabs;
        [SerializeField] private GameObject arrowLeft;
        [SerializeField] private GameObject arrowRight;

        private Key _buildKey, _editKey, _deleteKey;

        private void Start()
        {
            // Reset static variable
            Mode = BuildMode.build;

            // Get keys
            _buildKey = PlayerInput.GetKey("Build Mode");
            _editKey = PlayerInput.GetKey("Edit Mode");
            _deleteKey = PlayerInput.GetKey("Delete Mode");

            // Get image for each button
            _buildImg = buildButton.GetComponent<Image>();
            _editImg = editButton.GetComponent<Image>();
            _deleteImg = deleteButton.GetComponent<Image>();

            // Subscribe to events
            buildButton.onClick.AddListener(SetBuildMode);
            editButton.onClick.AddListener(SetEditMode);
            deleteButton.onClick.AddListener(SetDeleteMode);

            // Update the visuals
            UpdateVisuals();
        }

        private void Update()
        {
            // Toggle the correct mode if any of the corresponding keys are pressed down
            if (_buildKey.Pressed())
            {
                SetBuildMode();
            }
            if (_editKey.Pressed())
            {
                SetEditMode();
            }
            if (_deleteKey.Pressed())
            {
                SetDeleteMode();
            }
        }

        private void SetBuildMode()
        {
            // Don't switch to same mode twice
            if (Mode == BuildMode.build)
            {
                return;
            }

            // Set mode
            Mode = BuildMode.build;

            // Update the visuals
            UpdateVisuals();

            // Update arrows using object buttons script
            LevelEditorObjectButtons.Instance?.UpdateArrows();

            categoryTabs.SetActive(true);
        }

        private void SetEditMode()
        {
            // Don't switch to same mode twice
            if (Mode == BuildMode.edit)
            {
                return;
            }

            // Set mode
            Mode = BuildMode.edit;

            // Update the visuals
            UpdateVisuals();

            // Disable both arrows for edit mode
            arrowLeft.SetActive(false);
            arrowRight.SetActive(false);

            categoryTabs.SetActive(false);
        }

        private void SetDeleteMode()
        {
            // Don't switch to same mode twice
            if (Mode == BuildMode.delete)
            {
                return;
            }

            // Set mode
            Mode = BuildMode.delete;

            // Update the visuals
            UpdateVisuals();

            // Disable both arrows for delete mode
            arrowLeft.SetActive(false);
            arrowRight.SetActive(false);

            categoryTabs.SetActive(false);
        }

        private void UpdateVisuals()
        {
            // Pro Tip: Use override sprite, it is good
            _buildImg.overrideSprite = Mode == BuildMode.build ? onSprite : null;
            _editImg.overrideSprite = Mode == BuildMode.edit ? onSprite : null;
            _deleteImg.overrideSprite = Mode == BuildMode.delete ? onSprite : null;

            // Toggle the proper areas
            buildArea.SetActive(Mode == BuildMode.build);
            editArea.SetActive(Mode == BuildMode.edit);
            deleteArea.SetActive(Mode == BuildMode.delete);
        }
    }

    /// <summary>
    /// The 3 different build modes that can be switched between.
    /// </summary>
    [System.Serializable]
    public enum BuildMode
    {
        build,
        edit,
        delete
    }
}
