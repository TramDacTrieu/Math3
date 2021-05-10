using System;
using System.Collections;
using System.Collections.Generic;
using JewelryMatch3Kit.Scripts.Diamond;
using UnityEngine;

public class FlowerItem : MonoBehaviour, IHittable
{
    public void Hit(Vector2 pos, Action callback)
    {
        var obj = ObjectPooler.Instance.GetPooledObject("flower");
        obj.SetActive(true);
        obj.GetComponent<FlowerFly>().Launch(transform.position);
        callback();
    }
}
