using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillObjectsAfterTime : MonoBehaviour
{
    public float lifetime;
    public GameObject[] objectsToKill = new GameObject[] { };
    public bool unscaledTime;
    public bool killThisObject;

    void Start()
    {
        StartCoroutine(despawn());
    }

    IEnumerator despawn()
    {
        if (unscaledTime)
        {
            yield return new WaitForSecondsRealtime(lifetime);
        }
        else
        {
            yield return new WaitForSeconds(lifetime);
        }
        foreach (GameObject g in objectsToKill)
        {
            Destroy(g);
        }
        if (killThisObject) { Destroy(gameObject); }
    }
}
