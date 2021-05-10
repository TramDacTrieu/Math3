using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Counter_ : MonoBehaviour
{
    public int ingrTrackNumber;
    Text txt;
    private float lastTime;
    bool alert;
    public static int totalCount;
    TargetGUI parentGUI;

    // Use this for initialization
    void Start()
    {
        txt = GetComponent<Text>();
        if (transform.parent.GetComponent<TargetGUI>() != null)
            parentGUI = transform.parent.GetComponent<TargetGUI>();

        StartCoroutine(DelayUpdate());
    }

    void OnEnable()
    {
        lastTime = 0;
        alert = false;
        if (name == "TargetScore" && LevelManager.THIS != null)
        {
            if (LevelManager.THIS.target == Target.SCORE)
                txt.text = "" + LevelManager.THIS.GetScoresOfTargetStars();
        }
        UpdateCount();
    }

    // Update is called once per frame
    IEnumerator DelayUpdate()
    {
        while (true)
        {
            UpdateCount();
            yield return new WaitForSeconds(.5f);
        }

    }

    private void UpdateCount()
    {
        if (LevelManager.THIS == null || txt == null) return;
        if (name == "Score")
        {
            txt.text = "" + LevelManager.Score;
        }
        if (name == "LabelKeepPlay")
        {
            if (LevelManager.THIS.limitType == LIMIT.MOVES)
                txt.text = "GET + " + LevelManager.THIS.ExtraFailedMoves + " moves";
            else
                txt.text = "GET + " + LevelManager.THIS.ExtraFailedSecs + " secs";

        }


        if (name == "TargetDiamonds")
        {
            txt.text = "" + (totalCount - (LevelManager.THIS.TargetDiamonds));
            if (LevelManager.THIS.TargetDiamonds >= totalCount && LevelManager.THIS.gameStatus == GameState.Playing)
                parentGUI.Done();
        }


        // if (name == "TargetIngr2") {
        // 	txt.text = "" + LevelManager.THIS.ingrCountTarget [1];
        // }
        if (name == "Lifes")
        {
            txt.text = "" + InitScript.Instance.GetLife();
        }

        if (name == "Gems")
        {
            txt.text = "" + InitScript.Coins;
        }
        if (name == "Level")
        {
            txt.text = "LEVEL " + LevelManager.THIS.currentLevel;
        }
        if (name == "TargetDescription1")
        {
            txt.text = "" + totalCount;


        }
    }
}
