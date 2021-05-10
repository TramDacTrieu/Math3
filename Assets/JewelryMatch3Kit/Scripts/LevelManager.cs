using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using JewelryMatch3Kit.Scripts.Diamond;
using System.Text.RegularExpressions;
using UnityStandardAssets._2D;
using UnityEngine.Profiling;

[System.Serializable]
public enum GameState
{
    Map = 0,
    PrepareGame,
    PrepareBoosts,
    Playing,
    Highscore,
    GameOver,
    Pause,
    WaitBeforeWin,
    PreWinAnimations,
    Win,
    WaitForPopup,
    WaitAfterClose,
    BlockedGame,
    Tutorial,
    PreTutorial,
    WaitForPotion,
    PreFailed,
    PreFailedBomb,
    RegenLevel
}


public class LevelManager : MonoBehaviour
{
    //inctance of LevelManager for direct references
    public static LevelManager THIS;

    //inctance of LevelManager for direct references
    public static LevelManager Instance;

    //life shop scene object
    private LifeShop lifeShop;

    //array of combined items
    List<List<Item>> combinedItems = new List<List<Item>>();

    // latest touched item
    public IColorableComponent lastDraggedItem;

    //array for items prepeared to destory
    public List<IDestroyableComponent> destroyAnyway = new List<IDestroyableComponent>();

    //amount of scores for item
    public int scoreForItem = 10;
    
    [SerializeField] private GameObject EndingObject;

    //type of game limit (moves or time)
    public LIMIT limitType;

    //value of rest limit (moves or time)
    public int Limit = 30;

    //deprecated
    public int TargetScore = 1000;

    //current level number
    public int currentLevel = 1;

    //cost of continue playing after fail
    public int FailedCost;

    //extra moves that you get to continue game after fail
    public int ExtraFailedMoves = 5;

    //extra seconds that you get to continue game after fail
    public int ExtraFailedSecs = 30;

    // array of iapps products
    public List<GemProduct> gemsProducts = new List<GemProduct>();

    // product IDs
    public string[] InAppIDs;

    // google licnse key
    public string GoogleLicenseKey;

    //line object for effect
    public Line line;

    //is any growing blocks destroyed in that turn
    public bool thrivingBlockDestroyed;

    //inner using variable
    List<List<Item>> newCombines;

    // amount of boosts
    public int BoostFlower;
    public int BoostMulticolor;
    public int BoostSpiral;
    public bool Hammer;

    //inner using variable
    public BoostIcon emptyBoostIcon;

    // deprecated
    public BoostIcon AvctivatedBoostView;

    //currently active boost
    public BoostIcon activatedBoost;
    public string androidSharingPath;
    public string iosSharingPath;

    // field of getting and setting currently activated boost
    public BoostIcon ActivatedBoost
    {
        get
        {
            if (activatedBoost == null)
            {
                //BoostIcon bi = new BoostIcon();
                //bi.type = BoostType.None;
                return emptyBoostIcon;
            }
            else
                return activatedBoost;
        }
        set
        {
            if (value == null)
            {
                if (activatedBoost != null && gameStatus == GameState.Playing)
                    InitScript.Instance.SpendBoost(activatedBoost.type);
                UnLockBoosts();
            }

            //        if (activatedBoost != null) return;
            activatedBoost = value;

            if (value != null)
            {
                LockBoosts();
            }

            if (activatedBoost != null)
            {
                if (activatedBoost.type == BoostType.Moves)
                {
                    if (LevelManager.Instance.limitType == LIMIT.MOVES)
                        LevelManager.THIS.Limit += 5;
                    ActivatedBoost = null;
                }
            }
        }
    }

    //amount of blocks for collecting
    public int targetBlocks;

    //pool of explosion effects for items
    public GameObject[] itemExplPool = new GameObject[20];

    //pool of flowers
    public GameObject[] flowersPool = new GameObject[20];

    //global Score amount on current level
    public static int Score;

    // stars amount on current level
    public int stars;

    private int linePoint;    //deprecated

    // amount of scores is necessary for reaching first star
    public int star1;

    // amount of scores is necessary for reaching second star
    public int star2;

    // amount of scores is necessary for reaching third star
    public int star3;

    //editor option to show popup scores
    public bool showPopupScores;

    //inner using
    int nextExtraItems;

    //prefab of row explosion effect
    //UI star object
    public GameObject star1Anim;

    //UI star object
    public GameObject star2Anim;

    //UI star object
    public GameObject star3Anim;

    //snow particle prefab
    public GameObject snowParticle;

    //array of colors for popup scores
    public Color[] scoresColors;

    //array of outline colors for popup scores
    public Color[] scoresColorsOutline;

    //editor variable for limitation of colors
    public int colorLimit;
    public TouchBlocker touchBlocker;
    private TipsManager tipsManager;

