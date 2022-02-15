using UnityEngine;
using Cinemachine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GD3D.Cinemachine
{
    /// <summary>
    /// An add-on module for Cinemachine Virtual Camera that locks the camera's position to the given values
    /// </summary>
    [ExecuteInEditMode]
    [SaveDuringPlay]
    [AddComponentMenu("")] // Hide in menu
    public class CinemachinePositionLock : CinemachineExtension
    {
        //-- Bools
        [SerializeField] internal bool lockXPos;
        [SerializeField] internal bool lockYPos;
        [SerializeField] internal bool lockZPos;

        //-- Values
        [SerializeField] private float xPos;
        [SerializeField] private float yPos;
        [SerializeField] private float zPos;

        //-- Other Settings
        [SerializeField] private CinemachineCore.Stage stage = CinemachineCore.Stage.Finalize;

        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            // Check if the camera is at the finalize stage
            if (stage == this.stage)
            {
                // Get the raw position
                Vector3 pos = state.RawPosition;

                // Lock the positon according to if they are enabled
                if (lockXPos) pos.x = xPos;
                if (lockYPos) pos.y = yPos;
                if (lockZPos) pos.z = zPos;

                // Set the new raw position
                state.RawPosition = pos;
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CinemachinePositionLock))]
    public class CinemachinePositionLockEditor : Editor
    {
        private CinemachinePositionLock component;

        public override void OnInspectorGUI()
        {
            // Update to make sure we get the latest values
            serializedObject.Update();

            //-- Locked Axes
            EditorGUILayout.LabelField("Locked Axes", EditorStyles.boldLabel);

            Serialize("lockXPos");
            Serialize("lockYPos");
            Serialize("lockZPos");

            //-- Axis values
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Axis values", EditorStyles.boldLabel);

            // Enable the correct axis depending on which we are locking
            if (component.lockXPos) Serialize("xPos");
            if (component.lockYPos) Serialize("yPos");
            if (component.lockZPos) Serialize("zPos");

            //-- Other Settings
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Other Settings", EditorStyles.boldLabel);

            Serialize("stage");

            // Apply the modified properties
            serializedObject.ApplyModifiedProperties();
        }

        private void Serialize(string name)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(name));
        }

        private void OnEnable()
        {
            // Get the cinemachine axis lock component
            component = (CinemachinePositionLock)target;
        }
    }
#endif
}