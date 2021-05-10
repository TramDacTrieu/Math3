using UnityEngine;

public static class DistanceHandle
{
    public static float GetDistance(Transform item, Transform item2)
    {
        Vector3 pos1 = item2.transform.position + ((item.transform.position - item2.transform.position).normalized * (item2.transform.localScale.magnitude / 2));
        Vector3 pos2 = item.transform.position + ((item2.transform.position - item.transform.position).normalized * (item.transform.localScale.magnitude / 2));
        return Vector2.Distance(pos1, pos2);
    }
}