    //necessary amount of collectable items
    public int[] ingrCountTarget = new int[4];

    //necessary collectable items
    public List<CollectedIngredients> ingrTarget = new List<CollectedIngredients>();

    //necessary collectable items
    CollectItems[] collectItems = new CollectItems[6];

    //sprites of collectable items
    public Sprite[] ingrediendSprites;

    //editor values of description tasks
    public string[] targetDiscriptions;

    //UI object
    public GameObject ingrObject;

    //UI object
    public GameObject blocksObject;

    //UI object
    public GameObject scoreTargetObject;

    //UI object
    public GameObject cageTargetObject;

    //UI object
    public GameObject bombTargetObject;

    public GameObject diamondsTargetObject;

    //inner using
    private bool matchesGot;

    //inner using
    bool ingredientFly;

    //UI objects
    public GameObject[] gratzWords;

    //UI object
    public GameObject Level;

    //scene object
    public GameObject LevelsMap;

    public BoostIcon[] InGameBoosts;
    public List<IDestroyableComponent> gatheredTypes = new List<IDestroyableComponent>();
    List<Vector3> startPosFlowers = new List<Vector3>();
    public List<GameObject> friendsAvatars = new List<GameObject>();

    public Target target;

    public int TargetBlocks
    {
        get { return targetBlocks; }
        set
        {
            if (targetBlocks < 0)
                targetBlocks = 0;
            targetBlocks = value;
        }
    }

    public int TargetCages;
    public int TargetDiamonds;
    public int TargetBombs;



    int cageHP;

    private GameState GameStatus;
    public bool itemsHided;
    public int moveID;
    public int lastRandColor;
    public bool onlyFalling;
    public bool levelLoaded;
    public Hashtable countedSquares;
    public Sprite doubleBlock;
    public Sprite doubleSolidBlock;
    public bool FacebookEnable;
    public bool PlayFab;
    public int selectedColor;
    private bool stopSliding;
    private float offset;
    public GameObject flower;
    public float extraItemEvery = 6;

    public int bombsCollect;

    //1.4.5
    public int bombTimer;
    public int NumIngredients = 4;

    #region EVENTS

    public delegate void GameStateEvents();

    public static event GameStateEvents OnMapState;
    public static event GameStateEvents OnEnterGame;
    public static event GameStateEvents OnLevelLoaded;
    public static event GameStateEvents OnMenuPlay;
    public static event GameStateEvents OnMenuComplete;
    public static event GameStateEvents OnStartPlay;
    public static event GameStateEvents OnWin;
    public static event GameStateEvents OnLose;
    public static event GameStateEvents OnNextMove;


