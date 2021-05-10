using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestructor : MonoBehaviour
{

    public float SecondsToDestroy;

    private void OnEnable()
    {
        if (SecondsToDestroy > 0)
            Invoke("Destroy", SecondsToDestroy);
    }

    public void Destroy()
    {
        // Destroy(gameObject);
        // SecondsToDestroy = 0;
        gameObject.SetActive(false);
    }
}
