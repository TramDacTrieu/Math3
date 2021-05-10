using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpiralBomb : MonoBehaviour, IHittable, ISpecial, IExplodable, IPoolable
{
    private bool destroyed;
    public GameObject effect;
    public IColorableComponent[] GetDestroyItems(Vector2 pos)
    {
        return GetDestroyItems(pos).Select(i => i.GetComponent<IColorableComponent>()).ToArray();
    }

    public void Hit(Vector2 pos, Action callback)
    {
        DestroyAround(pos);
        callback();
    }

    void DestroyAround(Vector2 pos)
    {
        IHighlightableComponent[] hit = GetDestroyItems((Vector3)pos);
        ObjectPooler.Instance.GetPooledObject("explosion_wave")?.GetComponent<ExplosionWaveEffect>().StartExplosionRound(hit, pos);
        foreach (var item in hit)
        {
            IDestroyableComponent hittable = item?.GetComponent<IDestroyableComponent>();
            if (hittable != null)
                hittable.Hit(item.transform.position);
        }
    }

    public IHighlightableComponent[] GetDestroyItems(Vector3 pos)
    {
        return Physics2D.OverlapCircleAll(pos, 1f).Where(i => i.gameObject != gameObject).Where(i => i.GetComponent<Multicolor>() == null).Select(i => i.GetComponent<IHighlightableComponent>()).Distinct().ToArray();
    }

    public void OnGetFromPool(Vector2 pos)
    {
        destroyed = false;
    }

}
