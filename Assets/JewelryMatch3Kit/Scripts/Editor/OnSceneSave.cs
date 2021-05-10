using UnityEngine;
using UnityEditor;
using System.Collections;

public class OnSceneSave : UnityEditor.AssetModificationProcessor
{
    static string[] OnWillSaveAssets(string[] paths)
    {

        foreach (string path in paths)
        {
            if (path.ToString().Contains("level") || path.ToString().Contains("map.unity"))
                SaveCanvasMenuPrefab();
        }
        return paths;
    }

    static void SaveCanvasMenuPrefab()
    {
        GameObject canvasMenu = GameObject.Find("CanvasGlobal");
        SaveToPrefab(canvasMenu);
        GameObject levelUI = GameObject.Find("LevelUI");
        SaveToPrefab(levelUI);

    }

    private static void SaveToPrefab(GameObject obj)
    {
        Object prefab = PrefabUtility.GetCorrespondingObjectFromSource(obj);
        if (obj != null && prefab != null)
            PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
    }
}