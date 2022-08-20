using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.Level
{
    /// <summary>
    /// Add this Attribute to a field in order to mark it as a something the Level Editor should save in the JSON file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class LevelSaveAttribute : Attribute
    {
        // This attribute only exists to mark stuff so no values are needed here
    }
}
