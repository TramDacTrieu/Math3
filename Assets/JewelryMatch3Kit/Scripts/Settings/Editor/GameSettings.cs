using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.UI;
using System.Reflection;
using UnityEditor.SceneManagement;

public class GameSettings : EditorWindow
{

    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;
    int levelNumber = 1;
    int maxRows;
    int maxCols;
    private string fileName = "1.txt";
    private Texture squareTex;
    private Texture blockTex;
    private Texture blockTex2;
    private Texture wireBlockTex;
    private Texture solidBlockTex;
    private Texture doublesolidBlockTex;
    private Texture undestroyableBlockTex;
    private Texture thrivingBlockTex;
    public int star1;
    private int star2;
    private int star3;
    private Vector2 scrollViewVector;
    Target target;
    private int[] ingrCount = new int[4];
    private CollectedIngredients[] ingr = new CollectedIngredients[4];
    private CollectedItem[] collectItems = new CollectedItem[6];
    private int limit = 1;
    private bool update;
    private LIMIT limitType;
    private int colorLimit = 4;
    private static int selected;
    string[] toolbarStrings = new string[] { "Settings", "Shop", "In-apps", "Ads", "GUI", "Rate", "About" };
    private static GameSettings window;
    private bool life_settings_show;
    private bool score_settings;
    bool boost_show;
    private bool failed_settings_show;
    private bool gems_shop_show;
    private bool target_description_show;
    int cageHP;
    private int oldcageHP;
    private int bombsAtTheSameTime;
    private int oldbombsAtTheSameTime;
    private int bombTimer;
    private int oldbombTimer;
    private bool enableGoogleAdsProcessing;
    private CollectStars starsTargetCount;
    private CollectStars oldstarsTargetCount;
    private GameSettingsScriptable lm;
    private GameSettingsScriptable initscript;

    [MenuItem("JewelryMatch3Kit/Level Editor")]
    static void OpenLevel1Settings()
    {
        EditorSceneManager.OpenScene("Assets/JewelryMatch3Kit/Scenes/level1.unity");
    }
    [MenuItem("JewelryMatch3Kit/Map Editor")]
    static void OpenMapEditor()
    {
        EditorSceneManager.OpenScene("Assets/JewelryMatch3Kit/Scenes/map.unity");
    }

    [MenuItem("JewelryMatch3Kit/Prefabs Palette")]
    static void OpenPrefabsPalette()
    {
        if (!EditorApplication.ExecuteMenuItem("Window/2D/Tile Palette"))
            EditorApplication.ExecuteMenuItem("Window/Tile Palette");
    }

    [MenuItem("JewelryMatch3Kit/Pool settings")]
    static void OpenPoolSettings()
    {
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/JewelryMatch3Kit/Resources/Settings/PoolSettings.asset");

    }


    [MenuItem("JewelryMatch3Kit/Settings")]
    public static void Init()
    {
        // Get existing open window or if none, make a new one:
        window = (GameSettings)EditorWindow.GetWindow(typeof(GameSettings));
        window.Show();


    }

