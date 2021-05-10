using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GeneratorItems))]
public class GenerateItemsEditor : Editor
{
    List<GameObject> list = new List<GameObject>();

    private void Awake()
    {
        list.Clear();
        var PoolSettings = Resources.Load("Settings/PoolSettings") as PoolerScriptable;
        foreach (var item in PoolSettings.itemsToPool)
        {
            if (item.inEditor)
                list.Add(item.objectToPool);

        }
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GeneratorItems myTarget = (GeneratorItems)target;
        foreach (var item in list)
        {
            if (GUILayout.Button("Generate " + item.tag))
            {
                myTarget.GenItem<Item>(item.tag);
            }
        }


        // if (GUILayout.Button("Generate items"))
        // {
        //     myTarget.GenItem<Item>("Item");
        // }
        if (GUILayout.Button("Generate 20 items"))
        {
            for (int i = 0; i < 10; i++)
            {
                myTarget.GenItem<Item>("Item");
            }
        }
        // if (GUILayout.Button("Generate striped"))
        // {
        //     myTarget.GenItem<Striped>("striped");
        // }
        // if (GUILayout.Button("Generate flower items"))
        // {
        //     myTarget.GenItem<FlowerItem>("flower_item");
        // }
        // if (GUILayout.Button("Generate key holders"))
        // {
        //     myTarget.GenItem<KeyHolder>("key_holder");
        // }
        // if (GUILayout.Button("Generate spiral bombs"))
        // {
        //     myTarget.GenItem<SpiralBomb>("spiral_bomb");
        // }
        // if (GUILayout.Button("Generate multicolor bombs"))
        // {
        //     myTarget.GenItem<SpiralBomb>("multicolor");
        // }
        GUILayout.Space(50);
        if (GUILayout.Button("Simulate physics " + !Physics2D.autoSimulation))
        {
            Physics2D.autoSimulation = !Physics2D.autoSimulation;
        }
        GUILayout.Space(20);
        if (GUILayout.Button("Clear items"))
        {
            ObjectPooler.Instance.DestroyObjects("Item");
        }
        if (GUILayout.Button("Clear striped"))
        {
            ObjectPooler.Instance.DestroyObjects("striped");
        }
    }
}
