using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
public class RewardIcon : MonoBehaviour
{

    public GameObject[] obj;

    public Text coinsCount;
    public Sprite[] sprites;
    public Image icon;
    // Use this for initialization
    void Start()
    {

    }

    public void SetCoinsCount(int count)
    {
        coinsCount.text = "" + count;
    }

    public void SetIconSprite(int i)
    {
        obj.SetActiveAll(false);
        obj[i].SetActive(true);
    }
}
