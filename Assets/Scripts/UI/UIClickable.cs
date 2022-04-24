using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GD3D.Easing;
using GD3D.CustomInput;

namespace GD3D.UI
{
    /// <summary>
    /// Attach this script to a UI object and it'll do the classic GD <see cref="EasingType.bounceOut"/> size increase animation thingy when pressed!
    /// </summary>
    public class UIClickable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Tooltip("The object that'll change size when this is pressed. \nLeave this null for this object to be modified.")]
        [SerializeField] private Transform modifySize;
        [SerializeField] private bool isUnscaled;

        private UIClickableManager _manager;

        private Vector3 _startSize;

        private Transform _transform;

        private RectTransform rectTransform
        {
            get
            {
                Transform transform = _transform;

                if (transform == null)
                {
                    transform = this.transform;
                }

                return transform as RectTransform;
            }
        }

        private bool _mouseOnTop = false;
        private bool _isSizing;
        private long? _currentEaseId = null;

        //-- Selectable
        private Selectable _attachedSelectable;
        public bool Interactable
        {
            get
            {
                if (_attachedSelectable != null)
                {
                    return _attachedSelectable.interactable;
                }

                return true;
            }
        }

        private void Awake()
        {
            // Cache transform
            _transform = transform;

            // Set modify size to this object if it's null
            if (modifySize == null)
            {
                modifySize = _transform;
            }

            // Set the starting size
            _startSize = modifySize.localScale;
        }

        private void Start()
        {
            // Get the manager
            _manager = UIClickableManager.Instance;

            // Subscribe to event
            EasingManager.Instance.OnEaseObjectRemove += OnEaseObjectRemove;

            // Get selectable component
            if (!TryGetComponent(out _attachedSelectable))
            {
                // If that fails try to get the component in children
                _attachedSelectable = GetComponentInChildren<Selectable>();

                // Try for one last time but getting component in parent
                if (_attachedSelectable == null)
                {
                    _attachedSelectable = GetComponentInParent<Selectable>();
                }
            }
        }

        private void Update()
        {
            // Detect if the mouse is on top, the left mouse button is pressed and if this object is not being sized
            if (Interactable && _mouseOnTop && Input.GetMouseButton(0) && !_isSizing)
            {
                // Start sizing
                _isSizing = true;

                SizeIn();
            }
            // Detect when the mouse is let go or if the mouse goes off this object
            else if (!Interactable || (_isSizing && (Input.GetMouseButtonUp(0)|| !_mouseOnTop)))
            {
                // Stop sizing
                _isSizing = false;

                SizeOut();
            }
        }

        /// <summary>
        /// Will use <see cref="easeSettingsIn"/> to start sizing to become bigger. <para/>
        /// Called when <see cref="_isSizing"/> is set to true.
        /// </summary>
        private void SizeIn()
        {
            // Try remove ease object
            EasingManager.TryRemoveEaseObject(_currentEaseId);

            // Create new scale easing
            EaseObject ease = modifySize.EaseScale(_startSize * _manager.SizeIncrease, 1);

            // Set ease settings
            ease.SetSettings(_manager.EaseSettingsIn);
            ease.SetUnscaled(isUnscaled);

            // Set ID
            _currentEaseId = ease.ID;
        }

        /// <summary>
        /// Will use <see cref="easeSettingsOut"/> to start sizing to become normal. <para/>
        /// Called when <see cref="_isSizing"/> is set to false.
        /// </summary>
        private void SizeOut()
        {
            // Try remove ease object
            EasingManager.TryRemoveEaseObject(_currentEaseId);

            // If the mouse is on this object currently, then instantly change size
            if (_mouseOnTop)
            {
                // Set scale to start scale instantly
                modifySize.localScale = _startSize;

                // Set ID
                _currentEaseId = null;
            }
            else
            {
                // Create new scale easing
                EaseObject ease = modifySize.EaseScale(_startSize, 1);

                // Set ease settings
                ease.SetSettings(_manager.EaseSettingsOut);
                ease.SetUnscaled(isUnscaled);

                // Set ID
                _currentEaseId = ease.ID;
            }
        }

        private void OnEaseObjectRemove(long id)
        {
            // Set current ease ID to null if the current ease got removed
            if (id == _currentEaseId)
            {
                _currentEaseId = null;
            }
        }

        /// <summary>
        /// Will instantly stop this object from scaling.
        /// </summary>
        public void StopScaling()
        {
            // Try remove ease object
            EasingManager.TryRemoveEaseObject(_currentEaseId);

            // No mouse is on this object anymore
            _mouseOnTop = false;

            // We are no longer sizing
            _isSizing = false;

            // Set scale to start scale
            // We also add a null check here since this method could be called in a awake somewhere where modify size has not been set yet
            if (modifySize != null)
            {
                modifySize.localScale = _startSize;
            }

            // Set ID
            _currentEaseId = null;
        }

        // Set if the mouse is on top or not using the IPointerEnterHandler and IPointerExitHandler interfaces
        public void OnPointerEnter(PointerEventData eventData)
        {
            _mouseOnTop = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _mouseOnTop = false;
        }

#if UNITY_EDITOR
        // This will basically draw a gizmo for the image raycast padding zone since unity is too lazy to add it themselves
        private Image img;

        private void OnDrawGizmosSelected()
        {
            // Try get image
            if (img == null)
            {
                if (!TryGetComponent(out img))
                {
                    // If try fails, then try getting in children
                    img = GetComponentInChildren<Image>();

                    // Return if still null
                    if (img == null)
                    {
                        return;
                    }
                }
            }

            // Copied and modified from a answer on this post https://forum.unity.com/threads/ui-image-component-raycast-padding-needs-a-gizmo.1019260/
            Rect rect = rectTransform.rect;

            if (transform.parent != null)
            {
                rect.x += transform.localPosition.x;
                rect.y += transform.localPosition.y;
            }

            // Add raycast padding
            rect.xMin += img.raycastPadding.x;
            rect.xMax -= img.raycastPadding.z;
            rect.yMin += img.raycastPadding.y;
            rect.yMax -= img.raycastPadding.w;
            
            // Scale with local scale
            Vector2 oldScale = rect.size;
            rect.size *= transform.localScale;
            rect.position += (oldScale - rect.size) / 2;

            // Calculate world positions
            Vector3 p0 = transform.parent.TransformPoint(new Vector2(rect.x, rect.y));
            Vector3 p1 = transform.parent.TransformPoint(new Vector2(rect.x, rect.yMax));
            Vector3 p2 = transform.parent.TransformPoint(new Vector2(rect.xMax, rect.yMax));
            Vector3 p3 = transform.parent.TransformPoint(new Vector2(rect.xMax, rect.y));

            // Use handles to draw
            UnityEditor.Handles.color = Color.green;

            UnityEditor.Handles.DrawLine(p0, p1);
            UnityEditor.Handles.DrawLine(p1, p2);
            UnityEditor.Handles.DrawLine(p2, p3);
            UnityEditor.Handles.DrawLine(p3, p0);
        }
#endif
    }
}
