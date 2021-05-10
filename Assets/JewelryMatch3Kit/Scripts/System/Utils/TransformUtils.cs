using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TransformUtils
{
    public static Transform[] FindAllChildrenIncludingInactive(this Transform thisTransform)
    {
        List<Transform> trList = new List<Transform>();
        trList.Add(thisTransform);
        for (int i = 0; i < thisTransform.childCount; i++)
        {
            Transform newTransform = thisTransform.GetChild(i);
            trList.AddRange(FindAllChildrenIncludingInactive(newTransform));
        }

        return trList.ToArray();
    }

    public static SpriteRenderer[] GetAllSpriteRenderers(this Transform thisTransform)
    {
        var list = thisTransform.FindAllChildrenIncludingInactive();
        return list.Select(i => i.GetComponent<SpriteRenderer>()).WhereNotNull().ToArray();
    }

}