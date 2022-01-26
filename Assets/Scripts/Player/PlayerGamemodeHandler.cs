using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores which gamemode the player is in and handles when gamemodes are switched
/// </summary>
public class PlayerGamemodeHandler : PlayerScript
{
    public Gamemode currentGamemode;

    public GamemodeScript[] gamemodes;

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

    /// <summary>
    /// Updates the current gamemode and changes it to be <paramref name="newGamemode"/>
    /// </summary>
    public void ChangeGamemode(Gamemode newGamemode)
    {
        currentGamemode = newGamemode;
    }
}

/// <summary>
/// An enum for the very different player gamemodes
/// </summary>
public enum Gamemode
{
    /// <summary>
    /// Jumps
    /// </summary>
    cube = 0,
    /// <summary>
    /// Flies up
    /// </summary>
    ship = 1,
    /// <summary>
    /// Flappy birb
    /// </summary>
    ufo = 2,
    /// <summary>
    /// Changes gravity
    /// </summary>
    ball = 3,
    /// <summary>
    /// Jumps (But different)
    /// </summary>
    robot = 4,
}
