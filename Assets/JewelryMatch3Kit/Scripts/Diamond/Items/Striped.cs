using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using UnityStandardAssets._2D;

public class Striped : MonoBehaviour, IHittable, ISpecial, IExplodable, IPoolable
{
    public GameObject[] Arrows;
    public int itemType;
    public bool typeSet;
    private bool destroyed;
    public GameObject lightObject;
    public GameObject lightPrefab;
    public GameObject stripesEffect;
    public GameObject effect;
    private OrderKeeper layerKeeper;
    private List<IHighlightableComponent> itemsList2;

    private void Awake()
    {
        layerKeeper = new OrderKeeper(gameObject, GetComponent<IHighlightableComponent>().spriteRenderers);

        if (!typeSet)
            SetType(UnityEngine.Random.Range(0, 2));
    }

    public void SetType(int t)
    {
        if (Application.isPlaying)
        {
            layerKeeper.SetLayer(SortingLayer.NameToID("Level2"));
            GetComponent<AnimationItem>()?.BonusAppear(() => { layerKeeper.SetLayer(SortingLayer.NameToID("Default")); });
        }
        typeSet = true;
        itemType = t;
        Arrows.SetActiveAll(false);
        switch (itemType)
        {
            case 0://Horr
                Arrows[0].SetActive(true);
                break;
            case 1://Vertical
                Arrows[1].SetActive(true);
                break;
            case 2://Cross
                Arrows[0].SetActive(true);
                Arrows[1].SetActive(true);
                break;
            default:
                Arrows.SetActiveAll(true);
                break;
        }
    }

    public void OnGetFromPool(Vector2 pos)
    {
        SetType(UnityEngine.Random.Range(0, 2));
        destroyed = false;
        GetComponent<CircleCollider2D>().enabled = true;
    }


    public void Hit(Vector2 pos, Action callback)
    {
        StartCoroutine(HitStriped(pos, callback));
        // callback();
    }

    IEnumerator HitStriped(Vector2 pos, Action callback)
    {
        IHighlightableComponent[] highlightableComponent = GetDestroyItems(pos);
        StrippedShow(pos, itemType);
        var items = highlightableComponent.WhereNotNull().ToArray();//?.Where(i => i?.tag != "multicolor").WhereNotNull().ToArray();
        yield return new WaitForEndOfFrame();
        IOrderedEnumerable<IHighlightableComponent> orderedEnumerable = items.OrderBy(i => i.transform.position.x + i.transform.position.y);
        var min = orderedEnumerable.FirstOrDefault().transform.position - Vector3.one * 4f;
        var max = orderedEnumerable.LastOrDefault().transform.position + Vector3.one * 4f;
        IEnumerable<IHighlightableComponent> forWave = null;
        if (itemType <= 2)
            forWave = Physics2D.OverlapAreaAll(min, max, 1 << LayerMask.NameToLayer("Item")).Select(i => i.GetComponent<IHighlightableComponent>()).WhereNotNull();

        PullItemsAbove(min, max);
        IHighlightableComponent[] animatedItems = forWave?.ToArray();
        yield return new WaitForEndOfFrame();
        GameObject obj = new GameObject();
        obj.AddComponent<StripedEffect>().StartEffect(obj, items, pos, itemType, animatedItems);
        callback();
    }

    private void PullItemsAbove(Vector3 min, Vector3 max)
    {
        Collider2D[] list;
        if (Camera2DFollow.direction > 0)
            list = Physics2D.OverlapAreaAll(min + Vector3.left * 2, max + Vector3.up * 15 + Vector3.right * 2, 1 << LayerMask.NameToLayer("Item"));
        else
            list = Physics2D.OverlapAreaAll(min + Vector3.left * 2, max + Vector3.down * 15 + Vector3.right * 2, 1 << LayerMask.NameToLayer("Item"));
        // PhysicsOptimizer.THIS.WakeItems(list.Select(i => i.GetComponent<ItemPhysicsEditor>()));

        foreach (var item in list)
        {
            item.GetComponent<ItemPhysicsManger>()?.SetSleep(false);
        }

    }

    public void StrippedShow(Vector2 pos, int type)
    {
        // GameObject effect = null;
        if (type == 0)
            GetEffect(pos, 0);
        else if (type == 1)
            GetEffect(pos, 90);
        else if (type == 2)
        {
            GetEffect(pos, 0);
            GetEffect(pos, 90);
        }
        else
        {
            GetEffect(pos, 0);
            GetEffect(pos, 90);
            GetEffect(pos, 45);
            GetEffect(pos, -45);
        }
        // Invoke(effect, 1);
    }

