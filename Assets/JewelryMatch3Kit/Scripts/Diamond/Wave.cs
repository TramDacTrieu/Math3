using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Wave : MonoBehaviour
{
    public int textureX;

    // Use this for initialization
    void Start()
    {
        textureX = 1; // (how many world units one tile of your texture should use
        Material material = GetComponent<SpriteRenderer>().material;
        material.SetFloat("RepeatX", transform.localScale.x / textureX);
    }

    // Update is called once per frame
    void Update()
    {
        Material material = GetComponent<SpriteRenderer>().material;

        material.SetFloat("RepeatX", transform.localScale.x / textureX);

    }
}
