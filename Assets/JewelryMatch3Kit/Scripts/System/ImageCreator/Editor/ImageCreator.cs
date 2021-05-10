using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

[InitializeOnLoad]
public class ImageCreator : Editor
{
    static ImageCreator()
    {
        EditorApplication.hierarchyWindowChanged += OnChanged;
    }

    private static void OnChanged()
    {
        if (Application.isPlaying) return;

        var obj = Selection.activeGameObject;
        if (obj == null || obj.transform.parent == null) return;
        if ((obj.transform.parent.GetComponent<CanvasRenderer>() != null || obj.transform.parent.GetComponent<Canvas>() != null /* || obj.transform.parent.GetComponent<RectTransform>() != null */) && obj.GetComponent<SpriteRenderer>() != null)
        {
            var rectTransform = obj.AddComponent<RectTransform>();
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.localScale = Vector3.one;
            var spr = obj.GetComponent<SpriteRenderer>().sprite;
            var img = obj.AddComponent<Image>();
            img.sprite = spr;
            img.SetNativeSize();
            DestroyImmediate(obj.GetComponent<SpriteRenderer>());
        }
    }
}