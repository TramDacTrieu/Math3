using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPoolableComponent : MonoBehaviour
{
    int layerID;
    private PolygonCollider2D levelCollider;
    Collider2D coll;
    public bool justPooled;

    private void Awake()
    {
        levelCollider = GameObject.FindObjectOfType<LevelSettings>().GetComponent<PolygonCollider2D>();
        coll = GetComponent<Collider2D>();
        justPooled = false;
    }
    public void SetupFromPool(Vector2 pos)
    {
        layerID = gameObject.layer;
        // if (Application.isPlaying)
        // gameObject.layer = 0;
        transform.position = pos;
        GetComponent<Collider2D>().enabled = true;
        Rigidbody2D rigidbody2D1 = GetComponent<Rigidbody2D>();
        if (rigidbody2D1 != null)
        {
            rigidbody2D1.bodyType = RigidbodyType2D.Dynamic;
            rigidbody2D1.simulated = true;
        }
        GetComponents<IPoolable>()?.ForEachY(i => i.OnGetFromPool(pos));
        if (Application.isPlaying)
        {
            if (!IsInsideLevel())
            {
                StartCoroutine(WaitForCollider());
            }
            else
                gameObject.layer = layerID;

            IDestroyableComponent destroyableComponent = GetComponent<IDestroyableComponent>();
            if (destroyableComponent != null)
                destroyableComponent.forbidMoveID = LevelManager.THIS.moveID;
        }
        justPooled = true;
        Invoke("Wait", 2);

    }

    void Wait()
    {
        justPooled = false;
    }

    private bool IsInsideLevel()
    {
        return levelCollider.OverlapPoint(transform.position);
    }

    private IEnumerator WaitForCollider()
    {
        while (!IsInsideLevel())
        {
            yield return new WaitForSeconds(0.3f);
        }
        gameObject.layer = layerID;

    }
}
