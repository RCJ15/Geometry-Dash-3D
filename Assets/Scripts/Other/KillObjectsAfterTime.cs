using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillObjectsAfterTime : MonoBehaviour
{
    public float lifetime;
    public GameObject[] objectsToKill = new GameObject[] { };
    public bool unscaledTime;
    public bool killThisObject = true;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        StartCoroutine(Despawn());
    }

    /// <summary>
    /// Starts a timer that'll destroy objects when it runs out
    /// </summary>
    private IEnumerator Despawn()
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

        if (killThisObject)
        {
            Destroy(gameObject);
        }
    }
}
