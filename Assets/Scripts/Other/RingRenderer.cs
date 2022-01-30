using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingRenderer : MonoBehaviour
{
    [SerializeField] private int points = 100;
    [SerializeField] private float ringSize = 1;

    [SerializeField] private UpdateMode updateMode;

    private LineRenderer lr;

    /// <summary>
    /// Awake is called when the script instance is being loaded
    /// </summary>
    void Awake()
    {
        // Get line renderer
        lr = GetComponent<LineRenderer>();

        UpdateLines();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        if (updateMode == UpdateMode.everyFrame)
        {
            UpdateLines();
        }
    }

    /// <summary>
    /// Fixed Update is called once per physics frame
    /// </summary>
    private void FixedUpdate()
    {
        if (updateMode == UpdateMode.everyFixedFrame)
        {
            UpdateLines();
        }
    }

    /// <summary>
    /// Updates the line renderers lines to form a ring
    /// </summary>
    private void UpdateLines()
    {
        // Set no ring if there are no points or the size is 0
        if (points <= 0 || ringSize <= 0)
        {
            lr.positionCount = 0;
            return;
        }

        // Create list of Vector3
        List<Vector3> poses = new List<Vector3>();

        // Loop for the amount of points needed
        for (int i = 0; i < points; i++)
        {
            // Add the position to the list
            poses.Add(transform.position + GetPos(i));
        }

        // Add the first pos
        poses.Add(transform.position + GetPos(0));

        // Set the line renderers positions
        lr.positionCount = poses.Count;

        lr.SetPositions(poses.ToArray());
    }

    /// <summary>
    /// Returns the position of the ring at the given <paramref name="index"/>
    /// </summary>
    private Vector3 GetPos(int index)
    {
        // Calculate the angle
        float angle = MathE.Map(0, points, 0, 360, index);

        // Convert the angle to a normal
        Vector2 dir = MathE.AngleToNormal(angle);

        // Return the position
        return dir * ringSize;
    }

    /// <summary>
    /// Enum for what update mode a ring renderer can have
    /// </summary>
    [System.Serializable]
    public enum UpdateMode
    {
        onAwake = 0,
        everyFrame = 1,
        everyFixedFrame = 2,
    }
}
