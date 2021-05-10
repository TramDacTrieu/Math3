using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PreFailed : MonoBehaviour
{
    public Text price;
    int FailedCost;
    // Use this for initialization
    void OnEnable()
    {
        FailedCost = LevelManager.THIS.FailedCost;
        price.text = "" + FailedCost;


    }



}
