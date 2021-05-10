using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Nested : MonoBehaviour, IHittable, ISpecial, IPoolable
{
    private bool destroyed;
    private NestedColor[] childObjects;
    private Animator animator;

    public ParticleSystem[] effectPrefabs;

    private void Awake()
    {
        childObjects = GetComponentsInChildren<NestedColor>();

        animator = GetComponent<Animator>();
    }

    public void Hit(Vector2 pos, Action callback)
    {

        animator.SetTrigger("nested_open");
        // StartCoroutine(Wait(callback));

        // obj.GetComponent<NestedFlyAnimation>().FlyTo();
        // obj.GetComponent<ICollectable>().Collect();
    }

    public void OnOpenStarted()
    {
        Instantiate(effectPrefabs[0], transform.position, Quaternion.identity);
        foreach (var item in childObjects)
            item.GetComponent<SpriteRenderer>().sortingLayerName = "Item3";
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;
        var obj = ObjectPooler.Instance.GetPooledObject("diamond_midle");
        obj.GetComponent<Rigidbody2D>().simulated = false;
        obj.GetComponent<IPoolableComponent>().SetupFromPool(transform.position);
        obj.GetComponent<Diamond>().PrepareFinishAnim();
        obj.GetComponent<Diamond>().DelayedCollect(1f);
    }

    public void OnOpenFinished()
    {
        foreach (var item in childObjects)
            StartCoroutine(TimeCoroutine(item.GetComponent<SpriteRenderer>(), DestroyGameObject));
    }

    IEnumerator TimeCoroutine(SpriteRenderer SpriteRenderer, Action callback = null)
    {
        float startTime = Time.time;
        float value = 1;
        float duration = 1;
        float fps = 10;
        float step = value / (duration * fps);
        while (Time.time - startTime <= duration)
        {
            SpriteRenderer.color = new Color(1, 1, 1, value -= step);
            yield return new WaitForEndOfFrame();
        }
        callback?.Invoke();
    }

    IEnumerator Wait(Action callback)
    {
        yield return new WaitForSeconds(0.5f);
        // animator.enabled = false;
        foreach (var item in childObjects)
        {
            StartCoroutine(TimeCoroutine(item.GetComponent<SpriteRenderer>(), DestroyGameObject));

            // var falling = item.gameObject.AddComponent<FallingAction>();
            // falling.callback = DestroyGameObject;
        }
    }

    void DestroyGameObject()
    {
        Destroy(gameObject);
    }

    public void OnGetFromPool(Vector2 pos)
    {
        animator?.Rebind();
    }
}
