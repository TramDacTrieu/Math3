using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

public class ExplosionWaveEffect : MonoBehaviour
{
    public Transform target;
    Transform[][] secondRow;
    Transform[] thirdRow;
    public int rows = 2;
    public float force = 0.2f;
    public float duration = 1;

    public float delayBetweenWaves = 0.1f;
    public float attenuation = 0.1f;
    public GameObject[] effectsPrefabs;
    public GameObject[] effects;
    public List<IHighlightableComponent> items_;

    private void Awake()
    {
        effects = new GameObject[effectsPrefabs.Length];
        for (int i = 0; i < effectsPrefabs.Length; i++)
        {
            effects[i] = Instantiate(effectsPrefabs[i], transform.position, Quaternion.identity, transform);
            effects[i].SetActive(false);
        }
    }
    private void OnEnable()
    {
        // StartExplosion(target.GetComponent<IExplodable>().GetDestroyItems(target.transform.position) , target.transform.position);
    }

    public void StartExplosionStriped(IHighlightableComponent[] items, IHighlightableComponent[] animatedItems, Vector3 pos, int itemType)
    {
        IHighlightableComponent firstItem = items[0];
        var itemArray = animatedItems.WhereNotNull().Where(i => i.gameObject.activeSelf);//secondRow[i];

        foreach (var item in itemArray)
        {
            Vector3 pos1 = item.transform.position;
            pos1 = item.transform.InverseTransformPoint(pos1);
            Vector3 targetVector = new Vector3(item.transform.position.x, pos.y, 0);
            if (itemType == 1)
                targetVector = new Vector3(pos.x, item.transform.position.y, 0);
            float dist1 = Vector2.Distance(targetVector, item.transform.position);
            var pos2 = item.transform.position + (item.transform.position - targetVector) * force * (1f / dist1); // / ((0 + dist) * attenuation);
            float dist2 = 0;
            if (itemType > 1)
            {
                Vector3 targetVector2 = new Vector3(pos.x, item.transform.position.y, 0);
                dist2 = Vector2.Distance(targetVector2, item.transform.position);
                if (dist2 < dist1)
                    pos2 = item.transform.position + (item.transform.position - targetVector2) * force * (1f / dist2); // / ((0 + dist) * attenuation);

                // pos2 = item.transform.position + ((item.transform.position - targetVector) * (1f / dist1) + (item.transform.position - targetVector2) * (1f / dist2));
            }
            pos2 = item.transform.InverseTransformPoint(pos2);
            if (float.IsNaN(pos2.x)) pos2.x = 0;
            if (float.IsNaN(pos2.y)) pos2.y = 0;

            var move = item.moveWave;
            move.StartMove(pos1, pos2, Mathf.FloorToInt(Mathf.Min(dist1, dist2) / 2), delayBetweenWaves, duration);
        }

        ActivateEffects(pos);
        Invoke("Hide", duration * 2);
    }

    private Vector2 GetTargetPos(IHighlightableComponent item, Vector3 targetVector, out float dist)
    {
        dist = Vector2.Distance(targetVector, item.transform.position);
        var pos2 = item.transform.position + (item.transform.position - targetVector) * force * (1f / dist); // / ((0 + dist) * attenuation);
        pos2 = item.transform.InverseTransformPoint(pos2);
        if (float.IsNaN(pos2.x)) pos2.x = 0;
        if (float.IsNaN(pos2.y)) pos2.y = 0;
        return pos2;
    }

