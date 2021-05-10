using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NestedSmall : MonoBehaviour, IHittable, ISpecial
{
    private bool destroyed;

    public void Hit(Vector2 pos, Action callback)
    {
        var obj = ObjectPooler.Instance.GetPooledObject("diamond");
        obj.GetComponent<IPoolableComponent>().SetupFromPool(transform.position);
        obj.GetComponent<ICollectable>().Collect();
        callback();
    }
}
