using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettingsScriptable", menuName = "GameSettingsScriptable", order = 0)]
public class GameSettingsScriptable : ScriptableObject
{
    public int CapOfLife = 5;
    public float TotalTimeForRestLifeHours = 0;
    public float TotalTimeForRestLifeMin = 15;
    public float TotalTimeForRestLifeSec = 60;
    public int FirstGems = 20;
    public int rewardedGems = 5;
    public bool losingLifeEveryGame;
    public int CostIfRefill = 12;
    public int FailedCost;
    //extra moves that you get to continue game after fail
    public int ExtraFailedMoves = 5;
    public Color levelColor;
    // array of iapps products
    public List<GemProduct> gemsProducts = new List<GemProduct>();
    // product IDs
    public string[] InAppIDs;
    public List<BoostProduct> boostProducts = new List<BoostProduct>();
    public bool enableUnityAds;
    public bool enableGoogleMobileAds;
    public bool enableChartboostAds;
    public bool FacebookEnable;
    public bool PlayFab;
    //enabling iapps flag
    public bool enableInApps;

    public string admobUIDAndroid;
    public string admobUIDIOS;

    public int ShowRateEvery;
    public string RateURL;
    public string RateURLIOS;
    public List<AdEvents> adsEvents = new List<AdEvents>();

}