#define DEVELOPMENT_BUILD

using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif



public class AnimationManager : MonoBehaviour
{
    public bool PlayOnEnable = true;
    bool WaitForPickupFriends;
    public GameObject fireworkPrefab;

    bool WaitForAksFriends;
    System.Collections.Generic.Dictionary<string, string> parameters;

    void OnEnable()
    {

        if (name == "MenuPlay")
        {
            // LevelManager.THIS.LoadLevel();
        }


        if (name == "Settings" || name == "MenuPause")
        {
            transform.Find("Image/Sound/Sound").GetComponent<Image>().color = new Color(1, 1, 1, Mathf.Clamp(PlayerPrefs.GetInt("Sound"), 0.5f, 1));
            transform.Find("Image/Music/Music").GetComponent<Image>().color = new Color(1, 1, 1, Mathf.Clamp(PlayerPrefs.GetInt("Music"), 0.5f, 1));

        }



        if (transform.Find("Image/Video") != null)
        {

#if UNITY_ADS
            InitScript.Instance.rewardedVideoZone = "rewardedVideo";

            if (transform.Find("Image/Video/AdsNotAvailable") != null)
            {//1.4.8
                if (!InitScript.Instance.enableUnityAds || !InitScript.Instance.GetRewardedUnityAdsReady())
                    transform.Find("Image/Video/AdsNotAvailable").gameObject.SetActive(true);
                else
                    transform.Find("Image/Video/AdsNotAvailable").gameObject.SetActive(false);
            }

#else
            transform.Find("Image/Video").gameObject.SetActive(false);
#endif
        }
    }

