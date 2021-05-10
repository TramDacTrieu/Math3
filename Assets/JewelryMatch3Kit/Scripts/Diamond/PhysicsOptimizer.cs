using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsOptimizer : MonoBehaviour
{
    List<ItemPhysicsManger> itemsQueue = new List<ItemPhysicsManger>();
    public static PhysicsOptimizer THIS;
    private void Awake()
    {
        if (THIS != null)
            Destroy(gameObject);
        else
            THIS = this;
    }

    public void WakeItems(IEnumerable<ItemPhysicsManger> items)
    {
        foreach (var item in items)
        {
            itemsQueue.Add(item);
        }

        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        for (int i = 0; i < itemsQueue.Count; i++)
        {
            itemsQueue[i]?.SetSleep(false);
            if (i % 10 == 0) yield return new WaitForSeconds(0.5f);
        }
        // yield return new WaitForSeconds(1);
    }
}