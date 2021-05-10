using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class IDestroyableComponent : MonoBehaviour, IPoolable
{
    public int score = 10;
    public bool destroyed;
    private GeneratorItems[] spawners;
    private ItemSound ItemSound;
    private IHittable[] hittable;
    private IHighlightableComponent ihighlightableComponent;
    private Item item;
    public ItemPhysicsManger itemPhysicsEditor;
    public int? forbidMoveID;

    public string destroyEffectTag;

    private void Start()
    {
        spawners = GameObject.FindObjectsOfType<GeneratorItems>().ToArray();
        ItemSound = GetComponent<ItemSound>();
        hittable = GetComponents<IHittable>();
        ihighlightableComponent = GetComponent<IHighlightableComponent>();
        item = GetComponent<Item>();
        itemPhysicsEditor = GetComponent<ItemPhysicsManger>();
    }

    public void Hit(Vector2 pos, bool showEffect = true, bool pullItemAbove = true)
    {
        if (!gameObject.activeSelf) return;
        if (destroyed) return;
        if (forbidMoveID == LevelManager.THIS.moveID && LevelManager.THIS.gameStatus == GameState.Playing) return;
        destroyed = true;
        if (ihighlightableComponent != null)
        {
            ihighlightableComponent.SetOutline(false);
            ihighlightableComponent.SetSelected(false);
            if (ihighlightableComponent.selectable && GetComponent<SecretItem>() == null)
                DestroyAround(() => { FinishHit(pos, showEffect, pullItemAbove); });
            else FinishHit(pos, showEffect, pullItemAbove);
        }
    }

    private void FinishHit(Vector2 pos, bool showEffect = true, bool pullItemAbove = true)
    {
        hittable.ForEachY(i => i.Hit(pos, () => { DestroyItem(showEffect, pullItemAbove); }));
    }

    void DestroyItem(bool showEffect = true, bool pullItemAbove = true)
    {
        // GeneratorItems generator = null;
        // if (spawners.Any(i => i.worldRect.Contains(transform.position)))
        // generator = spawners.First(i => i.worldRect.Contains(transform.position));
        foreach (var generator in spawners)
        {
            if (item != null && generator != null && generator.worldRect.Contains(transform.position))
                generator.destroyCounter++;
        }
        if (showEffect)
        {
            EnableEffect("explosion1");
            if (destroyEffectTag != "")
                EnableEffect(destroyEffectTag);
        }
        if (pullItemAbove)
        {
            itemPhysicsEditor?.PullAbove();
        }
        ItemSound.DestroySound();
        gameObject.SetActive(false);
    }

    public void DestroyAround(Action callback = null)
    {
        var hit = Physics2D.OverlapCircleAll(transform.position, 1);
        foreach (var d in hit)
        {
            IDestroyableComponent destroyableComponent = d.GetComponent<IDestroyableComponent>();
            if (destroyableComponent != null && destroyableComponent != this && d.GetComponent<IColorableComponent>() == null)
            {
                destroyableComponent.Hit(transform.position, true, false);
            }
        }
        callback?.Invoke();
    }


    private void EnableEffect(string tagEffect)
    {
        var expl = ObjectPooler.Instance.GetPooledObject(tagEffect, true);
        if (expl)
            expl.transform.position = transform.position;
    }

    public void OnGetFromPool(Vector2 pos)
    {
        destroyed = false;
    }
}

