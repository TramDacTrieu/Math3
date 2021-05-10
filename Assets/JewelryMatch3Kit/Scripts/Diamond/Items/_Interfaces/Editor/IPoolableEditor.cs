using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;
[CustomEditor(typeof(IPoolableComponent))]
public class IPoolableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        IPoolableComponent mytarget = (IPoolableComponent)target;
        if (GUILayout.Button("Init item"))
        {
            if (mytarget.GetComponent<Rigidbody2D>() == null) mytarget.gameObject.AddComponent<Rigidbody2D>();
            if (mytarget.GetComponent<CircleCollider2D>() == null) mytarget.gameObject.AddComponent<CircleCollider2D>();
            if (mytarget.GetComponent<IColorableComponent>() == null) mytarget.gameObject.AddComponent<IColorableComponent>();
            mytarget.gameObject.layer = LayerMask.NameToLayer("Item");
            Preset preset = (Preset)AssetDatabase.LoadAssetAtPath("Assets/JewelryMatch3Kit/Scripts/JewelryMatch3KitItems/_Interfaces/ICombinableComponent.preset", typeof(Preset));
            preset.ApplyTo(mytarget.GetComponent<IColorableComponent>());
            if (mytarget.GetComponent<IHighlightableComponent>() == null) mytarget.gameObject.AddComponent<IHighlightableComponent>();
            preset = (Preset)AssetDatabase.LoadAssetAtPath("Assets/JewelryMatch3Kit/Scripts/JewelryMatch3KitItems/_Interfaces/IHighlightableComponent.preset", typeof(Preset));
            preset.ApplyTo(mytarget.GetComponent<IHighlightableComponent>());
            if (mytarget.GetComponent<IDestroyableComponent>() == null) mytarget.gameObject.AddComponent<IDestroyableComponent>();

        }
    }
}
