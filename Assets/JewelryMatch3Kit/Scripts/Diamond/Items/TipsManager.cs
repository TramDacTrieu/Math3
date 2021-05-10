using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets._2D;

public class TipsManager : MonoBehaviour
{
    private Camera2DFollow camera2DFollow;

    private void Start()
    {
        // CheckItems();
        camera2DFollow = Camera.main.GetComponent<Camera2DFollow>();

    }
    private void CheckItems()
    {
        StopAllCoroutines();
        StartCoroutine(Loop());
    }

    private IEnumerator Loop()
    {
        yield return new WaitForSeconds(2);
        IColorableComponent[] colorableComponent = FindObjectsOfType<IColorableComponent>();
        while (true)
        {
            if (LevelManager.THIS.gameStatus != GameState.Playing)
            {
                HighlightManager.DisableOutline();
                // StopCoroutine(Loop());
                yield return new WaitForSeconds(2);
                continue;
            }
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(2);
            var fallingItems = colorableComponent.Count(i => i.GetComponent<Rigidbody2D>() != null && i.GetComponent<Rigidbody2D>().velocity.magnitude > .1f);
            if (fallingItems > 0) continue;
            var items = camera2DFollow.GetVisibleItems();
            bool foundMatches = false;
            List<IColorableComponent> list = new List<IColorableComponent>();
            foreach (var item in items)
            {
                var preSelectedList = HighlightManager.GetNearMatches(item).Intersect(items);
                if (preSelectedList.Count() > 2)
                {
                    // preSelectedList.ForEachY(i => i?.IHighlightableComponent?.SetTip());
                    list.AddRange(preSelectedList);
                    foundMatches = true;
                    break;
                }

            }
            if (!LevelManager.THIS.touchBlocker.blocked && !foundMatches)
            {
                LevelManager.THIS.NoMatches();
                // break;
            }
            yield return new WaitForSeconds(5);
            if (LevelManager.THIS.gameStatus != GameState.Playing)
            {
                HighlightManager.DisableOutline();
                // yield break;
            }
            list.ForEachY(i => i?.IHighlightableComponent?.SetTip());
            // FindItems();
        }
    }

    private void OnEnable()
    {
        SelectionManager.OnSelectionStarted += StopTips;
        LevelManager.OnNextMove += CheckItems;
    }


    private void OnDisable()
    {
        SelectionManager.OnSelectionStarted -= StopTips;
        LevelManager.OnNextMove -= CheckItems;
    }

    private void StopTips()
    {
        StopAllCoroutines();
    }

    void FindItems()
    {
        if (LevelManager.THIS.gameStatus != GameState.Playing)
        {
            HighlightManager.DisableOutline();
            StopCoroutine(Loop());
            return;
        }
        var fallingItems = FindObjectsOfType<IColorableComponent>().Count(i => i.GetComponent<Rigidbody2D>() != null && i.GetComponent<Rigidbody2D>().velocity.magnitude > .5f);
        if (fallingItems > 0) return;
        var items = Camera.main.GetComponent<Camera2DFollow>().GetVisibleItems();
        bool foundMatches = false;
        foreach (var item in items)
        {
            var preSelectedList = HighlightManager.GetNearMatches(item);
            if (preSelectedList.Length > 2)
            {
                preSelectedList.ForEachY(i => i?.IHighlightableComponent?.SetTip());
                foundMatches = true;
                break;
            }

        }
        if (!LevelManager.THIS.touchBlocker.blocked && !foundMatches)
        {
            LevelManager.THIS.NoMatches();
        }
    }
}
