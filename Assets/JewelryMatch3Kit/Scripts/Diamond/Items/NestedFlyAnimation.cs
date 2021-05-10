using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class NestedFlyAnimation : MonoBehaviour
{
    public AnimationCurve curve;
    private IColorableComponent itemTarget;
    private OrderKeeper layerKeeper;
    private SpriteRenderer[] spriteRenderers;
    private int[] orders;
    public void FlyTo(IColorableComponent item, int index)
    {
        itemTarget = item;
        layerKeeper = new OrderKeeper(gameObject, GetComponent<IHighlightableComponent>().spriteRenderers);
        if (index == 0)
            layerKeeper.SetLayer(SortingLayer.NameToID("Item3"));
        else
            layerKeeper.SetLayer(SortingLayer.NameToID("Level2"));
        Vector2 pos2 = itemTarget.transform.position; //+ (transform.position - itemTarget.transform.position) * 0.2f;


        GetComponent<Rigidbody2D>().simulated = false;
        StartCoroutine(FlyToCoroutine(transform.position, pos2, index));
    }

    IEnumerator FlyToCoroutine(Vector2 pos1, Vector2 pos2, int index)
    {
        // Physics2D.autoSimulation = false;

        float duration = 1f;
        pos1 = transform.position;
        Vector2 saveScale = transform.localScale;
        Vector2 d = pos2 - pos1;
        index = index == 0 ? -1 : index;
        Vector2 posMid = pos1 - d.normalized * index * 2;

        var curveX = new AnimationCurve(new Keyframe(0, transform.localScale.x), new Keyframe(duration, transform.localScale.x * 2f));
        var curveY = new AnimationCurve(new Keyframe(0, transform.localScale.y), new Keyframe(duration, transform.localScale.y * 2f));

        yield return new WaitCurves(new CurveYield[]{
                new WaitCurve(LevelManager.THIS,curveX, curveY, (v) => { transform.localScale = v; transform.Rotate(0, 0, 10); }),
                new WaitNoCurve(LevelManager.THIS,pos1, pos2, duration, (v) => { transform.position = v; })
        });

        GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        var results = Physics2D.CircleCastAll(transform.position, 0.4f, Vector2.zero, 0.4f, 1 << LayerMask.NameToLayer("Item")); //GetComponent<Collider2D>().OverlapCollider(filter, results);
        for (int i = 0; i < results.Length; i++)
        {
            Collider2D item = results[i].collider;
            if (item?.GetComponent<Item>())
                item.gameObject.SetActive(false);
        }
        var curveX1 = new AnimationCurve(new Keyframe(0, transform.localScale.x), new Keyframe(0.5f, saveScale.x / 1.5f), new Keyframe(1, saveScale.x));
        var curveY1 = new AnimationCurve(new Keyframe(0, transform.localScale.y), new Keyframe(0.5f, saveScale.y / 1.5f), new Keyframe(1, saveScale.y));
        yield return new WaitCurve(LevelManager.THIS, curveX1, curveY1, (v) => { transform.localScale = v; }, (v) => v >= 0.5f, () => { FlyStop(); });
        GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<ItemPhysicsManger>().PullAbove();

    }

    void FlyStop()
    {
        layerKeeper.RestoreLayers();
        // var list = Physics2D.CircleCastAll(transform.position, 1, Vector2.zero, 1, 1 << LayerMask.NameToLayer("Item"));
        // foreach (var item in list)
        // {
        //     Rigidbody2D rigidbody2D1 = item.collider.GetComponent<Rigidbody2D>();
        //     if (rigidbody2D1 != null)
        //     {
        //         rigidbody2D1.mass = 10;
        //         rigidbody2D1.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        //     }
        // }

        GetComponent<Rigidbody2D>().simulated = true;
        GetComponent<ItemPhysicsManger>().PullAbove();
    }
}
