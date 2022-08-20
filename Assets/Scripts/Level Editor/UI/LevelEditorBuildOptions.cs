using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GD3D.CustomInput;
using GD3D.Player;

namespace GD3D.LevelEditor
{
    /// <summary>
    /// The different build options that can be toggled in the editor. Those being: Swipe, Rotate, Free Move & Snap.
    /// </summary>
    public class LevelEditorBuildOptions : MonoBehaviour
    {
        public static bool Swipe;
        public static bool Rotate;
        public static bool FreeMove;
        public static bool Snap;

        [SerializeField] private Toggle swipeToggle;
        [SerializeField] private Toggle rotateToggle;
        [SerializeField] private Toggle freeMoveToggle;
        [SerializeField] private Toggle snapToggle;

        private Key _swipeKey, _rotateKey, _freeMoveKey, _snapKey;

        private void Start()
        {
            // Reset static variables
            Swipe = false;
            Rotate = false;
            FreeMove = false;
            Snap = false;

            // Get keys
            _swipeKey = PlayerInput.GetKey("Toggle Swipe");
            _rotateKey = PlayerInput.GetKey("Toggle Rotate");
            _freeMoveKey = PlayerInput.GetKey("Toggle Free Move");
            _snapKey = PlayerInput.GetKey("Toggle Snap");

            // Subscribe to events
            swipeToggle.onValueChanged.AddListener((on) => ToggleSwipe(on, false));
            swipeToggle.onValueChanged.AddListener((on) => ToggleSwipe(on, false));
            swipeToggle.onValueChanged.AddListener((on) => ToggleSwipe(on, false));
            swipeToggle.onValueChanged.AddListener((on) => ToggleSwipe(on, false));
        }

        private void Update()
        {
            // Toggle the correct mode if any of the corresponding keys are pressed down
            if (_swipeKey.Pressed()) 
            {
                ToggleSwipe(!Swipe);
            }
            if (_rotateKey.Pressed())
            {
                ToggleRotate(!Rotate);
            }
            if (_freeMoveKey.Pressed())
            {
                ToggleFreeMove(!FreeMove);
            }
            if (_snapKey.Pressed())
            {
                ToggleSnap(!Snap);
            }
        }

        public void ToggleSwipe(bool enabled, bool updateToggle = true)
        {
            // Set swipe
            Swipe = enabled;

            // Update toggle if told to do so
            if (updateToggle)
            {
                swipeToggle.SetIsOnWithoutNotify(Swipe);
            }
        }

        public void ToggleRotate(bool enabled, bool updateToggle = true)
        {
            // Set rotate
            Rotate = enabled;

            // Update toggle if told to do so
            if (updateToggle)
            {
                rotateToggle.SetIsOnWithoutNotify(Rotate);
            }
        }

        public void ToggleFreeMove(bool enabled, bool updateToggle = true)
        {
            // Set free move
            FreeMove = enabled;

            // Update toggle if told to do so
            if (updateToggle)
            {
                freeMoveToggle.SetIsOnWithoutNotify(FreeMove);
            }
        }

        public void ToggleSnap(bool enabled, bool updateToggle = true)
        {
            // Set snap
            Snap = enabled;

            // Update toggle if told to do so
            if (updateToggle)
            {
                snapToggle.SetIsOnWithoutNotify(Snap);
            }
        }
    }
}
