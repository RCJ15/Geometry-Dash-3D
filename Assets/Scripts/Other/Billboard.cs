using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GD3D
{
    /// <summary>
    /// A billboard always rotates and faces towards the camera
    /// </summary>
    public class Billboard : MonoBehaviour
    {
        [Header("Look Direction")]
        [SerializeField] private bool lookX = true;
        [SerializeField] private bool lookY = true;

        [Header("Other")]
        [SerializeField] private bool invert = true;

        [SerializeField] private float zRot;
        public float ZRot
        {
            get => zRot;
            set => zRot = value;
        }

        private Transform cam;
        private Vector3 _oldCamPos;

        void Start()
        {
            cam = Helpers.Camera.transform;

            Look(cam);
            _oldCamPos = cam.position;
        }

        void Update()
        {
            if (_oldCamPos == cam.position)
            {
                return;
            }

            Look(cam);
            _oldCamPos = cam.position;
        }

        public void Look(Transform lookAt)
        {
            transform.LookAt(lookAt);

            transform.eulerAngles = Vector3.Scale(transform.eulerAngles, new Vector3(
                !lookX ? 0 : invert ? -1 : 1, !lookY ? 0 : 1, 1
                ));

            transform.eulerAngles += new Vector3(0, invert ? 180 : 0, -transform.eulerAngles.z + zRot);
        }
    }
}