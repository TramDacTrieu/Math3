using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchBlocker : MonoBehaviour
{
    private bool dragBlocker;

    public delegate void DragUnblocked();
    public static event DragUnblocked OnUnblocked;

    public bool blocked
    {
        get { return dragBlocker; }
        set
        {
            // Debug.Log("drag blocked " + value);
            dragBlocker = value;
            if (dragBlocker) StartCoroutine(Wait());
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        yield return new WaitWhileFall();
        blocked = false;
        OnUnblocked?.Invoke();
    }
}