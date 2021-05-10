using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lock : MonoBehaviour
{
    public bool opened;
    public UnityEvent OnOpen;
    private SpriteRenderer[] SpriteRenderers;

    private void OnEnable()
    {
        opened = false;
    }

    private void Start()
    {
        SpriteRenderers = transform.GetAllSpriteRenderers();
        SpriteRenderers.ForEachY(i => i.sortingLayerName = "Default");
    }

    public void Open()
    {
        opened = true;
        var effect = ObjectPooler.Instance.GetPooledObject("stars_lock");
        effect.transform.position = transform.position;
        var obj = ObjectPooler.Instance.GetPooledObject("diamond_midle");
        obj.GetComponent<IPoolableComponent>().SetupFromPool(transform.position);
        obj.SetActive(true);
        OnOpen?.Invoke();
        StartCoroutine(Falling(() => { gameObject.SetActive(false); }));

    }

    IEnumerator Falling(Action callback)
    {
        SpriteRenderers.ForEachY(i => i.sortingLayerName = "Item3");
        GetComponent<Collider2D>().enabled = false;
        var rb = gameObject.AddComponent<Rigidbody2D>();
        rb.AddRelativeForce(new Vector2(UnityEngine.Random.insideUnitCircle.x * UnityEngine.Random.Range(30, 400), UnityEngine.Random.Range(100, 150)), ForceMode2D.Force);
        float y = Camera.main.GetCameraWorldBottomBorder().y;
        while (y < transform.position.y)
        {
            transform.Rotate(0, 0, 10);
            yield return new WaitForFixedUpdate();
        }
        Destroy(rb);
        callback();
    }
}
