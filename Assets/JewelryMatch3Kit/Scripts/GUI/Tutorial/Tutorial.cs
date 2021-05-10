using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public static Tutorial THIS;
    public GameObject[] obj;
    public Image image;

    public Button LeftButton;
    public Button RightButton;
    public Button OkButton;
    int i;

    private void Awake()
    {
        THIS = this;

    }

    private void OnEnable()
    {
        obj.SetActiveAll(false);
        obj[0].SetActive(true);
        if (SceneManager.GetActiveScene().name != "map")
        {
            LeftButton.gameObject.SetActive(false);
            RightButton.gameObject.SetActive(false);
            OkButton.gameObject.SetActive(true);
        }
        else
        {
            LeftButton.gameObject.SetActive(true);
            RightButton.gameObject.SetActive(true);
            OkButton.gameObject.SetActive(false);
        }
    }
    public void Next()
    {
        obj.SetActiveAll(false);
        i++;
        i = Mathf.Clamp(i, 0, obj.Length - 1);
        obj[i].SetActive(true);
        if (i >= obj.Length - 1) Last();
    }
    public void Back()
    {
        obj.SetActiveAll(false);
        i--;
        i = Mathf.Clamp(i, 0, obj.Length - 1);
        obj[i].SetActive(true);
        if (i <= 0) First();
    }

    void First()
    {
        LeftButton.gameObject.SetActive(false);
        RightButton.gameObject.SetActive(true);
    }

    private void Last()
    {
        LeftButton.gameObject.SetActive(true);
        RightButton.gameObject.SetActive(false);
    }

    public void ShowInGameTutorial()
    {
        obj.SetActiveAll(false);
        obj[LevelSettings.THIS.LevelScriptable.Tutorial - 1].SetActive(true);

    }
}