    public static void ShowHelp()
    {
        selected = 7;
    }

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(GameSettings));

    }

    List<AdEvents> oldList;

    int NumIngredients;
    private bool initDone;

    void OnFocus()
    {
        if (!EditorSceneManager.GetActiveScene().name.Contains("level") && EditorSceneManager.GetActiveScene().name != "map")
        {
            EditorSceneManager.LoadScene("map");
            return;
        }
        initDone = false;
        if (maxRows <= 0)
            maxRows = 8;
        if (maxCols <= 0)
            maxCols = 7;

        if (Camera.main == null)
            return;

        lm = Resources.Load("Settings/GameSettingsScriptable") as GameSettingsScriptable;
        initscript = Resources.Load("Settings/GameSettingsScriptable") as GameSettingsScriptable;

        if (lm != null)
        {
            Initialize();

            // LoadDataFromLocal(levelNumber);
        }

    }

    void Initialize()
    {


        life_settings_show = true;
        score_settings = true;
        boost_show = true;
        failed_settings_show = true;
        gems_shop_show = true;
        target_description_show = true;
        // for (int i = 0; i < levelSquares.Length; i++)
        // {

        // 	SquareBlocks sqBlocks = new SquareBlocks();
        // 	sqBlocks.block = SquareTypes.EMPTY;
        // 	sqBlocks.obstacle = SquareTypes.NONE;

        // 	levelSquares[i] = sqBlocks;
        // }
        initDone = true;

    }



    void OnGUI()
    {
        if (!initDone)
            return;

        GUI.changed = false;
        if (levelNumber < 1)
            levelNumber = 1;
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        int oldSelected = selected;
        selected = GUILayout.Toolbar(selected, toolbarStrings, new GUILayoutOption[] { GUILayout.Width(450) });
        GUILayout.EndHorizontal();

        scrollViewVector = GUI.BeginScrollView(new Rect(25, 45, position.width - 30, position.height), scrollViewVector, new Rect(0, 0, 400, 1600));
        GUILayout.Space(-30);

        if (oldSelected != selected)
            scrollViewVector = Vector2.zero;

        if (selected == 0)
        {
            GUISettings();
        }
        else if (selected == 1)
        {
            GUIShops();
        }
        else if (selected == 2)
        {
            GUIInappSettings();
        }
        else if (selected == 3)
        {
            GUIAds();
        }
        else if (selected == 4)
        {
            GUIDialogs();
        }
        else if (selected == 5)
        {
            GUIRate();
        }
        else if (selected == 6)
        {
            GUIHelp();
        }
        if (GUILayout.Button("Save"))
        {
            Save();
        }

        GUI.EndScrollView();
        if (GUI.changed && !EditorApplication.isPlaying)
            EditorSceneManager.MarkAllScenesDirty();

        if (enableGoogleAdsProcessing)
            RunOnceGoogle();

        //if (enableChartboostAdsProcessing)
        //    RunOnceChartboost();
    }


    void GUIShowWarning()
    {
        GUILayout.Space(100);
        GUILayout.Label("CAUTION!", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(600) });
        GUILayout.Label("Please open scene - game ( Assets/JewelryMatch3Kit/Scenes/game.unity )", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(600) });

    }

    void SetScriptingDefineSymbols()
    {
        string defines = "";
        if (initscript.enableUnityAds)
            defines = defines + "; UNITY_ADS";
        if (initscript.enableGoogleMobileAds)
            defines = defines + "; GOOGLE_MOBILE_ADS";
        if (initscript.enableChartboostAds)
            defines = defines + "; CHARTBOOST_ADS";
        if (lm.FacebookEnable)
        {
            defines = defines + "; FACEBOOK";
            if (Directory.Exists("Assets/PlayFabSDK"))
                defines = defines + "; PLAYFAB";
        }
        if (lm.enableInApps)
            defines = defines + "; UNITY_INAPPS";

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defines);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, defines);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WP8, defines);

    }

    #region GUIRate

    void GUIRate()
    {

        GUILayout.Label("Rate settings:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        initscript.ShowRateEvery = EditorGUILayout.IntField("Show Rate after ", initscript.ShowRateEvery, new GUILayoutOption[] {
            GUILayout.Width (220),
            GUILayout.MaxWidth (220)
        });
        GUILayout.Label(" level (0 = disable)", EditorStyles.label, new GUILayoutOption[] { GUILayout.Width(150) });
        GUILayout.EndHorizontal();
        initscript.RateURL = EditorGUILayout.TextField("URL", initscript.RateURL, new GUILayoutOption[] {
            GUILayout.Width (220),
            GUILayout.MaxWidth (220)
        });
        initscript.RateURLIOS = EditorGUILayout.TextField("URL iOS", initscript.RateURLIOS, new GUILayoutOption[] {
            GUILayout.Width (220),
            GUILayout.MaxWidth (220)
        });

    }

    #endregion

    #region GUIDialogs

    void GUIDialogs()
    {
        GUILayout.Label("GUI elements:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });
        GUILayout.Space(10);
        ShowMenuButton("Menu Play", "MenuPlay");
        ShowMenuButton("Menu Complete", "MenuComplete");
        ShowMenuButton("Menu Failed", "MenuFailed");
        ShowMenuButton("Pause", "MenuPause");
        ShowMenuButton("Boost Shop", "BoostShop");
        ShowMenuButton("Live Shop", "LiveShop");
        ShowMenuButton("Gems Shop", "CoinsShop");
        ShowMenuButton("Reward", "Reward");
        ShowMenuButton("Tutorial", "Tutorial");

    }

    void ShowMenuButton(string label, string name)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, EditorStyles.label, new GUILayoutOption[] { GUILayout.Width(100) });
        GameObject obj = GameObject.Find("CanvasGlobal").transform.Find(name).gameObject;
        if (GUILayout.Button(obj.activeSelf ? "hide" : "show", new GUILayoutOption[] { GUILayout.Width(100) }))
        {
            EditorGUIUtility.PingObject(obj);
            Selection.activeGameObject = obj;
            obj.SetActive(!obj.activeSelf);
        }

        GUILayout.EndHorizontal();
    }

    public static void SetSearchFilter(string filter, int filterMode)
    {

        SearchableEditorWindow[] windows = (SearchableEditorWindow[])Resources.FindObjectsOfTypeAll(typeof(SearchableEditorWindow));
        SearchableEditorWindow hierarchy = null;
        foreach (SearchableEditorWindow window in windows)
        {

            if (window.GetType().ToString() == "UnityEditor.SceneHierarchyWindow")
            {

                hierarchy = window;
                break;
            }
        }

        if (hierarchy == null)
            return;

        MethodInfo setSearchType = typeof(SearchableEditorWindow).GetMethod("SetSearchFilter", BindingFlags.NonPublic | BindingFlags.Instance);
        object[] parameters = new object[] { filter, filterMode, false };

        setSearchType.Invoke(hierarchy, parameters);
    }

    #endregion

    #region ads_settings

    void RunOnceGoogle()
    {
        if (Directory.Exists("Assets/PlayServicesResolver"))
        {
            Debug.Log("assets try reimport");
#if GOOGLE_MOBILE_ADS && UNITY_ANDROID
            //			GooglePlayServices.PlayServicesResolver.MenuResolve ();1.4.5
            Debug.Log("assets reimorted");
            enableGoogleAdsProcessing = false;
#endif
        }


    }

    void GUIAds()
    {


        bool oldenableAds = initscript.enableUnityAds;

        GUILayout.Label("Ads settings:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });
        GUILayout.BeginHorizontal();

        //UNITY ADS

        //		initscript.enableUnityAds = EditorGUILayout.Toggle ("Enable Unity ads", initscript.enableUnityAds, new GUILayoutOption[] {//1.3
        //			GUILayout.Width (200)
        //		});
        GUILayout.Label("Unity ads", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });//1.3
        GUILayout.Label("Install: Windows->\n Services->Ads - ON", new GUILayoutOption[] { GUILayout.Width(130) });
        if (GUILayout.Button("Help", new GUILayoutOption[] { GUILayout.Width(80) }))
        {
            Application.OpenURL("https://docs.google.com/document/d/1HeN8JtQczTVetkMnd8rpSZp_TZZkEA7_kan7vvvsMw0");
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);


        //		if (oldenableAds != initscript.enableUnityAds)//1.3
        //			SetScriptingDefineSymbols ();
        //		if (initscript.enableUnityAds) {
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        initscript.rewardedGems = EditorGUILayout.IntField("Rewarded gems", initscript.rewardedGems, new GUILayoutOption[] {
            GUILayout.Width (200),
            GUILayout.MaxWidth (200)
        });
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        //		}

        //GOOGLE MOBILE ADS

        bool oldenableGoogleMobileAds = initscript.enableGoogleMobileAds;
        GUILayout.BeginHorizontal();
        //		initscript.enableGoogleMobileAds = EditorGUILayout.Toggle ("Enable Google Mobile Ads", initscript.enableGoogleMobileAds, new GUILayoutOption[] {//1.3
        //			GUILayout.Width (50),
        //			GUILayout.MaxWidth (200)
        //		});
        GUILayout.Label("Google mobile ads", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });//1.3
        if (GUILayout.Button("Install", new GUILayoutOption[] { GUILayout.Width(100) }))
        {
            Application.OpenURL("https://github.com/googleads/googleads-mobile-unity/releases");//1.3
        }
        if (GUILayout.Button("Help", new GUILayoutOption[] { GUILayout.Width(80) }))
        {
            Application.OpenURL("https://docs.google.com/document/d/1I69mo9yLzkg35wtbHpsQd3Ke1knC5pf7G1Wag8MdO-M/edit?usp=sharing");
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        //		if (oldenableGoogleMobileAds != initscript.enableGoogleMobileAds) {//1.3
        //
        //			SetScriptingDefineSymbols ();
        //			if (initscript.enableGoogleMobileAds) {
        //				enableGoogleAdsProcessing = true;
        //			}
        //		}
        //		if (initscript.enableGoogleMobileAds) {

        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        initscript.admobUIDAndroid = EditorGUILayout.TextField("Admob Interstitial ID Android ", initscript.admobUIDAndroid, new GUILayoutOption[] {
            GUILayout.Width (220),
            GUILayout.MaxWidth (220)
        });
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        initscript.admobUIDIOS = EditorGUILayout.TextField("Admob Interstitial ID iOS", initscript.admobUIDIOS, new GUILayoutOption[] {
            GUILayout.Width (220),
            GUILayout.MaxWidth (220)
        });
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        //		}

        //CHARTBOOST ADS

        GUILayout.BeginHorizontal();
        bool oldenableChartboostAds = initscript.enableChartboostAds;
        //		initscript.enableChartboostAds = EditorGUILayout.Toggle ("Enable Chartboost Ads", initscript.enableChartboostAds, new GUILayoutOption[] {
        //			GUILayout.Width (50),
        //			GUILayout.MaxWidth (200)
        //		});
        GUILayout.Label("Chartboost ads", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });//1.3
        if (GUILayout.Button("Install", new GUILayoutOption[] { GUILayout.Width(100) }))
        {
            Application.OpenURL("http://www.chartboo.st/sdk/unity");//1.3
        }
        if (GUILayout.Button("Help", new GUILayoutOption[] { GUILayout.Width(80) }))
        {
            Application.OpenURL("https://docs.google.com/document/d/1ibnQbuxFgI4izzyUtT45WH5m1ab3R5d1E3ke3Wrb10Y");
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        //		if (oldenableChartboostAds != initscript.enableChartboostAds) {//1.3
        //			SetScriptingDefineSymbols ();
        //			if (initscript.enableChartboostAds) {
        //				//enableChartboostAdsProcessing = true;
        //			}
        //
        //		}
        //		if (initscript.enableChartboostAds) {
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("menu Chartboost->Edit settings", new GUILayoutOption[] {
            GUILayout.Width (50),
            GUILayout.MaxWidth (200)
        });
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Put ad ID to appropriate platform to prevent crashing!", EditorStyles.boldLabel, new GUILayoutOption[] {
            GUILayout.Width (100),
            GUILayout.MaxWidth (400)
        });
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        //		}


        GUILayout.Space(10);

        GUILayout.Label("Ads controller:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });

        EditorGUILayout.Space();

        GUILayout.Label("Event:               Status:                            Show after n call:", new GUILayoutOption[] { GUILayout.Width(350) });



        foreach (AdEvents item in initscript.adsEvents)
        {
            EditorGUILayout.BeginHorizontal();
            item.gameEvent = (GameState)EditorGUILayout.EnumPopup(item.gameEvent, new GUILayoutOption[] { GUILayout.Width(100) });
            item.adType = (AdType)EditorGUILayout.EnumPopup(item.adType, new GUILayoutOption[] { GUILayout.Width(150) });
            item.everyLevel = EditorGUILayout.IntPopup(item.everyLevel, new string[] {
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                "10"
            }, new int[] {
                1,
                2,
                3,
                4,
                5,
                6,
                7,
                8,
                9,
                10
            }, new GUILayoutOption[] { GUILayout.Width(100) });

            EditorGUILayout.EndHorizontal();

        }
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add"))
        {
            AdEvents adevent = new AdEvents();
            adevent.everyLevel = 1;
            initscript.adsEvents.Add(adevent);

        }
        if (GUILayout.Button("Delete"))
        {
            if (initscript.adsEvents.Count > 0)
                initscript.adsEvents.Remove(initscript.adsEvents[initscript.adsEvents.Count - 1]);

        }


        GUILayout.Space(10);



    }

    #endregion

    #region inapps_settings

    void Save()
    {
        EditorUtility.SetDirty(lm);
        AssetDatabase.SaveAssets();
    }

    void GUIInappSettings()
    {

        GUILayout.Label("In-apps settings:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });

        if (GUILayout.Button("Reset to default", new GUILayoutOption[] { GUILayout.Width(150) }))
        {
            ResetInAppsSettings();
        }

        GUILayout.Space(10);

        bool oldenableInApps = lm.enableInApps;

        GUILayout.BeginHorizontal();
        //		lm.enableInApps = EditorGUILayout.Toggle ("Enable In-apps", lm.enableInApps, new GUILayoutOption[] {//1.3
        //			GUILayout.Width (180)
        //		});
        if (GUILayout.Button("Help", new GUILayoutOption[] { GUILayout.Width(80) }))
        {
            Application.OpenURL("https://docs.google.com/document/d/1HeN8JtQczTVetkMnd8rpSZp_TZZkEA7_kan7vvvsMw0#bookmark=id.b1efplsspes5");
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUILayout.Label("Install: Windows->Services->\n In-app Purchasing - ON->Import", new GUILayoutOption[] { GUILayout.Width(400) });
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.Space(10);

        //		if (oldenableInApps != lm.enableInApps) {//1.3
        //			SetScriptingDefineSymbols ();
        //		}


        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginVertical();
        for (int i = 0; i < lm.InAppIDs.Length; i++)
        {
            lm.InAppIDs[i] = EditorGUILayout.TextField("Product id " + (i + 1), lm.InAppIDs[i], new GUILayoutOption[] {
                GUILayout.Width (300),
                GUILayout.MaxWidth (300)
            });

        }
        GUILayout.Space(10);

        GUILayout.Label("Android:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        GUILayout.BeginVertical();
        GUILayout.Space(10);
        // GUILayout.Label(" Put Google license key into the field \n from the google play account ", EditorStyles.label, new GUILayoutOption[] { GUILayout.Width(300) });
        // GUILayout.Space(10);

        // lm.GoogleLicenseKey = EditorGUILayout.TextField("Google license key", lm.GoogleLicenseKey, new GUILayoutOption[] {
        //     GUILayout.Width (300),
        //     GUILayout.MaxWidth (300)
        // });

        GUILayout.Space(10);
        if (GUILayout.Button("Android account help", new GUILayoutOption[] { GUILayout.Width(400) }))
        {
            Application.OpenURL("http://developer.android.com/google/play/billing/billing_admin.html");
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginVertical();

        GUILayout.Space(10);
        GUILayout.Label("iOS:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        GUILayout.BeginVertical();

        // GUILayout.Label(" StoreKit library must be added \n to the XCode project, generated by Unity ", EditorStyles.label, new GUILayoutOption[] { GUILayout.Width(300) });
        GUILayout.Space(10);
        if (GUILayout.Button("iOS account help", new GUILayoutOption[] { GUILayout.Width(400) }))
        {
            Application.OpenURL("https://developer.apple.com/library/ios/qa/qa1329/_index.html");
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

    }

    void ResetInAppsSettings()
    {
        lm.InAppIDs[0] = "gems10";
        lm.InAppIDs[1] = "gems50";
        lm.InAppIDs[2] = "gems100";
        lm.InAppIDs[3] = "gems150";
    }

    #endregion

    void GUIHelp()
    {
        GUILayout.Label("JewelryMatch3Kit - v 1.1.8", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(350) });
        GUILayout.Space(10);

        GUILayout.Label("Please read our documentation:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(200) });
        if (GUILayout.Button("DOCUMENTATION", new GUILayoutOption[] { GUILayout.Width(150) }))
        {
            Application.OpenURL("https://docs.google.com/document/d/1b6Qe-CmnLX1VbV1Gw-blR5IlFGuxdDXCX3ZUjlpiV4c");
        }
        GUILayout.Space(10);
        GUILayout.Label("To start work with project - \n go to Editor Section of this menu", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(350) });
        GUILayout.Space(10);
        GUILayout.Label("To get support you should provide \n ORDER NUMBER (asset store) \n or NICKNAME and DATE of purchase (other stores):", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(350) });
        GUILayout.Space(10);
        GUILayout.TextArea("info@candy-smith.com", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(350) });

    }

    #region settings
    private bool share_settings;

    void GUISettings()
    {
        GUILayout.Label("Game settings:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });

        GUILayout.BeginHorizontal();//1.3.3
        if (GUILayout.Button("Reset to default", new GUILayoutOption[] { GUILayout.Width(150) }))
        {
            ResetSettings();
        }
        if (GUILayout.Button("Clear player prefs", new GUILayoutOption[] { GUILayout.Width(150) }))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("Player prefs cleared");
        }
        GUILayout.EndHorizontal();//1.3.3

        GUILayout.Space(10);

        // bool oldFacebookEnable = lm.FacebookEnable;
        // GUILayout.BeginHorizontal();
        // GUILayout.Label("Facebook", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });//1.6.1
        // if (GUILayout.Button("Install", new GUILayoutOption[] { GUILayout.Width(70) }))
        // {
        //     Application.OpenURL("https://developers.facebook.com/docs/unity/downloads");
        // }
        // if (GUILayout.Button("Account", new GUILayoutOption[] { GUILayout.Width(70) }))
        // {
        //     Application.OpenURL("https://developers.facebook.com");
        // }
        // if (GUILayout.Button("Help", new GUILayoutOption[] { GUILayout.Width(60) }))
        // {
        //     Application.OpenURL("https://docs.google.com/document/d/1bTNdM3VSg8qu9nWwO7o7WeywMPhVLVl8E_O0gMIVIw0/edit?usp=sharing");
        // }
        // GUILayout.EndHorizontal();

        // #if FACEBOOK
        // 		share_settings = EditorGUILayout.Foldout(share_settings, "Share settings:");
        // 		if (share_settings)
        // 		{
        // 			GUILayout.BeginHorizontal();
        // 			GUILayout.Space(30);
        // 			GUILayout.BeginVertical();
        // 			{
        // 				lm.androidSharingPath = EditorGUILayout.TextField("Android path", lm.androidSharingPath, new GUILayoutOption[] { GUILayout.MaxWidth(500) });
        // 				lm.iosSharingPath = EditorGUILayout.TextField("iOS path", lm.iosSharingPath, new GUILayoutOption[] { GUILayout.MaxWidth(500) });
        // 			}
        // 			GUILayout.EndVertical();
        // 			GUILayout.EndHorizontal();

        // 			GUILayout.Space(10);
        // 		}
        // #endif

        //         GUILayout.BeginHorizontal();
        //         GUILayout.Label("Leadboard Gamesparks", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });//1.6.1
        //         if (GUILayout.Button("Install", new GUILayoutOption[] { GUILayout.Width(70) }))
        //         {
        //             Application.OpenURL("https://docs.gamesparks.com/sdk-center/unity.html");
        //         }
        //         if (GUILayout.Button("Account", new GUILayoutOption[] { GUILayout.Width(70) }))
        //         {
        //             Application.OpenURL("https://portal.gamesparks.net");
        //         }
        //         if (GUILayout.Button("Help", new GUILayoutOption[] { GUILayout.Width(60) }))
        //         {
        //             Application.OpenURL("https://docs.google.com/document/d/1JcQfiiD2ALz6v_i9UIcG93INWZKC7z6FHXH_u6w9A8E");
        //         }
        //         GUILayout.EndHorizontal();
        // #if GAMESPARKS
        // 		GUILayout.BeginHorizontal();
        // 		{
        //             GUILayout.Space(150);
        // 			if(GUILayout.Button("Create game", GUILayout.Width(100)))//1.4.9
        // 			{
        // 				GamesparksConfiguration window = ScriptableObject.CreateInstance<GamesparksConfiguration>();
        // 				window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 200);
        // 				window.ShowPopup();
        // 			}

        // 		}
        // 		GUILayout.EndHorizontal();
        // #endif

        //		if (oldFacebookEnable != lm.FacebookEnable) {//1.3
        //			SetScriptingDefineSymbols ();
        //		}
        // if (lm.FacebookEnable)
        // {
        //     GUILayout.BeginHorizontal();
        //     GUILayout.Space(20);
        //     GUILayout.Label("menu Facebook-> Edit settings", new GUILayoutOption[] { GUILayout.Width(300) });
        //     GUILayout.EndHorizontal();
        // }

        // GUILayout.Space(10);

        life_settings_show = EditorGUILayout.Foldout(life_settings_show, "Lifes settings:");
        if (life_settings_show)
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            GUILayout.BeginVertical();


            initscript.CapOfLife = EditorGUILayout.IntField("Max of lifes", initscript.CapOfLife, new GUILayoutOption[] {
                GUILayout.Width (200),
                GUILayout.MaxWidth (200)
            });
            GUILayout.Space(10);

            GUILayout.Label("Total time for refill lifes:", EditorStyles.label);
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            GUILayout.Label("Hour", EditorStyles.label, GUILayout.Width(50));
            GUILayout.Label("Min", EditorStyles.label, GUILayout.Width(50));
            GUILayout.Label("Sec", EditorStyles.label, GUILayout.Width(50));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            initscript.TotalTimeForRestLifeHours = EditorGUILayout.FloatField("", initscript.TotalTimeForRestLifeHours, new GUILayoutOption[] { GUILayout.Width(50) });
            initscript.TotalTimeForRestLifeMin = EditorGUILayout.FloatField("", initscript.TotalTimeForRestLifeMin, new GUILayoutOption[] { GUILayout.Width(50) });
            initscript.TotalTimeForRestLifeSec = EditorGUILayout.FloatField("", initscript.TotalTimeForRestLifeSec, new GUILayoutOption[] { GUILayout.Width(50) });
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            lm.CostIfRefill = EditorGUILayout.IntField("Cost of refilling lifes", lm.CostIfRefill, new GUILayoutOption[] {
                GUILayout.Width (200),
                GUILayout.MaxWidth (200)
            });
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        GUILayout.Space(20);

        initscript.FirstGems = EditorGUILayout.IntField("Start gems", initscript.FirstGems, new GUILayoutOption[] {
            GUILayout.Width (200),
            GUILayout.MaxWidth (200)
        });
        GUILayout.Space(20);

        initscript.losingLifeEveryGame = EditorGUILayout.Toggle("Losing a life every game", initscript.losingLifeEveryGame, new GUILayoutOption[] {
            GUILayout.Width (200),
            GUILayout.MaxWidth (200)
        });
        GUILayout.Space(20);


        failed_settings_show = EditorGUILayout.Foldout(failed_settings_show, "Failed settings:");
        if (failed_settings_show)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            GUILayout.BeginVertical();

            lm.FailedCost = EditorGUILayout.IntField(new GUIContent("Cost of continue", "Cost of continue after failed"), lm.FailedCost, new GUILayoutOption[] {
                GUILayout.Width (200),
                GUILayout.MaxWidth (200)
            });
            lm.ExtraFailedMoves = EditorGUILayout.IntField(new GUIContent("Extra moves", "Extra moves after continue"), lm.ExtraFailedMoves, new GUILayoutOption[] {
                GUILayout.Width (200),
                GUILayout.MaxWidth (200)
            });
            // lm.ExtraFailedSecs = EditorGUILayout.IntField(new GUIContent("Extra seconds", "Extra seconds after continue"), lm.ExtraFailedSecs, new GUILayoutOption[] {
            //     GUILayout.Width (200),
            //     GUILayout.MaxWidth (200)
            // });
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

        }
        GUILayout.Space(20);
        GUILayout.Label("Level Color");
        lm.levelColor = EditorGUILayout.ColorField(lm.levelColor);
        //  EditorUtility.SetDirty(lm);
    }

    void ResetSettings()
    {
        initscript.CapOfLife = 5;
        initscript.TotalTimeForRestLifeHours = 0;
        initscript.TotalTimeForRestLifeMin = 15;
        initscript.TotalTimeForRestLifeSec = 0;
        lm.CostIfRefill = 12;
        lm.FailedCost = 12;
        lm.ExtraFailedMoves = 5;
        EditorUtility.SetDirty(lm);
    }

    #endregion

    #region shop

    void GUIShops()
    {

        GUILayout.Label("Shop settings:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });

        if (GUILayout.Button("Reset to default", new GUILayoutOption[] { GUILayout.Width(150) }))
        {
            ResetShops();
        }
        GUILayout.Space(10);
        gems_shop_show = EditorGUILayout.Foldout(gems_shop_show, "Gems shop settings:");
        if (gems_shop_show)
        {
            int i = 1;
            foreach (GemProduct item in lm.gemsProducts)
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                GUILayout.Label("Gems count", new GUILayoutOption[] { GUILayout.Width(100) });
                GUILayout.Label("Price $", new GUILayoutOption[] { GUILayout.Width(100) });
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                item.count = EditorGUILayout.IntField("", item.count, new GUILayoutOption[] {
                    GUILayout.Width (100),
                    GUILayout.MaxWidth (100)
                });
                item.price = EditorGUILayout.FloatField("", item.price, new GUILayoutOption[] {
                    GUILayout.Width (100),
                    GUILayout.MaxWidth (100)
                });
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                i++;
            }
        }

        GUILayout.Space(10);
        boost_show = EditorGUILayout.Foldout(boost_show, "Boosts shop settings:");
        if (boost_show)
        {
            BoostShop bs = GameObject.Find("CanvasGlobal").transform.Find("BoostShop").GetComponent<BoostShop>();
            List<BoostProduct> bp = bs.boostProducts;
            foreach (BoostProduct item in bp)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Description");
                item.description = EditorGUILayout.TextField("", item.description, new GUILayoutOption[] {
                    GUILayout.Width (400),
                    GUILayout.MaxWidth (400)
                });
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                // GUILayout.Label(item.icon.texture, new GUILayoutOption[] { GUILayout.Width(50), GUILayout.Height(50) });
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();

                GUILayout.Label("Count", EditorStyles.label, new GUILayoutOption[] { GUILayout.Width(80) });
                GUILayout.Label("Price(gem)", EditorStyles.label, new GUILayoutOption[] { GUILayout.Width(80) });

                GUILayout.EndHorizontal();

                for (int i = 0; i < 3; i++)
                {
                    GUILayout.BeginHorizontal();

                    item.count[i] = EditorGUILayout.IntField("", item.count[i], new GUILayoutOption[] {
                        GUILayout.Width (80),
                        GUILayout.MaxWidth (80)
                    });
                    item.GemPrices[i] = EditorGUILayout.IntField("", item.GemPrices[i], new GUILayoutOption[] {
                        GUILayout.Width (80),
                        GUILayout.MaxWidth (80)
                    });
                    GUILayout.EndHorizontal();

                }
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.Space(20);
            }

        }
    }

    void ResetShops()
    {

        lm.gemsProducts[0].count = 10;
        lm.gemsProducts[0].price = 0.99f;
        lm.gemsProducts[1].count = 50;
        lm.gemsProducts[1].price = 4.99f;
        lm.gemsProducts[2].count = 100;
        lm.gemsProducts[2].price = 9.99f;
        lm.gemsProducts[3].count = 150;
        lm.gemsProducts[3].price = 14.99f;

        BoostShop bs = GameObject.Find("CanvasGlobal").transform.Find("BoostShop").GetComponent<BoostShop>();
        bs.boostProducts[0].description = "Gives you the 5 extra moves";
        bs.boostProducts[1].description = "Place this special item in game";
        bs.boostProducts[2].description = "Place this special item in game";
        bs.boostProducts[3].description = "Gives you the 30 extra seconds";
        bs.boostProducts[4].description = "Destroy the item";
        bs.boostProducts[5].description = "Place this special item in game";
        bs.boostProducts[6].description = "Switch to item that don't match";
        bs.boostProducts[7].description = "Replace the near items color";

        for (int i = 0; i < 8; i++)
        {
            bs.boostProducts[i].count[0] = 3;
            bs.boostProducts[i].count[1] = 5;
            bs.boostProducts[i].count[2] = 10;

            bs.boostProducts[i].GemPrices[0] = 5;
            bs.boostProducts[i].GemPrices[1] = 6;
            bs.boostProducts[i].GemPrices[2] = 11;

        }
        EditorUtility.SetDirty(lm);
        EditorUtility.SetDirty(bs);

    }

    #endregion

    #region leveleditor

    void TestLevel(bool playNow = true, bool testByPlay = true)
    {
        PlayerPrefs.SetInt("OpenLevelTest", levelNumber);
        PlayerPrefs.SetInt("OpenLevel", levelNumber);
        PlayerPrefs.Save();
        if (!testByPlay)
        {
            PlayerPrefs.SetInt("OpenLevelTest", 0);
            PlayerPrefs.SetInt("OpenLevel", 0);
            PlayerPrefs.Save();
        }

        if (playNow)
        {
            if (EditorApplication.isPlaying)
                EditorApplication.isPlaying = false;
            else
                EditorApplication.isPlaying = true;
        }

        // lm.LoadLevel();
    }

    void SaveLevel()
    {
        if (!fileName.Contains(".txt"))
            fileName += ".txt";
    }


    public bool LoadDataFromLocal(int currentLevel)
    {
        //Read data from text file
        TextAsset mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
        if (mapText == null)
        {
            return false;
            SaveLevel();
            mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
        }
        ProcessGameDataFromString(mapText.text);
        return true;
    }

    void ProcessGameDataFromString(string mapText)
    {

        string[] lines = mapText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        int[] indexItems = new int[10];

    }

    #endregion
}

[System.Serializable]
public class CollectedItem
{
    public string name;
    public bool check;
    public int count;
    public bool enable;

}
