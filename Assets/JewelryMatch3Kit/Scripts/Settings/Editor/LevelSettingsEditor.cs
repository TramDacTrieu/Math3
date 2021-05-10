using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[CustomEditor(typeof(LevelSettings))]
public class LevelSettingsEditor : Editor
{
    private static LevelSettings myTarget;
    private Transform _transform;
    private EditorBehaviorMode editorMode;
    Vector3 position, eulerAngles, scale;

    private static LevelScriptable levelScriptable;
    private static GUIStyle localStyle;
    private static int levelNumber;
    private static bool levelChange;
    private Vector3 savedScale;
    private Vector3 savedRotation;
    private Vector3 savedPos;
    private Vector2 s;
    static bool editorEnabled;
    static GameObject[] chainList;
    static GameObject[] spawnList;
    static int chainIndex;
    static int spawnerIndex;
    private void OnEnable()
    {
        if (Application.isPlaying) { OnDisable(); return; }

        myTarget = (LevelSettings)target;
        // _transform = (Transform)target;
        levelNumber = Int32.Parse(Regex.Match(EditorSceneManager.GetActiveScene().name, @"\d+").Value);
        levelScriptable = myTarget.LevelScriptable;
        if (levelScriptable == null)
        {
            levelScriptable = CreateLevelSettingsScriptable(levelNumber);
        }
        chainList = GameObject.FindGameObjectsWithTag("chain");
        spawnList = GameObject.FindGameObjectsWithTag("spawner");
        if (!editorEnabled)
        {
            chainIndex = 0;
            spawnerIndex = 0;
            SceneView.onSceneGUIDelegate += OnScene;
        }

    }
    public static void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnScene;
    }

    private static void OnScene(SceneView sceneview)
    {
        if (Application.isPlaying) { OnDisable(); return; }
        if (levelScriptable == null) return;
        Handles.BeginGUI();
        localStyle = new GUIStyle(GUI.skin.label);
        localStyle.normal.textColor = Color.white;
        EditorGUI.BeginChangeCheck();

        GUILevelSelector();
        // if (levelChange) return;
        var fields = levelScriptable.GetType().GetFields();

        foreach (var item in fields)
        {
            ShowField(item, ref levelScriptable);
        }

        if (GUILayout.Button("Save", GUILayout.Width(150)))
        {
            Save();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Prefabs palette", GUILayout.Width(150)))
        {
            if (!myTarget.GetComponent<SpriteShapeController>().autoUpdateCollider)
                myTarget.GetComponent<SpriteShapeController>().BakeCollider();
            if (!EditorApplication.ExecuteMenuItem("Window/2D/Tile Palette"))
                EditorApplication.ExecuteMenuItem("Window/Tile Palette");
            Selection.activeObject = GameObject.Find("Grid");
        }
        if (GUILayout.Button("Select chain", GUILayout.Width(150)))
        {
            var listCopy = GameObject.FindGameObjectsWithTag("chain");
            if (listCopy.Length > chainList.Length) chainList = listCopy;
            Selection.activeObject = chainList[(int)Mathf.Repeat(chainIndex++, chainList.Length)];
        }
        if (GUILayout.Button("Select spawner", GUILayout.Width(150)))
        {
            var listCopy = GameObject.FindGameObjectsWithTag("spawner");
            if (listCopy.Length > spawnList.Length) spawnList = listCopy;
            Selection.activeObject = spawnList[(int)Mathf.Repeat(spawnerIndex++, spawnList.Length)];
        }
        GUILayout.Space(10);
        editorEnabled = true;

        Handles.EndGUI();
    }

    struct TileItem
    {
        public TileBase tile;
        public Vector2Int pos;
        public Vector2 worldPos;
    }
    public static void InspectType(string assemblyName, string typeName)
    {
        Assembly UnityEngine = Assembly.Load(assemblyName);
        Type type = UnityEngine.GetType(typeName);

        foreach (MemberInfo member in type.GetMembers())
        {
            Debug.Log(member.MemberType.ToString() + ": " + member.ToString());
        }
    }

    public void ShowList(IList list)
    {
        Type typeList = list.GetType().GetElementType();
        {
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var itemRef = item;
                Type type = list.GetType().GetGenericArguments().FirstOrDefault();
                if (type != typeof(UnityEngine.Object) && type != typeof(UnityEngine.GameObject) && itemRef != null)
                {
                    foreach (var field in IterateFields(ref itemRef))
                    {
                        ShowField(field, ref itemRef);
                    }
                }
                else
                    list[i] = EditorGUILayout.ObjectField((UnityEngine.Object)list[i], typeof(UnityEngine.Object), true);

            }
        }
    }
    public static void ShowArray(int[] list, string name)
    {
        Type typeList = list.GetType().GetElementType();
        {
            for (int i = 0; i < list.Length; i++)
            {
                var item = list[i];
                var itemRef = item;
                Type type = list.GetType().GetGenericArguments().FirstOrDefault();
                if (type != typeof(UnityEngine.Object) && type != typeof(UnityEngine.GameObject) && itemRef != null)
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(name + " " + (i + 1), localStyle, new GUILayoutOption[] { GUILayout.Width(80) });
                        list[i] = EditorGUILayout.IntField((int)list[i], GUILayout.Width(80));
                    }
                    GUILayout.EndHorizontal();
                }

            }
        }
    }

    static FieldInfo[] IterateFields<T>(ref T obj)
    {
        Type myType = obj.GetType();
        FieldInfo[] fieldInfo = myType.GetFields();
        return fieldInfo;
    }
    static void ShowField<T>(FieldInfo myField, ref T obj)
    {
        if (obj.GetType() != typeof(GameObject))
        {
            Type fieldType = myField.FieldType;
            var fieldValue = myField.GetValue(obj);
            var fieldName = myField.Name;
            if (fieldValue?.GetType() == typeof(string) || fieldValue?.GetType().BaseType == typeof(string))
            {
                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(fieldName.ToString(), localStyle, new GUILayoutOption[] { GUILayout.Width(80) });
                    myField.SetValue(obj, EditorGUILayout.TextField(fieldValue.ToString(), GUILayout.Width(80)));
                }
                GUILayout.EndHorizontal();
            }
            else if (fieldValue?.GetType() == typeof(Int32) || fieldValue?.GetType().BaseType == typeof(Int32))
            {
                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(fieldName.ToString(), localStyle, new GUILayoutOption[] { GUILayout.Width(80) });
                    myField.SetValue(obj, EditorGUILayout.IntField((int)fieldValue, GUILayout.Width(80)));
                }
                GUILayout.EndHorizontal();
            }
            else if (fieldValue?.GetType() == typeof(Enum) || fieldValue?.GetType().BaseType == typeof(Enum))
            {
                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(fieldName.ToString(), localStyle, new GUILayoutOption[] { GUILayout.Width(80) });
                    myField.SetValue(obj, EditorGUILayout.EnumPopup((Enum)fieldValue, GUILayout.Width(80)));
                }
                GUILayout.EndHorizontal();
            }
            else if (fieldValue?.GetType() == typeof(Int32[]) || fieldValue?.GetType().BaseType == typeof(Int32[]))
            {
                ShowArray((int[])fieldValue, fieldName);
            }
            else if (fieldValue?.GetType() == typeof(UnityEngine.GameObject))
            {
                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(fieldName.ToString(), localStyle, new GUILayoutOption[] { GUILayout.Width(80) });
                    myField.SetValue(obj, (GameObject)EditorGUILayout.ObjectField((GameObject)fieldValue, typeof(GameObject), true, new GUILayoutOption[] { GUILayout.Width(80) }));
                }
                GUILayout.EndHorizontal();
            }
        }
        // if (fieldType == typeof(GameObject))
        // myField.SetValue(EditorGUILayout.ObjectField((GameObject)fieldValue, typeof(GameObject)), (GameObject)fieldValue);


    }

    static void GUILevelSelector()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Level", localStyle, new GUILayoutOption[] { GUILayout.Width(50) });

        if (GUILayout.Button("<<", new GUILayoutOption[] { GUILayout.Width(30) }))
        {
            EditorSceneManager.SaveOpenScenes();
            PreviousLevel();
        }
        string changeLvl = GUILayout.TextField(" " + levelNumber, new GUILayoutOption[] { GUILayout.Width(30) });
        if (int.Parse(changeLvl) != levelNumber)
        {
            LoadLevel(int.Parse(changeLvl));
        }


        if (GUILayout.Button(">>", new GUILayoutOption[] { GUILayout.Width(30) }))
        {
            EditorSceneManager.SaveOpenScenes();
            NextLevel();
        }

        if (GUILayout.Button("+", new GUILayoutOption[] { GUILayout.Width(20) }))
        {
            EditorSceneManager.SaveOpenScenes();
            AddLevel();
        }


        GUILayout.EndHorizontal();
    }

    private static void AddLevel()
    {
        string newPath = GetScenePath(GetLastLevel() + 1);
        FileUtil.CopyFileOrDirectory("Assets/JewelryMatch3Kit/Scenes/level_prefab.unity", newPath);
        var originalSceneList = EditorBuildSettings.scenes;
        var newSceneList = new EditorBuildSettingsScene[originalSceneList.Length + 1];
        System.Array.Copy(originalSceneList, newSceneList, originalSceneList.Length);
        var sceneToAdd = new EditorBuildSettingsScene(newPath, true);
        newSceneList[newSceneList.Length - 1] = sceneToAdd;
        EditorBuildSettings.scenes = newSceneList;
        EditorSceneManager.OpenScene(newPath);

    }

    static int GetLastLevel()
    {
        for (int i = levelNumber; i < 50000; i++)
        {
            LevelScriptable lvl = Resources.Load("Levels/level" + i) as LevelScriptable;
            if (lvl == null)
            {
                return i - 1;
            }
        }
        return 0;
    }

    private static string GetScenePath(int num)
    {
        return "Assets/JewelryMatch3Kit/Scenes/level" + num + ".unity";
    }

    private static void NextLevel()
    {
        int newNum = levelNumber + 1;
        string sceneName = "level" + newNum;
        LoadLevel(newNum);
    }

    private static void LoadLevel(int newNum)
    {
        levelChange = true;
        try
        {
            EditorSceneManager.OpenScene(GetScenePath(newNum));
        }
        catch (System.Exception)
        {
            // throw;
        }
    }

    private static void PreviousLevel()
    {
        int newNum = levelNumber - 1;
        string sceneName = "level" + newNum;
        LoadLevel(newNum);
    }

    static void Save()
    {
        myTarget.ComputeCollectables();

        EditorUtility.SetDirty(levelScriptable);
        AssetDatabase.SaveAssets();
    }
    public LevelScriptable CreateLevelSettingsScriptable(int num)
    {
        LevelScriptable asset = ScriptableObject.CreateInstance<LevelScriptable>();

        string path = "Assets/JewelryMatch3Kit/Resources/Levels/level" + num + ".asset";
        AssetDatabase.CreateAsset(asset, path);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return asset;
    }

}


[InitializeOnLoad]
public static class OnSceneLoadedEditor
{
    private static string currentScene;
    static string loadedLevel;
    static bool working = false;
    static OnSceneLoadedEditor()
    {
        Constructor();
    }

    static void Constructor()
    {
        loadedLevel = EditorSceneManager.GetActiveScene().name;
        if (!working)
        {
            EditorApplication.hierarchyChanged += OnHierarchyChange;
            working = true;
        }
        else
        {
            working = false;
            EditorApplication.hierarchyChanged -= OnHierarchyChange;
        }
    }

    static void OnHierarchyChange()
    {
        if (EditorSceneManager.GetActiveScene().name != loadedLevel)
        {
            loadedLevel = EditorSceneManager.GetActiveScene().name;
            Selection.activeObject = GameObject.FindObjectOfType<LevelSettings>();
        }
    }

}


