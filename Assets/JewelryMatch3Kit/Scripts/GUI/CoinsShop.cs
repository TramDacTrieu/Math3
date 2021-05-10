using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsShop : MonoBehaviour
{
    public Text[] counts;
    public Text[] prices;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            counts[i].text = "" + LevelManager.THIS.gemsProducts[i].count;
            prices[i].text = "" + LevelManager.THIS.gemsProducts[i].price;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
