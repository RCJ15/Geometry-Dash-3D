using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Player;

public class FollowPlayer : MonoBehaviour
{
    private Transform player;

    [Header("Follow Axis")]
    [SerializeField] private bool followX;
    [SerializeField] private bool followY;
    [SerializeField] private bool followZ;

    [Header("Offset")]
    [SerializeField] private Vector3 offset;

    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        // Set the start position
        startPos = transform.position;

        // Get player instance
        player = PlayerMain.instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // The new position this object will move to
        Vector3 newPos = startPos + player.transform.position + offset;

        // Decide which axes to keep based on which axes bools are enabled
        newPos = new Vector3(
            followX ? newPos.x : transform.position.x,
            followY ? newPos.y : transform.position.y,
            followZ ? newPos.y : transform.position.z
            );

        // Set the position
        transform.position = newPos;
    }
}
