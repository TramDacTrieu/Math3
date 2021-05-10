using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

public class BoostIcon : MonoBehaviour
{
    public Text boostCount;
    public BoostType type;
    bool check;
    public Text price;
    public int count;
    public Sprite hammerSprite;

    void OnEnable()
    {
        InitBoost();
        if (name != "GameManager")
        {
            if (LevelManager.THIS != null)
            {
                if (LevelManager.THIS.gameStatus == GameState.Map)
                    transform.Find("Indicator/Count/Check")?.gameObject.SetActive(false);
                // if (!LevelManager.THIS.enableInApps)//1.4.9
                //     gameObject.SetActive(false);
            }
        }
        UpdateCount();
        InitScript.OnBoostBought += OnBought;
        InitScript.OnBoostSpent += OnSpent;
    }

    private void OnBought(BoostType boostType)
    {
        UpdateCount();
    }

    private void UpdateCount()
    {
        count = PlayerPrefs.GetInt("" + type);
        if (boostCount)
            boostCount.text = "" + count;
    }

    private void OnDisable()
    {
        InitScript.OnBoostBought -= OnBought;
        InitScript.OnBoostSpent -= OnSpent;
    }

    private void OnSpent(BoostType boostType)
    {
        UpdateCount();
    }

    public void ActivateBoost()
    {
        if (LevelManager.THIS.ActivatedBoost == this)
        {
            UnCheckBoost();
            return;
        }
        if (IsLocked()  || (LevelManager.THIS.gameStatus != GameState.Playing && LevelManager.THIS.gameStatus != GameState.Map))
            return;
        if (BoostCount() > 0)
        {
            if (type != BoostType.Flower && type != BoostType.Multicolor && type != BoostType.Spiral && !LevelManager.THIS.touchBlocker.blocked)
                LevelManager.THIS.ActivatedBoost = this;
            if (type == BoostType.Flower)
            {
                // LevelManager.THIS.BoostFlower = 1;
                if (!check)
                    SaveBoost.boostType.Add(type);
                else
                    SaveBoost.boostType.RemoveAt(SaveBoost.boostType.IndexOf(type));
                Check();
            }

            if (type == BoostType.Multicolor)
            {
                if (!check)
                    SaveBoost.boostType.Add(type);
                else
                    SaveBoost.boostType.RemoveAt(SaveBoost.boostType.IndexOf(type));
                // LevelManager.THIS.BoostMulticolor = 1;
                Check();
            }

            if (type == BoostType.Spiral)
            {
                if (!check)
                    SaveBoost.boostType.Add(type);
                else
                    SaveBoost.boostType.RemoveAt(SaveBoost.boostType.IndexOf(type));
                // LevelManager.THIS.BoostSpiral = 1;
                Check();
            }
        }
        else
        {
            OpenBoostShop(type);
        }
    }


    void UnCheckBoost()
    {
        LevelManager.THIS.activatedBoost = null;
        LevelManager.THIS.UnLockBoosts();
    }

    public void InitBoost()
    {
        transform.Find("Indicator/Count/Check")?.gameObject.SetActive(false);
        boostCount?.gameObject.SetActive(true);
        // LevelManager.THIS.BoostColorfullBomb = 0;
        // LevelManager.THIS.BoostPackage = 0;
        // LevelManager.THIS.BoostStriped = 0;
        check = false;
    }

    void Check()
    {
        check = !check;
        transform.Find("Indicator/Count/Check").gameObject.SetActive(check);
        boostCount?.gameObject.SetActive(!check);
        //InitScript.Instance.SpendBoost(type);
    }

    public void LockBoost()
    {
        Color c = GetComponent<Image>().color;
        GetComponent<Image>().color = new Color(c.a, c.g, c.b, 0.5f);
        //transform.Find("Lock").gameObject.SetActive(true);
        transform.Find("Indicator").gameObject.SetActive(false);
    }

    public void UnLockBoost()
    {
        Color c = GetComponent<Image>().color;
        GetComponent<Image>().color = new Color(1, 1, 1, 1);

        //transform.Find("Lock").gameObject.SetActive(false);
        transform.Find("Indicator").gameObject.SetActive(true);
    }

    bool IsLocked()
    {
        return false;
    }

    int BoostCount()
    {
        if (!boostCount.text.Contains("+"))
            return int.Parse(boostCount.text);
        else return 0;
    }

    public void OpenBoostShop(BoostType boosType)
    {
        SoundBase.Instance.PlaySound(SoundBase.Instance.click);
        GameObject.Find("CanvasGlobal").transform.Find("BoostShop").gameObject.GetComponent<BoostShop>().SetBoost(boosType);
    }

    void ShowPlus(bool show)
    {
        // transform.Find("Indicator/Plus").gameObject.SetActive(show);
        if (show)
            boostCount.text = "+";
        // boostCount.gameObject.SetActive(!show);
    }


    void Update()
    {
        if (boostCount != null)
        {
            if (!check)
            {
                if (BoostCount() > 0)
                    ShowPlus(false);
                else
                    ShowPlus(true);
            }
        }
    }

    public void Activate(IDestroyableComponent item)
    {
        if (type == BoostType.Hammer)
        {
            StartCoroutine(HammerExplode(item));
        }
    }


    public GameObject[] effects;

    IEnumerator HammerExplode(IDestroyableComponent item)
    {
        Vector3 pos = item.transform.position;
        var hitItems = Physics2D.OverlapCircleAll(pos, 1f).Where(i => i.gameObject != gameObject).Select(i => i.GetComponent<IHighlightableComponent>()).Distinct().ToArray();
        GameObject hammer = new GameObject();
        hammer.AddComponent<SpriteRenderer>().sprite = hammerSprite;
        hammer.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
        hammer.transform.position = pos + Vector3.down * .5f;
        float startTime = Time.time;
        Vector3 b = Vector2.one * 0.3f;
        while (hammer.transform.localScale != b)
        {
            var scale = Vector2.Lerp(Vector2.one * 0.6f, b, (Time.time - startTime) * 2);
            // hammer.transform.Rotate(Vector3.back * 10);
            hammer.transform.localScale = scale;
            yield return new WaitForEndOfFrame();
        }
        SoundBase.Instance.PlaySound(SoundBase.Instance.explosion);

        // Instantiate(effects[0]).GetComponent<ExplosionWaveEffect>().StartExplosionRound(hitItems, pos);
        ObjectPooler.Instance.GetPooledObject("explosion_wave").GetComponent<ExplosionWaveEffect>().StartExplosionRound(hitItems, pos);


        // GameObject effect1 = Instantiate(effects[0], item.transform.position, Quaternion.identity);
        // effect1.transform.localScale = Vector2.one * 2;
        Destroy(hammer);
        item.Hit(pos);
        // GameObject effect2 = Instantiate(effects[1], item.transform.position, Quaternion.identity);
        IHighlightableComponent[] hit1 = hitItems;
        foreach (var hitItem in hit1)
        {
            // yield return new WaitForSeconds(.1f);
            IDestroyableComponent hittable = hitItem?.GetComponent<IDestroyableComponent>();
            if (hittable != null)
                hittable.Hit(hitItem.transform.position);
        }
    }

}

public static class SaveBoost
{
    public static List<BoostType> boostType = new List<BoostType>();
}
