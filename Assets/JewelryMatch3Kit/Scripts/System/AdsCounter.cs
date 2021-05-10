using System.Collections.Generic;
using UnityEngine;

public class AdsCounter : MonoBehaviour
{
    public List<AdEvents> adsEvents = new List<AdEvents>();
    public int passLevelCounter;

    public static AdsCounter THIS;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (THIS == null)
            THIS = this;
        else
            Destroy(gameObject);

        GameSettingsScriptable gameSettingsScriptable = Resources.Load("Settings/GameSettingsScriptable") as GameSettingsScriptable;
        foreach (var item in gameSettingsScriptable.adsEvents)
        {
            adsEvents.Add(item.DeepCopy());
        }

    }
}