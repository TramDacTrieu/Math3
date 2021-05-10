using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NestedBig : MonoBehaviour, IHittable, ISpecial
{
    private bool destroyed;

    public void Hit(Vector2 pos, Action callback)
    {
        int nestedAmount = 2;
        IColorableComponent[] colorableComponent = LevelManager.THIS.GetItemsAround(Camera.main.transform.position, 10);
        IEnumerable<IColorableComponent> enumerable = colorableComponent.Where(i => i != null && Physics2D.OverlapCircleAll(i.transform.position, 2, 1 << LayerMask.NameToLayer("Border")).Any(x => x?.GetType() == typeof(EdgeCollider2D)));
        colorableComponent.Except(enumerable).ToArray();
        var itemTargets = colorableComponent.WhereNotNull().Where(i => i.GetComponent<Item>() != null && i.IDestroyableComponent.destroyed == false && !i.GetComponent<IPoolableComponent>().justPooled).TakeRandom(nestedAmount).ToArray();
        if (Vector2.Distance(itemTargets[0].transform.position, itemTargets[1].transform.position) < 2)
            itemTargets = colorableComponent.WhereNotNull().Where(i => i.GetComponent<Item>() != null && i.IDestroyableComponent.destroyed == false && !i.GetComponent<IPoolableComponent>().justPooled).TakeRandom(nestedAmount).ToArray();
        if (itemTargets.Length > 1)
        {
            float y = Mathf.Min(new float[] { ItemPhysicsManger.LowerDinamicTreshold.y, itemTargets[0].transform.position.y, itemTargets[1].transform.position.y });
            ItemPhysicsManger.LowerDinamicTreshold = new Vector2(0, y);
        }
        for (int i = 0; i < nestedAmount; i++)
        {
            var obj = ObjectPooler.Instance.GetPooledObject("nested");
            obj.GetComponent<IPoolableComponent>().SetupFromPool(transform.position);
            obj.GetComponent<NestedFlyAnimation>().FlyTo(itemTargets[i], i);
        }
        callback();
    }

}
