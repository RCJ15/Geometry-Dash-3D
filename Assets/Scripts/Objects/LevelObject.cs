using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Level;

namespace GD3D.Objects
{
    /// <summary>
    /// The base class for all objects in a single level.
    /// </summary>
    public class LevelObject : MonoBehaviour
    {
        protected bool InLevelEditor => LevelData.InLevelEditor;
        protected bool InPlayMode => LevelData.InPlayMode;

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

        }

        protected virtual void FixedUpdate()
        {

        }
    }
}
