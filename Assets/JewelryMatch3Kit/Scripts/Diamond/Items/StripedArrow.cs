using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StripedArrow : MonoBehaviour
{
    public float angle;
    Quaternion saveRotation;
    // Use this for initialization
    // void Start()
    // {
    //     saveRotation = transform.rotation;
    // }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
