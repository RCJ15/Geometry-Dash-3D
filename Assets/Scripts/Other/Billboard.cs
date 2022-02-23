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

        private Transform cam;

        // Start is called before the first frame update
        void Start()
        {
            cam = Camera.main.transform;
        }

        // Update is called once per frame
        void Update()
        {
            Look(cam);
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

#if UNITY_EDITOR
    [CustomEditor(typeof(Billboard), true)]
    public class BillboardEditor : Editor
    {
        private Billboard obj;
        private Transform cam;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Update looking"))
            {
                obj = (Billboard)target;
                cam = Camera.main.transform;

                obj.Look(cam);
            }
        }
    }
#endif
}