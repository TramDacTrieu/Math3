using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets._2D;

public class Chain : MonoBehaviour
{
    private void Start()
    {
        var box = gameObject.AddComponent<BoxCollider2D>();
        box.size = new Vector2(GetComponent<SpriteRenderer>().size.x, 0.3f);
        box.isTrigger = true;
        var findGameObjectWithTag = GameObject.FindObjectsOfType<Diamond>().Where(i => !i.Collected).Select(i => i.gameObject).ToArray();
        if (findGameObjectWithTag.Length == 0) findGameObjectWithTag = FindObjectsOfType<NestedBig>().Select(i => i.gameObject).Concat(FindObjectsOfType<Nested>().Select(c => c.gameObject).Concat(FindObjectsOfType<KeyHolder>().Select(c => c.gameObject))).ToArray();
        if (transform.position.y > findGameObjectWithTag.First().transform.position.y) Camera2DFollow.direction = -Camera2DFollow.direction;

    }
}
