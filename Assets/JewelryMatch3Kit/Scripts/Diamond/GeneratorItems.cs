using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneratorItems : MonoBehaviour
{
    public static GeneratorItems This;
    private PolygonCollider2D levelCollider;
    public int destroyCounter;
    public GameObject itemPrefab;
    BoxCollider2D boxcollider;
    private RectTransform rectTransform;
    public Rect worldRect;

    public float[] x_limit = new float[2];

    public float fracAmount = 1;

    private void Awake()
    {
        This = this;
        // levelCollider = GameObject.FindObjectOfType<LevelSettings>().GetComponent<PolygonCollider2D>();
        boxcollider = gameObject.AddComponent<BoxCollider2D>();
        boxcollider.isTrigger = true;
        rectTransform = GetComponent<RectTransform>();
        worldRect = rectTransform.GetWorldRect();
        boxcollider.size = rectTransform.rect.size;
        boxcollider.offset = rectTransform.rect.center;

        var itemsList = Physics2D.RaycastAll(transform.position, Vector2.left, 1 << LayerMask.NameToLayer("Border")).Where(i => i.collider.GetType() == typeof(EdgeCollider2D));
        var itemsList1 = Physics2D.RaycastAll(transform.position, Vector2.right, 1 << LayerMask.NameToLayer("Border")).Where(i => i.collider.GetType() == typeof(EdgeCollider2D));
        itemsList = itemsList.Concat(itemsList1);
        x_limit[0] = itemsList.Min(i => i.point.x) + 0.5f;
        x_limit[1] = itemsList.Max(i => i.point.x) - 0.5f;


    }

    // private void OnEnable()
    // {
    //     LevelManager.OnNextMove += GenerateItems;
    // }

    // private void OnDisable()
    // {
    //     LevelManager.OnNextMove -= GenerateItems;
    // }
    public void GenerateItems(int count)
    {
        if (count > 0)
        {
            int v = destroyCounter / count;
            if (v > 0)
            {
                if (LevelManager.THIS.gameStatus == GameState.Playing)
                    StartCoroutine(GenerateCoroutine(v));
            }
        }
        else if (destroyCounter > 0)
        {
            if (LevelManager.THIS.gameStatus == GameState.Playing)
                StartCoroutine(GenerateCoroutine(destroyCounter));
        }
    }

    IEnumerator GenerateCoroutine(int count)
    {
        int value = 50 - VisibilityListener.visibleItems;
        count = Mathf.Clamp(count, 0, Mathf.Clamp(value, 0, 20));
        count = Mathf.RoundToInt((float)count * fracAmount);
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(.2f);
            GenItem<Item>("Item");
            destroyCounter--;
        }
        if (destroyCounter > 0)
        {
            int restItems = LevelSettings.THIS.LevelScriptable.Collect - LevelManager.THIS.TargetDiamonds;
            restItems -= GetCollected(new Type[] { typeof(Diamond), typeof(NestedBig), typeof(NestedBig), typeof(Nested), typeof(NestedSmall), typeof(Lock) });
            if (restItems > 0) CreateNested();
        }
        // destroyCounter = 0;
    }

    private void CreateNested()
    {
        GenItem<Nested>("nested");
    }

    int GetCollected(Type[] types)
    {
        int collect = 0;
        foreach (Type item in types)
        {
            collect += GameObject.FindObjectsOfType(item).Count();
        }
        return collect;
    }

    public T GenItem<T>(string tag)
    {
        GameObject item = ObjectPooler.Instance.GetPooledObject(tag);
        if (item == null) { Debug.LogError("Make sure you have added " + tag + " to the pool OR marked it for editor"); }


        Vector2 pos = rectTransform.anchoredPosition + new Vector2(UnityEngine.Random.insideUnitCircle.x * 3, 0);
        pos = new Vector2(Mathf.Clamp(pos.x, x_limit[0], x_limit[1]), pos.y);
        item.GetComponent<IPoolableComponent>().SetupFromPool(pos);
        item.GetComponentsInChildren<IColorableComponent>().ForEachY(i => i.RandomColorOnAwake = false);
        return item.GetComponent<T>();
    }

}
