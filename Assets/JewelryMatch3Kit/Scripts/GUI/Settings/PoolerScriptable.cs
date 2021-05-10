using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
// [CreateAssetMenu(fileName = "Pooler", menuName = "Diamond splash/Add pooler", order = 1)]
public class PoolerScriptable : ScriptableObject
{
    public List<ObjectPoolItem> itemsToPool;

}
