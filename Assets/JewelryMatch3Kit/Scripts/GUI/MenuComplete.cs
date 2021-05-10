using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuComplete : MonoBehaviour
{
    public GameObject[] stars;
    private Text scores;
    public Animator characterAnimator;

    private void OnEnable()
    {
        scores = transform.Find("Image").Find("Score").GetComponent<Text>();
        scores.text = "0";
        for (int i = 0; i < 3; i++)
        {
            stars[i].SetActive(false);
        }
    }

    public void Show()
    {

        StartCoroutine(MenuCompleteStars());
        StartCoroutine(MenuCompleteScoring());
    }

    IEnumerator MenuCompleteStars()
    {
        for (int i = 0; i < LevelManager.THIS.stars; i++)
        {
            stars[i].SetActive(true);
            stars[i].transform.SetAsLastSibling();

            yield return new WaitForSeconds(0.5f);
        }


    }

    IEnumerator MenuCompleteScoring()
    {
        for (int i = 0; i <= LevelManager.Score; i += 100)
        {
            scores.text = "" + i;
            // SoundBase.Instance.audio.PlayOneShot( SoundBase.Instance.scoring );
            yield return new WaitForSeconds(0.00001f);
        }
        CompleteScoring();
    }

    public void CompleteScoring()
    {
        scores.text = "" + LevelManager.Score;
    }
}