using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class NestedColor : MonoBehaviour, IColorChangable, IColorEditor
{
    public Sprite[] Sprites;
    public Sprite randomEditorSprite;

    Sprite[] IColorEditor.Sprites
    {
        get
        {
            return Sprites;
        }
    }

    Sprite IColorEditor.randomEditorSprite
    {
        get
        {
            return randomEditorSprite;
        }
    }

    public void OnColorChanged(int color)
    {
        GetComponent<SpriteRenderer>().sprite = Sprites[color];
    }
}
