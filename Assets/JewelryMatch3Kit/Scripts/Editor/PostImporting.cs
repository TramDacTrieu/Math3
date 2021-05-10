using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Xml;
using System;
using System.Linq;

public class PostImporting : AssetPostprocessor
{
    static bool imported = false;
    private LevelManager lm;
    private InitScript initscript;



    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        CheckPoolObjects();

        if (Directory.Exists("Assets/Plugins/Android/google-play-services_lib") && Directory.Exists("Assets/PlayServicesResolver"))
        {
            bool check = AssetDatabase.DeleteAsset("Assets/Plugins/Android/google-play-services_lib");
            if (check)
                Debug.Log("deleted google-play-services_lib ");
        }


        SetScriptingDefineSymbols();//1.3

        if (Directory.Exists("Assets/FacebookSDK"))
        {
            ModifyManifest();
        }

        if (Directory.Exists("Assets/FacebookSDK") && Directory.Exists("Assets/GoogleMobileAds"))
        {//1.4.5
            AssetDatabase.DeleteAsset("Assets/FacebookSDK/Plugins/Android/libs/support-annotations-23.4.0.jar");
            AssetDatabase.DeleteAsset("Assets/FacebookSDK/Plugins/Android/libs/support-v4-23.4.0.aar");
        }

        if (Directory.Exists("Assets/PlayServicesResolver"))
        {
            //if (!imported)
            //{

            //    AssetDatabase.ImportAsset("Assets/PlayServicesResolver");
            //Debug.Log("assets reimorted");
            //}
        }

    }

    private static void CheckPoolObjects()
    {
        var PoolSettings = Resources.Load("Settings/PoolSettings") as PoolerScriptable;
        var data = Resources.LoadAll("Prefabs/GameItems/").Select(i => (GameObject)i);
        foreach (var item in data)
        {
            if (item?.GetComponent<IPoolableComponent>() != null)
            {
                if (PoolSettings.itemsToPool.FindIndex(i => i.objectToPool == item) < 0)
                    PoolSettings.itemsToPool.Add(new ObjectPoolItem { objectToPool = item, poolName = item.tag + "s", amountToPool = 2 });
            }
        }
        // AssetDatabase.SaveAssets();
        // AssetDatabase.Refresh();
    }

    //    void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
    //{
    //    Debug.Log("Sprites: " + sprites.Length);
    //}

    static void ModifyManifest()
    {
        var outputFile = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
        if (File.Exists(outputFile))
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(outputFile);

            if (doc == null)
            {
                //Debug.LogError("Couldn't load " + outputFile);
                return;
            }
            XmlNode manNode = FindChildNode(doc, "manifest");
            XmlNode dict = FindChildNode(manNode, "uses-sdk");
            if (dict == null)
            {
                //Debug.LogError("Error parsing " + outputFile);
                return;
            }

            XmlAttributeCollection attributes = dict.Attributes;
            XmlAttribute attr = attributes[0];
            if (attr.Name == "android:minSdkVersion")
            {
                attr.Value = "" + 15;
            }

            doc.Save(outputFile);

            //while (curr != null)
            //{
            //    var currXmlElement = curr as XmlElement;
            //    Debug.Log(curr.Name);
            //    curr = curr.NextSibling;
            //}
        }
    }

    private static XmlNode FindChildNode(XmlNode parent, string name)
    {
        XmlNode curr = parent.FirstChild;
        while (curr != null)
        {
            if (curr.Name.Equals(name))
            {
                return curr;
            }

            curr = curr.NextSibling;
        }

        return null;
    }

    private static BuildTargetGroup[] GetBuildTargets()
    {
        ArrayList _targetGroupList = new ArrayList();
        _targetGroupList.Add(BuildTargetGroup.Android);
        _targetGroupList.Add(BuildTargetGroup.iOS);
        _targetGroupList.Add(BuildTargetGroup.WSA);
        return (BuildTargetGroup[])_targetGroupList.ToArray(typeof(BuildTargetGroup));
    }

    static void SetScriptingDefineSymbols()
    {
        BuildTargetGroup[] _buildTargets = GetBuildTargets();
        foreach (BuildTargetGroup _target in _buildTargets)
        {
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(_target);
            CheckDefines(ref defines,"Assets/GoogleMobileAds", "GOOGLE_MOBILE_ADS");
            CheckDefines(ref defines,"Assets/Chartboost", "CHARTBOOST_ADS");
            if (CheckDefines(ref defines, "Assets/FacebookSDK", "FACEBOOK"))
            {
                CheckDefines(ref defines,"Assets/PlayFabSDK", "PLAYFAB");
                CheckDefines(ref defines,"Assets/GameSparks", "GAMESPARKS");
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(_target, defines);
        }
    }

    static bool CheckDefines(ref string defines, string path, string symbols)
    {
        if (Directory.Exists(path))
        {
            if (!defines.Contains(symbols))
            {
                defines = defines + "; " + symbols;
            }
            return true;
        }

        var replace = defines.Replace(symbols, "");
        defines = replace;
        return false;
    }



}