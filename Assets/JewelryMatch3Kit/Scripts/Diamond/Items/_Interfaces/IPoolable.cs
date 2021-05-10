using UnityEngine;

public interface IPoolable
{
    void OnGetFromPool(Vector2 pos);
}