    private GameObject GetEffect(Vector2 pos, float angle)
    {
        GameObject effect = ObjectPooler.Instance.GetPooledObject("striped_effect"); //Instantiate(stripesEffect, pos, Quaternion.identity) as GameObject;
        if (effect == null) return null;
        effect.transform.position = pos;
        effect.transform.rotation = Quaternion.Euler(0, 0, angle);
        return effect;
    }

    public IHighlightableComponent[] GetDestroyItems(Vector3 pos)
    {
        if (itemType == 1)
            return GetColumnRaycast(pos);
        else if (itemType == 0)
            return GetRowRaycast(pos);
        else if (itemType == 2)
            return GetRowRaycast(pos).Concat(GetColumnRaycast(pos)).ToArray();
        else
            return GetRowRaycast(pos).Concat(GetColumnRaycast(pos).Concat(GetLeftDiagonal(pos).Concat(GetRightDiagonal(pos)))).ToArray();
        return null;
    }

    IHighlightableComponent[] GetColumnRaycast(Vector2 pos)
    {
        float rayLengh = 10f;
        var itemsList = Physics2D.CapsuleCastAll(pos, new Vector2(.5f, 25), CapsuleDirection2D.Vertical, 0, Vector2.zero, 1 << LayerMask.NameToLayer("Item"));
        return itemsList.Where(i => i.transform.gameObject != this).Select(i => i.transform.GetComponent<IHighlightableComponent>()).Distinct().ToArray();
    }

    IHighlightableComponent[] GetRowRaycast(Vector2 pos)
    {
        float rayLengh = 10f;
        var itemsList = Physics2D.CapsuleCastAll(pos, new Vector2(25, .5f), CapsuleDirection2D.Horizontal, 90, Vector2.zero, 1 << LayerMask.NameToLayer("Item"));
        return itemsList.Where(i => i.transform.gameObject != this).Select(i => i.transform.GetComponent<IHighlightableComponent>()).Distinct().ToArray();
    }

    IHighlightableComponent[] GetLeftDiagonal(Vector2 pos)
    {
        float rayLengh = 10f;
        var itemsList = Physics2D.CapsuleCastAll(pos, new Vector2(.1f, 25), CapsuleDirection2D.Vertical, 45, Vector2.zero, 1 << LayerMask.NameToLayer("Item"));
        return itemsList.Where(i => i.transform.gameObject != this).Select(i => i.transform.GetComponent<IHighlightableComponent>()).Distinct().ToArray();
    }
    IHighlightableComponent[] GetRightDiagonal(Vector2 pos)
    {
        float rayLengh = 10f;
        var itemsList = Physics2D.CapsuleCastAll(pos, new Vector2(.1f, 25), CapsuleDirection2D.Vertical, -45, Vector2.zero, 1 << LayerMask.NameToLayer("Item"));
        return itemsList.Where(i => i.transform.gameObject != this).Select(i => i.transform.GetComponent<IHighlightableComponent>()).Distinct().ToArray();
    }

}


public class StripedEffect : MonoBehaviour
{
    public void StartEffect(GameObject effect, IHighlightableComponent[] items, Vector2 pos, int itemType, IHighlightableComponent[] animatedItems = null)
    {
        StartCoroutine(StartEffectCor(effect, items, pos, itemType, animatedItems));
    }

    void DestroyEffect()
    {
        Destroy(gameObject);

    }
    IEnumerator StartEffectCor(GameObject effect, IHighlightableComponent[] items, Vector2 pos, int itemType, IHighlightableComponent[] animatedItems = null, Action callback = null)
    {
        ExplosionWaveEffect explosionWaveEffect = ObjectPooler.Instance.GetPooledObject("explosion_wave")?.GetComponent<ExplosionWaveEffect>();
        if (explosionWaveEffect)
        {
            if (itemType <= 2)
                explosionWaveEffect.StartExplosionStriped(items, animatedItems, pos, itemType);
            else
                explosionWaveEffect.StartExplosionRound(items, pos);
        }
        bool showEffect = false;
        var itemsSorted = items.Select(i => new { item = i, dist = Vector2.Distance(i.transform.position, pos) }).OrderBy(i => i.dist).ToArray();
        // Physics2D.autoSimulation = false;
        for (int i = 0; i < itemsSorted.Length; i++)
        {
            var item = itemsSorted[i].item;
            if (!item) continue;
            showEffect = !showEffect;
            item.IDestroyableComponent?.Hit(item.transform.position, showEffect, false);

            if (i % Mathf.Clamp(items.Length / 10, 1, items.Length / 10) == 0)
                yield return new WaitForSeconds(.000002f);
        }
        yield return new WaitForSeconds(.1f);
        // if (callback != null)
        // Physics2D.autoSimulation = true;
        callback?.Invoke();
    }
}