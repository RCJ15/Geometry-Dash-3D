using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GD3D.Easing;

namespace GD3D.UI
{
    /// <summary>
    /// A transition that smoothly moves from scene to scene.
    /// </summary>
    public class Transition : MonoBehaviour
    {
        //-- Instance
        public static Transition Instance;

        //-- Static variables
        public static Scene CurrentScene;
        public static int CurrentSceneIndex => CurrentScene.buildIndex;

        /// <summary>
        /// Returns if the transition ease ID has a value, because that means that we are currenlty transitioning.
        /// </summary>
        public static bool IsTransitioning => _transitionEaseID.HasValue;

        private static bool s_subscribedToEvents;

        //-- Easing
        private static long? _transitionEaseID = null;

        //-- Ease settings
        [SerializeField] private float fadeTime = 1;

        private Image _img;

        private void Awake()
        {
            // Set the instance
            Instance = this;

            // Get components
            _img = GetComponent<Image>();

            // Set alpha to 1
            Color color = _img.color;
            color.a = 1;
            _img.color = color;
        }

        private void Start()
        {
            // Set the ease ID to null since it's static
            _transitionEaseID = null;

            // Subscribe to events
            EasingManager.Instance.OnEaseObjectRemove += OnEaseObjectRemove;

            TransitionOut();

            // Subscribe to events (static edition)
            if (!s_subscribedToEvents)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;

                s_subscribedToEvents = true;
            }
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            CurrentScene = scene;
        }

        private void OnEaseObjectRemove(long id)
        {
            // Set the transition ease ID to null if that ease ID just got removed
            if (_transitionEaseID.HasValue && id == _transitionEaseID)
            {
                _transitionEaseID = null;
            }
        }

        private void Update()
        {
            
        }

        /// <summary>
        /// Transitions to the main menu, which is scene index 0. <para/>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <returns>The newly created <see cref="EaseObject"/>.</returns>
        public static EaseObject TransitionToMainMenu()
        {
            EaseObject ease = TransitionToScene(0);

            // Reset timeScale on complete
            ease.SetOnComplete((obj) =>
            {
                Time.timeScale = 1;
            });

            // Play quit to menu sound effect
            Audio.SoundManager.PlaySound("Quit To Menu");

            return ease;
        }

        /// <summary>
        /// Transitions to black and then changes the scene.
        /// </summary>
        /// <returns>The newly created <see cref="EaseObject"/>.</returns>
        public static EaseObject TransitionToScene(int sceneIndex)
        {
            EaseObject ease = TransitionIn();

            ease.SetOnComplete((obj) =>
            {
                SceneManager.LoadScene(sceneIndex);
            });

            return ease;
        }

        /// <summary>
        /// Sets the raycast target of the <see cref="Transition"/> <see cref="Image"/>.
        /// </summary>
        /// <returns>The newly created <see cref="EaseObject"/>.</returns>
        public static void SetRaycastTarget(bool enable)
        {
            // Get the maskable graphic
            MaskableGraphic maskableGraphic = Instance._img;

            // Set the raycast target
            maskableGraphic.raycastTarget = enable;
        }

        /// <summary>
        /// Makes the screen fade to black.
        /// </summary>
        /// <returns>The newly created <see cref="EaseObject"/>.</returns>
        public static EaseObject TransitionIn()
        {
            // Get a black color
            Color color = Color.black;

            SetRaycastTarget(true);

            return FadeToColor(color, Instance.fadeTime);
        }

        /// <summary>
        /// Makes the screen fade out of being black.
        /// </summary>
        /// <returns>The newly created <see cref="EaseObject"/>.</returns>
        public static EaseObject TransitionOut()
        {
            // Get a black color with 0 alpha
            Color color = Color.black;
            color.a = 0;

            // Create fade ease
            EaseObject ease = FadeToColor(color, Instance.fadeTime);

            // Set on complete to disable raycast target on complete
            ease.SetOnComplete((obj) => SetRaycastTarget(false));

            return ease;
        }

        /// <summary>
        /// Makes the screen fade to the given <paramref name="target"/> color.
        /// </summary>
        /// <returns>The newly created <see cref="EaseObject"/>.</returns>
        public static EaseObject FadeToColor(Color target, float fadeTime)
        {
            // Try remove the current easing
            EasingManager.TryRemoveEaseObject(_transitionEaseID);

            // Create a easeObject from the fadeTime
            EaseObject ease = new EaseObject(fadeTime);

            // Get the image
            Image img = Instance._img;

            // Cache the start color
            Color startColor = img.color;

            // Set on update
            ease.SetOnUpdate((obj) =>
            {
                // Get the new color from the ease object
                Color newColor = obj.EaseColor(startColor, target);

                // Set the color on the iamge
                img.color = newColor;
            });

            // Set to unscaled by default
            ease.SetUnscaled(true);

            // Set ID
            _transitionEaseID = ease.ID;

            // Return the easeObject
            return ease;
        }
    }
}