    public GameState gameStatus
    {
        get { return GameStatus; }
        set
        {
            GameStatus = value;
            Debug.Log(value);
            checkAds = true;
            if (value == GameState.PrepareGame)
            {
                // MusicBase.Instance.GetComponent<AudioSource>().Stop();
                // MusicBase.Instance.GetComponent<AudioSource>().loop = true;
                // MusicBase.Instance.GetComponent<AudioSource>().clip = MusicBase.Instance.music[1];
                // MusicBase.Instance.GetComponent<AudioSource>().Play();
                if (SceneManager.GetActiveScene().name == "map")
                {
                    GameObject.Find("CanvasGlobal").transform.Find("Loading").gameObject.SetActive(true);
                    SceneManager.LoadScene("level" + currentLevel);
                }
                else
                {
                    PrepareGame();
                    InitLevel();
                    OnEnterGame?.Invoke();
                }
            }
            else if (value == GameState.WaitForPopup)
            {
                Level.transform.Find("Canvas/PrePlay").gameObject.SetActive(true); //1.3.3
                OnLevelLoaded();
                GameObject poptxt = ObjectPooler.Instance.GetPooledObject("score");
                poptxt.SetActive(false);
            }
            else if (value == GameState.PreFailedBomb)
            {
            }
            else if (value == GameState.PreFailed)
            {
                Level.transform.Find("Canvas/PreFailed").gameObject.SetActive(true);
            }
            else if (value == GameState.Map)
            {
                if (PlayerPrefs.GetInt("OpenLevelTest") <= 0 && SceneManager.GetActiveScene().name == "map")
                {
                    // MusicBase.Instance.GetComponent<AudioSource>().Stop();
                    // MusicBase.Instance.GetComponent<AudioSource>().loop = true;
                    // MusicBase.Instance.GetComponent<AudioSource>().clip = MusicBase.Instance.music[];
                    // MusicBase.Instance.GetComponent<AudioSource>().Play();
                    //					EnableMap(true);
                    OnMapState();
                }

                else if (SceneManager.GetActiveScene().name.Contains("level"))
                {
                    checkAds = false;
                    gameStatus = GameState.PrepareGame;
                    return;
                }

                // #if UNITY_ANDROID || UNITY_IOS || UNITY_WINRT
                if (AdsCounter.THIS.passLevelCounter > 0 && InitScript.Instance.ShowRateEvery > 0)
                {
                    if (AdsCounter.THIS.passLevelCounter % InitScript.Instance.ShowRateEvery == 0 &&
                        InitScript.Instance.ShowRateEvery > 0 && PlayerPrefs.GetInt("Rated", 0) == 0)
                        InitScript.Instance.ShowRate();
                }
                // #endif
            }
            else if (value == GameState.Tutorial)
            {
                if (LevelSettings.THIS.LevelScriptable.Tutorial > 0)
                {
                    GameObject.Find("CanvasGlobal").transform.Find("Tutorial").gameObject.SetActive(true);
                    Tutorial.THIS.ShowInGameTutorial();
                }
                else
                    gameStatus = GameState.Playing;
            }
            else if (value == GameState.Pause)
            {
                Time.timeScale = 0;
            }
            else if (value == GameState.PrepareBoosts)
            {
            }
            else if (value == GameState.Playing)
            {
                Time.timeScale = 1;


                //				StartCoroutine(TipsManager.THIS.CheckPossibleCombines());
            }
            else if (value == GameState.GameOver)
            {
                InitScript.Instance.SpendLife(1);
                MusicBase.Instance.GetComponent<AudioSource>().Stop();
                SoundBase.Instance.PlaySound(SoundBase.Instance.gameOver[0]);
                GameObject.Find("CanvasGlobal").transform.Find("MenuFailed").gameObject.SetActive(true);
                OnLose?.Invoke();
            }
            else if (value == GameState.PreWinAnimations)
            {
                // ClearSelections();
                MusicBase.Instance.GetComponent<AudioSource>().Stop();
                StartCoroutine(PreWinAnimationsCor());
            }
            else if (value == GameState.Win)
            {
                AdsCounter.THIS.passLevelCounter++;
                OnMenuComplete?.Invoke();
                GameObject.Find("CanvasGlobal").transform.Find("MenuComplete").gameObject.SetActive(true);
                OnWin?.Invoke();
            }
            if (checkAds)
                InitScript.Instance.CheckAdsEvents(value);
        }
    }

    private void OnEnable()
    {
        OnNextMove += CheckWin;

    }
    private void OnDisable()
    {
        OnNextMove -= CheckWin;
    }
    public int TotalCollect { get; private set; }

    public LifeShop LifeShop
    {
        get
        {
            if (lifeShop == null) lifeShop = GameObject.Find("CanvasGlobal").GetComponentInChildren<LifeShop>(true);
            return lifeShop;
        }

        set
        {
            lifeShop = value;
        }
    }

    public Transform CanvasScore { get; private set; }
    public Transform Canvas { get; private set; }

    public void MenuPlayEvent()
    {
        OnMenuPlay();
    }

    #endregion


    void LockBoosts()
    {
        foreach (BoostIcon item in InGameBoosts)
        {
            if (item != ActivatedBoost)
                item.LockBoost();
        }
    }

    public void UnLockBoosts()
    {
        foreach (BoostIcon item in InGameBoosts)
        {
            item.UnLockBoost();
        }
    }


    public void LoadLevel(int num)
    {
        currentLevel = num;//PlayerPrefs.GetInt("OpenLevel"); // TargetHolder.level;
        if (currentLevel == 0)
            currentLevel = 1;
        //		LoadDataFromLocal(currentLevel);
        //		NumIngredients = ingrTarget.Count;
        LevelSettings levelSettings = null;
        if (SceneManager.GetActiveScene().name == "map")
        {
            levelSettings = new LevelSettings();
            levelSettings.LoadLevel(currentLevel);
        }
        else
        {
            levelSettings = GameObject.FindObjectOfType<LevelSettings>();
        }

        target = Target.DIAMONDS;
        Limit = levelSettings.LevelScriptable.Moves;
        limitType = LIMIT.MOVES; //levelSettings.LevelScriptable.LimitType;
        colorLimit = levelSettings.LevelScriptable.ColorLimit;
        star1 = levelSettings.LevelScriptable.Stars[0];
        star2 = levelSettings.LevelScriptable.Stars[1];
        star3 = levelSettings.LevelScriptable.Stars[2];

        //		if(target == Target.DIAMONDS)
        //			TargetDiamonds = levelSettings.CollectCount;
    }

    void SetupGameCamera()
    {
        float aspect = (float)Screen.height / (float)Screen.width;
        Camera.main.orthographicSize = 10.05f;

        aspect = (float)Math.Round(aspect, 2);
    }
    
