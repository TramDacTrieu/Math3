using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

[InitializeOnLoad]
public class Autorun
{
    static Autorun()
    {
        EditorApplication.update += InitProject;

    }

    static void InitProject()
    {
        EditorApplication.update -= InitProject;
        // if (EditorApplication.timeSinceStartup < 10 || !EditorPrefs.GetBool("AlreadyOpened"))
        // {
        //     if (EditorSceneManager.GetActiveScene().name != "map" && Directory.Exists("Assets/JewelryMatch3Kit/Scenes"))
        //     {
        //         EditorSceneManager.OpenScene("Assets/JewelryMatch3Kit/Scenes/game.unity");

        //     }
        //     GameSettings.Init();
        //     GameSettings.ShowHelp();
        //     EditorPrefs.SetBool("AlreadyOpened", true);
        // }

    }
}