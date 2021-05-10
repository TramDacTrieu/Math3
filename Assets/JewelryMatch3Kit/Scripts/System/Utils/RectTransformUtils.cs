using UnityEngine;

public static class RectTransformUtils
{
    private static Vector3[] rectV;

    public static Rect GetWorldRect(this RectTransform rectTransfrorm)
    {
        rectV = new Vector3[4];
        rectTransfrorm.GetWorldCorners(rectV);
        float height = (rectV[1].y - rectV[0].y);
        float Left = rectV[0].x;
        float Top = rectV[1].y;
        float Bottom = rectV[0].y;
        float width = (int)((rectV[3].x - rectV[0].x));
        Rect rect = new Rect(Left, Bottom, width, height);
        Bounds bounds = new Bounds(rectV[0] + (rectV[2] - rectV[0]) / 2f, rect.size);
        return rect;
    }
}