    int GetLastLevel()
    {
        for (int i = currentLevel; i < 50000; i++)
        {
            LevelScriptable lvl = Resources.Load("Levels/level" + i) as LevelScriptable;
            if (lvl == null)
            {
                return i - 1;
            }
        }
        return 0;
    }

    public bool IsLastLevelPassed()
    {
        var lastestReachedLevel = LevelsMap.GetComponent<LevelsMap>().GetLastestReachedLevel();
        return lastestReachedLevel >= GetLastLevel() && PlayerPrefs.GetInt(string.Format("Level.{0:000}.StarsCount", lastestReachedLevel), 0) > 0;
    }

    public void EnableMap(bool enable)
    {
        float aspect = (float)Screen.height / (float)Screen.width; //1.4.7
        aspect = (float)Math.Round(aspect, 2);
        var camera = Camera.main.GetComponent<Camera>();
        if (enable)
        {
            if (!MusicBase.Instance.GetComponent<AudioSource>().isPlaying)
                MusicBase.Instance.GetComponent<AudioSource>().Play();


            camera.orthographicSize = 10.25f;

            if (aspect == 1.6f)
                camera.orthographicSize = 12.2f; //16:10
            else if (aspect == 1.78f)
                camera.orthographicSize = 13.6f; //16:9
            else if (aspect == 1.5f)
                camera.orthographicSize = 11.2f; //3:2
            else if (aspect == 1.33f)
                camera.orthographicSize = 10.25f; //4:3
            else if (aspect == 1.67f)
                camera.orthographicSize = 12.5f; //5:3
            else if (aspect == 2.06f)
                camera.orthographicSize = 15.75f; //2960:1440 S8   //1.4.7
            else if (aspect == 2.17f)
                camera.orthographicSize = 16.5f; //iphone x    //1.4.7
            //else if (aspect == 1.25f)
            //    GetComponent<Camera>().orthographicSize = 4.9f;                  //5:4

            camera.GetComponent<MapCamera>().SetPosition(new Vector2(0, camera.transform.position.y));
        }
        else
        {
            Physics2D.autoSimulation = true;
            InitScript.DateOfExit = DateTime.Now.ToString();
            SetupGameCamera();

            if (aspect == 2.06f)
                camera.orthographicSize = 11.5f; //2960:1440 S8
            else if (aspect == 2.17f)
                camera.orthographicSize = 12.26f; //iphone x
            GameObject.Find("CanvasGlobal").GetComponent<GraphicRaycaster>().enabled = false;
            GameObject.Find("CanvasGlobal").GetComponent<GraphicRaycaster>().enabled = true;
            var canvas = Level.transform.Find("Canvas");
            canvas.GetComponent<GraphicRaycaster>().enabled = false;
            canvas.GetComponent<GraphicRaycaster>().enabled = true;
            var panel = canvas.transform.Find("Panel/Panel").GetComponent<RectTransform>();
            if (aspect == 2.17f) //1.4.9
                panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, 935) + Vector2.down * 100;
        }

        //		camera.GetComponent<MapCamera>().enabled = enable;
        //1.4.4
        {
            //			LevelsMap.SetActive(!enable);//1.4.6
            //			LevelsMap.SetActive(enable);
            //			LevelsMap.GetComponent<LevelsMap>().Reset();
            //			foreach (Transform tr in LevelsMap.transform)
            //			{
            //				if (tr.name != "AvatarManager" && tr.name != "Character")
            //					tr.gameObject.SetActive(enable);
            //				if (tr.name == "Character")
            //				{
            //					tr.GetComponent<SpriteRenderer>().enabled = enable;
            //					tr.transform.GetChild(0).gameObject.SetActive(enable);
            //				}
            //			}
            CanvasMap.SetActive(enable);
            Level.SetActive(!enable);
            Level.transform.Find("Canvas").GetComponent<Canvas>().worldCamera = Camera.main;
        }

        //		if (enable)
        //			GameField.gameObject.SetActive(false);

