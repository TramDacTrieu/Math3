using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

public class PrefabsPallete : EditorWindow
{
    private static List<ItemEditorIcon> prefabs;

    // [MenuItem("JewelryMatch3Kit/PrefabsPalette")]
    private static void ShowWindow()
    {
        GetWindow<PrefabsPallete>().Show();
    }

    void OnFocus()
    {
        var list = Resources.LoadAll("Prefabs/GameItems/").Select(i => (GameObject)i).ToArray();
        prefabs = new List<ItemEditorIcon>();
        foreach (var item in list)
        {
            if (item.layer != LayerMask.NameToLayer("Item")) continue;
            ItemEditorIcon item1 = new ItemEditorIcon();
            item1.GameObject = item;
            var tr = item.transform;
            var sr = tr.GetComponent<SpriteRenderer>();
            item1.texPos = new TexPos { Texture2D = sr != null ? sr.sprite.texture : null, pos = (Vector2)tr.localPosition, scale = (Vector2)tr.localScale };
            item1.texPosList = GetChildrenRecursevly(item.transform);

            prefabs.Add(item1);
        }

        var Palette = GetAllWindows().First(i => i.Name.Contains("palette"));
        // var myWindow = (UnityEditor.GridPaintPaletteWindow) EditorWindow.GetWindow();
        Debug.Log(Palette);
    }

    private System.Type[] GetAllWindows()
    {
        var result = new System.Collections.Generic.List<System.Type>();
        System.Reflection.Assembly[] AS = System.AppDomain.CurrentDomain.GetAssemblies();
        System.Type editorWindow = typeof(EditorWindow);
        foreach (var A in AS)
        {
            System.Type[] types = A.GetTypes();
            foreach (var T in types)
            {
                if (T.IsSubclassOf(editorWindow))
                {
                    result.Add(T);

                }
            }
        }
        return result.ToArray();
    }

    List<TexPos> GetChildrenRecursevly(Transform item)
    {
        var list = new List<TexPos>();
        foreach (Transform tr in item.transform)
        {
            var sr = tr.GetComponent<SpriteRenderer>();
            TexPos texPos = new TexPos { Texture2D = sr != null ? sr.sprite.texture : null, pos = (Vector2)tr.localPosition, scale = (Vector2)tr.localScale };
            texPos.chidlren = GetChildrenRecursevly(tr);
            list.Add(texPos);
        }
        return list;
    }

    private void OnGUI()
    {
        if (prefabs == null) return;
        int length = prefabs.Count();
        int columns = 4;
        GUILayout.BeginVertical();
        {
            for (int i = 0; i < length; i++)
            {
                GUILayout.BeginHorizontal();
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (i + j >= length) break;
                        if (GUILayout.Button(new GUIContent("", prefabs[i + j].GameObject.name), GUILayout.Width(50), GUILayout.Height(50)))
                        {

                        }
                        var lastRect = GUILayoutUtility.GetLastRect();
                        DrawLayeredTextures(prefabs[i + j].texPos, prefabs[i + j].texPosList.ToArray(), lastRect, prefabs[i + j].GameObject.transform);
                    }
                    i += columns - 1;
                }
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndVertical();
    }

    private void DrawLayeredTextures(TexPos texPos, TexPos[] texPosList, Rect lastRect, Transform parentObj)
    {
        Rect newRect = lastRect;
        if (texPos.Texture2D != null)
        {
            var centerButton = lastRect.center;
            float x = lastRect.position.x + texPos.pos.x;
            float y = lastRect.position.y + texPos.pos.y;
            float width = (texPos.Texture2D.width * texPos.scale.x / parentObj.localScale.x) / 5;
            float height = (texPos.Texture2D.height * texPos.scale.y / parentObj.localScale.y) / 5;
            var texCenter = new Rect(x, y, width, height).center;
            var offset = centerButton - texCenter;
            x += offset.x;
            y += offset.y;
            newRect = new Rect(x, y, width, height);
            GUI.DrawTexture(newRect, texPos.Texture2D);
        }
        for (int i = 0; i < texPosList.Length; i++)
        {
            DrawTexRecursevly(newRect, parentObj, texPosList[i]);
        }
    }

    private void DrawTexRecursevly(Rect lastRect, Transform parentObj, TexPos texPos)
    {
        Rect newRect = lastRect;
        if (texPos.Texture2D != null)
        {
            var centerButton = lastRect.center;
            float x = lastRect.position.x + texPos.pos.x;
            float y = lastRect.position.y + texPos.pos.y;
            float width = (texPos.Texture2D.width * texPos.scale.x / parentObj.localScale.x) / 5;
            float height = (texPos.Texture2D.height * texPos.scale.y / parentObj.localScale.y) / 5;
            var texCenter = new Rect(x, y, width, height).center;
            var offset = centerButton - texCenter;
            x += offset.x - texPos.pos.x;
            y += offset.y - texPos.pos.y * 20;
            newRect = new Rect(x, y, width, height);
            GUI.DrawTexture(newRect, texPos.Texture2D);
        }
        foreach (var item in texPos.chidlren)
        {
            DrawTexRecursevly(lastRect, parentObj, item);
        }

    }

    struct TexPos
    {
        public Texture2D Texture2D;
        public Vector2 pos;
        public Vector2 scale;
        public List<TexPos> chidlren;

    }
    class ItemEditorIcon
    {
        public TexPos texPos;
        public List<TexPos> texPosList = new List<TexPos>();
        public GameObject GameObject;
    }
}

