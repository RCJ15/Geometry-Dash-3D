using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.LevelEditor
{
    /// <summary>
    /// Handles all the undo and redo in the level editor.
    /// </summary>
    public class LevelEditorUndo : MonoBehaviour
    {
        public static LevelEditorUndo Instance;

        private List<Action> _history = new List<Action>();
        public static List<Action> History => Instance._history;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            
        }

        /// <summary>
        /// Undoes an action in the recorded history.
        /// </summary>
        public static void Undo()
        {

        }

        /// <summary>
        /// Redoes an undo if any undoes have been done and if the no action has recently been recorded.
        /// </summary>
        public static void Redo()
        {

        }

        /// <summary>
        /// Records an action and allows it to be undone.
        /// </summary>
        public static void Record(Action action)
        {
            History.Add(action);
        }
    }
}