    private void ActivateEffects(Vector3 pos)
    {
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i].transform.position = pos;
            effects[i].SetActive(true);
        }
    }

    public void StartExplosionRound(IEnumerable<IHighlightableComponent> items, Vector3 pos)
    {
        items_ = items.ToList();
        secondRow = new Transform[rows][];
        for (int i = 0; i < rows; i++)
        {
            secondRow[i] = GetRow(pos, i + 2).Distinct().Except(items.Select(x => x?.transform)).ToArray();
            if (i > 0)
                secondRow[i] = secondRow[i].Except(secondRow[i - 1]).ToArray();
        }

        for (int i = 0; i < rows; i++)
        {
            var itemArray = secondRow[i];
            foreach (var item in itemArray)
            {
                Vector3 pos2 = item.position + (item.position - pos) * force / ((i + 1) * attenuation);
                Vector3 pos1 = item.position;
                pos1 = item.transform.InverseTransformPoint(pos1);
                pos2 = item.transform.InverseTransformPoint(pos2);
                // StartCoroutine(FlyToCoroutine(item.GetChild(0), pos1, pos2, i));
                var move = item.gameObject.GetComponent<MoveWave>();
                move?.StartMove(pos1, pos2, i, delayBetweenWaves, duration);
            }
        }
        ActivateEffects(pos);
        Invoke("Hide", duration * 2);

    }

    void Hide()
    {
        gameObject.SetActive(false);
    }

    private Transform[] GetRow(Vector2 pos, float radius)
    {
        List<Transform> list = new List<Transform>();
        // foreach (var item in items.WhereNotNull())
        {
            Collider2D[] collider2D1 = Physics2D.OverlapCircleAll(pos, radius, 1 << LayerMask.NameToLayer("Item"))./* Where(i => !IsTouchTheBorder(i.transform.position)). */ToArray();
            list.AddRange(collider2D1.Select(i => i.transform));
        }
        if (list.Count == 0) return list.ToArray();
        return list.ToArray();
    }
    private Dictionary<Transform, Vector3> GetRowStriped(IEnumerable<IHighlightableComponent> items, float radius)
    {
        Dictionary<Transform, Vector3> list = new Dictionary<Transform, Vector3>();
        IOrderedEnumerable<IHighlightableComponent> orderedEnumerable = items.OrderBy(i => i.transform.position.x + i.transform.position.y);
        var min = orderedEnumerable.FirstOrDefault().transform.position - Vector3.one * 1f;
        var max = orderedEnumerable.LastOrDefault().transform.position + Vector3.one * 1f;
        Vector3 size = max - min;
        Debug.DrawLine(min, max);
        // Debug.Break();
        Vector2 center = min + size * 0.5f;
        var raycastHit2D = Physics2D.OverlapAreaAll(min, max, 1 << LayerMask.NameToLayer("Item"));
        var array = raycastHit2D.Select(i => i.transform).Except(items.Select(i => i.transform));
        // foreach (var item in items.WhereNotNull())
        {
            // var collider2D2 = Physics2D.BoxCastAll(item.transform.position - Vector3.one * 2, Vector2.one * 2, 0, Vector2.zero, 4, 1 << LayerMask.NameToLayer("Item")); //Physics2D.OverlapCircleAll(item.transform.position, radius, 1 << LayerMask.NameToLayer("Item"));
            // var collider2D1 = collider2D2 /* && !IsTouchTheBorder(i.transform.position) */.Select(i => new { itemTransform = i.transform, mainPos = item.transform.position });
            foreach (var item in array)
            {
                Vector2 pos = Vector2.zero;
                if (!list.ContainsKey(item))
                {
                    if (size.x > size.y)
                    {
                        if (item.transform.position.y > center.y)
                            pos = item.transform.position + Vector3.down;
                        else
                            pos = item.transform.position + Vector3.up;
                    }
                    else if (size.x <= size.y)
                    {
                        if (item.transform.position.x > center.x)
                            pos = item.transform.position + Vector3.left;
                        else
                            pos = item.transform.position + Vector3.right;
                    }
                }
                list.Add(item, pos);
            }
        }
        if (list.Count == 0) return list;
        return list.ToDictionary(i => i.Key, i => i.Value);
    }

    bool IsTouchTheBorder(Vector2 pos)
    {
        return Physics2D.OverlapCircleAll(pos, 1, 1 << LayerMask.NameToLayer("Border")).Any(x => x?.GetType() == typeof(EdgeCollider2D));

    }

    IEnumerator FlyToCoroutine(Transform obj, Vector2 pos1, Vector2 pos2, int row)
    {
        yield return new WaitForSeconds(delayBetweenWaves * row);
        var startTime = Time.time;
        var journeyLength = Vector3.Distance(pos1, pos2);
        float speed = journeyLength / duration;
        float fracJourney = 0;
        bool checkParent = false;
        var curveX = new AnimationCurve(new Keyframe(0, pos1.x), new Keyframe(0.5f, pos2.x), new Keyframe(1, pos1.x));
        var curveY = new AnimationCurve(new Keyframe(0, pos1.y), new Keyframe(0.5f, pos2.y), new Keyframe(1, pos1.y));
        while (fracJourney <= 1)
        {
            if (fracJourney >= 0.5f && !checkParent)
            {
                checkParent = true;
                if (obj.parent.GetComponent<IDestroyableComponent>()?.destroyed ?? false)
                {
                    obj.localPosition = pos1;
                    yield break;
                }
            }
            float distCovered = (Time.time - startTime) * speed;
            fracJourney = distCovered / journeyLength;
            if (!float.IsNaN(curveX.Evaluate(fracJourney)) && !float.IsNaN(curveY.Evaluate(fracJourney)))
                obj.localPosition = new Vector2(curveX.Evaluate(fracJourney), curveY.Evaluate(fracJourney));
            yield return new WaitForEndOfFrame();
        }
        obj.localPosition = pos1;
    }

}

