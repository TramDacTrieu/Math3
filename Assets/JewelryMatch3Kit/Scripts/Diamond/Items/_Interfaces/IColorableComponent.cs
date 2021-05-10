using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[ExecuteInEditMode]
public class IColorableComponent : MonoBehaviour, IPoolable, IColorEditor
{
    public int color;

    public Sprite[] Sprites;

    public Sprite randomEditorSprite;
    public IHighlightableComponent IHighlightableComponent;
    public IDestroyableComponent IDestroyableComponent;
    public ItemSound ItemSound;

    // [HideInInspector]
    // public bool colorGenerated;
    public bool RandomColorOnAwake = true;

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

    private void Awake()
    {
        IHighlightableComponent = GetComponent<IHighlightableComponent>();
        ItemSound = GetComponent<ItemSound>();
        IDestroyableComponent = GetComponent<IDestroyableComponent>();
    }

    // private void OnEnable()
    // {
    //     // GetComponentsInChildren<IColorableComponent>().ToList().ForEach(i => i.SetColor(color));
    //     // GetComponentsInChildren<IColorChangable>().ToList().ForEach(i => i.OnColorChanged(color));

    // }


    public void SetColor(int _color)
    {
        if (_color < 0) return;
        // colorGenerated = true;
        color = _color;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (Sprites.Length > 0 && spriteRenderer)
            spriteRenderer.sprite = Sprites[_color];
        GetComponentsInChildren<IColorChangable>().ToList().ForEach(i => i.OnColorChanged(_color));
    }

    public void RandomizeColor()
    {
        color = ColorGenerator.GenColor();
        SetColor(color);
        GetComponentsInChildren<IColorableComponent>().ToList().ForEach(i => i.SetColor(color));

    }

    public void OnGetFromPool(Vector2 pos)
    {
        if (RandomColorOnAwake)
            RandomizeColor();
    }

    public IColorableComponent[] GetNearMatches()
    {
        float dist = 1.2f;
        if (tag.Contains("nested") || tag == "potion") dist = 1.5f;
        return Physics2D.OverlapCircleAll(transform.position, dist)
        .Where(i => i.gameObject != gameObject)
        .Select(i => i.GetComponent<IColorableComponent>()).WhereNotNull()
        .Where(i => i.color == color || i.color == -1)
        .Distinct()
        .ToArray();
    }

    public void OnAnimChangeColor()
    {
        if (GetComponent<Item>() != null)
        {
            RandomizeColor();
        }
    }

}
