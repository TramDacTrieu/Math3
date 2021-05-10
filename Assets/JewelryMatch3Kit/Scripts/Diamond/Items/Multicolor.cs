using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets._2D;

public class Multicolor : MonoBehaviour, IHittable, ISpecial, IPoolable
{
    public GameObject LightningPrefab;
    private bool destroyed;

    private void OnEnable()
    {
        GetComponent<IColorableComponent>().color = -1;
    }

    public void Hit(Vector2 pos, Action callback)
    {
        // IEnumerable<IColorableComponent> enumerable = LevelManager.THIS.GetComponent<SelectionManager>().selectedColor;
        // IColorableComponent colorableComponent = enumerable.Where(i => i?.color > -1).TryGetElement(0, null);
        StartCoroutine(DestroyItems(pos, LevelManager.THIS.GetComponent<SelectionManager>().selectedColor, callback));
    }

    IEnumerator DestroyItems(Vector2 pos, int color, Action callback)
    {
        var items = Camera.main.GetComponent<Camera2DFollow>().GetVisibleItems().Select(i => i.GetComponent<IDestroyableComponent>()).WhereNotNull().Where(i => i.destroyed == false && i.gameObject != gameObject && i.GetComponent<IColorableComponent>() != null && i.GetComponent<IColorableComponent>().color == color);
        float y = Camera2DFollow.direction > 0 ? items.Min(i => i.transform.position.y) : items.Max(i => i.transform.position.y);
        ItemPhysicsManger.LowerDinamicTreshold = new Vector2(0, y);
        // var items = Physics2D.OverlapCircleAll(transform.position, 10, 1 << LayerMask.NameToLayer("Item")).Select(i => i.GetComponent<IDestroyableComponent>()).WhereNotNull().Where(i => i.destroyed == false && i.gameObject != gameObject && i.GetComponent<IColorableComponent>() != null && i.GetComponent<IColorableComponent>().color == color);
        foreach (var item in items)
        {
            item?.Hit(item.transform.position);
            CreateLightning(transform.position, item.transform.position);
            yield return new WaitForSeconds(0.1f);
        }
        callback();
    }

    private void CreateLightning(Vector3 pos1, Vector3 pos2)
    {
        var go = Instantiate(LightningPrefab, Vector3.zero, Quaternion.identity);
        var lightning = go.GetComponent<Lightning>();
        lightning.SetLight(pos1, pos2);
    }

    public void OnGetFromPool(Vector2 pos)
    {
        destroyed = false;
    }

}