public class MoveWave : MonoBehaviour
{
    private IHighlightableComponent iHighlightableComponent;
    public bool animationGoing;

    private void Awake()
    {
        iHighlightableComponent = GetComponent<IHighlightableComponent>();

    }
    public void StartMove(Vector2 pos1, Vector2 pos2, int row, float delayBetweenWaves, float duration)
    {
        StartCoroutine(FlyToCoroutine(pos1, pos2, row, delayBetweenWaves, duration));

    }

    IEnumerator FlyToCoroutine(Vector2 pos1, Vector2 pos2, int row, float delayBetweenWaves, float duration)
    {
        if (transform.childCount == 0) yield break;
        if (iHighlightableComponent == null) yield break;
        animationGoing = true;
        // iHighlightableComponent.IDestroyableComponent.itemPhysicsEditor.SetSleepForced(true);
        Transform obj = transform.GetChild(0);
        var layerKeeper = iHighlightableComponent.layerKeeper;
        layerKeeper.SetLayer(SortingLayer.NameToID("Item1"));
        yield return new WaitForSeconds(delayBetweenWaves * row);
        var startTime = Time.time;
        var journeyLength = Vector3.Distance(pos1, pos2);
        float speed = journeyLength / duration;
        float fracJourney = 0;
        bool checkParent = false;
        var curveX = new AnimationCurve(new Keyframe(0, pos1.x), new Keyframe(0.5f, pos2.x), new Keyframe(1, pos1.x));
        var curveY = new AnimationCurve(new Keyframe(0, pos1.y), new Keyframe(0.5f, pos2.y), new Keyframe(1, pos1.y));
        if (pos1.x == pos2.x || pos1.y == pos2.y)
        {
            AnimFinished();
            yield break;
        }
        while (fracJourney <= 1)
        {
            float distCovered = (Time.time - startTime) * speed;
            fracJourney = distCovered / journeyLength;
            if (!float.IsNaN(curveX.Evaluate(fracJourney)) && !float.IsNaN(curveY.Evaluate(fracJourney)))
                obj.localPosition = new Vector2(curveX.Evaluate(fracJourney), curveY.Evaluate(fracJourney));
            yield return new WaitForEndOfFrame();
        }
        layerKeeper.RestoreLayers();
        // iHighlightableComponent.IDestroyableComponent.itemPhysicsEditor.SetSleepForced(false);

        // GetComponent<ItemPhysicsEditor>().PullAbove();
        obj.localPosition = pos1;
        AnimFinished();
        // Destroy(this);

    }

    void AnimFinished()
    {
        animationGoing = false;
    }
}