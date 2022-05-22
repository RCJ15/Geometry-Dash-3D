using System.Runtime.InteropServices;
using UnityEngine;

namespace GD3D.UI
{
    /// <summary>
    /// A script that will open a link when <see cref="OpenLink(string)"/> is called.
    /// </summary>
    public class LinkOpener : MonoBehaviour
    {
        /// <summary>
        /// Opens a hyperlink. Works as expected in WEBGL as well.
        /// </summary>
        public void OpenLink(string url)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                OpenWindow(url);
            }
            else
            {
                Application.OpenURL(url);
            }
        }

        // Need to use a special dll and jslib file for this to work properly
        [DllImport("__Internal")]
        private static extern void OpenWindow(string url);
    }
}
