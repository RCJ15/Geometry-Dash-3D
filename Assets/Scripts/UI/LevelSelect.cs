using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GD3D.Level;
using GD3D.Easing;
using GD3D.CustomInput;
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
        [SerializeField] private EaseSettings scrollEase;
        [SerializeField] private float minimumMouseMoveDistance;
        private Transform[] _levelTransforms;
        private int _length;

        [Header("Level Stats Menu")]
        [SerializeField] private Transform levelStatsMenu;
        [SerializeField] private Image levelStatsDarknessOverlay;
        private Color _darknessOverlayActiveColor;

        [Space]
        [SerializeField] private GameObject statsScreen;
        [SerializeField] private TMP_Text levelStatsNameText;
        [SerializeField] private TMP_Text totalAttemptsText;
        [SerializeField] private TMP_Text totalJumpsText;
        [SerializeField] private TMP_Text normalPercentText;
        [SerializeField] private TMP_Text practicePercentText;

        [Space]
        [SerializeField] private EaseSettings levelStatsEase;

        [Header("Coming Soon Screen")]
        [SerializeField] private GameObject comingSoonScreen;
        [SerializeField] private Color comingSoonBackgroundColor;

        [Space]
        [SerializeField] private GameObject comingSoonText;
        private Transform _comingSoonStartScreen;

        private long? _currentLevelStatsMenuEaseID = null;

        private static int s_scrollIndex;
        private int _oldScrollIndex;

        private float _scaleFactor;

        private float _levelOffset;

        private float _oldMouseX;
        private Vector2? _pressPoint = null;

        private bool _outOfDistance;

        private long? _currentScrollEaseID = null;

        private long? _currentDarknessOverlayEaseID = null;

        private Key _quitKey;

        private void Start()
        {
            _levelOffset = GetScrollOffsetPos();

            // Set the level color to the first levels color
            Color col = levels[s_scrollIndex].LevelBackgroundColor;
            LevelColors.ChangeColor(LevelColors.ColorType.background, col);
            LevelColors.ChangeColor(LevelColors.ColorType.ground, col);

            // Get the canvas so we can use the scale factor
            Canvas canvas = GetComponentInParent<Canvas>();
            _scaleFactor = canvas.scaleFactor;
            
            // Cache the transform for later use
            Transform levelTransform = levelTemplate.transform;
            Transform dotTransform = dotTemplate.transform;

            // Create new arrays which we will fill later
            _dots = new Image[levels.Length + 1];
            _levelTransforms = new Transform[levels.Length + 1];

            RectTransform dotRect = (RectTransform)dotTransform;
            float spacePerDot = (dotRect.rect.width + dotSpacing) * _scaleFactor;
            float totalDotSize = spacePerDot * (float)(_dots.Length - 1);

            _length = levels.Length + 1;

            // Clone multiple copies of the level object to populate the level select
            // Also create the coming soon screen that will be on the end of the level select screen
            for (int i = 0; i < _length; i++)
            {
                Vector3 levelPos = levelTransform.position;
                levelPos.x = GetScrollPos(i);

                // Create a new level panel
                if (i < levels.Length)
                {
                    // Get the level data
                    LevelData levelData = levels[i];

                    // Create a new level
                    LevelSelectOption newLevel = Instantiate(levelTemplate, levelPos, levelTransform.rotation, levelTransform.parent).GetComponent<LevelSelectOption>();
                    _levelTransforms[i] = newLevel.transform;

                    newLevel.gameObject.name = levelData.LevelName;

                    // Set the data for the level
                    newLevel.LevelData = levelData;
                }
                // Set the coming soon screen if we are out of range, this means that this screen will be the last object in the array
                else
                {
                    comingSoonScreen.transform.position = levelPos;

                    _levelTransforms[i] = comingSoonScreen.transform;
                }

                // Also create a dot for the level
                Vector3 dotPos = dotTransform.position;

                dotPos.x = spacePerDot * (float)i + (((float)Screen.width / 2f) - (totalDotSize / 2));

                Image newDot = Instantiate(dotTemplate, dotPos, dotTransform.rotation, dotTransform.parent).GetComponent<Image>();

                newDot.gameObject.name = $"Dot ({(i + 1)})";

                newDot.color = dotInactiveColor;
                _dots[i] = newDot;
            }

            // Set the active dot color
            _dotActiveColor = dotTemplate.GetComponent<Image>().color;

            // Set which dot is active currently
            _activeDot = _dots[s_scrollIndex];
            _activeDot.color = _dotActiveColor;

            // Disable the templates so they won't do anything stupid
            levelTemplate.SetActive(false);
            dotTemplate.SetActive(false);

            // Create a clone of the coming soon screen and place it at index -1
            // This coming soon screen will be used so that we can infinitely loop around the coming soon screens seamlessly
            Vector3 comingSoonPos = levelTransform.position;
            comingSoonPos.x = GetScrollPos(-1);

            GameObject comingSoonClone = Instantiate(comingSoonScreen, comingSoonPos, Quaternion.identity, comingSoonScreen.transform.parent);
            _comingSoonStartScreen = comingSoonClone.transform;

            // Hide the level stats screen
            levelStatsMenu.localScale = Vector3.zero;

            _darknessOverlayActiveColor = levelStatsDarknessOverlay.color;
            levelStatsDarknessOverlay.color = Color.clear;
            levelStatsDarknessOverlay.raycastTarget = false;

            // Subscribe to events
            EasingManager.Instance.OnEaseObjectRemove += OnEaseObjectRemove;

            // Set the last active menu scene index
            MenuData.LastActiveMenuSceneIndex = (int)Transition.SceneIndex.levelSelect;

            // Get the quit key
            _quitKey = Player.PlayerInput.GetKey("Escape");
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
                    _levelOffset += (Input.mousePosition.x - _oldMouseX) / _scaleFactor;

                    // Set the scroll index by converting the level offset into an index integer
                    s_scrollIndex = Mathf.RoundToInt(_levelOffset / -(float)Screen.width);
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
                    Scroll(_oldScrollIndex, s_scrollIndex);
                }

                _outOfDistance = false;
                _pressPoint = null;
            }

            // Update position of all levels and the 2 coming soon screens
            for (int i = -1; i < _levelTransforms.Length; i++)
            {
                // First coming soon screen will be chosen if the index is -1
                Transform level = i == -1 ? _comingSoonStartScreen : _levelTransforms[i];

                Vector3 pos = level.position;

                pos.x = GetScrollPos(i) + _levelOffset;

                level.position = pos;
            }

            _oldMouseX = Input.mousePosition.x;

            // Change the scene to the main menu if the quit key is pressed down
            if (!Transition.IsTransitioning && _quitKey.Pressed(PressMode.down))
            {
                GotoMenu(Transition.SceneIndex.mainMenu);
            }
        }

        /// <summary>
        /// Scrolls to the left.
        /// </summary>
        public void ScrollLeft()
        {
            int oldIndex = s_scrollIndex;

            UpdateScrollIndex(-1);

            Scroll(oldIndex, s_scrollIndex);
        }

        /// <summary>
        /// Scrolls to the right.
        /// </summary>
        public void ScrollRight()
        {
            int oldIndex = s_scrollIndex;

            UpdateScrollIndex(1);

            Scroll(oldIndex, s_scrollIndex);
        }

        /// <summary>
        /// Updates the scroll index by the given <paramref name="amount"/> and also clamps correctly.
        /// </summary>
        private void UpdateScrollIndex(int amount)
        {
            s_scrollIndex += amount;

            // Clamp the scroll index between 0 and the levels length so it doesn't go out of range
            if (s_scrollIndex < 0)
            {
                s_scrollIndex += _length;
            }
            else if (s_scrollIndex > _length - 1)
            {
                s_scrollIndex -= _length;
            }
            //s_scrollIndex = Mathf.Clamp(s_scrollIndex, 0, levels.Length - 1);

            // Deactivate the currently active dot and set the new active dot
            _activeDot.color = dotInactiveColor;

            _activeDot = _dots[s_scrollIndex];
            _activeDot.color = _dotActiveColor;

            Color col = s_scrollIndex < _length - 1 ? levels[s_scrollIndex].LevelBackgroundColor : comingSoonBackgroundColor;

            LevelColors.AddEase(LevelColors.ColorType.background, col, new EaseObject(0.3f));
            LevelColors.AddEase(LevelColors.ColorType.ground, col, new EaseObject(0.3f));
        }

        #region Easing
        /// <summary>
        /// Will use <see cref="EaseObject"/> in order to scroll to a certain level using the scroll index.
        /// </summary>
        private void Scroll(int oldIndex, int newIndex)
        {
            CancelScrollEasing();

            // Determine if we are going to loop around or not by checking if we went from the last index to the first and vice versa
            bool loopAround = (oldIndex == _length - 1 && newIndex == 0) || (oldIndex == 0 && newIndex == _length - 1);
            int movingDirection = (oldIndex == _length - 1 && newIndex == 0) ? 1 : -1;

            // Create the ease object
            EaseObject easeObj = scrollEase.CreateEase();

            // Set the OnUpdate event
            float startValue = _levelOffset;
            float targetValue = GetScrollOffsetPos(loopAround ? oldIndex + movingDirection : newIndex);

            easeObj.OnUpdate += (obj) =>
            {
                // Do special looping if we are going to loop around
                if (loopAround)
                {
                    // Check how far we are in the operation
                    float t = obj.GetValue(0, 1);

                    // Check if we are more than halfway
                    if (t > 0.5f)
                    {
                        // Set loop around to false so we don't loop around multiple times
                        loopAround = false;

                        // Teleport to the other end of the chain and set a new target direction to move towards
                        startValue = GetScrollOffsetPos(newIndex - movingDirection);
                        targetValue = GetScrollOffsetPos(newIndex);
                    }
                }

                _levelOffset = obj.GetValue(startValue, targetValue);
            };
            
            // Set the ID
            _currentScrollEaseID = easeObj.ID;
        }

        private float GetScrollOffsetPos()
        {
            return GetScrollOffsetPos(s_scrollIndex);
        }

        private float GetScrollOffsetPos(int i)
        {
            return -GetScrollPos(i) + (Screen.width / 2f);
        }

        private float GetScrollPos(int i)
        {
            return ((float)Screen.width / 2f) + ((float)Screen.width * (float)i);
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
            // Update the level stats screen with data from the JSON save file if we are in range
            if (s_scrollIndex < _length - 1)
            {
                SaveFile.LevelSaveData levelSave = null;
                string levelName = levels[s_scrollIndex].LevelName;
                levelSave = SaveData.SaveFile.LevelData.FirstOrDefault((levelData) => levelData.name == levelName);

                // Set the name
                levelStatsNameText.text = levelName;

                // Enable all the stats screen so we can see the stats
                statsScreen.SetActive(true);

                // Also disable the coming soon text
                comingSoonText.SetActive(false);

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
            }
            // If we are out of range, then we can assume we are at the coming soon screen, so show that text
            else
            {
                // Disable the stats screen so we won't see the stats
                statsScreen.SetActive(false);

                // Enable the coming soon text so we can see the text
                comingSoonText.SetActive(true);
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
