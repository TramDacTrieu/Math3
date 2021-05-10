using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class HighlightManager
{
    static List<IHighlightableComponent> crossedExtraItems = new List<IHighlightableComponent>();
    static List<IHighlightableComponent> extraItems = new List<IHighlightableComponent>();
    static List<IHighlightableComponent> itemsAdditional = new List<IHighlightableComponent>();
    static bool enableHighlights;

    public static void HideLights(IHighlightableComponent[] items)
    {
        foreach (var item in items)
        {
            item?.transform?.GetComponent<IHighlightableComponent>()?.SetSelected(false);
            item?.transform?.GetComponent<IHighlightableComponent>()?.SetOutline(false);
        }
    }


    static void ClearHighlight(bool boost = false)
    {
        HideLights(itemsAdditional.Except(LevelManager.THIS.destroyAnyway.Select(i => i.GetComponent<IHighlightableComponent>())).ToArray());
        itemsAdditional.Clear();
        //if (!boost)
        //    return;
        crossedExtraItems.Clear();

    }

    public static void StopAndClearAll()
    {
        extraItems.Clear();
        ClearHighlight();
    }

    public static void BlurOtherObjectsAll(IHighlightableComponent[] list, Color color)
    {
        var items = list.Select(i => i.spriteRenderers).SelectMany(i => i);
        foreach (var item in items)
        {
            item.color = color;
        }
        DisableCircleLight();
    }

    public static void DisableCircleLight()
    {
        var list = GameObject.FindObjectsOfType<IHighlightableComponent>();
        list.ForEachY(i => i.SetLight(false));
    }
    public static void DisableOutline()
    {
        var list = GameObject.FindObjectsOfType<IHighlightableComponent>();
        list.ForEachY(i => i.SetOutline(false));
    }

    public static void WhiteOtherObjects(IHighlightableComponent[] items, Color color)
    {
        foreach (var item in items)
        {
            if (item == null) continue;
            SpriteRenderer[] spriteRenderer = item.spriteRenderers;
            spriteRenderer.ForEachY(i => i.color = color);
            item.SetLight(true);
        }
    }
    static List<IColorableComponent> itemToDestroyGlobalList = new List<IColorableComponent>();
    public static IColorableComponent[] GetNearMatches(IColorableComponent item)
    {
        itemToDestroyGlobalList.Clear();
        itemToDestroyGlobalList.Add(item);
        GetNearMatchesRecursively(item);
        return itemToDestroyGlobalList.ToArray();
    }

    public static void GetNearMatchesRecursively(IColorableComponent item)
    {

        var items = item.GetNearMatches();
        foreach (var item1 in items)
        {
            if (!itemToDestroyGlobalList.Contains(item1))
            {
                itemToDestroyGlobalList.Add(item1);
                GetNearMatchesRecursively(item1);
            }
        }
    }

    public static IHighlightableComponent[] GetLinePath(IHighlightableComponent item, ref IHighlightableComponent closestSelectedItem, IHighlightableComponent[] list)
    {
        List<IHighlightableComponent> path = new List<IHighlightableComponent>();
        var enumerable = list.Select(i => new { item = i, dist = Vector2.Distance(item.transform.position, i.transform.position) });
        closestSelectedItem = enumerable.OrderBy(i => i.dist).FirstOrDefault()?.item;
        if (closestSelectedItem == null) return path?.WhereNotNull().ToArray();
        Vector2 direction = closestSelectedItem.transform.position - item.transform.position;
        GetPathRecursively(closestSelectedItem, direction, path, item);
        return path?.WhereNotNull().ToArray();
    }

    private static void GetPathRecursively(IHighlightableComponent item, Vector2 direction, List<IHighlightableComponent> countedItems, IHighlightableComponent destItem)
    {
        if (item == null) return;
        if (item == destItem) return;
        if (countedItems.Contains(item)) return;
        var items = item.GetComponent<IColorableComponent>().GetNearMatches().Select(i => i.IHighlightableComponent).Except(countedItems).WhereNotNull();
        var first = items.WhereNotNull().Select(i => new { item = i, angle = Vector2.Angle(direction, item.transform.position - i.transform.position) }).OrderBy(i => i.angle).FirstOrDefault()?.item;
        countedItems.Add(first);
        // Debug.DrawLine(item.transform.position, first.transform.position, Color.green);
        GetPathRecursively(first, direction, countedItems, destItem);

    }


}
