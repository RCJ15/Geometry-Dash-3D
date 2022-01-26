using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class all player scripts inherit from
/// </summary>
public class PlayerScript : MonoBehaviour
{
    //-- Component references
    internal Rigidbody rb;
    internal MeshRenderer mr;
    internal BoxCollider boxCol;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    public virtual void Start()
    {
        GetComponents();


    }

    /// <summary>
    /// Gets components
    /// </summary>
    private void GetComponents()
    {
        rb = GetChildComponent<Rigidbody>();
        mr = GetChildComponent<MeshRenderer>();
        boxCol = GetChildComponent<BoxCollider>();
    }

    /// <summary>
    /// Same as GetComponent<>(), but if the method return null, GetComponentInChildren() is used instead
    /// </summary>
    public T GetChildComponent<T>()
    {
        // Get component regularly
        T component = GetComponent<T>();

        // If it's null, get component in children
        if (component == null)
        {
            component = GetComponentInChildren<T>();
        }

        return component;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    public virtual void Update()
    {
        
    }
}

// ############
// # Template #
// ############
/*
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class INSERTNAME : PlayerScript
{
    
    
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
 */
