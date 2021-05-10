using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class WaterTemplate : MonoBehaviour
{
    public Image sprite;
    public RectTransform rectTransfrorm;

    private void Start()
    {
        if (Application.isPlaying) sprite.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (rectTransfrorm == null) rectTransfrorm = GetComponent<RectTransform>();
        var rectV = new Vector3[4];
        rectTransfrorm.GetWorldCorners(rectV);
        var height = (rectV[1].y - rectV[0].y);
        float Left = rectV[0].x;
        float Top = rectV[1].y;
        float width = (int)((rectV[3].x - rectV[0].x));
        Rect rect = new Rect(Left, Top, width, height);
        sprite.rectTransform.sizeDelta = new Vector2(width, height);
    }
}