    void Update()
    {

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (name == "MenuPlay" || name == "Settings" || name == "BoostInfo" || name == "CoinsShop" || name == "LiveShop" || name == "BoostShop" || name == "Reward")
                CloseMenu();
        }
    }

    public void ShowAds()
    {
        if (name == "CoinsShop")
            InitScript.Instance.currentReward = RewardedAdsType.GetGems;
        else if (name == "LiveShop")
            InitScript.Instance.currentReward = RewardedAdsType.GetLifes;
        else if (name == "PreFailed")
            InitScript.Instance.currentReward = RewardedAdsType.GetGoOn;
        InitScript.Instance.ShowRewardedAds();
        if (name != "MenuFailed")
            CloseMenu();
    }

    public void GoRate()
    {
#if UNITY_ANDROID
        Application.OpenURL(InitScript.Instance.RateURL);
#elif UNITY_IOS
        Application.OpenURL(InitScript.Instance.RateURLIOS);
#endif

        PlayerPrefs.SetInt("Rated", 1);
        PlayerPrefs.Save();
    }

    void OnDisable()
    {
        if (transform.Find("Image/Video") != null)
        {
            transform.Find("Image/Video").gameObject.SetActive(true);
        }
    }

    public void OnFinished()
    {
        if (name == "MenuComplete")
        {
            GetComponent<MenuComplete>().Show();

        }
        if (name == "Reward")
        {
            // StartCoroutine(FireworkParticles());
        }
        if (name == "MenuPlay")
        {
            //            InitScript.Instance.currentTarget = InitScript.Instance.targets[PlayerPrefs.GetInt( "OpenLevel" )];
            // transform.Find("Image/Boost1").GetComponent<BoostIcon>().InitBoost();
            // transform.Find("Image/Boost2").GetComponent<BoostIcon>().InitBoost();
            // transform.Find("Image/Boost3").GetComponent<BoostIcon>().InitBoost();

        }
        if (name == "MenuPause")
        {
            if (LevelManager.THIS.gameStatus == GameState.Playing)
                LevelManager.THIS.gameStatus = GameState.Pause;
        }

        if (name == "MenuFailed")
        {
            if (LevelManager.Score < LevelManager.THIS.star1)
            {
                TargetCheck(false, 2);
            }
            else
            {
                TargetCheck(true, 2);
            }
        }
        if (name == "PrePlay")
        {
            CloseMenu();

        }
        if (name == "MenuFailed")
        {
            if (LevelManager.THIS.Limit <= 0)
            {
                if (LevelManager.THIS.gameStatus != GameState.GameOver)//1.3.3
                    LevelManager.THIS.gameStatus = GameState.GameOver;
            }
            //transform.Find("Image/Video").gameObject.SetActive(false);

            //    CloseMenu();

        }

        if (name.Contains("gratzWord"))
            gameObject.SetActive(false);
        if (name == "NoMoreMatches")
            gameObject.SetActive(false);
        if (name == "CompleteLabel")
            gameObject.SetActive(false);

    }

    void TargetCheck(bool check, int n = 1)
    {
        //Transform TargetCheck = transform.Find("Image/TargetCheck" + n);
        //Transform TargetUnCheck = transform.Find("Image/TargetUnCheck" + n);
        //TargetCheck.gameObject.SetActive(check);
        //TargetUnCheck.gameObject.SetActive(!check);
    }

    public void WaitForGiveUp()
    {
        if (name == "MenuFailed")
        {
            GetComponent<Animation>()["bannerFailed"].speed = 0;
#if UNITY_ADS

            if (InitScript.Instance.enableUnityAds)
            {

                if (InitScript.Instance.GetRewardedUnityAdsReady())
                {
                    transform.Find("Image/Video").gameObject.SetActive(true);
                }
            }
#endif
        }
    }

    public void Info()
    {
        GameObject.Find("CanvasGlobal").transform.Find("Tutorial").gameObject.SetActive(true);
        CloseMenu();
    }



    public void PlaySoundButton(AudioClip clip)
    {
        SoundBase.Instance.PlaySound(clip);

    }

    public IEnumerator Close()
    {
        yield return new WaitForSeconds(0.5f);
    }

    public void ChangeGameState(int status)
    {
        LevelManager.THIS.gameStatus = (GameState)status;
    }

    public void CloseMenu()
    {

        SoundBase.Instance.PlaySound(SoundBase.Instance.click);
        if (gameObject.name == "MenuPreGameOver")
        {
            ShowGameOver();
        }
        if (gameObject.name == "MenuComplete")
        {
            // LevelManager.THIS.gameStatus = GameState.Map;
            // SceneManager.LoadScene("map");
        }
        if (gameObject.name == "PreFailed")
        {
            gameObject.SetActive(false);

        }

        if (gameObject.name == "MenuFailed")
        {
            if (!keepGaming)
                SceneManager.LoadScene("map");
            // LevelManager.THIS.gameStatus = GameState.Map;
            keepGaming = false;
        }
        if (gameObject.name == "Tutorial")
        {
            LevelManager.Instance.gameStatus = GameState.Playing;
        }

        if (Application.loadedLevelName == "map")
        {
            if (LevelManager.Instance.gameStatus == GameState.Pause)
            {
                LevelManager.Instance.gameStatus = GameState.WaitAfterClose;

            }
        }

        if (name == "MenuPlay")
        {
            // Transform tr = transform.Find("Image/TargetIngr/TargetIngr");
            // foreach (Transform item in tr)
            // {
            //     Destroy(item.gameObject);
            // }
        }
        // SoundBase.Instance.PlaySound(SoundBase.Instance.swish[1]);

        gameObject.SetActive(false);
    }

    public void SwishSound()
    {
        // SoundBase.Instance.PlaySound(SoundBase.Instance.swish[1]);

    }

    public void ShowInfo()
    {
        GameObject.Find("CanvasGlobal").transform.Find("BoostInfo").gameObject.SetActive(true);

    }

    public void Play()
    {
        if (gameObject.name == "MenuPreGameOver")
        {
            SoundBase.Instance.PlaySound(SoundBase.Instance.click);

            if (InitScript.Coins >= 12)
            {
                InitScript.Instance.SpendGems(12);
                //                LevelData.LimitAmount += 12;
                LevelManager.Instance.gameStatus = GameState.WaitAfterClose;
                gameObject.SetActive(false);

            }
            else
            {
                BuyGems();
            }
        }
        else if (gameObject.name == "MenuFailed")
        {
            LevelManager.Instance.gameStatus = GameState.Map;
        }
        else if (gameObject.name == "MenuPlay")
        {
            if (InitScript.lifes > 0)
            {
                //InitScript.Instance.SpendLife(1);
                LevelManager.THIS.gameStatus = GameState.PrepareGame;
                CloseMenu();
                //Application.LoadLevel( "map" );
            }
            else
            {
                BuyLifeShop();
            }

        }
        else if (gameObject.name == "MenuPause")
        {
            CloseMenu();
            LevelManager.Instance.gameStatus = GameState.Playing;
        }
    }

    public void PlayTutorial()
    {
        SoundBase.Instance.PlaySound(SoundBase.Instance.click);
        CloseMenu();
        //    mainscript.Instance.dropDownTime = Time.time + 0.5f;
        //        CloseMenu();
    }

    public void BackToMap()
    {
        Time.timeScale = 1;
        LevelManager.THIS.gameStatus = GameState.GameOver;
        CloseMenu();
    }

    public void Next()
    {
        SoundBase.Instance.PlaySound(SoundBase.Instance.click);
        if (gameObject.name == "Warning") InitScript.Instance.SpendLife(1);
        CloseMenu();
        GameObject.Find("CanvasGlobal").transform.Find("Loading").gameObject.SetActive(true);
        SceneManager.LoadScene("map");
        // LevelManager.THIS.gameStatus = GameState.Map;
    }

    public void Again()
    {
        GetComponent<PlayableDirector>().Stop();
        if (InitScript.lifes > 0)
        {
            SoundBase.Instance.PlaySound(SoundBase.Instance.click);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            keepGaming = true;
            CloseMenu();
        }
        else
        {
            BuyLifeShop();
        }
    }


    public void BuyGems()
    {

        SoundBase.Instance.PlaySound(SoundBase.Instance.click);
        GameObject.Find("CanvasGlobal").transform.Find("CoinsShop").gameObject.SetActive(true);
    }

    public void Buy(GameObject pack)
    {
        SoundBase.Instance.PlaySound(SoundBase.Instance.click);
        var i = pack.transform.GetSiblingIndex();
        InitScript.waitedPurchaseGems = int.Parse(pack.transform.Find("Count").GetComponent<Text>().text.Replace("x ", ""));

#if UNITY_WEBPLAYER || UNITY_WEBGL
			InitScript.Instance.PurchaseSucceded ();
			return;
#endif
#if UNITY_PURCHASING
        UnityInAppsIntegration.THIS.BuyProductID(LevelManager.THIS.InAppIDs[i]);
#elif AMAZON
			AmazonInapps.THIS.Purchase (LevelManager.THIS.InAppIDs [i]);
#else
        Debug.LogError("Unity-inapps not enable. More info: https://docs.google.com/document/d/1HeN8JtQczTVetkMnd8rpSZp_TZZkEA7_kan7vvvsMw0/edit#heading=h.60xg5ccbex9m");//1.4.9
#endif
    }

    public void BuyLifeShop()
    {

        SoundBase.Instance.PlaySound(SoundBase.Instance.click);
        if (InitScript.lifes < InitScript.Instance.CapOfLife)
            GameObject.Find("CanvasGlobal").transform.Find("LiveShop").gameObject.SetActive(true);

    }

    public void BuyLife(GameObject button)
    {
        SoundBase.Instance.PlaySound(SoundBase.Instance.click);
        if (InitScript.Coins >= int.Parse(button.transform.Find("Price").GetComponent<Text>().text))
        {
            InitScript.Instance.SpendGems(int.Parse(button.transform.Find("Price").GetComponent<Text>().text));
            InitScript.Instance.RestoreLifes();
            CloseMenu();
        }
        else
        {
            GameObject.Find("CanvasGlobal").transform.Find("CoinsShop").gameObject.SetActive(true);
        }

    }

    public void BuyFailed(GameObject button)
    {
        //if (GetComponent<Animation>()["bannerFailed"].speed == 0)
        //{
        if (InitScript.Coins >= LevelManager.THIS.FailedCost)
        {
            InitScript.Instance.SpendGems(LevelManager.THIS.FailedCost);
            //button.GetComponent<Button>().interactable = false;
            GoOnFailed();
        }
        else
        {
            GameObject.Find("CanvasGlobal").transform.Find("CoinsShop").gameObject.SetActive(true);
        }
        //}
    }

    public void GoOnFailed()
    {
        if (LevelManager.THIS.limitType == LIMIT.MOVES)
            LevelManager.THIS.Limit += LevelManager.THIS.ExtraFailedMoves;
        else
        {
            LevelManager.THIS.Limit += LevelManager.THIS.ExtraFailedSecs;
        }

        // if (LevelManager.THIS.target == Target.BOMBS)//1.3
        // LevelManager.THIS.RechargeBombs();
        //GetComponent<Animation>()["bannerFailed"].speed = 1;
        keepGaming = true;
        MusicBase.Instance.GetComponent<AudioSource>().Play();
        CloseMenu();

        LevelManager.THIS.gameStatus = GameState.Playing;
        LevelManager.THIS.RestartTimer();

    }

    public void GiveUp()
    {
        GetComponent<Animation>()["bannerFailed"].speed = 1;
    }

    void ShowGameOver()
    {
        // SoundBase.Instance.PlaySound(SoundBase.Instance.gameOver[1]);

        GameObject.Find("Canvas").transform.Find("MenuGameOver").gameObject.SetActive(true);
        gameObject.SetActive(false);

    }

    #region boosts

    public void BuyBoost(BoostType boostType, int price, int count)
    {
        SoundBase.Instance.PlaySound(SoundBase.Instance.click);
        if (InitScript.Coins >= price)
        {
            // SoundBase.Instance.PlaySound(SoundBase.Instance.cash);
            InitScript.Instance.SpendGems(price);
            InitScript.Instance.BuyBoost(boostType, price, count);
            //InitScript.Instance.SpendBoost(boostType);
            CloseMenu();
        }
        else
        {
            BuyGems();
        }
    }

    #endregion

    public void SoundOff(Image Off)
    {
        var sr = Off;
        if (sr.color.a == 1f)
        {
            SoundBase.Instance.GetComponent<AudioSource>().volume = 0;
            InitScript.sound = false;

            sr.color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            SoundBase.Instance.GetComponent<AudioSource>().volume = 1;
            InitScript.sound = true;
            sr.color = new Color(1, 1, 1, 1f);

        }
        PlayerPrefs.SetInt("Sound", (int)SoundBase.Instance.GetComponent<AudioSource>().volume);
        PlayerPrefs.Save();

    }

    public void MusicOff(Image Off)
    {
        var sr = Off;

        if (sr.color.a == 1f)
        {
            GameObject.Find("Music").GetComponent<AudioSource>().volume = 0;
            InitScript.music = false;

            sr.color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            GameObject.Find("Music").GetComponent<AudioSource>().volume = 1;
            InitScript.music = true;

            sr.color = new Color(1, 1, 1, 1);

        }
        PlayerPrefs.SetInt("Music", (int)GameObject.Find("Music").GetComponent<AudioSource>().volume);
        PlayerPrefs.Save();

    }

    Target target;
    LIMIT limitType;
    int[] ingrCountTarget;
    Ingredients[] ingrTarget;
    CollectItems[] collectItems;
    private bool keepGaming;

    void LoadLevel(int n)
    {
        TextAsset map = Resources.Load("Levels/" + n) as TextAsset;
        if (map != null)
        {
            string mapText = map.text;
            string[] lines = mapText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            ingrTarget = new Ingredients[LevelManager.THIS.NumIngredients];
            ingrCountTarget = new int[LevelManager.THIS.NumIngredients];
            collectItems = new CollectItems[LevelManager.THIS.NumIngredients];
            int mapLine = 0;
            foreach (string line in lines)
            {
                //check if line is game mode line
                if (line.StartsWith("MODE"))
                {
                    //Replace GM to get mode number, 
                    string modeString = line.Replace("MODE", string.Empty).Trim();
                    //then parse it to interger
                    target = (Target)int.Parse(modeString);
                    //Assign game mode
                }
                else if (line.StartsWith("LIMIT"))
                {
                    string blocksString = line.Replace("LIMIT", string.Empty).Trim();
                    string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    limitType = (LIMIT)int.Parse(sizes[0]);
                }
                else if (line.StartsWith("COLLECT COUNT "))
                {
                    string blocksString = line.Replace("COLLECT COUNT", string.Empty).Trim();
                    string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < blocksNumbers.Length; i++)
                    {
                        ingrCountTarget[i] = int.Parse(blocksNumbers[i]);

                    }
                }
                else if (line.StartsWith("COLLECT ITEMS "))
                {
                    string blocksString = line.Replace("COLLECT ITEMS", string.Empty).Trim();
                    string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < blocksNumbers.Length; i++)
                    {
                        if (target == Target.COLLECT)
                            ingrTarget[i] = (Ingredients)int.Parse(blocksNumbers[i]);
                        else if (target == Target.ITEMS)
                            collectItems[i] = (CollectItems)int.Parse(blocksNumbers[i]);


                    }
                }

            }
        }

    }

}
