using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GD3D.Level;
using GD3D.Easing;
using TMPro;

namespace GD3D.UI
{
    /// <summary>
    /// The level select screen.
    /// </summary>
    public class LevelSelect : MonoBehaviour
    {
        [SerializeField] private LevelData[] levels;
        [SerializeField] private GameObject levelTemplate;

        [Header("Level Dots")]
        [SerializeField] private GameObject dotTemplate;
        [SerializeField] private Color dotInactiveColor;
        [SerializeField] private float dotSpacing;
        private Color _dotActiveColor;

        private Image _activeDot;
        private Image[] _dots;

        [Header("Scrolling")]
        [SerializeField] private HorizontalLayoutGroup layoutGroup;
        [SerializeField] private EaseSettings scrollEase;
        [SerializeField] private float minimumMouseMoveDistance;

        [Header("Level Stats Menu")]
        [SerializeField] private Transform levelStatsMenu;
        [SerializeField] private Image levelStatsDarknessOverlay;
        private Color _darknessOverlayActiveColor;

        [Space]
        [SerializeField] private TMP_Text levelStatsNameText;
        [SerializeField] private TMP_Text totalAttemptsText;
        [SerializeField] private TMP_Text totalJumpsText;
        [SerializeField] private TMP_Text normalPercentText;
        [SerializeField] private TMP_Text practicePercentText;

        [Space]
        [SerializeField] private EaseSettings levelStatsEase;

        private long? _currentLevelStatsMenuEaseID = null;

        private static int s_scrollIndex;
        private int _oldScrollIndex;

        private float _scaleFactor;
        private float _padding;

        private float _realLeftPadding;
        private float _oldMouseX;
        private Vector2? _pressPoint = null;

        private bool _outOfDistance;

        private long? _currentScrollEaseID = null;

        private long? _currentDarknessOverlayEaseID = null;

        private void Start()
        {
            // Set the level color to the first levels color
            Color col = levels[s_scrollIndex].LevelBackgroundColor;
            LevelColors.ChangeColor(LevelColors.ColorType.background, col);
            LevelColors.ChangeColor(LevelColors.ColorType.ground, col);

            // Get the canvas so we can use the scale factor
            Canvas canvas = GetComponentInParent<Canvas>();
            _scaleFactor = canvas.scaleFactor;

            // Calculate the needed units for the horizontal layout group
            float width = ((RectTransform)levelTemplate.transform).rect.width;
            _padding = Screen.width / 2 / _scaleFactor - width / 2;

            _realLeftPadding = GetScrollPos();
            layoutGroup.spacing = _padding * 2;
            
            // Cache the transform for later use
            Transform levelTransform = levelTemplate.transform;
            Transform dotTransform = dotTemplate.transform;

            // Create dots array which we will fill later
            _dots = new Image[levels.Length];

            RectTransform dotRect = (RectTransform)dotTransform;
            float spacePerDot = (dotRect.rect.width + dotSpacing) * _scaleFactor;
            float totalSize = spacePerDot * (float)(_dots.Length - 1);

            // Clone multiple copies of the level object to populate the level select
            int i = 0;
            foreach (LevelData levelData in levels)
            {
                // Create a new level
                LevelSelectOption newLevel = Instantiate(levelTemplate, levelTransform.position, levelTransform.rotation, levelTransform.parent).GetComponent<LevelSelectOption>();

                newLevel.gameObject.name = levelData.LevelName;

                // Set the data for the level
                newLevel.LevelData = levelData;

                // Also create a dot for the level
                Image newDot = Instantiate(dotTemplate, dotTransform.position, dotTransform.rotation, dotTransform.parent).GetComponent<Image>();
                
                newDot.gameObject.name = $"{newLevel.gameObject.name} Dot";
                
                newDot.color = dotInactiveColor;
                _dots[i] = newDot;

                // Set position
                Vector3 pos = dotTransform.position;

                pos.x = spacePerDot * (float)i + (((float)Screen.width / 2f) - (totalSize / 2));
                
                newDot.transform.position = pos;

                i++;
            }

            // Set the active dot
            _dotActiveColor = dotTemplate.GetComponent<Image>().color;

            _activeDot = _dots[s_scrollIndex];
            _activeDot.color = _dotActiveColor;

            // Disable the templates so they won't do anything stupid
            levelTemplate.SetActive(false);
            dotTemplate.SetActive(false);

            // Hide the level stats screen
            levelStatsMenu.localScale = Vector3.zero;

            _darknessOverlayActiveColor = levelStatsDarknessOverlay.color;
            levelStatsDarknessOverlay.color = Color.clear;
            levelStatsDarknessOverlay.raycastTarget = false;

            // Subscribe to events
            EasingManager.Instance.OnEaseObjectRemove += OnEaseObjectRemove;

            // Set the last active menu scene index
            MenuData.LastActiveMenuSceneIndex = (int)Transition.SceneIndex.levelSelect;
        }

