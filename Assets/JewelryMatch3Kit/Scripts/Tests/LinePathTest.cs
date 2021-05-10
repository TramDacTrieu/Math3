using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[ExecuteInEditMode]
public class LinePathTest : MonoBehaviour
{
    public Transform destItem;
    private IColorableComponent item;
    private IColorableComponent[] list;
    public Transform nearBall;
    public float angle;
    public Vector2 size = new Vector2(1, 10);
    public Vector2 dir = Vector2.up;
    public CapsuleDirection2D capsuleDir;

    public AnimationCurve curve;
    public float inWeight;
    public float inTangent;
    public float outTangent;
    public WeightedMode mode;

    private void OnEnable()
    {
        item = GetComponent<IColorableComponent>();
        list = HighlightManager.GetNearMatches(item).ToArray();
    }

    private void Update()
    {

        var curveX = new AnimationCurve(new Keyframe(0, transform.localScale.x, 0, outTangent), new Keyframe(1, transform.localScale.x * 3, inTangent, outTangent));
        curve = curveX;
        // var itemsList = Physics2D.CapsuleCastAll(transform.position, size, capsuleDir, angle, dir, 1 << LayerMask.NameToLayer("Item"));
        // foreach (var item in itemsList)
        // {
        //     Debug.DrawLine(item.transform.position, item.transform.position + ((item.transform.position + Vector3.down) - item.transform.position).normalized, Color.green);
        // }
        // if (nearBall == null) return;
        // var bounds = transform.GetComponent<Collider2D>().bounds;
        // Vector3 position = transform.position;
        // Vector3 size = bounds.size;

        // Vector3 end = transform.position + ((nearBall.transform.position - transform.position).normalized * (transform.localScale.magnitude / 2));

        // Debug.DrawLine(transform.position, end);
        // list = list.Select(i => i.GetComponent<Transform>()).OrderByDistance().Select(i => i.GetComponent<IColorableComponent>()).ToArray();

        // for (int i = 1; i < list.Length; i++)
        // {
        //     Debug.DrawLine(list[i - 1].transform.position, list[i].transform.position, Color.green);
        // }

    }


}