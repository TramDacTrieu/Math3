using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public enum BoostType
{
    Flower,
    Multicolor,
    Spiral,
    Moves,
    Hammer,
    None
}

public class BoostShop : MonoBehaviour
{
    public GameObject[] boosts;
    public int[] prices;
    public Image icon;
    public Text description;
    public Text price;
    public int count;

    BoostType boostType;

    public List<BoostProduct> boostProducts = new List<BoostProduct>();

    // Use this for initialization
    void Start()
    {

    }

    void Update()
    {
    }

    // Update is called once per frame
    public void SetBoost(BoostType _boostType)
    {
        boosts.SetActiveAll(false);
        boosts[(int)_boostType].SetActive(true);
        boostType = _boostType;
        gameObject.SetActive(true);
        // icon.sprite = boostProducts[(int)_boostType].icon;
        description.text = boostProducts[(int)_boostType].description;
        price.text = "" + boostProducts[(int)_boostType].GemPrices[0];
        count = boostProducts[(int)_boostType].count[0];

    }

    public void BuyBoost(GameObject button)
    {
        int price_ = int.Parse(price.text);
        GetComponent<AnimationManager>().BuyBoost(boostType, price_, count);
    }
}

[System.Serializable]
public class BoostProduct
{
    public Sprite icon;
    public string description;
    public int[] count;
    public int[] GemPrices;
}