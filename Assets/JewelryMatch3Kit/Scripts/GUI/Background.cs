using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class Background : MonoBehaviour
{
    public Sprite[] pictures;

    // Use this for initialization
    void OnEnable()
    {
        LevelManager.OnLevelLoaded += SetBack;

    }

    private void OnDisable()
    {
        LevelManager.OnLevelLoaded -= SetBack;

    }

    private void SetBack()
    {
        if (LevelManager.THIS != null)
            GetComponent<Image>().sprite = pictures[(int)((float)LevelManager.Instance.currentLevel / 20f - 0.01f)];

        FindObjectsOfType<Backpanel>().ForEachY(i => i.GetComponent<Image>().sprite = GetComponent<Image>().sprite);
    }
}
