using UnityEngine;

public interface IExplodable
{
    IHighlightableComponent[] GetDestroyItems(Vector3 pos);

}