using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.LevelEditor
{
    /// <summary>
    /// Base class for all windows in the level editor.
    /// </summary>
    public class LevelEditorWindow<T> : MonoBehaviour where T : LevelEditorWindow<T>
    {
        public static T Instance;

        protected virtual void Awake()
        {
            // Set singleton instance
            Instance = (T)this;

            // Hide by default
            gameObject.SetActive(false);
        }

        public static void Open()
        {
            Instance.LocalOpen();
        }

        public static void Close()
        {
            Instance.LocalClose();
        }

        public virtual void LocalOpen()
        {
            LevelEditorDarknessOverlay.Appear();

            gameObject.SetActive(true);
        }

        public virtual void LocalClose()
        {
            LevelEditorDarknessOverlay.Disappear();

            gameObject.SetActive(false);
        }
    }
}
