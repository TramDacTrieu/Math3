using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TouchCounter : MonoBehaviour
{
    public Text text;
    public GameObject[] Arrows;

    bool switchedType;
    public static int type;

    void OnEnable()
    {
        Arrows.ForEachY(x => x.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, .5f));
        switchedType = !switchedType;
        if (switchedType) type = 0;
        else type = 1;
        // SetType(type);
    }

    bool statusReached;

    public void NextType(int count, bool reachedNextBonus)
    {
        // if (!reachedNextBonus && !statusReached)
        // {
        //     Arrows.ForEachY(x => x.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, .5f));
        //     statusReached = true;
        // }
        // else if (reachedNextBonus)
        // {
        //     Arrows.ForEachY(x => x.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1));
        //     statusReached = false;
        // }
        int newType = 0;
        if (count >= 6) { SetType(type); }
        if (count >= 9) { type = 2; SetType(type); }
        if (count >= 12) { type = 3; SetType(type); }
        // if (newType != type && !reachedNextBonus)
        // {
        //     newType = type;
        //     SetType(type);
        // }
    }

    void SetType(int t)
    {
        Arrows.ForEachY(x => x.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, .5f));
        switch (t)
        {
            case 1://Vertical
                Arrows[0].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                break;
            case 0://Horrizontal
                Arrows[1].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                break;
            case 2://Cross
                Arrows[0].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                Arrows[1].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                break;
            default:
                Arrows.ForEachY(x => x.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1));
                break;
        }
    }
    // private void Update()
    // {
    //     text.text = LevelManager.THIS.destroyAnyway.Count.ToString();
    // }

    public void UpdateCount(int count)
    {
        text.text = "" + count;

    }

    internal void SetColor(int selectedColor)
    {
        GetComponentsInChildren<IColorableComponent>().ForEachY(i => i.SetColor(selectedColor));
    }
}
