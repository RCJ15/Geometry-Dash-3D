using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D
{
    /// <summary>
    /// Implement this interface in order to allow the <see cref="MaterialColorer"/> to copy this objects colors
    /// </summary>
    public interface IMaterialColorable
    {
        /// <summary>
        /// Override this to change what array of colors are given to the <see cref="MaterialColorer"/> 
        /// </summary>
        Color[] GetColors { get; set; }

        /// <summary>
        /// Override this to change what single color that is given to the <see cref="MaterialColorer"/>
        /// </summary>
        Color GetColor { get; set; }

        /// <summary>
        /// Override this to change what color mode is given to the <see cref="MaterialColorer"/>
        /// </summary>
        MaterialColorer.ColorMode GetColorMode { get; }

        /// <summary>
        /// Override this to change what material index is given to the <see cref="MaterialColorer"/>
        /// </summary>
        /// <returns></returns>
        int GetMaterialIndex { get; }
    }
}