        private void Update()
        {
            // Set the press point when the mouse is pressed
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                _pressPoint = Input.mousePosition;

                _oldScrollIndex = s_scrollIndex;

                CancelScrollEasing();
            }

            // Scroll according to the mouse when it's held down and out of a specific range
            if (Input.GetMouseButton(0) && _pressPoint.HasValue)
            {
                float distance = Vector2.Distance(_pressPoint.Value, Input.mousePosition);

                _outOfDistance = distance >= minimumMouseMoveDistance * _scaleFactor;

                if (_outOfDistance)
                {
                    // Move the padding by the screen mouse delta divided by the scale factor
                    _realLeftPadding += (Input.mousePosition.x - _oldMouseX) / _scaleFactor;

                    s_scrollIndex = Mathf.RoundToInt((_realLeftPadding - _padding) / -layoutGroup.spacing / 2.5f);
                    UpdateScrollIndex(0);
                }
            }

            // Snap the scroll when the mouse is let go
            if (Input.GetMouseButtonUp(0) && _pressPoint.HasValue && _outOfDistance)
            {
                // Check if the scroll index hasn't changed at all
                if (_oldScrollIndex == s_scrollIndex)
                {
                    // Scroll left/rigth depending on where we pressed and where the mouse is currently on the X axis
                    if (_pressPoint.Value.x < Input.mousePosition.x)
                    {
                        ScrollLeft();
                    }
                    else
                    {
                        ScrollRight();
                    }
                }
                else
                {
                    Scroll();
                }

                _outOfDistance = false;
                _pressPoint = null;
            }

            // Update padding
            if (layoutGroup.padding.left != (int)_realLeftPadding)
            {
                layoutGroup.padding.left = (int)_realLeftPadding;
                layoutGroup.SetLayoutHorizontal();
            }

            _oldMouseX = Input.mousePosition.x;
        }

        /// <summary>
        /// Scrolls to the left.
        /// </summary>
        public void ScrollLeft()
        {
            UpdateScrollIndex(-1);

            Scroll();
        }

        /// <summary>
        /// Scrolls to the right.
        /// </summary>
        public void ScrollRight()
        {
            UpdateScrollIndex(1);

            Scroll();
        }

        /// <summary>
        /// Updates the scroll index by the given <paramref name="amount"/> and also clamps correctly.
        /// </summary>
        private void UpdateScrollIndex(int amount)
        {
            s_scrollIndex += amount;

            // Clamp the scroll index between 0 and the levels length so it doesn't go out of range
            s_scrollIndex = Mathf.Clamp(s_scrollIndex, 0, levels.Length - 1);

            // Deactivate the currently active dot and set the new active dot
            _activeDot.color = dotInactiveColor;

            _activeDot = _dots[s_scrollIndex];
            _activeDot.color = _dotActiveColor;

            Color col = levels[s_scrollIndex].LevelBackgroundColor;
            LevelColors.AddEase(LevelColors.ColorType.background, col, new EaseObject(0.3f));
            LevelColors.AddEase(LevelColors.ColorType.ground, col, new EaseObject(0.3f));
        }

        #region Easing
        /// <summary>
        /// Will use <see cref="EaseObject"/> in order to scroll to a certain level using the scroll index.
        /// </summary>
        private void Scroll()
        {
            CancelScrollEasing();

            // Create the ease object
            EaseObject easeObj = scrollEase.CreateEase();

            // Set the OnUpdate event
            float startValue = _realLeftPadding;
            float targetValue = GetScrollPos();

            easeObj.OnUpdate += (obj) =>
            {
                _realLeftPadding = obj.GetValue(startValue, targetValue);
            };
            
            // Set the ID
            _currentScrollEaseID = easeObj.ID;
        }

        private float GetScrollPos()
        {
            return _padding * -((float)s_scrollIndex * 5f) + _padding;
        }

        /// <summary>
        /// Cancels our current easing if it exists.
        /// </summary>
        private void CancelScrollEasing()
        {
            // Try to remove the ease object ID
            if (EasingManager.TryRemoveEaseObject(_currentScrollEaseID))
            {
                _currentScrollEaseID = null;
            }
        }

