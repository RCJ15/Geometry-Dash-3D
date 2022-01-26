using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the players constant movement and detects when the player is on ground or not.
/// </summary>
public class PlayerMovement : PlayerScript
{
    [Header("Stats")]
    [SerializeField] private float moveSpeed;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    public override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    public override void Update()
    {
        base.Update();
    }
}
