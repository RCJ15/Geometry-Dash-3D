using UnityEngine;
using Cinemachine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// An add-on module for Cinemachine Virtual Camera that locks the camera's orientation to the given values
/// </summary>
[ExecuteInEditMode]
[SaveDuringPlay]
[AddComponentMenu("")] // Hide in menu
public class CinemachineOrientationLock : CinemachineExtension
{
    //-- Bools
    [SerializeField] internal bool lockXRot;
    [SerializeField] internal bool lockYRot;
    [SerializeField] internal bool lockZRot;

    //-- Values
    [SerializeField] private float xRot;
    [SerializeField] private float yRot;
    [SerializeField] private float zRot;

    //-- Other Settings
    [SerializeField] private CinemachineCore.Stage stage = CinemachineCore.Stage.Finalize;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        // Check if the camera is at the finalize stage
        if (stage == this.stage)
        {
            // Get the raw orientation
            Vector3 rot = state.RawOrientation.eulerAngles;

            // Lock the orientation according to if they are enabled
            if (lockXRot) rot.x = xRot;
            if (lockYRot) rot.y = yRot;
            if (lockZRot) rot.z = zRot;

            // Set the new raw orientation
            state.RawOrientation = Quaternion.Euler(rot);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CinemachineOrientationLock))]
public class CinemachineOrientationLockEditor : Editor
{
    private CinemachineOrientationLock component;

    public override void OnInspectorGUI()
    {
        // Update to make sure we get the latest values
        serializedObject.Update();

        //-- Locked Axes
        EditorGUILayout.LabelField("Locked Axes", EditorStyles.boldLabel);

        Serialize("lockXRot");
        Serialize("lockYRot");
        Serialize("lockZRot");

        //-- Axis values
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Axis Values", EditorStyles.boldLabel);

        // Enable the correct axis depending on which we are locking
        if (component.lockXRot) Serialize("xRot");
        if (component.lockYRot) Serialize("yRot");
        if (component.lockZRot) Serialize("zRot");

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
        component = (CinemachineOrientationLock)target;
    }
}
#endif