        /// <summary>
        /// Opens the level stats screen and updates the menus info correctly to the current level selected.
        /// </summary>
        public void OpenLevelStats()
        {
            // Update the level stats screen with data from the JSON save file
            SaveFile.LevelSaveData levelSave = null;
            string levelName = levels[s_scrollIndex].LevelName;
            levelSave = SaveData.SaveFile.LevelData.FirstOrDefault((levelData) => levelData.name == levelName);

            // Set the name
            levelStatsNameText.text = levelName;

            // Set to default if level save is null
            if (levelSave == null)
            {
                totalAttemptsText.text = "Total Attempts<color=white>: 0";
                totalJumpsText.text = "Total Jumps<color=white>: 0";

                normalPercentText.text = "Normal<color=white>: 0%";
                practicePercentText.text = "Practice<color=white>: 0%";
            }
            else
            {
                totalAttemptsText.text = $"Total Attempts<color=white>: {levelSave.totalAttempts}";
                totalJumpsText.text = $"Total Jumps<color=white>: {levelSave.totalJumps}";

                normalPercentText.text = $"Normal<color=white>: {ProgressBar.ToPercent(levelSave.normalPercent)}";
                practicePercentText.text = $"Practice<color=white>: {ProgressBar.ToPercent(levelSave.practicePercent)}";
            }

            // Create ease object from the level stats ease
            EaseObject easeObj = levelStatsEase.CreateEase();

            // Set on update to scale the menu
            easeObj.OnUpdate = (obj) =>
            {
                levelStatsMenu.localScale = obj.EaseVector(Vector3.zero, Vector3.one);
            };

            // Set the level stats ease ID
            _currentLevelStatsMenuEaseID = easeObj.ID;

            // Also create a linear easing for the darkness overlay
            EaseObject overlayEaseObj = new EaseObject(0.2f);

            Color darkColor = _darknessOverlayActiveColor;
            darkColor.a = 0;

            levelStatsDarknessOverlay.raycastTarget = true;

            // Set on update method
            overlayEaseObj.OnUpdate = (obj) =>
            {
                levelStatsDarknessOverlay.color = obj.EaseColor(darkColor, _darknessOverlayActiveColor);
            };

            // Set current ease ID
            _currentDarknessOverlayEaseID = overlayEaseObj.ID;
        }

        /// <summary>
        /// Closes the level stats screen if it's opened
        /// </summary>
        public void CloseLevelStats()
        {
            EasingManager.TryRemoveEaseObject(_currentLevelStatsMenuEaseID);

            levelStatsMenu.localScale = Vector3.zero;

            EasingManager.TryRemoveEaseObject(_currentDarknessOverlayEaseID);

            levelStatsDarknessOverlay.color = Color.clear;
            levelStatsDarknessOverlay.raycastTarget = false;
        }

        /// <summary>
        /// Called when an <see cref="EaseObject"/> is removed.
        /// </summary>
        private void OnEaseObjectRemove(long easeID)
        {
            // Set the ease ID to null if the object just got removed
            if (easeID == _currentScrollEaseID)
            {
                _currentScrollEaseID = null;
            }
            else if (easeID == _currentLevelStatsMenuEaseID)
            {
                _currentLevelStatsMenuEaseID = null;
            }
            else if (easeID == _currentDarknessOverlayEaseID)
            {
                _currentDarknessOverlayEaseID = null;
            }
        }
        #endregion

        /// <summary>
        /// Transitions to the given menu scene <paramref name="index"/>.
        /// </summary>
        public void GotoMenu(Transition.SceneIndex index)
        {
            // Don't transition if we are already transitioning
            if (Transition.IsTransitioning)
            {
                return;
            }

            Transition.TransitionToScene((int)index);
        }

        /// <summary>
        /// Transitions to the given scene <paramref name="index"/>.
        /// </summary>
        public void GotoScene(int index)
        {
            // Don't transition if we are already transitioning
            if (Transition.IsTransitioning)
            {
                return;
            }

            Transition.TransitionToScene(index);
        }

        /// <summary>
        /// Sets s_scrollIndex to 0. Mainly used in UnityEvents.
        /// </summary>
        public void ResetLevelIndex()
        {
            s_scrollIndex = 0;
        }

        /// <summary>
        /// The level data struct for a single level in the <see cref="levelse"/> menu.
        /// </summary>
        [System.Serializable]
        public struct LevelData
        {
            public string LevelName;
            public int StartAwarded;
            public Sprite DifficultyFace;

            [Space]
            public Color LevelBackgroundColor;
            public int LevelBuildIndex;
        }
    }
}
