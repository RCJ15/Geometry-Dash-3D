using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes the object follow the camera in a stepped motion to allow for looping a object of a certain size.
/// </summary>
public class BackgroundFollowCamera : MonoBehaviour
{
    [Header("Main Settings")]
    public Vector3 roundValue = new Vector3(16, 16, 16);

    public bool followX = true;
    public bool followY = true;
    public bool followZ = true;

    public bool useLocalScale = true;

    [Header("General Offset")]
    public Vector3 offset;

    [Header("Random Offset")]
    public bool haveRandomXOffset = false;
    private float randomXOffset;

    public bool haveRandomYOffset = false;
    private float randomYOffset;

    public bool haveRandomZOffset = false;
    private float randomZOffset;

    [Header("Generate Background")]
    public GameObject backgroundObjectToCopy;
    public Vector3Int amount = new Vector3Int(1, 1, 1);

    private Transform cam;
    private Vector3 startPos;
    private Vector3 pieceOffset;

    // Start is called before the first frame update
    void Start()
    {
        // Set the start position
        startPos = transform.position;

        // Get the camera
        cam = Camera.main.transform;

        // Generate a random X offset if it should have a random X offset
        if (haveRandomXOffset)
        {
            randomXOffset = Random.Range(-roundValue.x, roundValue.x);
        }

        // Generate a random Y offset if it should have a random Y offset
        if (haveRandomYOffset)
        {
            randomYOffset = Random.Range(-roundValue.y, roundValue.y);
        }

        // Generate a random Z offset if it should have a random Z offset
        if (haveRandomZOffset)
        {
            randomZOffset = Random.Range(-roundValue.z, roundValue.z);
        }

        // If there is no object to copy or the amount of objects to copy is below or equal to 0, then return
        if (backgroundObjectToCopy == null || (amount.x <= 0 && amount.y <= 0 && amount.z <= 0))
        {
            return;
        }

        int amountSpawned = 1;

        // Set the pieceOffset
        pieceOffset = backgroundObjectToCopy.transform.localPosition;

        // Loop for the amount of objects in the X
        for (int x = 1; x < amount.x + 1; x++)
        {
            // Loop for the amount of objects in the Y
            for (int y = 0; y < amount.y; y++)
            {
                //Loop for the amount of objects in the Z
                for (int z = 0; z < amount.z; z++)
                {
                    // Set the X pos
                    float xPos = amount.x <= 1 ? // Check if the amount of objects in the X are less or equal to 1
                                                 // If true:
                        backgroundObjectToCopy.transform.localPosition.x // Defaults to the local X position
                        : // Else
                        (x * roundValue.x) - ((amount.x * roundValue.x) / 2); // Calculation for getting the middle pos for X

                    // Set the Y pos
                    float yPos = amount.y <= 1 ? // Check if the amount of objects in the Y are less or equal to 1
                                                 // If true:
                        backgroundObjectToCopy.transform.localPosition.y // Defaults to the local Y position
                        : // Else:
                        (y * roundValue.y) - ((amount.y * roundValue.y) / 2); // Calculation for getting the middle pos for Y

                    // Set the Z pos
                    float zPos = amount.z <= 1 ? // Check if the amount of objects in the Z are less or equal to 1
                                                 // If true:
                        backgroundObjectToCopy.transform.localPosition.z // Defaults to the local Z position
                        : // Else:
                        (z * roundValue.z) - ((amount.z * roundValue.z) / 2); // Calculation for getting the middle pos for Z

                    GameObject newObj = Instantiate(backgroundObjectToCopy, Vector3.zero, backgroundObjectToCopy.transform.rotation, transform);
                    newObj.transform.SetParent(transform);
                    newObj.transform.localPosition = new Vector3(xPos, yPos, zPos) + pieceOffset + offset;
                    newObj.name = backgroundObjectToCopy.name + " (" + amountSpawned + ")";

                    // Increment amount spawned by 1
                    amountSpawned++;
                }
            }
        }

        // Destroy the orignal object cuz it's useless now
        Destroy(backgroundObjectToCopy);
    }

    // Update is called once per frame
    void Update()
    {
        // Set the position to follow the camera (with steps)
        transform.position = new Vector3(
            // X pos
            followX ?
            (Mathf.Round(cam.position.x / (useLocalScale ? transform.localScale.x : 1) / roundValue.x) // Round it after decreasing the value
            * roundValue.x * (useLocalScale ? transform.localScale.x : 1)) + randomXOffset // Make the value big again
            :
            startPos.x, // Otherwise if it's not supposed to follow the X then set it to just be at it's start X

            // Y pos
            followY ?
            (Mathf.Round(cam.position.y / (useLocalScale ? transform.localScale.y : 1) / roundValue.y) // Round it after decreasing the value
            * roundValue.y * (useLocalScale ? transform.localScale.y : 1)) + randomYOffset // Make the value big again
            :
            startPos.y, // Otherwise if it's not supposed to follow the Y then set it to just be at it's start Y

            // Z pos
            followZ ?
            (Mathf.Round(cam.position.z / (useLocalScale ? transform.localScale.z : 1) / roundValue.z) // Round it after decreasing the value
            * roundValue.z * (useLocalScale ? transform.localScale.z : 1)) + randomZOffset // Make the value big again
            :
            startPos.z) // Otherwise if it's not supposed to follow the Z then set it to just be at it's start Z

            // Add the offset
            + offset;
    }
}