        if (!enable)
            Camera.main.transform.position = new Vector3(0, 0, -10);
        //		foreach (Transform item in GameField.transform)
        //		{
        //			Destroy(item.gameObject);
        //		}
    }

    private void Awake()
    {
        THIS = this;
        Instance = this;
        if (Level != null) Level.gameObject.SetActive(true);

        GameSettingsScriptable gameSettingsScriptable = Resources.Load("Settings/GameSettingsScriptable") as GameSettingsScriptable;
        var fields = gameSettingsScriptable.GetType().GetFields();
        foreach (var field in fields)
        {
            this.GetType().GetField(field.Name)?.SetValue(this, field.GetValue(gameSettingsScriptable));
        }
        // if (SceneManager.GetActiveScene().name == "map")
        gameStatus = GameState.Map;
        lastLevel = GetLastLevel();
    }
    // Use this for initialization
    void Start()
    {
        if (!MusicBase.Instance.GetComponent<AudioSource>().isPlaying)
            MusicBase.Instance.GetComponent<AudioSource>().Play();
        if (SceneManager.GetActiveScene().name == "map")
        {
            LevelsMap = GameObject.Find("LevelsMap");
            if (IsLastLevelPassed() && EndingObject) EndingObject.SetActive(true);
        }
        else if (SceneManager.GetActiveScene().name.Contains("level"))
        {
            // itemsSpawner = GameObject.Find("SpawnItems").GetComponent<GeneratorItems>();

            touchBlocker = gameObject.AddComponent<TouchBlocker>();
            tipsManager = gameObject.AddComponent<TipsManager>();
            ingrCountTarget = new int[NumIngredients]; //necessary amount of collectable items
                                                       //ingrTarget = InitScript.Instance.collectedIngredients.ToArray();  //necessary collectable items
                                                       //collectItems = new CollectItems[NumIngredients];   //necessary collectable items
            var grid = FindObjectOfType<Grid>();
            grid?.gameObject.SetActive(false);
            // CanvasScore = GameObject.Find("CanvasScore").transform;
            Canvas = Level.transform.Find("Canvas");
            gameObject.AddComponent<PhysicsOptimizer>();
            ObjectPooler.Instance.GetPooledObject("score");

        }

#if FACEBOOK
		FacebookEnable = true;//1.3.2
		if (FacebookEnable)
			FacebookManager.THIS.CallFBInit();

#else
        FacebookEnable = false;

#endif
#if UNITY_PURCHASING

        gameObject.AddComponent<UnityInAppsIntegration>();
#endif
        // AdsCounter.THIS.passLevelCounter = 0;

#if UNITY_PURCHASING
        if (GetComponent<UnityInAppsIntegration>() == null)
            gameObject.AddComponent<UnityInAppsIntegration>();

#endif
        gameObject.AddComponent<SelectionManager>();
    }

    void InitLevel()
    {
        //        itemPrefab = Resources.Load("Prefabs/Item " + currentLevel)as GameObject;
        //		GenerateOutline();
        nextExtraItems = 0;
        bombTimers.Clear(); //1.3
        //        ReGenLevel();
        RestartTimer();
        InitTargets();
        SetPreBoosts();

        //		GameField.gameObject.SetActive(true);
    }

    public void RestartTimer()
    {
        if (limitType == LIMIT.TIME)
        {
            StopCoroutine(TimeTick());
            StartCoroutine(TimeTick());
        }
    }

    List<GameObject> listIngredientsGUIObjects = new List<GameObject>();

    void InitTargets()
    {
        GameObject ingrPrefab = Resources.Load("Prefabs/CollectGUIObj") as GameObject;
        foreach (GameObject item in listIngredientsGUIObjects)
        {
            Destroy(item);
        }

        listIngredientsGUIObjects.Clear();

        if (LevelManager.THIS.target == Target.DIAMONDS)
        {
            TotalCollect = LevelSettings.THIS.LevelScriptable.Collect;
            Counter_.totalCount = TotalCollect;
            ;
        }
    }

    void PrepareGame()
    {

        ActivatedBoost = null;
        Score = 0;
        stars = 0;
        moveID = 0;
        selectedColor = -1; //1.3
        highlightedItems = new List<Item>();
        if (ProgressBarScript.Instance)
            ProgressBarScript.Instance.ResetBar();

        star1Anim = GameObject.Find("LevelUI").transform.Find("Canvas/Panel/Panel/Stars/Star1/Star1Anim").gameObject;
        star2Anim = GameObject.Find("LevelUI").transform.Find("Canvas/Panel/Panel/Stars/Star2/Star2Anim").gameObject;
        star3Anim = GameObject.Find("LevelUI").transform.Find("Canvas/Panel/Panel/Stars/Star3/Star3Anim").gameObject;
        star1Anim.SetActive(false);
        star2Anim.SetActive(false);
        star3Anim.SetActive(false);
        ingrTarget = new List<CollectedIngredients>();
        if (target != Target.COLLECT)
            ingrTarget.Add(new CollectedIngredients());

        //ingrTarget = InitScript.Instance.collectedIngredients.ToArray();  //necessary collectable items

        for (int i = 0; i < collectItems.Length; i++)
        {
            collectItems[i] = CollectItems.None;
            //ingrTarget[i] = Ingredients.None;
            //ingrCountTarget[i] = 0;
        }

        TargetBlocks = 0;
        TargetCages = 0;
        TargetBombs = 0;
        //		EnableMap(false);

        LoadLevel(Int32.Parse(Regex.Match(SceneManager.GetActiveScene().name, @"\d+").Value));

    }

    public void CheckWinLose()
    {
        if (Limit <= 0)
        {
            bool lose = false;
            Limit = 0;

            if (LevelManager.THIS.target == Target.DIAMONDS && (TotalCollect - LevelManager.THIS.TargetDiamonds) > 0 && !GameObject.FindObjectsOfType<Diamond>().Any(i => i.Collected))
            {
                lose = true;
            }


            if (lose)
                gameStatus = GameState.PreFailed;

            else if (LevelManager.THIS.target == Target.DIAMONDS && (TotalCollect - LevelManager.THIS.TargetDiamonds) <= 0)
            {
                gameStatus = GameState.PreWinAnimations;
            }
        }
        else
        {
            bool win = false;

            if (LevelManager.THIS.target == Target.DIAMONDS && LevelManager.THIS.TargetDiamonds >= TotalCollect)
            {
                win = true;
            }

            // if (LevelManager.Score < LevelManager.THIS.star1 && LevelManager.THIS.target != Target.SCORE)
            // {
            // 	win = false;

            // }
            if (win)
                gameStatus = GameState.PreWinAnimations;
        }
    }

    public int GetScoresOfTargetStars()
    {
        return (int)this.GetType().GetField("star" + (int)starsTargetCount)
            .GetValue(this); //get value of appropriate field (star1, star2 or star3)
    }

    IEnumerator PreWinAnimationsCor()
    {
        if (PlayerPrefs.GetInt("Music") == 1)
            SoundBase.Instance.PlaySound(SoundBase.Instance.complete[1]);
        GameObject.Find("LevelUI/Canvas").transform.Find("PreCompleteBanner").gameObject.SetActive(true); //1.4.5
        yield return new WaitForSeconds(3);
        GameObject.Find("LevelUI/Canvas").transform.Find("PreCompleteBanner").gameObject.SetActive(false); //1.4.5
        Vector3 pos1 = GameObject.Find("Limit").transform.position;

        yield return new WaitForSeconds(1);

        int countFlowers = Mathf.Clamp(Limit, 0, 5);
        var items = Camera.main.GetComponent<Camera2DFollow>().GetVisibleItems().TakeRandom(countFlowers); //GetRandomItems(countFlowers);
        int flowerFinished = 0;
        foreach (var item in items)
        {
            var flower = ObjectPooler.Instance.GetPooledObject("flower").GetComponent<FlowerFly>();
            flower.targetTransform = item.transform;
            flower.Launch(GameObject.FindObjectOfType<Moves>().transform.position, () =>
            {
                flowerFinished++;
                item.GetComponent<Item>()?.SetType("striped");
            });
            Limit--;
            if (Limit < 0) Limit = 0;
            yield return new WaitForSeconds(0.5f);
        }
        Limit = 0;

        yield return new WaitUntil(() => flowerFinished >= items.Count());
        yield return new WaitForSeconds(0.5f);

        var stripedItems = GameObject.FindObjectsOfType<Striped>();
        foreach (var item in stripedItems)
        {
            IDestroyableComponent destroyableComponent = item.GetComponent<IDestroyableComponent>();
            destroyableComponent.Hit(item.transform.position);
            Score += destroyableComponent.score;
            yield return new WaitForSeconds(0.5f);
        }

        if (PlayerPrefs.GetInt(string.Format("Level.{0:000}.StarsCount", currentLevel), 0) < stars)
            PlayerPrefs.SetInt(string.Format("Level.{0:000}.StarsCount", currentLevel), stars);
        if (Score > PlayerPrefs.GetInt("Score" + currentLevel))
        {
            PlayerPrefs.SetInt("Score" + currentLevel, Score);
        }

        if (LevelsMap != null)
        {
            LevelsMap?.SetActive(false); //1.4.4
            LevelsMap?.SetActive(true); //1.4.4
        }
#if PLAYFAB || GAMESPARKS
		NetworkManager.dataManager.SetPlayerScore(currentLevel, Score);
		NetworkManager.dataManager.SetPlayerLevel(currentLevel + 1);
		NetworkManager.dataManager.SetStars();
#endif

        gameStatus = GameState.Win;
    }


    void DestroyGatheredExtraItems(IColorableComponent item)
    {
        foreach (IDestroyableComponent specialItem in gatheredTypes)
        {
            specialItem.Hit(item.transform.position);
            // item.DestroyVertical();
        }
    }

    public BoostIcon waitingBoost;
    TouchCounter touch_counter;
    List<IColorableComponent> preSelectedList;
    List<IColorableComponent> extraHightlightedItems;
    List<IHighlightableComponent> selectedPath;
    List<IHighlightableComponent> highlightedList;
    List<IExplodable> specialItems;
    bool touchBegin;
    IHighlightableComponent[] itemList;

    Vector2 startPosTouch = Vector2.zero;
    Vector2 currentPos = Vector2.zero;
    Vector2 directionTouch = Vector2.zero;

    void Update()
    {
        // Debug keys events for editor   
        //if (Application.isEditor)
        //    SetupGameCamera();
        if (SceneManager.GetActiveScene().name == "map") return;
        if (gameStatus == GameState.Playing)
        {
            //  AvctivatedBoostView = ActivatedBoost;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                NoMatches();
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                //Instant win
                Score += star1;
                gameStatus = GameState.PreWinAnimations;
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                //last move 
                Limit = 1;
            }


        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            //restart scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            //Back button for Android
            if (LevelManager.THIS.gameStatus == GameState.Playing)
                GameObject.Find("CanvasGlobal").transform.Find("MenuPause").gameObject.SetActive(true);
            else if (LevelManager.THIS.gameStatus == GameState.Map)
                Application.Quit();
        }
    }


    IEnumerator TimeTick()
    {
        while (true)
        {
            if (gameStatus == GameState.Playing)
            {
                if (LevelManager.Instance.limitType == LIMIT.TIME)
                {
                    LevelManager.THIS.Limit--;
                    CheckWinLose();
                }
            }

            if (gameStatus == GameState.Map || LevelManager.THIS.Limit <= 0 || gameStatus == GameState.GameOver)
                yield break;

            yield return new WaitForSeconds(1);
        }
    }


    public void NoMatches()
    {
        StartCoroutine(NoMatchesCor());
    }

    IEnumerator NoMatchesCor()
    {
        if (gameStatus == GameState.Playing)
        {
            gameStatus = GameState.RegenLevel;
            SoundBase.Instance.PlaySound(SoundBase.Instance.noMatch);
            FindObjectsOfType<Item>().ForEachY(i => i.GetComponent<AnimationItem>().NoMatches(i.GetComponent<IColorableComponent>().OnAnimChangeColor));
            GameObject.Find("LevelUI/Canvas").transform.Find("NoMoreMatches").gameObject.SetActive(true);
            // gameStatus = GameState.RegenLevel;
            yield return new WaitForSeconds(1);
            gameStatus = GameState.Playing;

            // ReGenLevel();
        }
    }

    List<int> bombTimers = new List<int>();
    //1.3
    void SetPreBoosts()
    {
        //activate boosts from map
        bool NoBoosts = true;
        foreach (var item in SaveBoost.boostType)
        {
            if (item == BoostType.Flower)
            {
                InitScript.Instance.SpendBoost(BoostType.Flower);
                GameObject.FindObjectsOfType<Item>().TakeRandom(3).ForEachY(i => i.SetType("flower_item"));
                // BoostFlower = 0;
                NoBoosts = false;
            }

            if (item == BoostType.Multicolor)
            {
                InitScript.Instance.SpendBoost(BoostType.Multicolor);
                GameObject.FindObjectsOfType<Item>().TakeRandom(3).ForEachY(i => i.SetType("multicolor"));
                // BoostMulticolor = 0;
                NoBoosts = false;
            }

            if (item == BoostType.Spiral)
            {
                InitScript.Instance.SpendBoost(BoostType.Spiral);
                GameObject.FindObjectsOfType<Item>().TakeRandom(3).ForEachY(i => i.SetType("spiral_bomb"));
                // BoostMulticolor = 0;
                NoBoosts = false;
            }
        }

        SaveBoost.boostType.Clear();
    }

    public void PopupScore(int value, Vector3 pos)
    {
        // Score += value;
        UpdateBar();
        CheckStars();
        if (showPopupScores)
        {
            GameObject poptxt = ObjectPooler.Instance.GetPooledObject("score");//Instantiate(popupScore, pos, Quaternion.identity) as GameObject;
            poptxt.transform.GetComponentInChildren<Text>().text = "" + value;
            // if (color <= scoresColors.Length - 1)
            // {
            // 	poptxt.transform.GetComponentInChildren<Text>().color = scoresColors[color];
            // 	poptxt.transform.GetComponentInChildren<Outline>().effectColor = scoresColorsOutline[color];
            // }
            poptxt.transform.SetParent(Canvas);
            //   poptxt.transform.position += Vector3.right * 1;
            poptxt.transform.position = pos;
            poptxt.transform.localScale = Vector3.one;
            // Destroy(poptxt, 1.4f);
        }
    }
    public void FindMatches()
    {
        StartCoroutine(FallingDown());
    }

    IEnumerator FallingDown()
    {
        bool throwflower = false;
        extraCageAddItem = 0;
        bool nearEmptySquareDetected = false;
        int combo = 0;
        // AI.THIS.allowShowTip = false;

        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            combinedItems.Clear();

            combo = destroyAnyway.Count;
            if (destroyAnyway.Count >= 6)
            {
                if (lastDraggedItem == null)
                    lastDraggedItem = destroyAnyway.Last().transform.GetComponent<IColorableComponent>();
                if (lastDraggedItem != null)
                {
                    Item item2 = lastDraggedItem.GetComponent<Item>();
                    if (item2 != null)
                        item2.ChangeType("striped", TouchCounter.type);
                }
            }

            if (lastDraggedItem != null)
                DestroyGatheredExtraItems(lastDraggedItem.GetComponent<IColorableComponent>());

            if (destroyAnyway.Count >= extraItemEvery)
            {
                LevelManager.THIS.nextExtraItems = destroyAnyway.Count / (int)extraItemEvery;
            }

            int destroyArrayCount = destroyAnyway.Count;
            int iCounter = 0;
            int scoreCounter = 0;
            foreach (var item in destroyAnyway)
            {
                iCounter++;

                yield return new WaitForSeconds(0.03f);
                IDestroyableComponent destroyableComponent = item.transform.GetComponent<IDestroyableComponent>();
                scoreCounter += destroyableComponent.score;
                Vector2 pos = lastDraggedItem == null ? item.transform.position : lastDraggedItem.transform.position;
                destroyableComponent.Hit(pos);
            }

            Score += scoreCounter * destroyAnyway.Count;
            LevelManager.THIS.PopupScore(scoreCounter * destroyAnyway.Count, destroyAnyway[destroyAnyway.Count() - 1].transform.position);
            destroyAnyway.Clear();

            if (destroyAnyway.Count > 0)
                nearEmptySquareDetected = true;
            if (!nearEmptySquareDetected)
                break;
        }

        if (combo > 11 && gameStatus == GameState.Playing)
        {
            gratzWords[2].SetActive(true);
        }
        else if (combo > 8 && gameStatus == GameState.Playing)
        {
            gratzWords[1].SetActive(true);
        }
        else if (combo > 5 && gameStatus == GameState.Playing)
        {
            gratzWords[0].SetActive(true);
        }

        combo = 0;

        nextExtraItems = 0;

        gatheredTypes.Clear();
        startPosFlowers.Clear();
        yield return new WaitForEndOfFrame();

        touchBlocker.blocked = false;
        yield return new WaitForSeconds(1);
        yield return new WaitWhileFall();
        lastDraggedItem = null;
        OnNextMove?.Invoke();

    }

    public void CheckWin()
    {
        if (gameStatus == GameState.Playing || gameStatus == GameState.WaitBeforeWin)
            LevelManager.THIS.CheckWinLose();
    }


    public List<Item> highlightedItems;
    public bool testByPlay;
    public CollectStars starsTargetCount;
    public int extraCageAddItem;
    public GameObject CanvasMap;
    private GeneratorItems itemsSpawner;


    public void ClearHighlight(bool boost = false)
    {
        if (!boost)
            return;
        foreach (var item in highlightedItems)
        {
            item.SleepItem();
        }

        highlightedItems.Clear();
    }


    public IColorableComponent[] GetItemsAround(Vector2 pos, float r)
    {
        return Physics2D.OverlapCircleAll(pos, r, 1 << LayerMask.NameToLayer("Item")).Select(i => i.GetComponent<IColorableComponent>()).ToArray();
    }

    void UpdateBar()
    {
        ProgressBarScript.Instance.UpdateDisplay((float)Score / (float)star3);
    }

    void CheckStars()
    {
        if (Score >= star1 && stars <= 0)
        {
            stars = 1;
        }

        if (Score >= star2 && stars <= 1)
        {
            stars = 2;
        }

        if (Score >= star3 && stars <= 2)
        {
            stars = 3;
        }

        if (Score >= star1)
        {
            // if (!star1Anim.activeSelf)
            //     SoundBase.Instance.PlaySound(SoundBase.Instance.getStarIngr);
            star1Anim.SetActive(true);
        }

        if (Score >= star2)
        {
            // if (!star2Anim.activeSelf)
            //     SoundBase.Instance.PlaySound(SoundBase.Instance.getStarIngr);
            star2Anim.SetActive(true);
        }

        if (Score >= star3)
        {
            // if (!star3Anim.activeSelf)
            //     SoundBase.Instance.PlaySound(SoundBase.Instance.getStarIngr);
            star3Anim.SetActive(true);
        }
    }
    public bool mouseDown;
    private Vector3 startPos;
    internal Vector3 lowerDestroyItemPos = new Vector2(0, 100);
    private bool checkAds;
    public int lastLevel;
}

[System.Serializable]
public class GemProduct
{
    public int count;
    public float price;
}