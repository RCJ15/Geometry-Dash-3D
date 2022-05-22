using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using GD3D.Easing;
using GD3D.CustomInput;

namespace GD3D.UI
{
    /// <summary>
    /// The main menu.
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject quitButton;

        [Space]
        [SerializeField] private Transform infoMenu;
        [SerializeField] private Transform quitMenu;

        [Space]
        [SerializeField] private Image darknessOverlay;
        private Color _darknessOverlayActiveColor;

        [Space]
        [SerializeField] private EaseSettings menuPopupEaseSettings;

        private long? _currentMenuPopupEaseID = null;
        private long? _currentDarknessOverlayEaseID = null;

        private Key _quitKey;

        private void Awake()
        {
            // Set the starting color as the active color for the darkness overlay
            _darknessOverlayActiveColor = darknessOverlay.color;

            // Set the size of both menus to 0
            quitMenu.localScale = Vector3.zero;
            infoMenu.localScale = Vector3.zero;

            // Make the darkness overlay disappear
            darknessOverlay.color = Color.clear;
            darknessOverlay.raycastTarget = false;
        }

        private void Start()
        {
            // Set the last active menu scene index
            MenuData.LastActiveMenuSceneIndex = (int)Transition.SceneIndex.mainMenu;

            // Subscribe to events
            EasingManager.Instance.OnEaseObjectRemove += OnEaseObjectRemove;

            // Disable the quit button in WebGL since the player can just exit the page to quit so having this button is unnecessary
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                quitButton.SetActive(false);
            }

            // Get the quit key
            _quitKey = Player.PlayerInput.GetKey("Escape");
        }

        private void Update()
        {
            // If the pause key is pressed, open the quit menu
            if (quitMenu.localScale == Vector3.zero && _quitKey.Pressed(PressMode.down))
            {
                ShowQuitMenu();
            }
        }

        /// <summary>
        /// Called when a <see cref="EaseObject"/> is removed from the <see cref="EasingManager"/>.
        /// </summary>
        private void OnEaseObjectRemove(long easeID)
        {
            // Set the appropriate ease ID to null if that ease ID just got removed
            if (easeID == _currentMenuPopupEaseID)
            {
                _currentMenuPopupEaseID = null;
            }
            else if (easeID == _currentDarknessOverlayEaseID)
            {
                _currentDarknessOverlayEaseID = null;
            }
        }

        /// <summary>
        /// Shows the quit menu. Used in <see cref="UnityEngine.Events.UnityEvent"/> for <see cref="Button"/>.
        /// </summary>
        public void ShowQuitMenu()
        {
            // Return in WebGL since the player can just exit the page to quit so having this menu open is unnecessary
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                return;
            }
            
            ShowMenu(quitMenu);

            ShowDarknessOverlay();
        }

        /// <summary>
        /// Shows the info menu. Used in <see cref="UnityEngine.Events.UnityEvent"/> for <see cref="Button"/>.
        /// </summary>
        public void ShowInfoMenu()
        {
            ShowMenu(infoMenu);

            ShowDarknessOverlay();
        }

        /// <summary>
        /// Shows the given <paramref name="menu"/> and makes it ease in properly.
        /// </summary>
        private void ShowMenu(Transform menu)
        {
            EasingManager.TryRemoveEaseObject(_currentMenuPopupEaseID);

            // Create ease object from the menu popup ease settings
            EaseObject easeObj = menuPopupEaseSettings.CreateEase();

            // Set on update to scale the menu
            easeObj.OnUpdate = (obj) =>
            {
                menu.localScale = obj.EaseVector(Vector3.zero, Vector3.one);
            };

            // Set the menu popup ease ID
            _currentMenuPopupEaseID = easeObj.ID;
        }

        /// <summary>
        /// Makes the screen darkness overlay linearly appear.
        /// </summary>
        private void ShowDarknessOverlay()
        {
            // Also create a linear easing for the darkness overlay
            EaseObject overlayEaseObj = new EaseObject(0.2f);

            Color darkColor = _darknessOverlayActiveColor;
            darkColor.a = 0;

            darknessOverlay.raycastTarget = true;

            // Set on update method
            overlayEaseObj.OnUpdate = (obj) =>
            {
                darknessOverlay.color = obj.EaseColor(darkColor, _darknessOverlayActiveColor);
            };

            // Set current ease ID
            _currentDarknessOverlayEaseID = overlayEaseObj.ID;
        }

        /// <summary>
        /// Closes the currently opened popup menu.
        /// </summary>
        public void ClosePopupMenu()
        {
            // Try remove eases
            EasingManager.TryRemoveEaseObject(_currentMenuPopupEaseID);
            EasingManager.TryRemoveEaseObject(_currentDarknessOverlayEaseID);

            // Set the size of both menus to 0
            quitMenu.localScale = Vector3.zero;
            infoMenu.localScale = Vector3.zero;

            // Make the darkness overlay disappear
            darknessOverlay.color = Color.clear;
            darknessOverlay.raycastTarget = false;
        }

        /// <summary>
        /// Quits the application. If called whilst in the editor, then the editor application is stopped instead.
        /// </summary>
        public void QuitApplication()
        {
#if UNITY_EDITOR
            Debug.Log("Quit Editor");
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

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
    }
}
