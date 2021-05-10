using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrderKeeper
{
    private SpriteRenderer[] spriteRenderers;
    private GameObject obj;
    private int[] orders;
    private int[] layers;

    public OrderKeeper(GameObject obj_, SpriteRenderer[] _spriteRenderers)
    {
        obj = obj_;
        spriteRenderers = _spriteRenderers;
        KeepOrder();
    }

    public void KeepOrder()
    {
        // spriteRenderers = obj.transform.GetAllSpriteRenderers();
        orders = spriteRenderers.Select(i => i.sortingOrder).ToArray();
        layers = spriteRenderers.Select(i => i.sortingLayerID).ToArray();
    }

    public void SetLayer(int layerID)
    {
        for (int i = 0; i < layers.Length; i++)
        {
            spriteRenderers[i].sortingLayerID = layerID;
        }
    }

    public void RestoreLayers()
    {
        for (int i = 0; i < layers.Length; i++)
        {
            spriteRenderers[i].sortingLayerID = layers.TryGetElement(i, 0);
        }
    }
}