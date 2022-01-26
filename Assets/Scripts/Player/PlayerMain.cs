using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script contains a reference to all the other player scripts and acts as a communicator between them. <para/>
/// Also has crucial player methods like Win() and Die()
/// </summary>
public class PlayerMain : PlayerScript
{
    internal PlayerMovement movement;
    internal PlayerColors colors;
    internal PlayerMesh mesh;
    internal PlayerGamemodeHandler gamemode;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    public override void Start()
    {
        base.Start();

        GetPlayerScripts();
    }

    private void GetPlayerScripts()
    {
        movement = GetChildComponent<PlayerMovement>();
        colors = GetChildComponent<PlayerColors>();
        mesh = GetChildComponent<PlayerMesh>();
        gamemode = GetChildComponent<PlayerGamemodeHandler>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    public override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Win
    /// </summary>
    public void Win()
    {

    }

    /// <summary>
    /// Die
    /// </summary>
    public void Die()
    {

    }
}
