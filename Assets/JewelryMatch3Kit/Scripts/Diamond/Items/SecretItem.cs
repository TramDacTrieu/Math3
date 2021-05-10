using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretItem : MonoBehaviour, IHittable, ISpecial
{
    private bool destroyed;

    public void Hit(Vector2 pos, Action callback)
    {
        var obj = ObjectPooler.Instance.GetPooledObject("Item");
        obj.GetComponent<IPoolableComponent>().SetupFromPool(transform.position);
        callback();
    }
}
