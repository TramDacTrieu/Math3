using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondAnimation : MonoBehaviour
{
    public Animator anim;

    // Update is called once per frame
    void Update()
    {
        if (Random.value > 0.99f) anim.SetTrigger("blink");
    }
}
