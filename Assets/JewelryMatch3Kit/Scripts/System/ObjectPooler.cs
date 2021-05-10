using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[global::System.Serializable]
public class ObjectPoolItem
{
    public GameObject objectToPool;
    public string poolName;
    public int amountToPool;
    public bool shouldExpand = true;
    public bool inEditor = true;
}

[ExecuteInEditMode]
public class ObjectPooler : MonoBehaviour
{
    public const string DefaultRootObjectPoolName = "Pooled Objects";

    public static ObjectPooler Instance;
    public string rootPoolName = DefaultRootObjectPoolName;
    public List<GameObject> pooledObjects = new List<GameObject>();
    private List<ObjectPoolItem> itemsToPool;
    private PoolerScriptable PoolSettings;


    void OnEnable()
    {
        LoadFromScriptable();
        Instance = this;
    }

    private void LoadFromScriptable()
    {
        PoolSettings = Resources.Load("Settings/PoolSettings") as PoolerScriptable;
        itemsToPool = PoolSettings.itemsToPool;
    }

    private void Start()
    {
        if (!Application.isPlaying) return;
        ClearNullElements();

        foreach (var item in itemsToPool)
        {
            if (item == null) continue;
            if (item.objectToPool == null) continue;
            var pooledCount = pooledObjects.Count(i => i.CompareTag(item.objectToPool.tag));
            for (int i = 0; i < item.amountToPool - pooledCount; i++)
            {
                CreatePooledObject(item);
            }
        }
    }

    private void ClearNullElements()
    {
        pooledObjects.RemoveAll(i => i == null);
    }

    private GameObject GetParentPoolObject(string objectPoolName)
    {
        // Use the root object pool name if no name was specified
        // if (string.IsNullOrEmpty(objectPoolName))
        //     objectPoolName = rootPoolName;

        // if (GameObject.Find(rootPoolName) == null) new GameObject { name = rootPoolName };
        GameObject parentObject = GameObject.Find(objectPoolName);
        // Create the parent object if necessary
        if (parentObject == null)
        {
            parentObject = new GameObject();
            parentObject.name = objectPoolName;

            // Add sub pools to the root object pool if necessary
            if (objectPoolName != rootPoolName)
                parentObject.transform.parent = transform;
        }

        return parentObject;
    }

    public void HideObjects(string tag)
    {
        // Debug.Log("hide");
        var objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (var item in objects)
            item.gameObject.SetActive(false);
    }

    public GameObject GetPooledObject(string tag, bool active = true, bool canBeActive = false)
    {
        ClearNullElements();

        GameObject obj = null;
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (pooledObjects[i] == null) continue;
            if ((!pooledObjects[i].activeSelf || canBeActive) && pooledObjects[i].CompareTag(tag))
            {
                obj = pooledObjects[i];
                break;
            }
        }

        if (itemsToPool == null) LoadFromScriptable();
        if (obj == null)
        {
            foreach (var item in itemsToPool)
            {
                if (item.objectToPool == null) continue;
                if (item?.objectToPool?.CompareTag(tag) ?? false)
                {
                    if (item.shouldExpand)
                    {
                        obj = CreatePooledObject(item);
                        break;
                    }
                }
            }
        }
        obj?.SetActive(active);
        return obj;
    }

    private GameObject CreatePooledObject(ObjectPoolItem item)
    {
        // if (!Application.isPlaying && !item.inEditor)
        // {
        //     Debug.Log("not play not editor - " + item.objectToPool);
        //     return null;
        // }
        GameObject obj = Instantiate<GameObject>(item.objectToPool);
        // Get the parent for this pooled object and assign the new object to it
        var parentPoolObject = GetParentPoolObject(item.poolName);
        obj.transform.parent = parentPoolObject.transform;
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            obj = PrefabUtility.ConnectGameObjectToPrefab(obj, item.objectToPool);
        }
#endif

        obj.SetActive(false);
        pooledObjects.Add(obj);


        return obj;
    }

    public void DestroyObjects(string tag)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            // if (pooledObjects[i].CompareTag(tag))
            {
                DestroyImmediate(pooledObjects[i]);
            }
        }
        ClearNullElements();
    }
}