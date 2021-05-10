using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyHolder : MonoBehaviour, IHittable, ISpecial
{
    public GameObject key;
    public GameObject lightObject;
    public GameObject lightPrefab;

    private void Start()
    {
        if (key == null)
        {
            key = ObjectPooler.Instance.GetPooledObject("key");
            key.SetActive(true);
            key.transform.position = transform.position;
            key.transform.parent = transform;
        }
    }

    public void Hit(Vector2 pos, Action callback)
    {
        key.GetComponent<Key>().StartKey();
        key.transform.parent = null;
        callback();
    }


}
